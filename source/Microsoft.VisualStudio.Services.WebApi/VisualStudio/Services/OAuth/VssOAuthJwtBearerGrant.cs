// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.OAuth.VssOAuthJwtBearerGrant
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.OAuth
{
  public sealed class VssOAuthJwtBearerGrant : VssOAuthGrant
  {
    private readonly VssOAuthJwtBearerAssertion m_assertion;

    public VssOAuthJwtBearerGrant(
      string issuer,
      string subject,
      string audience,
      VssSigningCredentials signingCredentials)
      : this(new VssOAuthJwtBearerAssertion(issuer, subject, audience, signingCredentials))
    {
    }

    public VssOAuthJwtBearerGrant(VssOAuthJwtBearerAssertion assertion)
      : base(VssOAuthGrantType.JwtBearer)
    {
      ArgumentUtility.CheckForNull<VssOAuthJwtBearerAssertion>(assertion, nameof (assertion));
      this.m_assertion = assertion;
    }

    protected override void SetParameters(IDictionary<string, string> parameters)
    {
      parameters["grant_type"] = "urn:ietf:params:oauth:grant-type:jwt-bearer";
      parameters["assertion"] = this.m_assertion.GetBearerToken().EncodedToken;
    }
  }
}
