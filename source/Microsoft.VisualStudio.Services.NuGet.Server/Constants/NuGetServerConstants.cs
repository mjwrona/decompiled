// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Constants.NuGetServerConstants
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Constants
{
  public static class NuGetServerConstants
  {
    public const int QueryMaxTake = 10000;
    public const int QueryV3DefaultTake = 20;
    public const int QueryV2DefaultTake = 100;
    public const string FeedIndexNugetProtocolType = "NuGet";
    public const string FeedChangeProcessingJobExtensionName = "Microsoft.VisualStudio.Services.NuGet.Server.Plugins.ChangeProcessing.NuGetFeedChangeProcessingJob";
    public const string BlobScope = "nuget";
    public const string NuspecFileExtension = ".nuspec";
    public const string NupkgFileExtension = ".nupkg";
    public const string ProgressReportIntervalRegistryKey = "/Configuration/Packaging/PushProgressReportIntervalBytes";
    public const string AlternateMetadataKey = "/Configuration/Packaging/NuGet/AlternateMetadata";
    public static readonly Guid CollectionChangeProcessingJobId = Guid.Parse("01B89ADD-20E0-4A74-8892-D51088F36FA5");
    public static readonly Guid CollectionDeletedPackageJobId = Guid.Parse("5C37BEC3-DCA8-491F-8C2D-F98A6B13FBCE");
    public const string WithSemVer2SupportRouteDefaultsKey = "/withSemVer2Support";
    public static readonly JobCreationInfo FeedDeletedPackageJobCreationInfo = new JobCreationInfo("NuGetFeedDeletedPackageJob", "Microsoft.VisualStudio.Services.NuGet.Server.Plugins.ChangeProcessing.DeletedPackages.NuGetFeedDeletedPackageJob", TeamFoundationHostType.ProjectCollection);
    public static readonly BookmarkTokenKey NuGetOrgCatalogBookmarkTokenKey = new BookmarkTokenKey(Protocol.NuGet.LowercasedName, "deployment", "nuGetOrgCatalogBookmarkToken", 1);

    public static class ChangeProcessingJobConstants
    {
      public static readonly JobCreationInfo NuGetChangeProcessingJobCreationInfo = new JobCreationInfo("FeedChangeProcessingJob", "Microsoft.VisualStudio.Services.NuGet.Server.Plugins.ChangeProcessing.NuGetFeedChangeProcessingJob", TeamFoundationHostType.ProjectCollection);
    }

    public static class RecoveryJobConstants
    {
      public static readonly Guid RecoveryMasterJobId = Guid.Parse("4C0A872F-43E5-42B5-B521-4F77F0C7A798");
      public static readonly JobCreationInfo RecoveryWorkerJobCreationInfo = new JobCreationInfo("NuGetRecoveryWorkerJob", "Microsoft.VisualStudio.Services.NuGet.Server.Plugins.RecoveryJobs.NuGetRecoveryWorkerJob", TeamFoundationHostType.ProjectCollection, JobPriorityClass.High);
    }
  }
}
