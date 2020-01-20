
// configuration to initialize msal
const msalConfig = {
    auth: {
        clientId: "@@@CLIENT_ID@@@", //This is your client ID
        authority: "https://login.microsoftonline.com/common", //This is your tenant info
        redirectURI: "@@@REDIRECT_URI@@@"
    },
    cache: {
        cacheLocation: "localStorage",
        storeAuthStateInCookie: true
    }
};


// instantiate MSAL
const myMSALObj = new Msal.UserAgentApplication(msalConfig);
// request to signin - returns an idToken
var requestObj = {
    scopes: ["user.read"]
};

// signin and acquire a token silently with POPUP flow. Fall back in case of failure with silent acquisition to popup
function signIn(session) {
    myMSALObj.loginPopup(requestObj).then(function (loginResponse) {
        // Call Blazor Code
        session.invokeMethodAsync('CallSetSessionData', JSON.stringify(loginResponse.account))
        .then((message) => {
            console.log(message);
        });
    }).catch(function (error) {
        console.log(error);
    });
}

// signout the user
function logout(session) {
    // Removes all sessions, need to call AAD endpoint to do full logout
    myMSALObj.logout();
  }

// Blazor jsinterop functions:
window.signIn = signIn;
window.logout = logout;