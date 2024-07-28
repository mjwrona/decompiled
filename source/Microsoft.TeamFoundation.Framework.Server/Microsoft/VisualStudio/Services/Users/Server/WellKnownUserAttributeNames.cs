// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Server.WellKnownUserAttributeNames
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.VisualStudio.Services.Users.Server
{
  public static class WellKnownUserAttributeNames
  {
    public const string TFSContainerName = "TFS";
    public static readonly string TFSContainerWildcardQuery = "TFS.*";
    public static readonly string TFSCulture = "TFS.Culture";
    public static readonly string TFSAccountCreateIDEDate = "TFS.AccountCreateIDEDate";
    public static readonly string AvatarPreview = "avatar.preview";
    public const string SystemContainerName = "Private";
    public static readonly string Region = "Private.Region";
    public static readonly string AvatarPrefix = "Private.Avatar.";
    public static readonly string ProviderContainerName = "Private.Provider";
    public static readonly string UserPrincipalName = WellKnownUserAttributeNames.ProviderContainerName + ".UserPrincipalName";
    public static readonly string Puid = WellKnownUserAttributeNames.ProviderContainerName + ".Puid";
    public static readonly string TenantId = WellKnownUserAttributeNames.ProviderContainerName + ".TenantId";
    public static readonly string ObjectId = WellKnownUserAttributeNames.ProviderContainerName + ".ObjectId";
    public static readonly string ProviderDisplayName = WellKnownUserAttributeNames.ProviderContainerName + ".DisplayName";
    public static readonly string ProviderMail = WellKnownUserAttributeNames.ProviderContainerName + ".Mail";
    public const string UserSettingsContainerName = "UserSettings";
    public static readonly string UserSettingsContainerNamePrefix = "UserSettings.";
    public static readonly string UserSettingsContainerWildcardQuery = WellKnownUserAttributeNames.UserSettingsContainerNamePrefix + "*";
    public static readonly string AadTokenCache = "Private.Aad.TokenCache";
    public static readonly string UserProfileSyncState = "User.ProfileSyncState";
  }
}
