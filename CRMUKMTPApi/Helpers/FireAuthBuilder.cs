using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace CRMUKMTPApi.Helpers;

public sealed class FireAuthBuilder
{
    static FirebaseAuth defaultAuth = null;
    private static void InitFireSingle()
    {
        var value = System.Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp defaultApp = FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.GetApplicationDefault(),
            });
            defaultAuth = FirebaseAuth.GetAuth(defaultApp);
        }
    }



    private static readonly object _lock = new object();
    private static FireAuthBuilder instance = null;
    public static FirebaseAuth Instance
    {
        get
        {
            if (defaultAuth == null)
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        InitFireSingle();
                    }
                }
            }
            return defaultAuth;
        }
    }
}
