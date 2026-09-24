namespace AndroidWebAPI.Models
{
    public class BaseClass
    {

        public bool IsDeleted { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public Guid UserCreated { get; set; }

        public Guid UserModified { get; set; }

    }
}
