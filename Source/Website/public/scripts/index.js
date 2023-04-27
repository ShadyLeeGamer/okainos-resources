import { auth } from "./db.js";

onAuthStateChanged(auth, currentUser =>
    {
        if (currentUser)
            $("#createAnAccountForClosedBeta").remove();
    });