// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.JwtClaims
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  public class JwtClaims
  {
    private Dictionary<string, string> m_claims = new Dictionary<string, string>();

    public JwtClaims()
    {
    }

    public JwtClaims(JwtClaims jwtClaims)
    {
      this.Issuer = jwtClaims.Issuer;
      this.Subject = jwtClaims.Subject;
      this.Audience = jwtClaims.Audience;
      this.NotBefore = jwtClaims.NotBefore;
      this.IssuedTime = jwtClaims.IssuedTime;
      this.Identifier = jwtClaims.Identifier;
      this.Expiration = jwtClaims.Expiration;
      this.m_claims = jwtClaims.ExtraClaims;
    }

    public string Issuer { get; set; }

    public string Subject { get; set; }

    public string Audience { get; set; }

    public DateTime? NotBefore { get; set; }

    public DateTime? IssuedTime { get; set; }

    public string Identifier { get; set; }

    public DateTime? Expiration { get; set; }

    public Dictionary<string, string> ExtraClaims
    {
      get => this.m_claims;
      set => this.m_claims = value;
    }
  }
}
