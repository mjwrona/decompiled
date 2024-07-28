// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Constants.NpmConstants
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using System;

namespace Microsoft.VisualStudio.Services.Npm.Server.Constants
{
  public class NpmConstants
  {
    public const string LatestDistTag = "latest";
    public const string SuccessTrue = "true";
    public const string FeedChangeProcessingJobExtensionName = "Microsoft.VisualStudio.Services.Npm.Server.Plugins.ChangeProcessing.NpmFeedChangeProcessingJob";
    public const string FeedContentVerificationJobExtensionName = "Microsoft.VisualStudio.Services.Npm.ContentVerification.NpmFeedContentVerificationJob";
    public const string NpmIndexEntryProtocolType = "Npm";
    public const string NpmProtocolName = "npm";
    public static readonly Guid CollectionChangeProcessingJobId = Guid.Parse("DDE314B6-928B-4E21-A9A3-FC463D7307AD");

    public static class ChangeProcessingJobConstants
    {
      public static readonly JobCreationInfo NpmChangeProcessingJobCreationInfo = new JobCreationInfo("FeedChangeProcessingJob", "Microsoft.VisualStudio.Services.Npm.Server.Plugins.ChangeProcessing.NpmFeedChangeProcessingJob", TeamFoundationHostType.ProjectCollection);
    }

    public static class RecoveryJobConstants
    {
      public static readonly Guid RecoveryMasterJobId = Guid.Parse("310341C0-D5AC-4C1B-81F3-A3D38BE51813");
      public static readonly JobCreationInfo RecoveryWorkerJobCreationInfo = new JobCreationInfo("NpmRecoveryWorkerJob", "Microsoft.VisualStudio.Services.Npm.RecoveryJob.NpmRecoveryWorkerJob", TeamFoundationHostType.ProjectCollection, JobPriorityClass.High);
    }

    public static class ContentVerificationJobConstants
    {
      public static readonly JobCreationInfo NpmContentVerificationJobCreationInfo = new JobCreationInfo("NpmFeedContentVerificationJob", "Microsoft.VisualStudio.Services.Npm.ContentVerification.NpmFeedContentVerificationJob", TeamFoundationHostType.ProjectCollection);
      public static readonly Guid CollectionContentVerificationJobId = Guid.Parse("90D1CEFD-410D-4A7E-8313-0E3608FCD7C4");
    }

    public static class MigrationJobConstants
    {
      public static readonly Guid MigrationKickerJobId = Guid.Parse("04917d31-1d17-4935-8850-b4dd8135ba77");
      public static readonly JobCreationInfo NpmMigrationKickerJobCreationInfo = new JobCreationInfo("NpmMigrationKickerJob", "Microsoft.VisualStudio.Services.Npm.Server.Plugins.Migration.NpmMigrationKickerJob", TeamFoundationHostType.Deployment);
      public static readonly JobCreationInfo NpmMigrationProcessingJobCreationInfo = new JobCreationInfo("NpmFeedMigrationProcessingJob", "Microsoft.VisualStudio.Services.Npm.Server.Plugins.Migration.NpmMigrationProcessingJob", TeamFoundationHostType.ProjectCollection);
    }

    public static class UpstreamMetadata
    {
      public const string RevisionSeparator = ",";
    }
  }
}
