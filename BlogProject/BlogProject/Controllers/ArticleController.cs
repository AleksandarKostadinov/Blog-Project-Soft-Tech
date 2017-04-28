namespace BlogProject.Controllers
{
    using BlogProject.Models;
    using Microsoft.AspNet.Identity;
    using PagedList;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;

    public class ArticleController : Controller
    {
        public ActionResult List(int? page)
        {
            using (var db = new BlogDbContext())
            {
                var articles = db.Articles
                    .OrderByDescending(a => a.Id)
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
        public ActionResult Create(Article article, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                using (var db = new BlogDbContext())
                {
                    var authorId = this.User.Identity.GetUserId();

                    article.AuthorId = authorId;

                    if (image != null)
                    {
                        var allowedContentTypes = new[] {
                            "image/jpg",
                            "image/jpeg",
                            "image/pjpeg",
                            "image/gif",
                            "image/x-png",
                            "image/png"
                        };

                        if (allowedContentTypes.Contains(image.ContentType.ToLower()))
                        {
                            var imagesPath = "/Content/Images/";

                            var fileName = image.FileName;

                            var uploadPath = imagesPath + fileName;
                            var physicalPath = Server.MapPath(uploadPath);

                            image.SaveAs(physicalPath);

                            article.ImagePath = uploadPath;
                        }
                    }

                    db.Articles.Add(article);
                    db.SaveChanges();

                    return RedirectToAction("List");
                }
            }

            return View(article);
        }

        public ActionResult Details(int? id, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                var article = db.Articles
                    .Include(a => a.Author)
                    .Where(a => a.Id == id)
                    .FirstOrDefault();

                if (article == null)
                {
                    return HttpNotFound();
                }

                article.Comments = db.Comments
                    .Where(c => c.ArticleId == article.Id)
                    .OrderByDescending(c => c.Id)
                    .ToPagedList(page ?? 1, 6);

                return View(article);
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                var article = db.Articles.Find(id);

                if (article == null || !IsAuthorized(article))
                {
                    return HttpNotFound();
                }

                return View(article);
            }
        }

        [Authorize]
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                var article = db.Articles.Find(id);

                if (article == null || !IsAuthorized(article))
                {
                    return HttpNotFound();
                }

                db.Articles.Remove(article);
                db.SaveChanges();

                return RedirectToAction("List");
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                var article = db.Articles.Find(id);

                if (article == null || !IsAuthorized(article))
                {
                    return HttpNotFound();
                }

                var articleViewModel = new ArticleViewModel
                {
                    Id = article.Id,
                    Title = article.Title,
                    Content = article.Content,
                    AuthorId = article.AuthorId

                };

                return View(articleViewModel);
            }
        }
        [Authorize]
        [HttpPost]
        public ActionResult Edit(ArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new BlogDbContext())
                {
                    var article = db.Articles.Find(model.Id);

                    if (article == null || !IsAuthorized(article))
                    {
                        return HttpNotFound();
                    }

                    article.Title = model.Title;
                    article.Content = model.Content;

                    db.SaveChanges();
                }

                    return RedirectToAction("Details", new { id = model.Id });
            }

            return View(model);
        }

        private bool IsAuthorized(Article article)
        {
            bool isAdmin = this.User.IsInRole("Admin");
            bool isAuthor = article.IsAuthor(this.User.Identity.GetUserId());

            return isAdmin || isAuthor;
        }
    }
}