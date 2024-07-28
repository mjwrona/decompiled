// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tokens.DelegatedAuthResourceIds
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Tokens
{
  public static class DelegatedAuthResourceIds
  {
    public const string AreaName = "DelegatedAuth";
    public const string AreaId = "A0848FA1-3593-4AEC-949C-694C73F4C4CE";
    public const string AuthorizationResource = "Authorizations";
    public static readonly Guid Authorization = new Guid("EFBF6E0C-1150-43FD-B869-7E2B04FC0D09");
    public const string HostAuthorizationResource = "HostAuthorization";
    public static readonly Guid HostAuthorizeId = new Guid("{7372FDD9-238C-467C-B0F2-995F4BFE0D94}");
    public const string RegistrationResource = "Registration";
    public static readonly Guid Registration = new Guid("{909CD090-3005-480D-A1B4-220B76CB0AFE}");
    public const string RegistrationSecretResource = "RegistrationSecret";
    public static readonly Guid RegistrationSecret = new Guid("{F37E5023-DFBE-490E-9E40-7B7FB6B67887}");
  }
}
