<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>

<html>
    <head>
        <script src="<%= StaticResources.ThirdParty.Scripts.GetLocation("msal-browser-3.7.1.min.js") %>" <%= Html.GenerateNonce(true) %>></script>
    </head>
    <body>
        <script type="text/javascript" <%= Html.GenerateNonce(true) %>>
            let config = {
                auth: {
                    clientId: "<%= ViewBag.ClientId %>",
                    authority: "<%= ViewBag.Authority %>"
                },
                cache: {
                    cacheLocation: "<%= ViewBag.CacheLocation %>"
                },
                system: {
                    // https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-browser/docs/logout.md
                    allowRedirectInIframe: true,
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
                pca.logoutRedirect({
                    onRedirectNavigate: (url) => {
                        // this will prevent navigation to the identity provider after "local logout"
                        // because we want the normal logout flow to continue and take us to SPS
                        return false;
                    }
                }).catch((e) => {
                    console.log(e);
                }).finally(() => {
                    // we don't necessarily know which SPS instance will be hosting the page
                    // i.e. app.vssps.visualstudio.com vs spsprodwcus0.vssps.visualstudio.com
                    // so we'll verify here that the roots are the same and, if so, use it

                    const targetOrigin = "<%= ViewBag.TargetOriginRoot %>";
                    const referrerHost = new URL(document.referrer).host;
                    if (referrerHost.endsWith(targetOrigin)) {
                        const hash = window.location.hash;
                        let labelIndex = "";
                        if (hash.startsWith("#labelIndex=")) {
                            labelIndex = parseInt(hash.substring(12));
                        }

                        window.parent.postMessage("msalLogoutComplete:" + labelIndex, `https://${referrerHost}`);
                    }
                });
            }).catch((error) => {
                console.log("PublicClientApplication.initialize failed: " + error);
            });
        </script>
    </body>
</html>