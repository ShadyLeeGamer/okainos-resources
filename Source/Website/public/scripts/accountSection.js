import { auth, storage, database } from "./db.js";

onAuthStateChanged(auth, user =>
    {
        setupHeaderAccountSection(user);
        if (user)
            console.log("auth");
        else
            console.log("unauth");
    });

const accountSectionEn = document.querySelector('.AccountSectionEn');
const accountSectionFr = document.querySelector('.AccountSectionFr');
export const setupHeaderAccountSection = (authUser) =>
{
    let html = '';
    if (authUser)
    {
        get(refDB(database, 'users/' + authUser.uid)).then(dataSnapshot =>
        {
            const user = dataSnapshot.val();
            
            if (accountSectionEn)
            {
                html = `
                    <td><div class="connection" id="profileBtn"> <a href="profile.html"><div class="BaseTitle" style="float:left; margin-top:-23px; margin-right: 120px"> ${user.username}<img class="socialIcon" src="imgs/AccountIcon.png" style="float:left; margin-right:-10px;"></img> </div></a></div></td>
                    <td><div class="connection" id="logoutBtn"> <a href="/"><div class="BaseTitle" style="float:right; margin-left:-180px;"> Logout <img class="socialIcon" src="imgs/AccountIcon.png" style="float:left; margin-right:-10px;"></img></div></a></div></td>
                    `;
                    accountSectionEn.innerHTML = html;
            }
            else if(accountSectionFr)
            {
                html = `
                <td><div class="connection" id="profileBtn"> <a href="profile.html"><div class="BaseTitle" style="float:left; margin-top:-23px; margin-right: 170px"> ${user.username}<img class="socialIcon" src="../imgs/AccountIcon.png" style="float:left; margin-right:0px;"></img> </div></a></div></td>
                <td><div class="connection" id="logoutBtn"> <a href="../fr/"><div class="BaseTitle" style="float:right; margin-left:-180px;"> Déconnexion  <img class="socialIcon" src="../imgs/AccountIcon.png" style="float:left; margin-right:-10px;"></img></div></a></div></td>
                `;
                accountSectionFr.innerHTML = html;
            }


        RefreshLogoutButton();
        }).catch(error => console.log(error.message));
    }
    else
    {
        if(accountSectionEn)
        {
            html = `
                <td><div class="connection" id="connectBtn"> <a href="register.html"><div class="BaseTitle" style="float:right; margin-left:-180px;"> Connect <img class="socialIcon" src="imgs/AccountIcon.png" style="float:left; margin-right:-10px;"></img></div></a></div></td>
                `;
            accountSectionEn.innerHTML = html;
        }
        else if (accountSectionFr)
            {
                html = `
                <td><div class="connection" id="connectBtn"> <a href="register.html"><div class="BaseTitle" style="float:right; margin-left:-180px;"> Connexion <img class="socialIcon" src="../imgs/AccountIcon.png" style="float:left; margin-right:-10px;"></img></div></a></div></td>
                `;
                accountSectionFr.innerHTML = html;
            }

        RefreshLogoutButton();
    }
}

function RefreshLogoutButton()
{
    const logoutBtn = document.querySelector('#logoutBtn');
    if(logoutBtn)
        logoutBtn.addEventListener('click', (event) =>
        {
            signOut(auth);
        });
}