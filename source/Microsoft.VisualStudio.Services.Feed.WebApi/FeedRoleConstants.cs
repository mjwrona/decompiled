// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.FeedRoleConstants
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8DACB936-5231-4131-8ED8-082A1F46DC54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.dll

namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  public class FeedRoleConstants
  {
    public const FeedPermissionConstants Reader = FeedPermissionConstants.ReadPackages;
    public const FeedPermissionConstants Collaborator = FeedPermissionConstants.ReadPackages | FeedPermissionConstants.AddUpstreamPackage;
    public const FeedPermissionConstants Contributor = FeedPermissionConstants.ReadPackages | FeedPermissionConstants.AddPackage | FeedPermissionConstants.UpdatePackage | FeedPermissionConstants.DelistPackage | FeedPermissionConstants.AddUpstreamPackage;
    public const FeedPermissionConstants Administrator = FeedPermissionConstants.AdministerFeed | FeedPermissionConstants.ArchiveFeed | FeedPermissionConstants.DeleteFeed | FeedPermissionConstants.EditFeed | FeedPermissionConstants.ReadPackages | FeedPermissionConstants.AddPackage | FeedPermissionConstants.UpdatePackage | FeedPermissionConstants.DeletePackage | FeedPermissionConstants.DelistPackage | FeedPermissionConstants.AddUpstreamPackage;
    public const int None = 0;
  }
}
