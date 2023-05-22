using bfsv.Models;

namespace bfsv.ServiceModels
{
    public class CommentResponse
    {
        public string Id { get; set; }
        public string AuthorId { get; set; }
        public string EntityId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
