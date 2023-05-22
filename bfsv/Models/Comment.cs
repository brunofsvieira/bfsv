namespace bfsv.Models
{
    public class Comment : BaseEntity
    {
        public string AuthorId { get; set; }
        public string EntityId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<UserView> UsersViews { get; set; }
    }
}
