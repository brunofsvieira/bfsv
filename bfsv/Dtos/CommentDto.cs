namespace bfsv.Dtos
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public int EntityId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
