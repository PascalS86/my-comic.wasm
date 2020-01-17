using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Csandra.Comics.App.wasm.Data{
    public class SessionService{
        private readonly IJSRuntime _jsRuntime;
        private readonly string name = "name";
        private readonly string accountidentifier = "accountIdentifier";
        protected Dictionary<string, string> SessionData{get;private set;} = new Dictionary<string, string>();
        
        public event EventHandler<EventArgs> SessionDataChanged;
        public bool IsLoggedIn{get=> SessionData.Count() > 0;}
        
        internal string GetIdentifier(){
            if(SessionData.ContainsKey(accountidentifier))
                return SessionData[accountidentifier];
            else
                return "";
        }
        
        public SessionService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
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
        public void CallSetSessionData(string json){
            try{
                var data = JObject.Parse(json);
                Console.WriteLine(data.GetValue(accountidentifier));
                SetSessionData(accountidentifier, (string)data.GetValue(accountidentifier));
                Console.WriteLine(data.GetValue(name));
                SetSessionData(name, (string)data.GetValue(name));
            }
            catch(Exception ex){
                Console.WriteLine(ex);
            }
        }
    }
}