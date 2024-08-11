namespace KiRa.Infrastructure.Models.DB_Model
{
    public class SynonymEntry
    {
        public int Id { get; set; }
        public string Synonym { get; set; }
        public int CommandId { get; set; }
        public CommandEntry Command { get; set; }
    }
}