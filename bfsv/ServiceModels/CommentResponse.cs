using bfsv.Models;

namespace bfsv.ServiceModels
{
    public class CommentResponse
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public int EntityId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
