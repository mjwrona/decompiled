// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.JobMovementController
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
  [VersionedApiControllerCustomName(Area = "Migration", ResourceName = "JobMovement")]
  public class JobMovementController : TfsApiController
  {
    [HttpGet]
    public HttpResponseMessage GetQueuedAdHocJobs(Guid jobSource)
    {
      HostMigrationUtil.CheckPermission(this.TfsRequestContext, HostMigrationPermissions.Read);
      return this.Request.CreateResponse<List<AdHocJobInfo>>(HttpStatusCode.OK, AdHocJobMover.GetQueuedAdHocJobsForHost(this.TfsRequestContext, jobSource));
    }
  }
}
