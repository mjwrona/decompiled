// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tokens.TokenAuthResourceIds
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Tokens
{
  public static class TokenAuthResourceIds
  {
    public const string AreaName = "TokenAuth";
    public const string AreaId = "c5a2d98b-985c-432e-825e-3c6971edae87";
    public const string AuthorizationResource = "Authorizations";
    public static readonly Guid Authorization = new Guid("7d7ddc0d-60bd-4978-a0b5-295cb099a400");
    public const string HostAuthorizationResource = "HostAuthorization";
    public static readonly Guid HostAuthorizeId = new Guid("{817d2b46-1507-4efe-be2b-adccf17ffd3b}");
    public const string RegistrationResource = "Registration";
    public static readonly Guid Registration = new Guid("{522ad1a0-389d-4c6f-90da-b145fd2d3ad8}");
    public const string RegistrationSecretResource = "RegistrationSecret";
    public static readonly Guid RegistrationSecret = new Guid("{74896548-9cdd-4315-8aeb-9ecd88fceb21}");
  }
}
