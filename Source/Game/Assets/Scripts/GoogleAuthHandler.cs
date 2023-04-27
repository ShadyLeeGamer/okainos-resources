using UnityEngine;
using System;
using Proyecto26;

//https://developers.google.com/identity/protocols/oauth2/native-app
public static class GoogleAuthHandler
{
    public static OAuthSignInResponse GoogleUserInfo { get; set; }

    const string CLIENT_ID =
        "CLIENT ID";
    const string CLIENT_SECRET =
        "CLIENT SECRET";

    // MANUAL COPY/PASTE
    const string REDIRECT_URI = "urn:ietf:wg:oauth:2.0:oob";
    // LOOPBACK IP ADDRESS
    //const string REDIRECT_URI = "urn:ietf:wg:oauth:2.0:oob";

    const string OAUTH2_URL =
        "https://accounts.google.com/o/oauth2/v2/auth";

    public static void GetAuthCode()
    {
        string responseType = "code";
        string scope = "email";
        // OPEN GOOGLE AUTH PAGE IN BROWSER
        Application.OpenURL(OAUTH2_URL+$"?client_id={CLIENT_ID}&redirect_uri={REDIRECT_URI}&response_type={responseType}&scope={scope}");
    }

    public static void ExchangeAuthCodeWithIdToken(string code, Action<string> callback)
    {
        string exchangeAuthCode = "https://oauth2.googleapis.com/token";
        string grantType = "authorization_code";
        RestClient
            .Post($"{exchangeAuthCode}?code={code}&client_id={CLIENT_ID}&client_secret={CLIENT_SECRET}&redirect_uri={REDIRECT_URI}&grant_type={grantType}", null)
            .Then(response =>
            {
                var data =
                    StringSerializationAPI.Deserialize(
                        typeof(GoogleIdTokenResponse), response.Text) as GoogleIdTokenResponse;
                callback(data.id_token);
            })
            .Catch(Debug.LogWarning);
    }

    [Serializable]
    class GoogleIdTokenResponse
    {
        public string id_token;
    }
}

[Serializable]
public class OAuthSignInResponse
{
    public string email;
    public bool emailVerified;

    public string localId;
    public string idToken;

    public OAuthSignInResponse(string email, bool emailVerified, string localId, string idToken)
    {
        this.email = email;
        this.emailVerified = emailVerified;
        this.localId = localId;
        this.idToken = idToken;
    }
}