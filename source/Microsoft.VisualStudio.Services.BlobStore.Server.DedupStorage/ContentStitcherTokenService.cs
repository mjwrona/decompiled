// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ContentStitcher.ContentStitcherTokenService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.DedupStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9521FAE3-5DB1-49D0-98DB-6A544E3AB730
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.DedupStorage.dll

using Microsoft.Identity.Client;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.ContentStitcher
{
  public class ContentStitcherTokenService : IContentStitcherTokenService, IVssFrameworkService
  {
    private IConfidentialClientApplication _confidentialClientApplication;
    private string[] _contentStitcherAuthScopes;

    public async Task<AuthenticationResult> GetAccessTokenAsync(IVssRequestContext requestContext)
    {
      AsyncRetryPolicy asyncRetryPolicy = AsyncRetrySyntax.WaitAndRetryAsync(Policy.Handle<MsalServiceException>((Func<MsalServiceException, bool>) (ex => ex.ErrorCode == "temporarily_unavailable")), 3, (Func<int, TimeSpan>) (retryAttempt => TimeSpan.FromSeconds(Math.Pow(5.0, (double) retryAttempt))), (Action<Exception, TimeSpan, int, Context>) ((exception, timeSpan, retryCount, context) => this.TraceOnFailureToAcquireToken(retryCount, nameof (GetAccessTokenAsync), exception, requestContext)));
      requestContext.TraceAlways(5708101, TraceLevel.Info, "ContentStitcher", nameof (GetAccessTokenAsync), "Starting to acquire the access token with retry policy...");
      AuthenticationResult authenticationResult = (AuthenticationResult) null;
      Func<Task> func = (Func<Task>) (async () => authenticationResult = await this._confidentialClientApplication.AcquireTokenForClient((IEnumerable<string>) this._contentStitcherAuthScopes).ExecuteAsync(requestContext.CancellationToken).ConfigureAwait(true));
      PolicyResult policyResult = await ((AsyncPolicy) asyncRetryPolicy).ExecuteAndCaptureAsync(func).ConfigureAwait(true);
      if (policyResult.Outcome == null)
        requestContext.TraceAlways(5708101, TraceLevel.Info, "ContentStitcher", nameof (GetAccessTokenAsync), "Successfully retrieved \"" + authenticationResult.TokenType + "\" access token with scope: " + authenticationResult.Scopes.FirstOrDefault<string>() + ". " + string.Format("CorrelationId: {0}.", (object) authenticationResult.CorrelationId));
      else
        this.TraceOnFailureToAcquireToken(-1, nameof (GetAccessTokenAsync), policyResult.FinalException, requestContext);
      return authenticationResult;
    }

    public void ServiceStart(IVssRequestContext deploymentRequestContext)
    {
      deploymentRequestContext.CheckDeploymentRequestContext();
      this.InitializeConfidentialClientApplication(deploymentRequestContext);
      this._contentStitcherAuthScopes = new string[1]
      {
        ContentStitcherServiceConstants.GetAppIdWithScope(deploymentRequestContext.ExecutionEnvironment.IsCloudDeployment && !deploymentRequestContext.ExecutionEnvironment.IsDevFabricDeployment)
      };
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private void InitializeConfidentialClientApplication(IVssRequestContext requestContext)
    {
      IS2SAuthSettings s2SauthSettings = requestContext.GetService<IOAuth2SettingsService>().GetS2SAuthSettings(requestContext);
      requestContext.TraceAlways(5708101, TraceLevel.Info, "ContentStitcher", nameof (InitializeConfidentialClientApplication), "Starting MSAL client app with login authority: https://login.windows.net/ and tenant: " + s2SauthSettings.TenantDomain + ".");
      X509Certificate2 certificate = s2SauthSettings.GetSigningCertificate(requestContext) ?? throw new ArgumentNullException("Unable to retrieve the signing certificate.");
      this._confidentialClientApplication = ConfidentialClientApplicationBuilder.Create(s2SauthSettings.PrimaryServicePrincipal.ToString("D")).WithAuthority("https://login.windows.net/" + s2SauthSettings.TenantDomain).WithCertificate(certificate).Build();
      requestContext.TraceAlways(5708101, TraceLevel.Info, "ContentStitcher", nameof (InitializeConfidentialClientApplication), "Successfully built MSAL client app with login authority: https://login.windows.net/ and tenant: " + s2SauthSettings.TenantDomain + ".");
    }

    private void TraceOnFailureToAcquireToken(
      int retryCount,
      string callSite,
      Exception ex,
      IVssRequestContext requestContext)
    {
      requestContext.TraceAlways(5708101, TraceLevel.Error, "ContentStitcher", callSite, string.Format("Failed to acquire token. Retry #: {0} Exception: {1}", (object) retryCount, (object) ex.Message));
    }
  }
}
