// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Constants.PyPiJobConstants
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using System;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Constants
{
  public class PyPiJobConstants
  {
    private const string ContainerTypeName = "feed";
    private const string ChangeProcessingBookmarkTokenName = "searchIndexLastUpdatedBookmarkToken";
    private const int CurrentBookmarkTokenVersion = 1;
    public static readonly BookmarkTokenKey ChangeProcessingBookmarkTokenKey = new BookmarkTokenKey(Protocol.PyPi.LowercasedName, "feed", "searchIndexLastUpdatedBookmarkToken", 1);
    private const int DeleteBookmarkTokenVersion = 1;
    public static readonly BookmarkTokenKey DeleteProcessingBookmarkTokenKey = new BookmarkTokenKey(Protocol.PyPi.LowercasedName, "feed", "DeletedPackageLastUpdatedBookmarkToken", 1);

    public static class ChangeProcessingJobConstants
    {
      public static readonly Guid CollectionChangeProcessingJobId = Guid.Parse("7387D2DC-0A5E-4992-8D25-A9B379A2E680");
      public const string FeedChangeProcessingJobExtensionName = "Microsoft.VisualStudio.Services.PyPi.Server.Plugins.ChangeProcessing.PyPiFeedChangeProcessingJob";
      public static readonly JobCreationInfo PyPiChangeProcessingJobCreationInfo = new JobCreationInfo("FeedChangeProcessingJob", "Microsoft.VisualStudio.Services.PyPi.Server.Plugins.ChangeProcessing.PyPiFeedChangeProcessingJob", TeamFoundationHostType.ProjectCollection);
    }

    public static class DeleteProcessingJobConstants
    {
      public static readonly Guid CollectionDeletedPackageJobId = Guid.Parse("D51125B3-0391-499D-A48D-6433C85C5EF5");
      public static readonly JobCreationInfo FeedDeletedPackageJobCreationInfo = new JobCreationInfo("PyPiFeedDeletedPackageJob", "Microsoft.VisualStudio.Services.PyPi.Server.Plugins.ChangeProcessing.DeletedPackages.PyPiFeedDeletedPackageJob", TeamFoundationHostType.ProjectCollection);
    }

    public static class ContentVerificationJobConstants
    {
      public static readonly Guid CollectionContentVerificationJobId = Guid.Parse("FEF711B8-763F-4BB3-BAF6-32A256471909");
      public const string FeedContentVerificationJobExtensionName = "Microsoft.VisualStudio.Services.PyPi.Server.Plugins.ContentVerification.PyPiFeedContentVerificationJob";
      public static readonly JobCreationInfo PyPiContentVerificationJobCreationInfo = new JobCreationInfo("FeedContentVerificationJob", "Microsoft.VisualStudio.Services.PyPi.Server.Plugins.ContentVerification.PyPiFeedContentVerificationJob", TeamFoundationHostType.ProjectCollection);
    }

    public static class RecoveryJobConstants
    {
      public const string AlternateMetadataKey = "/Configuration/Packaging/PyPi/AlternateMetadata";
      public static readonly Guid RecoveryMasterJobId = Guid.Parse("8599437D-885C-4EDA-8A08-986F5674F763");
      public static readonly JobCreationInfo RecoveryWorkerJobCreationInfo = new JobCreationInfo("PyPiRecoveryWorkerJob", "Microsoft.VisualStudio.Services.PyPi.Server.Plugins.RecoveryJobs.PyPiRecoveryWorkerJob", TeamFoundationHostType.ProjectCollection, JobPriorityClass.High);
    }

    public static class MigrationJobConstants
    {
      public static readonly Guid MigrationKickerJobId = Guid.Parse("07D884EB-53F7-4F0A-A827-4EDD9E23F3AB");
      public static readonly JobCreationInfo PyPiMigrationKickerJobCreationInfo = new JobCreationInfo("PyPiMigrationKickerJob", "Microsoft.VisualStudio.Services.PyPi.Server.Plugins.Migration.PyPiMigrationKickerJob", TeamFoundationHostType.Deployment);
      public static readonly JobCreationInfo PyPiMigrationProcessingJobCreationInfo = new JobCreationInfo("PyPiFeedMigrationProcessingJob", "Microsoft.VisualStudio.Services.PyPi.Server.Plugins.Migration.PyPiMigrationProcessingJob", TeamFoundationHostType.ProjectCollection);
    }
  }
}
