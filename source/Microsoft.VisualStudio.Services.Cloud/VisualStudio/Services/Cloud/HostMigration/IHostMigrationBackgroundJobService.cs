// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.IHostMigrationBackgroundJobService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  [DefaultServiceImplementation(typeof (HostMigrationBackgroundJobService))]
  public interface IHostMigrationBackgroundJobService : IVssFrameworkService
  {
    bool HasRunningMigrationJobs(IVssRequestContext requestContext, IMigrationEntry migrationEntry);

    void QueueStopRunningMigrationJobs(
      IVssRequestContext requestContext,
      IMigrationEntry migrationEntry);

    void QueueBackgroundMigrationJob(
      IVssRequestContext requestContext,
      string[] operations,
      IMigrationEntry migrationEntry,
      bool rollback,
      MigrationJobStage jobStage,
      int retires = 0);

    void RegisterResourceMigrationJob(
      IVssRequestContext requestContext,
      IMigrationEntry migrationEntry,
      Guid jobId,
      string name,
      MigrationJobStage jobStage,
      int retries);

    void CheckBackgroundMigrationJobs(
      IVssRequestContext requestContext,
      Guid migrationId,
      MigrationJobStage? expectedJobStage,
      Action<Guid, string> onMigrationJobsFailed,
      Action<Guid> onMigrationJobsCompleted);
  }
}
