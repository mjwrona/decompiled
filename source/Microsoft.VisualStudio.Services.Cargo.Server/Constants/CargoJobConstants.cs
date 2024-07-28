// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Constants.CargoJobConstants
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using System;
using System.Diagnostics.CodeAnalysis;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Constants
{
  [ExcludeFromCodeCoverage]
  public static class CargoJobConstants
  {
    public static class ChangeProcessingJobConstants
    {
      public static readonly Guid CollectionChangeProcessingJobId = Guid.Parse("A4A7F95E-72B6-40A6-90DC-C757D354D712");
      public const string FeedChangeProcessingJobExtensionName = "Microsoft.VisualStudio.Services.Cargo.Server.Plugins.ChangeProcessing.CargoFeedChangeProcessingJob";
      public static readonly JobCreationInfo CargoChangeProcessingJobCreationInfo = new JobCreationInfo("FeedChangeProcessingJob", "Microsoft.VisualStudio.Services.Cargo.Server.Plugins.ChangeProcessing.CargoFeedChangeProcessingJob", TeamFoundationHostType.ProjectCollection);
    }

    public static class DeleteProcessingJobConstants
    {
      public static readonly Guid CollectionDeletedPackageJobId = Guid.Parse("4BF737E5-DFD5-44C4-B51B-9F2A9923CBB8");
      public static readonly JobCreationInfo FeedDeletedPackageJobCreationInfo = new JobCreationInfo("FeedDeleteProcessingJob", "Microsoft.VisualStudio.Services.Cargo.Server.Plugins.ChangeProcessing.CargoFeedDeletedPackageJob", TeamFoundationHostType.ProjectCollection);
    }

    public static class RecoveryJobConstants
    {
      public const string AlternateMetadataKey = "/Configuration/Packaging/Cargo/AlternateMetadata";
      public static readonly Guid RecoveryMasterJobId = Guid.Parse("48608607-0294-4306-BDAC-E89A04A82C7C");
      public static readonly JobCreationInfo RecoveryWorkerJobCreationInfo = new JobCreationInfo("CargoRecoveryWorkerJob", "Microsoft.VisualStudio.Services.Cargo.Server.Plugins.RecoveryJobs.CargoRecoveryWorkerJob", TeamFoundationHostType.ProjectCollection, JobPriorityClass.High);
    }

    public static class MigrationJobConstants
    {
      public static readonly Guid MigrationKickerJobId = Guid.Parse("81D59D03-8875-4807-909D-CD676ED69187");
      public static readonly JobCreationInfo CargoMigrationKickerJobCreationInfo = new JobCreationInfo("CargoMigrationKickerJob", "Microsoft.VisualStudio.Services.Cargo.Server.Plugins.Migration.CargoMigrationKickerJob", TeamFoundationHostType.Deployment);
      public static readonly JobCreationInfo CargoMigrationProcessingJobCreationInfo = new JobCreationInfo("CargoFeedMigrationProcessingJob", "Microsoft.VisualStudio.Services.Cargo.Server.Plugins.Migration.CargoMigrationProcessingJob", TeamFoundationHostType.ProjectCollection);
    }
  }
}
