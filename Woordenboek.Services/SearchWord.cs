using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Woordenboek.Services
{
    public class SearchWord
    {
        public string Word { get; set; }
        //public string Description { get; set; }

        public List<ResultWord> Results { get; set; }

        public string FirstLetter
        {
            get { return Word.Substring(0,1).ToUpper(); }
        }

        public string GetAsHtml()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var d in this.Results)
            {
                sb.AppendLine(d.Word + "<br/>");
                //sb.AppendLine(d.Description + "<br/>");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public string GetAsText()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var d in this.Results)
            {
                sb.AppendLine(d.Word);
                sb.Append(Environment.NewLine);
                //sb.AppendLine(d.Description);
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);

            }

            return sb.ToString();
        }
    }

    public class ResultWord
    {
        public string Word { get; set; }
        //public string Description { get; set; }


        public string WordCap
        {
            get { return Word.ToUpper(); }
        }
        
    }
}
