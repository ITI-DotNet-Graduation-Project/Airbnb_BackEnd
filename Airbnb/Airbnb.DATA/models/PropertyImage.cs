namespace Airbnb.DATA.models
{
    public class PropertyImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public int PropertyId { get; set; }
        public virtual Property Property { get; set; }
    }
}
