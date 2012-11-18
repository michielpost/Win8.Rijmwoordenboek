using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using mpost.WP7.Client.Storage;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Woordenboek.Services
{
    public static class HistoryService
    {

        public async static Task<List<SearchWord>> GetHistory()
        {
            try
                {
            List<SearchWord> info = await IsolatedStorageCacheManager.LoadData<List<SearchWord>>("History.xml");
            if (info == null)
                info = new List<SearchWord>();


            return info;
                }
           catch
                {
               return new List<SearchWord>();
        }}

        public async static Task SaveSearchWord(SearchWord word)
        {
            var list = await GetHistory();
            list.Reverse();
            list.Add(word);

            list.Reverse();

            list = list.Take(50).ToList();

            await SaveHistory(list);
        }

        private static async Task SaveHistory(List<SearchWord> info)
        {
            await IsolatedStorageCacheManager.SaveData("History.xml", info);
        }




        public static void Clear()
        {
            SaveHistory(new List<SearchWord>());
        }
    }
}
