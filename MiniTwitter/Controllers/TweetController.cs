using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniTwitter.Data;

namespace MiniTwitter.Controllers
{
    public class TweetsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<TweetsController> _logger;
        public TweetsController(ApplicationDbContext context, ILogger<TweetsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var tweets = _context.Tweets.ToList();
            return View(tweets);
        }
    }
}


// tutorial: https://www.youtube.com/watch?v=QtiM87MV27w