namespace PleskEmailAliasManager.Models
{
    public class DomainData
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DomainData(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
