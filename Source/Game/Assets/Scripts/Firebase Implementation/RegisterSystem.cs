using UnityEngine;
using TMPro;
using System;
using Proyecto26;
using UnityEngine.UI;
using LiquidUI;

// IMPORTANT NOTES:
// IF SUBMITTING DATA TO DATABASE TAKES MORE THAN 1 HOUR,
// THEN LOOK INTO REFRESHING THE ID TOKEN SINCE IT EXPIRES IN ONE HOUR

public class RegisterSystem : MonoBehaviour
{
    [System.Serializable]
    public struct ValidationElement
    {
        public Selectable element;
        public TextMeshProUGUI errorDisplay;

        public bool FieldIsFilled()
        {
            var input = (TMP_InputField)element;
            if (input.text.Trim().Length == 0)
            {
                ShowErrorMessage(element.gameObject.name + " " + errorMessages[0]);
                return false;
            }
            return true;
        }

        public void ShowErrorMessage(string message)
        {
            SetErrorMessageActive(true);
            errorDisplay.text = message;
        }

        public void SetErrorMessageActive(bool setActive)
        {
            if (errorDisplay.gameObject.activeSelf != setActive)
                errorDisplay.gameObject.SetActive(setActive);
        }

        public static string[] errorMessages =
        {
        "field is empty",
        "Username is already taken",
        "Email is already being used",
        "Passwords do not match",
        "Email is not verified",
        "Email is not valid",
        "Details do not match",
        "Something went wrong"
    };
    }

    [Header("Sign Up")]
    [SerializeField, Tooltip("0-Email,1-Password,2-Confirm")]
    ValidationElement[] signUpInputsEmailPassword = new ValidationElement[3];
    [SerializeField, Tooltip("0-Username")]
    ValidationElement[] signUpInputsUsername = new ValidationElement[1];

    [Header("Log In")]
    [SerializeField, Tooltip("0-Username/Email,1-Password")]
    ValidationElement[] logInInputs = new ValidationElement[2];

    public string[] errorMessages;

    [SerializeField] ValidationElement signUpBtn, logInBtn;

    [SerializeField] TabManager tabManager;
    [SerializeField] GameObject signUpPageEmailPassword, signUpPageUsername;

    [SerializeField] TMP_InputField googleCodeSignUpInput, googleCodeLogInInput;

    public static RegisterSystem Instance { get; private set; }

