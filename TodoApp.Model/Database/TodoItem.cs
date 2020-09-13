using System.ComponentModel.DataAnnotations;

namespace TodoApp.Model.Database
{
    public class TodoItem
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(512)]
        [Required]
        public string UserName { get; set; }

        public bool IsCompleted { get; set; }

        [MaxLength(512)]
        [Required]
        public string Description { get; set; }
    }
}
