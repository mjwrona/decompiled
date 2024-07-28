// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TokenAdmin.Client.TokenAdminResourceIds
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.TokenAdmin.Client
{
  public static class TokenAdminResourceIds
  {
    public const string AreaName = "TokenAdmin";
    public const string AreaId = "af68438b-ed04-4407-9eb6-f1dbae3f922e";
    public const string PersonalAccessTokensResource = "PersonalAccessTokens";
    public static readonly Guid PersonalAccessTokensLocationId = new Guid("{af68438b-ed04-4407-9eb6-f1dbae3f922e}");
    public const string RevocationsResource = "Revocations";
    public static readonly Guid RevocationsLocationId = new Guid("{a9c08b2c-5466-4e22-8626-1ff304ffdf0f}");
    public const string RevocationRulesResource = "RevocationRules";
    public static readonly Guid RevocationRulesLocationId = new Guid("{ee4afb16-e7ab-4ed8-9d4b-4ef3e78f97e4}");
    public const string TokenGetPersonalAccessToken = "TokenGetPersonalAccessToken";
    public static readonly Guid TokenGetPersonalAccessTokenId = new Guid("{11E3D37F-FA7E-4721-AB2D-2D931BD944C4}");
    public const string TokenRevokePersonalAccessToken = "TokenRevokePersonalAccessToken";
    public static readonly Guid TokenRevokePersonalAccessTokenId = new Guid("{55687C95-C811-41E7-889F-25AFB03EDA19}");
  }
}
