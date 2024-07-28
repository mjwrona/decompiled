// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureOidcFederationAadTokenProvider
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.Identity.Client;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  internal class AzureOidcFederationAadTokenProvider : IAzureOidcFederationAadTokenProvider
  {
    private readonly string _authorityUrl;
    private readonly bool _validateAuthority;

    public AzureOidcFederationAadTokenProvider(string authorityUrl, bool validateAuthority)
    {
      ArgumentUtility.CheckForNull<string>(authorityUrl, nameof (authorityUrl));
      this._authorityUrl = authorityUrl;
      this._validateAuthority = validateAuthority;
    }

    public string IssueAadAccessToken(
      IVssRequestContext requestContext,
      string resourceUrl,
      string servicePrincipalId,
      string oidcToken)
    {
      Guid correlationId = Guid.NewGuid();
      return ConfidentialClientApplicationBuilder.Create(servicePrincipalId).WithAuthority(this._authorityUrl, this._validateAuthority).WithClientAssertion(oidcToken).Build().AcquireTokenForClient((IEnumerable<string>) new string[1]
      {
        resourceUrl + "/.default"
      }).WithCorrelationId(correlationId).ExecuteAsync().ConfigureAwait(false).GetAwaiter().GetResult().AccessToken;
    }
  }
}
