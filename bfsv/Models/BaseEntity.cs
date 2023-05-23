namespace bfsv.Models
{
    /// <summary>
    /// Base entity.
    /// </summary>
    public class BaseEntity
    {
        public string Id { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
