namespace bfsv.Models
{
    /// <summary>
    /// The user view. This class save the users who saw the documents.
    /// </summary>
    public class UserView
    {
        public string UserId { get; set; }
        public DateTime ViewDate { get; set; }
    }
}
