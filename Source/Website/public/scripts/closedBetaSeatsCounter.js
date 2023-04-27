import { auth, storage } from './db.js';

onAuthStateChanged(auth, currentUser =>
    {
        if (!currentUser)
        {
            const closedBetaSeatsCounterEn = document.querySelector('.ClosedBetaSeatsCounterEn');
            const closedBetaSeatsCounterFr = document.querySelector('.ClosedBetaSeatsCounterFr');
            getDownloadURL(refGS(storage, 'closed-beta-tester-keys.txt')).then((url) =>
            {
                fetch(url).then(keysResponse =>
                    {
                        keysResponse.text().then(keysText =>
                            {
                                const keyCount = keysText.split('\n').length;
                                if (closedBetaSeatsCounterEn)
                                    closedBetaSeatsCounterEn.innerHTML = `<div class="TribesText">${keyCount} seats available!</div>`;
                                else if (closedBetaSeatsCounterFr)
                                    closedBetaSeatsCounterFr.innerHTML = `<div class="TribesText">Il ne reste plus que ${keyCount} places disponibles!</div>`;
                            });
                    });
            });
        }
    });