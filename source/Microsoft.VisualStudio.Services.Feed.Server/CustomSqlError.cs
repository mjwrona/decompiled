// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.CustomSqlError
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  internal static class CustomSqlError
  {
    public const int GenericWrapperCode = 50000;
    public const int FeedNameAlreadyExists = 1620000;
    public const int FeedNameNotFound = 1620001;
    public const int FeedIdNotFound = 1620002;
    public const int FeedNotReleased = 1620003;
    public const int PackageVersionByIdNotFound = 1620004;
    public const int PackageVersionByNameNotFound = 1620005;
    public const int KnownStateIsNoLongerValid = 1620006;
    public const int PackageVersionByIdDeleted = 1620007;
    public const int PackageVersionByNameDeleted = 1620008;
    public const int UnknownDatabaseError = 1620009;
    public const int FeedViewNameAlreadyExists = 1620010;
    public const int FeedViewIdNotFound = 1620011;
    public const int FeedViewNotReleased = 1620012;
    public const int PackageByNameNotFound = 1620014;
    public const int RecycleBinPackageVersionByIdNotFound = 1620015;
    public const int GenericDatabaseFailure = 1620016;
    public const int TransactionRequired = 1620017;
  }
}
