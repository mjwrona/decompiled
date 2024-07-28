// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SourceMigrationController
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
  [VersionedApiControllerCustomName(Area = "Migration", ResourceName = "SourceHostMigration")]
  public class SourceMigrationController : TfsApiController
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
      }
    };

    [HttpPut]
    public HttpResponseMessage CreateMigrationRequest(SourceHostMigration migrationRequest)
    {
      HostMigrationUtil.CheckPermission(this.TfsRequestContext, HostMigrationPermissions.Read | HostMigrationPermissions.Write);
      IVssRequestContext context = this.TfsRequestContext.Elevate();
      SourceHostMigrationService service = context.GetService<SourceHostMigrationService>();
      migrationRequest.HostId = migrationRequest.HostProperties.Id;
      IVssRequestContext deploymentContext = context;
      SourceHostMigration migrationEntry = migrationRequest;
      return this.Request.CreateResponse<SourceHostMigration>(HttpStatusCode.Accepted, service.StartCreateSourceMigrationJob(deploymentContext, migrationEntry));
    }

    [HttpPatch]
    public HttpResponseMessage UpdateMigrationRequest(SourceHostMigration migrationRequest)
    {
      HostMigrationUtil.CheckPermission(this.TfsRequestContext, HostMigrationPermissions.Read | HostMigrationPermissions.Write);
      this.TfsRequestContext.CheckServicePrincipal();
      IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
      SourceHostMigrationService service = vssRequestContext.GetService<SourceHostMigrationService>();
      SourceHostMigration migrationEntry = service.GetMigrationEntry(vssRequestContext, migrationRequest.MigrationId);
      HttpResponseMessage httpResponseMessage;
      if (migrationEntry == null)
        httpResponseMessage = this.Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format("Could not find a migration with MigrationId = {0} to update.", (object) migrationRequest.MigrationId));
      else if ((migrationRequest.State != SourceMigrationState.BeginPrepareBlobs || migrationEntry.State != SourceMigrationState.PrepareBlobs) && migrationRequest.State < migrationEntry.State)
      {
        httpResponseMessage = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format("Invalid state transition request. Cannot move from state {0} to {1}.", (object) migrationRequest.State, (object) migrationEntry.State));
      }
      else
      {
        migrationEntry.State = migrationRequest.State;
        migrationEntry.StatusMessage = migrationRequest.StatusMessage;
        switch (migrationRequest.State)
        {
          case SourceMigrationState.BeginCreate:
            httpResponseMessage = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format("Migration already exists with Id = {0}.", (object) migrationRequest.MigrationId));
            break;
          case SourceMigrationState.BeginPrepareDatabase:
            httpResponseMessage = this.Request.CreateResponse<SourceHostMigration>(HttpStatusCode.Accepted, service.StartPrepareSourceDatabaseJob(vssRequestContext, migrationEntry));
            break;
          case SourceMigrationState.BeginPrepareBlobs:
            httpResponseMessage = this.Request.CreateResponse<SourceHostMigration>(HttpStatusCode.Accepted, service.StartPrepareBlobsMigrationJob(vssRequestContext, migrationRequest));
            break;
          case SourceMigrationState.BeginUpdateLocation:
            httpResponseMessage = this.Request.CreateResponse<SourceHostMigration>(HttpStatusCode.Accepted, service.QueueUpdateLocation(vssRequestContext, migrationEntry));
            break;
          case SourceMigrationState.BeginComplete:
            httpResponseMessage = this.Request.CreateResponse<SourceHostMigration>(HttpStatusCode.Accepted, service.QueueFinalizeMigrationOnSource(vssRequestContext, migrationEntry));
            break;
          default:
            httpResponseMessage = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format("Unsupported migration state for update: {0}", (object) migrationRequest.State));
            break;
        }
      }
      return httpResponseMessage;
    }

    [HttpDelete]
    public HttpResponseMessage DeleteMigrationRequest(Guid migrationId)
    {
      HostMigrationUtil.CheckPermission(this.TfsRequestContext, HostMigrationPermissions.Read | HostMigrationPermissions.Write);
      IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
      SourceHostMigrationService service = vssRequestContext.GetService<SourceHostMigrationService>();
      SourceHostMigration migrationEntry = service.GetMigrationEntry(vssRequestContext, migrationId);
      if (migrationEntry == null)
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      service.QueueFinalizeMigrationOnSource(vssRequestContext, migrationEntry, true);
      return this.Request.CreateResponse<SourceHostMigration>(HttpStatusCode.OK, migrationEntry);
    }

    [HttpGet]
    public HttpResponseMessage GetMigrationEntry(Guid migrationId, string hostId = "")
    {
      HostMigrationUtil.CheckPermission(this.TfsRequestContext, HostMigrationPermissions.Read);
      IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
      SourceHostMigrationService service = vssRequestContext.GetService<SourceHostMigrationService>();
      SourceHostMigration sourceHostMigration = !(migrationId != Guid.Empty) ? service.GetLatestMigrationEntry(vssRequestContext, Guid.Parse(hostId)) : service.GetMigrationEntry(vssRequestContext, migrationId);
      return sourceHostMigration == null ? this.Request.CreateResponse(HttpStatusCode.NotFound) : this.Request.CreateResponse<SourceHostMigration>(HttpStatusCode.OK, sourceHostMigration);
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) SourceMigrationController.s_httpExceptions;

    public override string TraceArea => SourceHostMigrationService.s_area;

    public override string ActivityLogArea => "Framework";
  }
}
