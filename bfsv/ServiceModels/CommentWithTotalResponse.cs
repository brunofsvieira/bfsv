namespace bfsv.ServiceModels
{
    public class CommentWithTotalResponse
    {
        public List<CommentResponse> Comments { get; set; }
        public int TotalResults { get; set; }
    }
}
