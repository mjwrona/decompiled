// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CloudConnected.CloudConnectedConnectionService
// Assembly: Microsoft.VisualStudio.Services.CloudConnected.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6AAFC756-39E6-4247-9102-7DC33B035E4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CloudConnected.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Security.Cryptography;
using System.Threading;

namespace Microsoft.VisualStudio.Services.CloudConnected
{
  public class CloudConnectedConnectionService : 
    ICloudConnectedConnectionService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckOnPremisesDeployment();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public VssConnection GetConnection(IVssRequestContext requestContext)
    {
      IRSAKeyManagementService keyManager = requestContext.GetService<IRSAKeyManagementService>();
      requestContext.Elevate();
      VssSigningCredentials signingCredentials = VssSigningCredentials.Create((Func<RSACryptoServiceProvider>) (() => keyManager.GetKey()));
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string str1 = service.GetValue<string>(requestContext, new RegistryQuery("/Configuration/ConnectedServer/Authorization/RegistrationId"), string.Empty);
      string str2 = service.GetValue<string>(requestContext, new RegistryQuery("/Configuration/ConnectedServer/Authorization/Url"), string.Empty);
      string str3 = service.GetValue<string>(requestContext, new RegistryQuery("/Configuration/ConnectedServer/SpsUrl"), string.Empty);
      ArgumentUtility.CheckStringForNullOrEmpty(str1, "clientIdString");
      ArgumentUtility.CheckStringForNullOrEmpty(str2, "authorizationUriString");
      ArgumentUtility.CheckStringForNullOrEmpty(str3, "spsUriString");
      Guid guid = Guid.Parse(str1);
      Uri authorizationUrl = new Uri(str2);
      Uri serverUri = new Uri(str3);
      VssOAuthJwtBearerClientCredential clientCredential = new VssOAuthJwtBearerClientCredential(guid.ToString(), authorizationUrl.ToString(), signingCredentials);
      VssCredentials credentials = new VssCredentials((WindowsCredential) null, (FederatedCredential) new VssOAuthCredential(authorizationUrl, (VssOAuthGrant) VssOAuthGrant.ClientCredentials, (VssOAuthClientCredential) clientCredential), CredentialPromptType.DoNotPrompt);
      VssConnection connection = this.CreateConnection(serverUri, credentials);
      connection.ConnectAsync(new CancellationToken()).SyncResult();
      return connection;
    }

    private VssConnection CreateConnection(Uri serverUri, VssCredentials credentials)
    {
      VssClientHttpRequestSettings settings = VssClientHttpRequestSettings.Default.Clone();
      settings.MaxRetryRequest = 5;
      return new VssConnection(serverUri, credentials, (VssHttpRequestSettings) settings);
    }
  }
}
