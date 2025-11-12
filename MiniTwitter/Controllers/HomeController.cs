using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

    public IActionResult Index(int page = 1, int pageSize = 10)
    {
        List<Tweet> Tweets = _context.Tweets.OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        foreach (var i in Tweets)
        {
            User? user = _context.Users.FirstOrDefault(u => u.Id == i.UserId);
            i.UserId = user?.DisplayName ?? "Unknown user";
        }

        TweetsPageViewModel model = new()
        {
            AllTweets = Tweets,
            NewTweet = new Tweet { Content = "", UserId = "" }
        };

        ViewBag.CurrentPage = page;
        ViewBag.HasMorePages = _context.Tweets.Count() > page * pageSize;
        ViewBag.TotalPages = _context.Tweets.Count() / pageSize + (_context.Tweets.Count() % pageSize > 0 ? 1 : 0);

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
            model.AllTweets = [.. _context.Tweets.OrderByDescending(t => t.CreatedAt)];
            return View("Index", model);
        }

        if (string.IsNullOrWhiteSpace(model.NewTweet.Content))
        {
            ModelState.AddModelError("NewTweet.Content", "Tweet content cannot be empty!");
            model.AllTweets = [.. _context.Tweets.OrderByDescending(t => t.CreatedAt)];
            return View("Index", model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        Tweet tweet = new()
        {
            Content = model.NewTweet.Content,
            UserId = user.Id,
            LikesCount = 0,
            ParentTweetId = null,
            CreatedAt = DateTime.Now
        };

        _context.Tweets.Add(tweet);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int tweetId)
    {
        var tweet = await _context.Tweets.FindAsync(tweetId);
        if (tweet != null)
        {
            _context.Tweets.Remove(tweet);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}