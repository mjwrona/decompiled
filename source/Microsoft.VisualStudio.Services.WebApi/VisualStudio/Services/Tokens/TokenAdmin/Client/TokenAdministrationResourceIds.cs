// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tokens.TokenAdmin.Client.TokenAdministrationResourceIds
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Tokens.TokenAdmin.Client
{
  public static class TokenAdministrationResourceIds
  {
    public const string TokenAreaName = "TokenAdministration";
    public const string TokenAreaId = "95935461-9E54-44BD-B9FB-04F4DD05D640";
    public const string TokenPersonalAccessTokensResource = "TokenPersonalAccessTokens";
    public static readonly Guid TokenPersonalAccessTokensLocationId = new Guid("{1BB7DB14-87C5-4762-BF77-A70AD34A9AB3}");
    public const string TokenRevocationsResource = "TokenRevocations";
    public static readonly Guid TokenRevocationsLocationId = new Guid("{A2E4520B-1CC8-4526-871E-F3A8F865F221}");
    public const string TokenListGlobalIdentities = "TokenListGlobalIdentities";
    public static readonly Guid TokenListGlobalIdentitiesId = new Guid("{30D3A12B-66C3-4669-B016-ECB0706C8D0F}");
    public const string TokenGetPersonalAccessToken = "TokenGetPersonalAccessToken";
    public static readonly Guid TokenGetPersonalAccessTokenId = new Guid("{EAE33623-82E3-4E2D-B633-670AF4986F59}");
    public const string TokenRevokePersonalAccessToken = "TokenRevokePersonalAccessToken";
    public static readonly Guid TokenRevokePersonalAccessTokenId = new Guid("{A2F6DF16-FAD2-4680-A644-507C6832EA11}");
  }
}
