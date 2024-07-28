// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.IHostMigrationManagementService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  [DefaultServiceImplementation(typeof (HostMigrationManagementService))]
  public interface IHostMigrationManagementService : IVssFrameworkService
  {
    bool IsParallelMigrationEnabled(IVssRequestContext requestContext);

    bool IsBackgroundMigrationEnabled(IVssRequestContext requestContext);

    List<HostMigrationRequest> GetRunningRequests(IVssRequestContext requestContext);

    List<HostMigrationRequest> GetNextSchedulableRequests(
      IVssRequestContext requestContext,
      DateTime maxLastUserAccess,
      int maxNumberOfRequests,
      string[] targetInstanceNamesToIgnore);

    HostMigrationRequestResultSegment GetNextSchedulableRequestsSegmented(
      IVssRequestContext requestContext,
      DateTime maxLastUserAccess,
      int maxNumberOfRequests,
      HostMigrationRequestContinuationToken continuationToken,
      string[] targetInstanceNamesToIgnore);

    void SetQueueRequestDriverJobId(IVssRequestContext requestContext, Guid hostId, Guid jobId);

    void ClearQueueRequestDriverJobId(IVssRequestContext requestContext, Guid hostId);

    void DeleteQueueRequest(IVssRequestContext requestContext, Guid hostId);

    HostMigrationRequest GetMigrationRequestForHost(IVssRequestContext requestContext, Guid hostId);

    bool IsMigrationEnabledOnInstance(IVssRequestContext requestContext, string instanceName);

    string GetMigrationCertificateThumbprintForInstance(
      IVssRequestContext requestContext,
      string instanceName);
  }
}
