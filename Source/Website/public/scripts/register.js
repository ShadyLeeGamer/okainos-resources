import { auth, database, keyfullObjectSnapshotToArray, User } from './db.js';

const registerForm = document.querySelector('#registerForm'); // FETCH REGISTER FORM BY ID
const registerFormInputsEn = document.querySelector('.RegisterFormInputsEn');
const registerFormInputsFr = document.querySelector('.RegisterFormInputsFr');
export const setupRegisterFormInputs = (step) =>
{
    console.log(step);

    let html = '';
    var emailPassword;
    var username;
    if (registerFormInputsEn)
    {
        emailPassword = `
        <div class="Title">Email</div>
        <input type="email" class="Input" id="emailInput"/>
        <div class="Title">Password</div></center>
        <input type="password" class="Input" id="passwordInput"/>

        <br></br>

        <button class="CollectionButton" id="signIn">Sign In</button>
        `;
        username = `
        <div class="Title">Username</div>
        <input type="username" class="Input" id="usernameInput"/>

        <br></br>

        <button class="CollectionButton" id="signInWithUsername">Sign In</button>
        `;
    }
    else if (registerFormInputsFr)
    {
        emailPassword = `
        <div class="Title">Email</div>
        <input type="email" class="Input" id="emailInput"/>
        <div class="Title">Mot de passe</div>
        <input type="password" class="Input" id="passwordInput"/>

        <br></br>

        <button class="CollectionButton" id="signIn">Sign In</button>
        `;
        username = `
        <div class="Title">Username</div>
        <input type="username" class="Input" id="usernameInput"/>

        <br></br>

        <button class="CollectionButton" id="signInWithUsername">Sign In</button>
        `;
    }

    if (step == 1)
        html += emailPassword;
    else if (step == 2)
        html += username;

    if(registerFormInputsEn)
        registerFormInputsEn.innerHTML = html;
    else if (registerFormInputsFr)
        registerFormInputsFr.innerHTML = html;
}

setupRegisterFormInputs(1);

var email;
var password;
export var registerFormStep = 1;
if (registerForm)
    registerForm.addEventListener('submit', (event) =>
    {
        event.preventDefault(); // PREVENT FORM FROM DOING DEFAULT ACTION (REFRESH PAGE ON CLICKING SIGN UP, ETC.)
        OnRegisterInput();
    });

function OnRegisterInput()
{
    if (registerFormStep == 1) // Sign in mode
    {
        // Get user input
        email = registerForm['emailInput'].value;
        password = registerForm['passwordInput'].value;

        // Check if user already exists in the database, if so sign in, else sign up new account
        get(refDB(database, 'users/')).then(dataSnapshots =>
            {
                const users = keyfullObjectSnapshotToArray(dataSnapshots);
                users.forEach(user =>
                    {
                        if (email == user.email)
                        {
                            if(ValidateSignIn(email, password, users)) // Check if details are valid
                                SignIn(email, password);
                            return;
                        }
                    });

                if (ValidateSignUpEmailPassword(email, password, users)) // Check if details are valid
                    setupRegisterFormInputs(registerFormStep = 2); // Setup register for sign up
            });
    }
    else if (registerFormStep == 2) // Sign up mode
    {
        get(refDB(database, 'users/')).then(dataSnapshots =>
            {
                const users = keyfullObjectSnapshotToArray(dataSnapshots);

                const username = registerForm['usernameInput'].value;

                if (ValidateSignUpUsername(username, users)) // Check if username already exists
                    SignUp(email, password, username);
            });
    }
}

function ShowErrorMessage(element, message)
{
    if (message == "")
        element.html = `<input type="email" class="Input" id="emailInput"/>`;
    else 
        element.insertAdjacentHTML('beforebegin', `<div class="ErrorMessage"><div>${message}</div><br></div>`);
}

function ClearErrorMessages()
{
    const errorMessages = document.querySelectorAll(".ErrorMessage");
    errorMessages.forEach(errorMessage => {
        $(errorMessage).remove();
    });
}

function ValidateSignUpEmailPassword(email, password, users)
{
    var isValid = true;
    const emailInput = registerForm.elements['emailInput'];
    const passwordInput = registerForm.elements['passwordInput'];
    ClearErrorMessages();

    if (email == "")
    {
        ShowErrorMessage(emailInput, "Field is empty");
        isValid = false;
    }

    if (password == "")
    {
        ShowErrorMessage(passwordInput, "Field is empty");
        isValid = false;
    }
    else if (password.length < 6)
    {
        ShowErrorMessage(passwordInput, "Password needs 6 characters");
        isValid = false;
    }
    
    return isValid;
}

function ValidateSignUpUsername(username, users)
{
    var isValid = true;
    const usernameInput = registerForm.elements['usernameInput'];
    ClearErrorMessages();

    if (username == "")
    {
        ShowErrorMessage(usernameInput, "Field is empty");
        isValid = false;
    }

    if (isValid)
    {
        users.forEach(user =>
            {
                if (username == user.username)
                {
                    ShowErrorMessage(usernameInput, "Username already exists");
                    isValid = false;
                }
            });
    }
    return isValid;
}

function ValidateSignIn(email, password, users)
{
    var isValid = true;
    const emailInput = registerForm.elements['emailInput'];
    const passwordInput = registerForm.elements['passwordInput'];
    ClearErrorMessages();

    if (email == "")
    {
        ShowErrorMessage(emailInput, "Field is empty");
        isValid = false;
    }

    if (password == "")
    {
        ShowErrorMessage(passwordInput, "Field is empty");
        isValid = false;
    }
    else if (password.length < 6)
    {
        ShowErrorMessage(passwordInput, "Password needs 6 characters");
        isValid = false;
    }
    
    return isValid;
}

function SignUp(email, password, username)
{
    createUserWithEmailAndPassword(auth, email, password)
        .then(credential =>
            {
                const user = new User(email, username, credential.user.uid);

                set(refDB(database, 'users/' + user.localId), user)
                    .then(() =>
                    {
                        sendEmailVerification(auth.currentUser)
                        .then(() =>
                        {
                            window.location.href = "profile.html";
                        });
                        
                        document.querySelector('#registerForm').reset();
                    });
            });
}

function SignIn(email, password)
{
    signInWithEmailAndPassword(auth, email, password)
        .then(credential =>
            {
                console.log("sign in");

                window.location.href = "profile.html";

                document.querySelector('#registerForm').reset();
            });
}