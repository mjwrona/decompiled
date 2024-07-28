// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.MigrationJobsController
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.HostMigration;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [ControllerApiVersion(1.0)]
  [ClientInternalUseOnly(true)]
  [VersionedApiControllerCustomName(Area = "Migration", ResourceName = "MigrationJobs")]
  public class MigrationJobsController : TfsApiController
  {
    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage StopMigrationJobs(Guid migrationId)
    {
      HostMigrationUtil.CheckPermission(this.TfsRequestContext, HostMigrationPermissions.Read | HostMigrationPermissions.Write);
      IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
      IMigrationEntry migrationEntry = MigrationJobsController.GetMigrationEntry(vssRequestContext, migrationId);
      if (migrationEntry == null)
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      vssRequestContext.GetService<IHostMigrationBackgroundJobService>().QueueStopRunningMigrationJobs(vssRequestContext, migrationEntry);
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }

    [HttpGet]
    [ClientResponseType(typeof (MigrationJobInformation), null, null)]
    public HttpResponseMessage GetMigrationJobInformation(Guid migrationId)
    {
      HostMigrationUtil.CheckPermission(this.TfsRequestContext, HostMigrationPermissions.Read);
      IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
      IMigrationEntry migrationEntry = MigrationJobsController.GetMigrationEntry(vssRequestContext, migrationId);
      if (migrationEntry == null)
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      bool flag = vssRequestContext.GetService<IHostMigrationBackgroundJobService>().HasRunningMigrationJobs(vssRequestContext, migrationEntry);
      return this.Request.CreateResponse<MigrationJobInformation>(HttpStatusCode.OK, new MigrationJobInformation()
      {
        HasRunningMigrationJobs = flag
      });
    }

    private static IMigrationEntry GetMigrationEntry(
      IVssRequestContext requestContext,
      Guid migrationId)
    {
      return (IMigrationEntry) requestContext.GetService<SourceHostMigrationService>().GetMigrationEntry(requestContext, migrationId) ?? (IMigrationEntry) requestContext.GetService<TargetHostMigrationService>().GetMigrationEntry(requestContext, migrationId, true);
    }
  }
}
