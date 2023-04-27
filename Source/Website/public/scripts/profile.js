import { database, auth } from "./db.js";

var user;

onAuthStateChanged(auth, currentUser =>
    {
        if (currentUser)
        {
            get(refDB(database, "users/" + auth.currentUser.uid))
                .then((snapshot) =>
                {
                    user = snapshot.val();

                    RefreshDisplays();
                    if (currentUser.emailVerified)
                    {
                        GetClosedBetaTesterKey();
                        
                    }
                });
        }
        else
            window.location.href = "/";
    });

function RefreshDisplays()
{
    var verificationMessageHtml = '';
    var verified, unverified;
    const VerificationMessageDisplayEn = document.querySelector('.VerificationMessageDisplayEn');
    const VerificationMessageDisplayFr = document.querySelector('.VerificationMessageDisplayFr');
    if (VerificationMessageDisplayEn)
    {
        verified = `<div class="Title">Your account is verified !</div>
                    `;
        unverified =
            `<div class="Title">Your account needs to be verified for Okainos Closed Beta.</div>
             <div class="SimpleBodyText">Please confirm the verification email sent to ${user.email} to have access to Okainos Closed Beta and features.</div>
            `
    }
    else if (VerificationMessageDisplayFr)
    {
        verified = `<div class="Title">Votre compte est maintenant actif !</div>`;
        unverified =
            `<div class="Title">Votre compte doit être vérifié pour la bêta fermée d'Okainos.</div>
            <div class="SimpleBodyText">Veuillez confirmer l'e-mail de vérification envoyé à ${user.email}  pour avoir accès à la bêta fermée et aux fonctionnalités d'Okainos.</div>
            `
    }
    const emailVerified = auth.currentUser.emailVerified;
    if (emailVerified)
        verificationMessageHtml = verified;
    else
        verificationMessageHtml = unverified;

    if (VerificationMessageDisplayEn)
        SetDisplayHtml(VerificationMessageDisplayEn,
                       verificationMessageHtml);
    else if (VerificationMessageDisplayFr)
        SetDisplayHtml(VerificationMessageDisplayFr,
                       verificationMessageHtml);
    if(emailVerified)
    {
        const UsernameDisplayEn = document.querySelector('.UsernameDisplayEn');
        const UsernameDisplayFr = document.querySelector('.UsernameDisplayFr');
        if (UsernameDisplayEn)
            SetDisplayHtml(UsernameDisplayEn,
                           `<div class="SimpleBodyText">Username: ${user.username}</div>
                            <br>
                            <div class="SimpleBodyText">Profile features coming soon!</div>
                            `);
        else if (UsernameDisplayFr)
            SetDisplayHtml(UsernameDisplayFr,
                           `<div class="SimpleBodyText">Nom utilisateur: ${user.username}</div>
                            <br>
                            <div class="SimpleBodyText">Les fonctionnalités du profil vont arriver bientôt!</div>
                            `);
    }
}

function GetClosedBetaTesterKey()
{
    const userClosedBetaTesterItchIoKeyURLRef = refDB(database, "users/" + auth.currentUser.uid + "/closedBetaTesterItchIoKeyURL");
    get(userClosedBetaTesterItchIoKeyURLRef)
        .then(userClosedBetaTesterKeyResponse =>
            {
                var urlKey = userClosedBetaTesterKeyResponse.val();
                // Check if user already has a key, if so then assign them one, else refresh the game page button with the key url
                if (urlKey == "")
                {
                    // Add user to mail list
                    SubscribeEmailToList(user.email);

                    // Get keys list from Firebase Cloud Storage
                    const closedBetaTesterKeysRef = refGS(storage, 'closed-beta-tester-keys.txt');
                    getDownloadURL(closedBetaTesterKeysRef).then((url) =>
                        {
                            fetch(url).then(keysResponse =>
                                {
                                    keysResponse.text().then(keysText =>
                                        {
                                            // Grab first key
                                            var nextKey = keysText.split('\n')[0];
                                            // Exclude the key from the list
                                            keysText = keysText.substring(nextKey.length + 1)
                                            uploadString(closedBetaTesterKeysRef, keysText);
                                            // Assign player with key
                                            set(userClosedBetaTesterItchIoKeyURLRef, nextKey)
                                                .then((response) =>
                                                {
                                                    urlKey = nextKey
                                                    RefreshSendClosedBetaTesterKeyButton(urlKey);
                                                });
                                        });
                                });
                        });
                }
                else
                    RefreshSendClosedBetaTesterKeyButton(urlKey);
            });
}

function RefreshSendClosedBetaTesterKeyButton(urlKey)
{
    const ClosedBetaTesterContentEn = document.querySelector('.ClosedBetaTesterContentEn');
    const ClosedBetaTesterContentFr = document.querySelector('.ClosedBetaTesterContentFr');
    if (ClosedBetaTesterContentEn != null)
        SetDisplayHtml(ClosedBetaTesterContentEn, 
                        `<div class="SimpleBodyText">Click the button below to be taken to the Closed Beta download page.</div>
                        <br>
                        <center><button class="CollectionButton" id="sendClosedBetaTesterKey"><a href=${urlKey}>OKAINOS CLOSED BETA</button></a></center>
                        <br>
                        `);
    else if (ClosedBetaTesterContentFr != null)
        SetDisplayHtml(ClosedBetaTesterContentFr, 
            `<div class="SimpleBodyText">Cliquez sur le bouton ci-dessous pour accéder à la page de téléchargement de la bêta fermée.</div>
            <br>
            <center><button class="CollectionButton" id="sendClosedBetaTesterKey"><a href=${urlKey}>OKAINOS CLOSED BETA</button></a></center>
            <br>
            `);
}

function SetDisplayHtml(document, innerHtml)
{
    document.innerHTML = innerHtml;
}

function SubscribeEmailToList(email)
{
    get(refDB(database, 'mailingList')).then(dataSnapshots =>
        {
            var mailingList;
            if(dataSnapshots.val() != null)
            {
                mailingList = dataSnapshots.val();
                mailingList.push(email);
            }
            else
                mailingList = [ email ];

            set(refDB(database, 'mailingList/'), mailingList)
                .then(() => { console.log("subscribed " + email + "!!") })
                .catch((error) => console.log(error.message));
        });
}