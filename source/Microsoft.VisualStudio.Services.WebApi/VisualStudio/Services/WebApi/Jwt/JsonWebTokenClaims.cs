// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Jwt.JsonWebTokenClaims
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

namespace Microsoft.VisualStudio.Services.WebApi.Jwt
{
  public static class JsonWebTokenClaims
  {
    public const string ActorToken = "actort";
    public const string Audience = "aud";
    public const string IssuedAt = "iat";
    public const string Issuer = "iss";
    public const string NameId = "nameid";
    public const string IdentityProvider = "identityprovider";
    public const string ValidTo = "exp";
    public const string ValidFrom = "nbf";
    public const string Scopes = "scp";
    public const string Roles = "roles";
    public const string RefreshToken = "ret";
    public const string Source = "src";
    public const string Subject = "sub";
    public const string TrustedForDelegation = "trustedfordelegation";
    public const string NameIdLongName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
    public const string IdentityProviderLongName = "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider";
    public const string TenantId = "tid";
    public const string TenantIdLongName = "http://schemas.microsoft.com/identity/claims/tenantid";
    public const string TokenId = "jti";
    public const string AppId = "appid";
    public const string JobRef = "jobref";
  }
}
