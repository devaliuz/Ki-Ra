using System.Collections.Generic;

namespace KiRa.Infrastructure.Models.DB_Model
{
    public class CommandEntry
    {
        public int Id { get; set; }
        public string Command { get; set; }
        public List<AnswerEntry> Answers { get; set; } = new List<AnswerEntry>();
        public List<SynonymEntry> Synonyms { get; set; } = new List<SynonymEntry>();
    }
}