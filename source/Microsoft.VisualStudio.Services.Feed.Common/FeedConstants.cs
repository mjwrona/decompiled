// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.FeedConstants
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  public static class FeedConstants
  {
    public const string FeedMessageBusName = "Microsoft.VisualStudio.Services.Feed";
    public const string FeedCache = "Packaging.Feed.CachingEnabled";
    public const string FeedArtifactToolName = "PackageManagement";
    public const string UseProjectServiceCache = "Packaging.Feed.UseProjectServiceCache";
    public const string FeedRegistryRootPath = "/Configuration/Feed/";
    public const string FeedUnexpectedExceptionThresholdPath = "/Configuration/Feed/CriticalUnexpectedExceptionsThreshold";
    public const string FeedCriticalUnexpectedExceptionsTimeWindowInSecondsPath = "/Configuration/Feed/CriticalUnexpectedExceptionsTimeWindowInSeconds";
    public const string FeedIncidentLifeSpanInSecondsPath = "/Configuration/Feed/IncidentLifeSpanInSeconds";
    public const int FeedUnexpectedExceptionThresholdDefault = 5;
    public const int FeedCriticalUnexpectedExceptionsTimeWindowInSecondsDefault = 300;
    public const int FeedIncidentLifeSpanInSecondsDefault = 3600;
    public static readonly Guid FeedSecurityNamespaceId = new Guid("{9FED0191-DCA2-4112-86B7-A6A48D1B204C}");
    public static readonly Guid FeedIndexSecurityNamespaceId = new Guid("{441A416D-DD0E-4B10-A401-A5DA40BD6AF8}");
    public static readonly Guid FeedServiceSecurityNamespaceId = new Guid("{0BDF233F-D8D4-4CF5-B0FF-181C4CECFD83}");
    public static readonly Guid FeedArtifactKindId = new Guid("{58FD173A-80A0-4D02-8223-1DCDE357541A}");
    public const string FeedMinimumSnapshotRetentionInstanceCountPath = "/Configuration/Feed/SnapshotRetention/MinimumSnapshotInstanceCount";
    public const int DefaultMinimumSnapshotInstanceCount = 25;
    public const string MaxPackagesBatchRequestSizeRegKey = "/Configuration/Packaging/MaxPackagesBatchRequestSize";
    public const int MaxPackagesBatchRequestDefaultSize = 100;
    public const string MaxConcurrentPackagingWritesWithinFeedProtocolKey = "/Configuration/Feed/MaxConcurrentPackagingWritesWithinFeedProtocol";
    public const int MaxConcurrentPackagingWritesWithinFeedProtocolDefault = 3;
    public const string FeedLocalViewName = "Local";
    public const string NugetProtocolType = "NuGet";
    public const string NpmProtocolType = "npm";
    public const int MaxPackageNameLength = 255;
    public const int MaxPackageVersionLength = 127;
    public const int MaxSortableVersionLength = 127;
  }
}
