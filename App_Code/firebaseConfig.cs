using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for firebaseConfig
/// </summary>
public class firebaseConfig
{
    public firebaseConfig()
    {
        //
        // TODO: Add constructor logic here
        //
        initializeDb();
    }

    private void initializeDb()
    {
        var credentials = GoogleCredential.FromFile("");
        var app = FirebaseApp.Create(new AppOptions()
        {
            Credential = credentials
        });

        var auth = FirebaseAuth.GetAuth(app);
 
    }

}