using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Csandra.Comics.App.wasm.Data
{
    public class ComicDataService
    {   
        private readonly string apiUri = "https://xq2izutcxg.execute-api.eu-west-1.amazonaws.com/Prod";
        public static readonly string[] Genres = new[]{
            "SciFi", "Action", "Adventure", 
            "Thriller", "Romance", "Crime", 
            "Horror", "Fantasy", "Christmas",
            "Comedy", "Drama", "Dystopian",
            "Martial Arts", "Mystery", "Pirate",
            "Spy", "Sports"
        };

        public async Task<List<ComicData>> GetComics (string client=""){
           try{
                if(string.IsNullOrWhiteSpace(client))
                    throw new UnauthorizedAccessException();
                 using(var httpClient = new HttpClient()){
                    var response = await  httpClient.GetAsync($"{apiUri}?client={client}");
                    if(response.IsSuccessStatusCode){
                        var json = await response.Content.ReadAsStringAsync();
                        
                        var comics = (IEnumerable<ComicData>)JsonConvert.DeserializeObject(json, typeof(IEnumerable<ComicData>));
                        return comics.ToList();
                    }
                    else{
                        throw new Exception(await response.Content.ReadAsStringAsync());
                    }
                   
                }
           }
           catch(Exception ex){
               Console.WriteLine(ex.Message);
               return new List<ComicData>();
           }
        }

        public async void Save(ComicData data, string client=""){
            try{
                if(string.IsNullOrWhiteSpace(client))
                    throw new UnauthorizedAccessException();
                 using(var httpClient = new HttpClient()){
                    var json = JsonConvert.SerializeObject(
                        new {
                            Client = client,
                            Data = data
                        });
                    StringContent content = new StringContent(json);
                    var response = await  httpClient.PostAsync($"{apiUri}", content);
                    if(!response.IsSuccessStatusCode){
                        throw new Exception(await response.Content.ReadAsStringAsync());
                    }
                }
           }
           catch(Exception ex){
               Console.WriteLine(ex.Message);
           }
        }
 
    }
}