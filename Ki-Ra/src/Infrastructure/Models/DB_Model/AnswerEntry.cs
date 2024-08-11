namespace KiRa.Infrastructure.Models.DB_Model
{
    public class AnswerEntry
    {
        public int Id { get; set; }
        public string Answer { get; set; }
        public int CommandId { get; set; }
        public CommandEntry Command { get; set; }
    }
}