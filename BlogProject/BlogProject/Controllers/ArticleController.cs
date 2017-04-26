namespace BlogProject.Controllers
{
    using BlogProject.Models;
    using Microsoft.AspNet.Identity;
    using PagedList;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;

    public class ArticleController : Controller
    {
        public ActionResult List(int? page)
        {
            using (var db = new BlogDbContext())
            {
                var articles = db.Articles
                    .Include(a => a.Author)
                    .ToList();

                return View(articles.ToPagedList(page ?? 1, 6));
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(Article article)
        {
            if (ModelState.IsValid)
            {
                using (var db = new BlogDbContext())
                {
                    var authorId = this.User.Identity.GetUserId();

                    article.AuthorId = authorId;

                    db.Articles.Add(article);
                    db.SaveChanges();

                    return RedirectToAction("List");
                }
            }

            return View(article);
        }
    }
}