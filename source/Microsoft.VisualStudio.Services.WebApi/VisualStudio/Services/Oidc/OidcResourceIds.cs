// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Oidc.OidcResourceIds
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Oidc
{
  public static class OidcResourceIds
  {
    public const string AreaName = "TokenOidc";
    public const string AreaId = "b5da59b3-d611-433b-a121-4cdc4fda898a";
    public const string OrganizationAreaName = "OrganizationTokenOidc";
    public const string OrganizationAreaId = "d65b01c1-a9d1-40dc-b28a-1b04d47629bb";
    public const string ConfigurationResource = "openid-configuration";
    public static readonly Guid Configuration = new Guid("{4afe55bc-d4a5-4d78-8fbd-5eea4b84c28e}");
    public static readonly Guid OrganizationConfiguration = new Guid("{61c20c8a-8483-4d14-bdcb-35ee0d50a754}");
    public const string JwksResource = "jwks";
    public static readonly Guid Jwks = new Guid("{869a990d-6d7e-408f-9324-9d3c0b3da06b}");
  }
}
