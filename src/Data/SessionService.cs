using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Csandra.Comics.App.wasm.Data{
    public class SessionService{
        private readonly IJSRuntime _jsRuntime;
        private readonly ComicDataService _comicDataService;
        private readonly string name = "name";
        private readonly string accountidentifier = "accountIdentifier";
        private readonly string accessCheck = "accessCheck";
        protected Dictionary<string, string> SessionData{get;private set;} = new Dictionary<string, string>();
        
        public event EventHandler<EventArgs> SessionDataChanged;
        public bool IsLoggedIn{get=> SessionData.Count() > 0;}
        public bool CanChange{get=> SessionData.ContainsKey(accessCheck) && SessionData[accessCheck] == "";}
        
        internal string GetIdentifier(){
            if(SessionData.ContainsKey(accountidentifier))
                return SessionData[accountidentifier];
            else
                return "";
        }
        
        public SessionService(IJSRuntime jsRuntime, ComicDataService comicService)
        {
            _jsRuntime = jsRuntime;
            _comicDataService = comicService;
        }
        
        internal void SetSessionData(IEnumerable<KeyValuePair<string,string>> data)
        {
            foreach(var kv in data)
                SetSessionData(kv);
        }

        internal void SetSessionData(KeyValuePair<string, string> keyValuePair){
            SetSessionData(keyValuePair.Key, keyValuePair.Value);
        }
        internal void SetSessionData(string key, string value){
            if(!SessionData.ContainsKey(key))
                SessionData.Add(key, "");
            SessionData[key] = value;
            SessionDataChanged?.Invoke(this, new EventArgs());
        }

        internal void ClearSessionData(){
            SessionData.Clear();
            SessionDataChanged?.Invoke(this, new EventArgs());
        }

        public async void SignOut(){
            if(_jsRuntime != null)
                await _jsRuntime.InvokeVoidAsync("logout", this);
            ClearSessionData();
        } 

	
        public async void SignIn(){
            if(_jsRuntime != null)
                await _jsRuntime.InvokeVoidAsync("signIn", DotNetObjectReference.Create(this));
        }
        
        [JSInvokable("CallSetSessionData")]
        public async void CallSetSessionData(string json){
            try{
                var data = JObject.Parse(json);
                Console.WriteLine(data.GetValue(accountidentifier));
                SetSessionData(accountidentifier, (string)data.GetValue(accountidentifier));
                Console.WriteLine(data.GetValue(name));
                SetSessionData(name, (string)data.GetValue(name));
                await CheckAccess((string)data.GetValue(accountidentifier));
            }
            catch(Exception ex){
                Console.WriteLine(ex.Message);
            }
        }

         internal async Task CheckAccess(string client=""){
            try{
                if(string.IsNullOrWhiteSpace(client))
                    throw new UnauthorizedAccessException();
                 using(var httpClient = new HttpClient()){
                    var json = JsonConvert.SerializeObject(
                        new {
                            Client = client
                        });
                    StringContent content = new StringContent(json);
                    var response = await httpClient.PostAsync($"{_comicDataService.GetAPI()}/access", content);
                    if(!response.IsSuccessStatusCode){
                        throw new Exception(await response.Content.ReadAsStringAsync());
                    }
                    else{
                        var result = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(result);
                        SetSessionData(accessCheck, result);
                    }
                }
           }
           catch(Exception ex){
               Console.WriteLine(ex.Message);
           }
        }
    }
}