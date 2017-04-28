namespace BlogProject.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ArticleComment
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        public string AutorName { get; set; }

        public int ArticleId { get; set; }

        public virtual Article Article { get; set; }
    }
}