    string idToken;
    string localId, getLocalId;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        StartCoroutine(DB.CheckInternetConnection(internetConnection =>
        {
            if (!internetConnection)
                if (PersistentData.GetCachedUserData().localId != "") // CHECK IF DATA HAS BEEN CACHED YET
                {
                    PersistentData.LoadCachedUserData();
                    SceneLoader.LoadSceneAsync(SceneIndexes.Main_Menu);
                }
                else
                    Debug.Log("Data not cached yet and theres no internet... SOOO... poop");
        }));
    }

    public void ClearWindow(int windowIndex)
    {
        foreach (ValidationElement input in (windowIndex == 0) ? signUpInputsEmailPassword
                                                               : logInInputs)
            ((TMP_InputField)input.element).text = "";
        if(windowIndex == 0)
            ((TMP_InputField)signUpInputsUsername[0].element).text = "";
    }

    public void ClearErrorMessages(int windowIndex)
    {
        if (windowIndex == 0)
            signUpBtn.SetErrorMessageActive(false);
        else
            logInBtn.SetErrorMessageActive(false);

        foreach(ValidationElement input in (windowIndex == 0) ? signUpInputsEmailPassword
                                                              : logInInputs)
            input.SetErrorMessageActive(false);
    }

    bool FieldsAreFilled(int windowIndex, int pageIndex)
    {
        bool fieldsAreFilled = true;
        ValidationElement[] inputs = (windowIndex == 0) ? (pageIndex == 0) ? signUpInputsEmailPassword
                                                                           : signUpInputsUsername
                                                        : logInInputs;
        foreach (ValidationElement input in inputs)
            if (!input.FieldIsFilled())
                fieldsAreFilled = false;
        return fieldsAreFilled;
    }

    bool IsEmailValid(string email)
    {
        string[] atCharacter; // @
        string[] dotCharacter; // .
        // SPLIT @ FROM EMAIL AND ADD CHAR BEFORE AND AFTER @ TO INDEX 0 AND 1 OF AT CHAR ARRAY
        atCharacter = email.Split("@"[0]);
        // IF THERES ONE @ THEN THERE SHOULD BE TWO CHARS IN AT CHAR ARRAY (CHAR BEFORE AND AFTER @)
        if (atCharacter.Length == 2)
        {
            // SPLIT . FROM AT CHAR ARRAY AND ADD CHAR BEFORE AND AFTER . TO INDEX 0 AND 1 OF DOT CHAR ARRAY
            dotCharacter = atCharacter[1].Split("."[0]);
            // IF THERES ONE OR MORE . THEN THERE SHOULD BE AT LEAST TWO CHARS IN DOT CHAR ARRAY
            // (CHAR BEFORE AND AFTER .)
            if (dotCharacter.Length >= 2)
            {
                // CHECK IF THERE ARE MORE CHARS AFTER .
                if (dotCharacter[dotCharacter.Length - 1].Length != 0)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        else
            return false;
    }

    bool IsPasswordValid(string password)
    {
        return password.Length >= 6;
    }

    public void ContinueBtn()
    {
        ClearErrorMessages(0);

        if (googleCodeSignUpInput.text == "")
        {
            TMP_InputField[] inputs = new TMP_InputField[signUpInputsEmailPassword.Length];
            for (int i = 0; i < inputs.Length; i++)
                inputs[i] = (TMP_InputField)signUpInputsEmailPassword[i].element;
            string email = inputs[0].text,
                   password = inputs[1].text,
                   confirm = inputs[2].text;
            ValidateSignUpDetails(email, password, confirm,
                isValid =>
                {
                    if (isValid)
                        PromptUsername();
                });
        }
        else
            GoogleConnectWithAuthCode(googleCodeSignUpInput.text);
    }

    public void ValidateSignUpDetails(string email, string password, string confirm, Action<bool> isValid)
    {
        if (!FieldsAreFilled(0, 0))
        {
            isValid(false);
            return;

        }
        if (password != confirm)
        {
            signUpBtn.ShowErrorMessage(errorMessages[3]); // PASSWORDS DO NOT MATCH
            isValid(false);
            return;
        }
        if (!IsEmailValid(email))
        {
            signUpInputsEmailPassword[0].ShowErrorMessage(errorMessages[5]); // EMAIL IS INVALID
            isValid(false);
            return;
        }
        if (!IsPasswordValid(password))
        {
            signUpInputsEmailPassword[1].ShowErrorMessage(errorMessages[9]); // PASSWORD NEED 6 CHARACTERS
            isValid(false);
            return;
        }

        // CHECK FOR COMMON EMAIL
        DB.GetAllObjects<User>(DB.DATABASE_USERS_URL + DB.DotJson(false), users =>
        {
            if (users != null)
            {
                foreach (User user in users)
                    if (email != "")
                        if (user.email == email)
                        {
                            signUpInputsEmailPassword[0].ShowErrorMessage(errorMessages[2]); // EMAIL IS ALREADY BEING USED
                            isValid(false);
                            return;
                        }
                isValid(true);
            }
            else
                isValid(true); // NO USERS FOUND, THIS MUST BE THE FIRST USER
        });
    }

    void PromptUsername()
    {
        signUpPageEmailPassword.SetActive(false);
        signUpPageUsername.SetActive(true);
    }

    public void SignUpBtn()
    {
        signUpInputsUsername[0].SetErrorMessageActive(false);

        TMP_InputField[] inputs = new TMP_InputField[signUpInputsEmailPassword.Length];
        for (int i = 0; i < inputs.Length; i++)
            inputs[i] = (TMP_InputField)signUpInputsEmailPassword[i].element;
        string email = inputs[0].text,
               password = inputs[1].text,
               confirm = inputs[2].text;
        string username = ((TMP_InputField)signUpInputsUsername[0].element).text;

        ValidateUsername(username, isValid =>
        {
            if (isValid)
                SignUpUser(email, password, username);
        });
    }

    void ValidateUsername(string username, Action<bool> isValid)
    {
        if (!signUpInputsUsername[0].FieldIsFilled())
        {
            isValid(false);
            return;
        }

        // CHECK FOR COMMON EMAIL
        DB.GetAllObjects<User>(DB.DATABASE_USERS_URL + DB.DotJson(false), users =>
        {
            if (users != null)
            {
                foreach (User user in users)
                    if (username != "")
                        if (user.username == username)
                        {
                            signUpInputsUsername[0].ShowErrorMessage(errorMessages[1]); // USERNAME IS ALREADY TAKEN
                            isValid(false);
                            return;
                        }
                isValid(true);
            }
            else
                isValid(true); // NO USERS FOUND, THIS MUST BE THE FIRST USER
        });
    }

    void SignUpUser(string email, string password, string username)
    {
        string userData =
            "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient
            .Post<SignResponse>(DB.SIGN_UP_URL, userData)
            .Then(response =>
            {
                string emailVerification =
                    "{\"requestType\":\"VERIFY_EMAIL\",\"idToken\":\"" + response.idToken + "\"}";
                    // WE DONT NEED TO GET ANYTHING FROM THIS ENDPOINT SINCE IT JUST SENDS AN EMAIL TO THE USER
                RestClient
                    .Post(DB.VERIFY_EMAIL_URL, emailVerification); // SEND EMAIL
                localId = response.localId;
                DB.PostUser(email, username, localId, response.idToken, user =>
                {
                    tabManager.ChangeTab(1);
                    signUpPageEmailPassword.SetActive(true);
                    signUpPageUsername.SetActive(false);
                });
            })
            .Catch(error =>
            {
                signUpBtn.ShowErrorMessage(errorMessages[7]); // SOMETHING WENT WRONG
                Debug.LogWarning(error);
            });
    }
    
    public void LogInBtn()
    {
        ClearErrorMessages(1);

        if (googleCodeLogInInput.text == "")
        {
            TMP_InputField[] inputs = new TMP_InputField[logInInputs.Length];
            for (int i = 0; i < inputs.Length; i++)
                inputs[i] = (TMP_InputField)logInInputs[i].element;
            string usernameEmail = inputs[0].text,
                   password = inputs[1].text;
            ValidateLoginDetails(usernameEmail,
                isValid =>
                {
                    if (isValid)
                        LogInUser(tempEmail, password);
                });
        }
        else
            GoogleConnectWithAuthCode(googleCodeLogInInput.text);
    }

    bool emailIsUsername;
    string tempEmail;
    void ValidateLoginDetails(string usernameEmail,
                              Action<bool> isValid)
    {
        emailIsUsername = false;
        tempEmail = usernameEmail;

        if (!FieldsAreFilled(1, 0))
        {
            isValid(false);
            return;
        }
        if (!IsEmailValid(usernameEmail))
        {
            DB.GetAllObjects<User>(DB.DATABASE_USERS_URL + DB.DotJson(false), users =>
            {
                foreach (User user in users)
                    if (user.username == tempEmail)
                    {
                        emailIsUsername = true;
                        tempEmail = user.email;
                        isValid(emailIsUsername);
                        return;
                    }
                if (tempEmail == usernameEmail) // IF INVALID EMAIL IS UNCHANGED
                {
                    logInBtn.ShowErrorMessage(errorMessages[6]); // DETAILS DO NOT MATCH
                    isValid(false);
                    return;
                }
            });
        }
        else
            isValid(true);
    }

    void LogInUser(string email, string password)
    {
        string userData =
            "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient
            .Post<SignResponse>(DB.SIGN_IN_URL, userData)
            .Then(response =>
            {
                string emailVerification =
                "{\"idToken\":\"" + response.idToken + "\"}";
                RestClient
                    .Post(DB.GET_USER_DATA_URL, emailVerification)
                    .Then(emailResponse =>
                    {
                        EmailConfirmationInfo emailConfirmationInfo =
                            Serialiser.DeserialiseDataFromJson<EmailConfirmationInfo>(emailResponse.Text);

                        if (emailConfirmationInfo.users[0].emailVerified)
                        {
                            idToken = response.idToken;
                            localId = response.localId;

                            GetUserByEmail(email, user =>
                            {
                                DB.GetAllObjects<CardAsset.Data>(DB.DATABASE_CARDS_URL + DB.DotJson(idToken), cardDatas =>
                                {
                                    PersistentData.SetData(user, idToken, cardDatas);
                                    SceneLoader.LoadSceneAsync(SceneIndexes.Main_Menu);
                                });
                            });
                        }
                        else
                            logInBtn.ShowErrorMessage(errorMessages[4]); // EMAIL IS NOT VERIFIED
                    });
            })
            .Catch(error =>
            {
                GetEmailProviders(email, providers =>
                {
                    Debug.LogWarning(error);

                    foreach (string provider in providers.allProviders)
                        if (provider == "google.com")
                        {
                            logInBtn.ShowErrorMessage(errorMessages[8]); // TRY SIGNING IN WITH GOOGLE
                            return;
                        }

                    logInBtn.ShowErrorMessage(errorMessages[6]); // DETAILS DO NOT MATCH
                });
            });
    }

    public void GetEmailProviders(string email, Action<EmailProvidersResponse> providersFetchResponse)
    {
        string payload =
            "{\"identifier\":\""+email+"\",\"continueUri\":\""+"http://localhost"+"\"}";
        RestClient
            .Post<EmailProvidersResponse>(DB.CREATE_AUTH_URI_URL, payload)
            .Then(providersFetchResponse);
    }

    [Serializable]
    public class EmailProvidersResponse
    {
        public string[] allProviders;
        //public bool registered;
    }

    public void GetGoogleCodeBtn()
    {
        GoogleAuthHandler.GetAuthCode();
    }

    public void GoogleConnectWithAuthCode(string googleCode)
    {
        GoogleAuthHandler.ExchangeAuthCodeWithIdToken(googleCode, googleIdToken =>
            SignInWithIdToken(googleIdToken, "google.com", googleUser =>
            {
                DB.GetAllObjects<CardAsset.Data>(DB.DATABASE_CARDS_URL + DB.DotJson(GoogleAuthHandler.GoogleUserInfo.idToken), cardDatas =>
                {
                    PersistentData.SetData(googleUser, GoogleAuthHandler.GoogleUserInfo.idToken, cardDatas);
                    SceneLoader.LoadSceneAsync(SceneIndexes.Main_Menu);
                });
            }));
    }

    void SignInWithIdToken(string idToken, string providerId, Action<User> googleUser)
    {
        string payLoad =
            $"{{\"postBody\":\"id_token={idToken}&providerId={providerId}\",\"requestUri\":\"http://localhost\",\"returnIdpCredential\":true,\"returnSecureToken\":true}}";
        RestClient
            .Post<OAuthSignInResponse>(DB.SIGN_IN_OAUTH_URL, payLoad)
            .Then(response =>
            {
                OAuthSignInResponse googleUserInfo = GoogleAuthHandler.GoogleUserInfo = response;
                localId = googleUserInfo.localId;
                DB.GetUserByLocalId(localId, existingUser =>
                {
                    if (existingUser == null)
                        DB.PostUser(googleUserInfo.email, "", localId, googleUserInfo.idToken, postedUser =>
                                    googleUser(postedUser));
                    else
                        googleUser(existingUser);
                }, false);
            })
            .Catch(Debug.LogWarning);
    }

    void GetUserByEmail(string email, Action<User> getUser)
    {
        DB.GetAllObjects<User>(DB.DATABASE_USERS_URL + ".json?auth" + idToken, users =>
        {
            foreach (User user in users)
                if (user.email == email)
                {
                    getLocalId = user.localId;
                    DB.GetUserByLocalId(getLocalId, user =>
                    {
                        if (user != null)
                            getUser(user);
                        else
                            getUser(null);
                    }, false);
                    break;
                }
        });
    }

    [Serializable]
    class SignResponse
    {
        public string idToken;
        public string localId;
    }
}