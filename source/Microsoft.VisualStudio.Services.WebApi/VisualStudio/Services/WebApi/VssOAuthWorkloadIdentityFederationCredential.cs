// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.VssOAuthWorkloadIdentityFederationCredential
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.OAuth;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public sealed class VssOAuthWorkloadIdentityFederationCredential : VssOAuthClientCredential
  {
    private readonly VssOAuthJwtBearerAssertion m_assertion;

    public VssOAuthWorkloadIdentityFederationCredential(
      string clientId,
      VssOAuthJwtBearerAssertion assertion)
      : base(VssOAuthClientCredentialType.JwtBearer, clientId)
    {
      ArgumentUtility.CheckForNull<VssOAuthJwtBearerAssertion>(assertion, nameof (assertion));
      this.m_assertion = assertion;
    }

    protected override void SetParameters(IDictionary<string, string> parameters)
    {
      parameters["client_id"] = this.ClientId;
      parameters["client_assertion_type"] = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
      parameters["client_assertion"] = this.m_assertion.GetBearerToken().EncodedToken;
    }
  }
}
