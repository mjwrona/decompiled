// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.OAuth.VssOAuthJwtBearerAssertion
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Microsoft.VisualStudio.Services.OAuth
{
  public class VssOAuthJwtBearerAssertion
  {
    private List<Claim> additionalClaims;
    private readonly string m_issuer;
    private readonly string m_subject;
    private readonly string m_audience;
    private readonly JsonWebToken m_bearerToken;
    private readonly VssSigningCredentials m_signingCredentials;
    private static readonly TimeSpan BearerTokenLifetime = TimeSpan.FromMinutes(5.0);

    public VssOAuthJwtBearerAssertion(JsonWebToken bearerToken) => this.m_bearerToken = bearerToken;

    public VssOAuthJwtBearerAssertion(
      string issuer,
      string subject,
      string audience,
      VssSigningCredentials signingCredentials)
      : this(issuer, subject, audience, (IList<Claim>) null, signingCredentials)
    {
    }

    public VssOAuthJwtBearerAssertion(
      string issuer,
      string subject,
      string audience,
      IList<Claim> additionalClaims,
      VssSigningCredentials signingCredentials)
    {
      this.m_issuer = issuer;
      this.m_subject = subject;
      this.m_audience = audience;
      this.m_signingCredentials = signingCredentials;
      if (additionalClaims == null)
        return;
      this.additionalClaims = new List<Claim>((IEnumerable<Claim>) additionalClaims);
    }

    public string Issuer => this.m_issuer;

    public string Subject => this.m_subject;

    public string Audience => this.m_audience;

    public IList<Claim> AdditionalClaims
    {
      get
      {
        if (this.additionalClaims == null)
          this.additionalClaims = new List<Claim>();
        return (IList<Claim>) this.additionalClaims;
      }
    }

    public JsonWebToken GetBearerToken()
    {
      if (this.m_bearerToken != null)
        return this.m_bearerToken;
      List<Claim> additionalClaims = new List<Claim>((IEnumerable<Claim>) ((object) this.AdditionalClaims ?? (object) Array.Empty<Claim>()));
      if (!string.IsNullOrEmpty(this.m_subject))
        additionalClaims.Add(new Claim("sub", this.m_subject));
      additionalClaims.Add(new Claim("jti", Guid.NewGuid().ToString()));
      DateTime utcNow = DateTime.UtcNow;
      return JsonWebToken.Create(this.m_issuer, this.m_audience, utcNow, utcNow.Add(VssOAuthJwtBearerAssertion.BearerTokenLifetime), (IEnumerable<Claim>) additionalClaims, this.m_signingCredentials);
    }
  }
}
