import { database, keyfullObjectSnapshotToArray } from './db.js';

const cardCollectionCountDisplayEn = document.querySelector('.CardCollectionCountDisplayEn');

if(cardCollectionCountDisplayEn)
    get(refDB(database, "cards")).then(dataSnapshots =>
        {
            console.log(dataSnapshots.val());
            const cards = keyfullObjectSnapshotToArray(dataSnapshots);
            
            const html = `
                <div class="SimpleBodyText">There are currently <b>${cards.length}</b> different Okainos cards. Stay tuned for more upcoming cards!</div>
                `;
                cardCollectionCountDisplayEn.innerHTML = html;
        });

const cardCollectionCountDisplayFr = document.querySelector('.CardCollectionCountDisplayFr');

if(cardCollectionCountDisplayFr)
    get(DATABASE_CARDS_URL).then(dataSnapshots =>
        {
            console.log(dataSnapshots.val());
            const cards = keyfullObjectSnapshotToArray(dataSnapshots);
            
            const html = `
                <div class="SimpleBodyText">Il y a actuellement <b>${cards.length}</b> cartes dans Okainos. Restez connectez pour plus de cartes à venir!</div>
                `;
                cardCollectionCountDisplayFr.innerHTML = html;
        });