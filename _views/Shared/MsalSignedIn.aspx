<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ Import Namespace="Newtonsoft.Json" %>

<html>
    <head>
        <script src="<%= StaticResources.ThirdParty.Scripts.GetLocation("msal-browser-3.7.1.min.js") %>"></script>
    </head>
    <body>
        <noscript>
            <span class="error"><%: PlatformResources.NoScriptMessage %></span>
        </noscript>
        <script type="text/javascript">
            let config = {
                auth: {
                    clientId: "<%= ViewBag.ClientId %>",
                    authority: "<%= ViewBag.Authority %>"
                },
                cache: {
                    cacheLocation: "<%= ViewBag.CacheLocation %>"
                },
                system: {
                    loggerOptions: {
                        logLevel: <%= ViewBag.LogLevel %>,
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
                pca.acquireTokenByCode({
                    code: "<%= ViewBag.Code %>",
                    scopes: <%= JsonConvert.SerializeObject(ViewBag.Scopes) %>,
                    correlationId: "<%= ViewBag.ActivityId %>"
                }).then((authenticationResult) => {
                    pca.setActiveAccount(authenticationResult.account);
                    window.location.href = "<%= ViewBag.Location %>";
                });
            }).catch((error) => {
                console.log("PublicClientApplication.initialize failed: " + error);
            });
        </script>
    </body>
</html>