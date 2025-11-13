using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniTwitter.Data;
using MiniTwitter.Models;
using MiniTwitter.Models.Classes;
using MiniTwitter.ViewModels;

namespace MiniTwitter.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<User> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    public async Task GetSetPagesAsync(int page = 1, int pageSize = 10)
    {
        ViewBag.CurrentPage = page;

        int totalTweets = 0;
        try
        {
            totalTweets = await _context.Tweets.CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to count tweets for pagination.");
            ViewBag.CurrentPage = 1;
        }

        ViewBag.HasMorePages = totalTweets > page * pageSize;
        ViewBag.TotalPages = totalTweets / pageSize + (totalTweets % pageSize > 0 || totalTweets == 0 ? 1 : 0);
    }

    public async Task<TweetsPageViewModel> getTweetsAsync(TweetsPageViewModel model, int page = 1, int pageSize = 10)
    {
        try
        {
            model.AllTweets = await _context.Tweets
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new Tweet
                {
                    TweetId = t.TweetId,
                    Content = t.Content,
                    CreatedAt = t.CreatedAt,
                    LikesCount = t.LikesCount,
                    ParentTweetId = t.ParentTweetId,
                    UserId = _context.Users
                        .Where(u => u.Id == t.UserId)
                        .Select(u => u.DisplayName)
                        .FirstOrDefault() ?? "Unknown user"
                }).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while loading the tweets.");
            ViewBag.ErrorMessage = "Unable to load tweets at the moment. Please try again later.";
            model.AllTweets = new List<Tweet>();
        }
        return model;
    }
    
    public async Task<IActionResult> ReturnIndexViewAsync(TweetsPageViewModel model)
    {
        model = await getTweetsAsync(model);
        await GetSetPagesAsync(1, 10);
        return View("Index", model);
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        TweetsPageViewModel model = await getTweetsAsync(new TweetsPageViewModel
        {
            NewTweet = new Tweet { Content = "", UserId = "" }
        }, page, pageSize);

        await GetSetPagesAsync(page, pageSize);

        return View(model);
    }

    public IActionResult Create()
    {
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TweetsPageViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return await ReturnIndexViewAsync(model);
        }

        if (string.IsNullOrWhiteSpace(model.NewTweet.Content))
        {
            ModelState.AddModelError("NewTweet.Content", "Tweet content cannot be empty!");
            return await ReturnIndexViewAsync(model);
        }

        Tweet tweet;
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            tweet = new()
            {
                Content = model.NewTweet.Content,
                UserId = user.Id,
                LikesCount = 0,
                ParentTweetId = null,
                CreatedAt = DateTime.Now
            };
        }
        catch
        {
            _logger.LogError("An error occurred while retrieving the current user.");
            ModelState.AddModelError("", "Unable to retrieve user information. Please try again later.");
            return await ReturnIndexViewAsync(model);
        }

        _context.Tweets.Add(tweet);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating a new tweet.");
            ModelState.AddModelError("", "Unable to create tweet at the moment. Please try again later.");
            return await ReturnIndexViewAsync(model);
        }

        return RedirectToAction("Index");
    }
    
    [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Remove(int tweetId)
{
    try
    {
        var tweet = await _context.Tweets.FindAsync(tweetId);
        if (tweet != null)
        {
            _context.Tweets.Remove(tweet);
            await _context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"Failed to remove tweet {tweetId}.");
    }
    return RedirectToAction("Index");
}

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}