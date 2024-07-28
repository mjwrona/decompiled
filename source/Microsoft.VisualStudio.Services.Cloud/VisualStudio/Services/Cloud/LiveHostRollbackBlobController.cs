// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.LiveHostRollbackBlobController
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.HostMigration;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [ControllerApiVersion(3.2)]
  [ClientInternalUseOnly(true)]
  [VersionedApiControllerCustomName(Area = "Migration", ResourceName = "LiveHostRollbackBlob")]
  public class LiveHostRollbackBlobController : TfsApiController
  {
    internal static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (InvalidAccessException),
        HttpStatusCode.Forbidden
      }
    };

    [HttpPost]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage CopyNewTargetBlobsToSourceForRollback(
      Guid migrationId,
      CopyNewTargetBlobsToSourceForRollbackRequest copyNewTargetBlobsToSourceForRollbackRequest)
    {
      HostMigrationUtil.CheckPermission(this.TfsRequestContext, HostMigrationPermissions.Read);
      IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
      SourceHostMigrationService service = vssRequestContext.GetService<SourceHostMigrationService>();
      if (service.GetMigrationEntry(vssRequestContext, migrationId) == null)
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      service.StartCopyNewTargetBlobsToSourceForRollbackJob(vssRequestContext, migrationId, copyNewTargetBlobsToSourceForRollbackRequest);
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) LiveHostRollbackBlobController.s_httpExceptions;

    public override string TraceArea => SourceHostMigrationService.s_area;

    public override string ActivityLogArea => "Framework";
  }
}
