using HelpMyStreet.Utils.Enums;
using System.Collections.Generic;

namespace HelpMyStreet.Utils.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public QuestionType Type {get;set;}
        public bool Required { get; set; }
        public string SubText { get; set; }
        public string PlaceholderText { get; set; }
        public RequestHelpFormStage FormStage { get; set; }
        public string Location { get; set; }
        public string Answer { get; set; }
        public List<AdditonalQuestionData> AddtitonalData { get; set; }
    }


}
