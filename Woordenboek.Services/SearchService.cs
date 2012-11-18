using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Woordenboek.Services
{
    public static class SearchService
    {

        public static async Task<SearchWord> SearchAsync(string text)
        {
            var history = await HistoryService.GetHistory();
            var item = history.Where(x => x.Word.ToLower() == text.ToLower()).FirstOrDefault();
            if (item != null)
                return item;

            try
            {
                HttpClient client = new HttpClient();

                HttpResponseMessage response = await client.GetAsync(string.Format("http://mobile.rijmwoordenboek.nl/?word={0}", text));

                var result = ParseResult(response, text);

                if(result != null && result.Results != null && result.Results.Count > 0)
                    await HistoryService.SaveSearchWord(result);


                return result;


            }
            catch { }

            return null;

        }



        private static SearchWord ParseResult(HttpResponseMessage response, string text)
        {
            var searchWord = new SearchWord { Word = text };


            string result = string.Empty;

            if (response.IsSuccessStatusCode)
            {
               string resultString = response.Content.ReadAsStringAsync().Result;

               int start = resultString.IndexOf("<div style=\"margin-left: 70px;\">");
               int end = resultString.IndexOf("</div>");


               string allWords = resultString.Substring(start, end - start);
                var split = new string[] { "<br />" };
                var ar = allWords.Split(split, StringSplitOptions.RemoveEmptyEntries);

                // DateTime.Now.DayOfWeek == DayOfWeek.
                searchWord.Results = new List<ResultWord>();

                foreach (var item in ar)
                {

                    var wordResult = StripHtml(item).Trim();

                    if(!string.IsNullOrEmpty(wordResult) && wordResult.Length > 1)
                        searchWord.Results.Add(new ResultWord() { Word = wordResult });

                }

                if (string.IsNullOrEmpty(result))
                    result = "No results found.";
                else if (!string.IsNullOrEmpty(result))
                {

                   
                    
                }

            }
            else
            {
                result = "Unable to contact server.";
                return null;
            }

            return searchWord;
        }


        private static string StripHtml(string input)
        {
            Regex reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            return reg.Replace(input, string.Empty).Trim();
        }
    }
}
