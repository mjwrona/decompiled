// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Jwt.JsonWebTokenValidationParameters
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebApi.Jwt
{
  public sealed class JsonWebTokenValidationParameters
  {
    public JsonWebTokenValidationParameters()
    {
      this.ValidateActor = false;
      this.ValidateAudience = true;
      this.ValidateIssuer = true;
      this.ValidateExpiration = true;
      this.ValidateNotBefore = false;
      this.ValidateSignature = true;
      this.ClockSkewInSeconds = 300;
      this.IdentityNameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
    }

    public bool ValidateActor { get; set; }

    public bool ValidateAudience { get; set; }

    public bool ValidateIssuer { get; set; }

    public bool ValidateExpiration { get; set; }

    public bool ValidateNotBefore { get; set; }

    public bool ValidateSignature { get; set; }

    public JsonWebTokenValidationParameters ActorValidationParameters { get; set; }

    public IEnumerable<string> AllowedAudiences { get; set; }

    public int ClockSkewInSeconds { get; set; }

    public VssSigningCredentials SigningCredentials { get; set; }

    public IEnumerable<string> ValidIssuers { get; set; }

    public string IdentityNameClaimType { get; set; }
  }
}
