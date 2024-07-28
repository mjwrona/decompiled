// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Implementations.MavenJobConstants
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using System;

namespace Microsoft.VisualStudio.Services.Maven.Server.Implementations
{
  public class MavenJobConstants
  {
    private const string SnapshotCleanupJobName = "MavenSnapshotCleanupJob";
    public static readonly JobType SnapshotCleanupJobType = new JobType(Guid.Parse("C46BD3A6-1687-4C57-BDA6-B52A971E736B"), "MavenSnapshotCleanupJob");
    public static readonly JobCreationInfo SnapshotCleanupJobCreationInfo = new JobCreationInfo("MavenSnapshotCleanupJob", "Microsoft.VisualStudio.Services.Maven.Server.Plugins.SnapshotCleanup.MavenFeedSnapshotCleanupJob", TeamFoundationHostType.ProjectCollection);
    public static readonly Guid CollectionDeletedPackageJobId = Guid.Parse("42A4678D-C18C-4F67-910A-CF4C30142348");
    public static readonly JobCreationInfo FeedDeletedPackageJobCreationInfo = new JobCreationInfo("MavenFeedDeletedPackageJob", "Microsoft.VisualStudio.Services.Maven.Server.Plugins.ChangeProcessing.DeletedPackages.MavenFeedDeletedPackageJob", TeamFoundationHostType.ProjectCollection);

    public static class MigrationJobConstants
    {
      public static readonly Guid MigrationKickerJobId = Guid.Parse("7F521C79-BE4D-4355-8CD4-06C584119705");
      public static readonly JobCreationInfo MavenMigrationKickerJobCreationInfo = new JobCreationInfo("MavenMigrationKickerJob", "Microsoft.VisualStudio.Services.Maven.Server.Plugins.Migration.MavenMigrationKickerJob", TeamFoundationHostType.Deployment);
      public static readonly JobCreationInfo MavenMigrationProcessingJobCreationInfo = new JobCreationInfo("MavenFeedMigrationProcessingJob", "Microsoft.VisualStudio.Services.Maven.Server.Plugins.Migration.MavenMigrationProcessingJob", TeamFoundationHostType.ProjectCollection);
    }
  }
}
