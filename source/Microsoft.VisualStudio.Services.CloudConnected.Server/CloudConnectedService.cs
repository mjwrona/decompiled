// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CloudConnected.CloudConnectedService
// Assembly: Microsoft.VisualStudio.Services.CloudConnected.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6AAFC756-39E6-4247-9102-7DC33B035E4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CloudConnected.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Microsoft.VisualStudio.Services.CloudConnected
{
  public class CloudConnectedService : ICloudConnectedService, IVssFrameworkService
  {
    private static readonly RegistryQuery s_accountQuery = new RegistryQuery(CloudConnectedConstants.ConnectedServerAccount);
    private static readonly RegistryQuery s_accountIdQuery = new RegistryQuery(CloudConnectedConstants.ConnectedServerAccountId);
    private static readonly RegistryQuery s_registrationIdQuery = new RegistryQuery(CloudConnectedConstants.ConnectedServerRegistrationId);

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.IsProduction)
        return;
      systemRequestContext.CheckOnPremisesDeployment();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ConfigureServer(
      IVssRequestContext requestContext,
      string accountName,
      Guid accountId,
      string spsUrl,
      string authorizationUrl)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(accountName, nameof (accountName));
      ArgumentUtility.CheckStringForNullOrEmpty(authorizationUrl, nameof (authorizationUrl));
      ArgumentUtility.CheckStringForNullOrEmpty(spsUrl, nameof (spsUrl));
      ArgumentUtility.CheckForEmptyGuid(accountId, nameof (accountId));
      Uri uri1 = new Uri(authorizationUrl);
      Uri uri2 = new Uri(spsUrl);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      service.SetValue<string>(requestContext, CloudConnectedConstants.ConnectedServerAuthorizationUrl, uri1.AbsoluteUri);
      service.SetValue<string>(requestContext, CloudConnectedConstants.ConnectedServerAccount, accountName);
      service.SetValue<string>(requestContext, CloudConnectedConstants.ConnectedServerAccountId, accountId.ToString("D"));
      service.SetValue<string>(requestContext, CloudConnectedConstants.ConnectedServerSpsUrl, uri2.AbsoluteUri);
    }

    public string GetConnectedAccountName(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue(requestContext, in CloudConnectedService.s_accountQuery, string.Empty);

    public Guid GetConnectedAccountId(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<Guid>(requestContext, in CloudConnectedService.s_accountIdQuery, Guid.Empty);

    internal virtual ConnectedServerPublicKey GetServerPublicKey(IVssRequestContext requestContext)
    {
      RSAParameters rsaParameters;
      using (RSACryptoServiceProvider key = requestContext.Elevate().GetService<IRSAKeyManagementService>().GetKey())
        rsaParameters = key.ExportParameters(false);
      return new ConnectedServerPublicKey(rsaParameters.Exponent, rsaParameters.Modulus);
    }

    public virtual string GetRegistrationKey(IVssRequestContext requestContext)
    {
      ConnectedServerPublicKey serverPublicKey = this.GetServerPublicKey(requestContext.Elevate());
      return CloudConnectedUtilities.EncodeToken(new Dictionary<string, string>()
      {
        {
          CloudConnectedServerShortNameConstants.Exponent,
          Convert.ToBase64String(serverPublicKey.Exponent)
        },
        {
          CloudConnectedServerShortNameConstants.Modulus,
          Convert.ToBase64String(serverPublicKey.Modulus)
        }
      });
    }

    public Guid GetRegistrationId(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      Guid registrationId = service.GetValue<Guid>(requestContext, in CloudConnectedService.s_registrationIdQuery, Guid.Empty);
      if (registrationId.Equals(Guid.Empty))
      {
        registrationId = Guid.NewGuid();
        service.SetValue<string>(requestContext, CloudConnectedConstants.ConnectedServerRegistrationId, registrationId.ToString("D"));
      }
      return registrationId;
    }
  }
}
