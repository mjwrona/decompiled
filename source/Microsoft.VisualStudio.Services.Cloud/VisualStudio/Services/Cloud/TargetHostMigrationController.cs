// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.TargetHostMigrationController
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.HostMigration;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [ControllerApiVersion(1.0)]
  [ClientIgnore]
  [VersionedApiControllerCustomName(Area = "Migration", ResourceName = "TargetHostMigration")]
  public class TargetHostMigrationController : TfsApiController
  {
    internal static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (InvalidAccessException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (DataMigrationEntryAlreadyExistsException),
        HttpStatusCode.Conflict
      },
      {
        typeof (HostDoesNotExistException),
        HttpStatusCode.NotFound
      }
    };

    [HttpPut]
    public HttpResponseMessage CreateMigrationRequest(TargetHostMigration migrationRequest)
    {
      HostMigrationUtil.CheckPermission(this.TfsRequestContext, HostMigrationPermissions.Read | HostMigrationPermissions.Write);
      IVssRequestContext context = this.TfsRequestContext.Elevate();
      TargetHostMigrationService service = context.GetService<TargetHostMigrationService>();
      migrationRequest.HostId = migrationRequest.HostProperties.Id;
      IVssRequestContext deploymentContext = context;
      TargetHostMigration migrationEntry = migrationRequest;
      return this.Request.CreateResponse<TargetHostMigration>(HttpStatusCode.Accepted, service.StartCreateTargetMigrationJob(deploymentContext, migrationEntry));
    }

    [HttpPatch]
    public HttpResponseMessage UpdateMigrationRequest(TargetHostMigration migrationRequest)
    {
      HostMigrationUtil.CheckPermission(this.TfsRequestContext, HostMigrationPermissions.Read | HostMigrationPermissions.Write);
      IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
      TargetHostMigrationService service = vssRequestContext.GetService<TargetHostMigrationService>();
      TargetHostMigration migrationEntry = service.GetMigrationEntry(vssRequestContext, migrationRequest.MigrationId);
      if (migrationEntry == null)
        this.Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format("Could not find a migration with MigrationId = {0} to update.", (object) migrationRequest.MigrationId));
      else if (migrationEntry.State == migrationRequest.State)
        this.Request.CreateResponse<TargetHostMigration>(HttpStatusCode.OK, migrationEntry);
      else if (migrationRequest.State < migrationEntry.State)
      {
        this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format("Invalid state transition request. Cannot move from state {0} to {1}.", (object) migrationRequest.State, (object) migrationEntry.State));
      }
      else
      {
        switch (migrationRequest.State)
        {
          case TargetMigrationState.BeginCopyJobs:
            migrationEntry = service.StartCopyJobs(vssRequestContext, migrationEntry);
            this.Request.CreateResponse<TargetHostMigration>(HttpStatusCode.Accepted, migrationEntry);
            break;
          case TargetMigrationState.BeginCompletePendingBlobs:
            migrationEntry = service.QueueFinalizeMigrationOnTarget(vssRequestContext, migrationEntry);
            this.Request.CreateResponse<TargetHostMigration>(HttpStatusCode.Accepted, migrationEntry);
            break;
          case TargetMigrationState.BeginComplete:
            migrationEntry = service.QueueCleanupMigrationOnTarget(vssRequestContext, migrationEntry);
            this.Request.CreateResponse<TargetHostMigration>(HttpStatusCode.Accepted, migrationEntry);
            break;
          case TargetMigrationState.ResumeBlobCopy:
            migrationEntry = service.ResumeBlobCopyOnTarget(vssRequestContext, migrationEntry);
            this.Request.CreateResponse<TargetHostMigration>(HttpStatusCode.Accepted, migrationEntry);
            break;
          default:
            this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format("Unsupported migration entry state for PUT: {0}", (object) migrationRequest.State));
            break;
        }
      }
      return this.Request.CreateResponse<TargetHostMigration>(HttpStatusCode.OK, migrationEntry);
    }

    [HttpDelete]
    public HttpResponseMessage DeleteMigrationRequest(Guid migrationId)
    {
      HostMigrationUtil.CheckPermission(this.TfsRequestContext, HostMigrationPermissions.Read | HostMigrationPermissions.Write);
      IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
      TargetHostMigrationService service = vssRequestContext.GetService<TargetHostMigrationService>();
      TargetHostMigration migrationEntry = service.GetMigrationEntry(vssRequestContext, migrationId);
      return migrationEntry == null ? this.Request.CreateResponse(HttpStatusCode.NotFound) : this.Request.CreateResponse<TargetHostMigration>(HttpStatusCode.OK, service.QueueFinalizeMigrationOnTarget(vssRequestContext, migrationEntry, true));
    }

    [HttpGet]
    public HttpResponseMessage GetMigrationEntry(Guid migrationId)
    {
      HostMigrationUtil.CheckPermission(this.TfsRequestContext, HostMigrationPermissions.Read);
      IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
      TargetHostMigration migrationEntry = vssRequestContext.GetService<TargetHostMigrationService>().GetMigrationEntry(vssRequestContext, migrationId, true);
      return migrationEntry == null ? this.Request.CreateResponse(HttpStatusCode.NotFound) : this.Request.CreateResponse<TargetHostMigration>(HttpStatusCode.OK, migrationEntry);
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) TargetHostMigrationController.s_httpExceptions;

    public override string TraceArea => TargetHostMigrationService.s_area;

    public override string ActivityLogArea => "Framework";
  }
}
