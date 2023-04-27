// Import the functions you need from the SDKs you need
import { initializeApp } from "https://www.gstatic.com/firebasejs/9.0.2/firebase-app.js";
import { getAuth, createUserWithEmailAndPassword, signInWithEmailAndPassword, signOut, onAuthStateChanged, sendEmailVerification } from "https://www.gstatic.com/firebasejs/9.0.2/firebase-auth.js";
import { getDatabase, ref as refDB, set, get } from "https://www.gstatic.com/firebasejs/9.0.2/firebase-database.js";
import { getStorage, ref as refGS , getDownloadURL, uploadBytes, uploadString } from "https://www.gstatic.com/firebasejs/9.0.2/firebase-storage.js";

window.createUserWithEmailAndPassword = createUserWithEmailAndPassword;
window.signInWithEmailAndPassword = signInWithEmailAndPassword;
window.signOut = signOut;
window.onAuthStateChanged = onAuthStateChanged;
window.sendEmailVerification = sendEmailVerification;

window.getDatabase = getDatabase;
window.refDB = refDB;
window.set = set;
window.get = get;

window.refGS = refGS;
window.getDownloadURL = getDownloadURL;
window.uploadBytes = uploadBytes;
window.uploadString = uploadString;

// TODO: Add SDKs for Firebase products that you want to use
// https://firebase.google.com/docs/web/setup#available-libraries

// Your web app's Firebase configuration
// For Firebase JS SDK v7.20.0 and later, measurementId is optional
const firebaseConfig = window.firebaseConfig = {
    apiKey: "API KEY",
    authDomain: "AUTH DOMAIN",
    databaseURL: "DATABASE URL",
    projectId: "PROJECT ID",
    appId: "APP ID",
    measurementId: "MEASUREMENT ID",
    storageBucket: "STORAGE BUCKET"
};

// Initialize Firebase
export const app = window.app = initializeApp(firebaseConfig);

export const auth = window.auth = getAuth(app);
export const database = window.database = getDatabase(app);
export const storage = window.storage = getStorage(app);

export const DATABASE_USER_URL = refDB(database, 'users/');
export const DATABASE_CARDS_URL = refDB(database, 'cards/');

export const keyfullObjectSnapshotToArray = (snapshot) =>
{
    var array = [];
    snapshot.forEach(function(childSnapshot) {
        var item = childSnapshot.val();
        item.key = childSnapshot.key;
    
        array.push(item);
    });
    return array;
}

export class CardInGroup
{
    constructor(id, quantity)
    {
        this.id = id;
        this.quantity = quantity;
    }
}

export class User
{
    constructor(email, username, localId)
    {
        this.email = email;
        this.username = username;
        this.localId = localId;

        this.pearls = 175;
        this.salt = this.boosterChests = 0;
        this.cardCollection = [ /*new CardInGroup("Average Koi", 1)*/ ];
        this.cardDecks = this.pearlDealsClaimed = [];
        this.isWelcomed = this.boughtUsACoffee = this.isAPatron = false;
        this.closedBetaTesterItchIoKeyURL = "";
    }
}