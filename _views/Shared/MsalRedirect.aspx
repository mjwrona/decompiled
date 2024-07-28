<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>

<html>
    <head>
        <script type="text/javascript" src="<%= StaticResources.ThirdParty.Scripts.GetLocation("msal-browser-3.7.1.min.js") %>"></script>
    </head>
    <body>
        <script type="text/javascript">
            let config = {
                auth: {
                    clientId: "<%= ViewBag.ClientId %>",
                },
                system: {
                    logLevel: <%= ViewBag.LogLevel %>,
                    loggerOptions: {
                        loggerCallback: (level, message, containsPii) => {
                            if (containsPii) {
                                return;
                            }
                            switch (level) {
                                case msal.LogLevel.Error:
                                    console.error(message);
                                    return;
                                case msal.LogLevel.Info:
                                    console.info(message);
                                    return;
                                case msal.LogLevel.Verbose:
                                    console.debug(message);
                                    return;
                                case msal.LogLevel.Warning:
                                    console.warn(message);
                                    return;
                            }
                        }
                    }
                }
            }

            let pca = new msal.PublicClientApplication(config);
            pca.initialize().then(() => {
                pca.handleRedirectPromise()
                .catch((error) => {
                    console.log(error)
                });
            }).catch((error) => {
                console.log("PublicClientApplication.initialize failed: " + error);
            });
        </script>
    </body>
</html>