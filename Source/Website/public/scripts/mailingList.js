import { database } from './db.js';

const mailingListForm = document.querySelector('#mailingListForm'); // FETCH REGISTER FORM BY ID
if (mailingListForm)
    mailingListForm.addEventListener('submit', (event) =>
    {
        event.preventDefault(); // PREVENT FORM FROM DOING DEFAULT ACTION (REFRESH PAGE ON CLICKING SIGN UP, ETC.)

        const email = mailingListForm['emailInput'].value;

        SubscribeEmailToList(email);
    });

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