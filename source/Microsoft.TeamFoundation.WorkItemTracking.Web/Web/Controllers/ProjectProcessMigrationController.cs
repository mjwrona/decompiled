// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProjectProcessMigrationController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "projectProcessMigration", ResourceVersion = 1)]
  public class ProjectProcessMigrationController : ProcessControllerBase
  {
    [HttpPost]
    [ClientResponseType(typeof (ProcessMigrationResultModel), null, null)]
    [ClientLocationId("19801631-D4E5-47E9-8166-0330DE0FF1E6")]
    [TraceFilter(5922000, 5922010)]
    public HttpResponseMessage MigrateProjectsProcess([FromBody] ProcessIdModel newProcess)
    {
      ArgumentUtility.CheckForNull<ProcessIdModel>(newProcess, nameof (newProcess));
      IWorkItemTrackingProcessService service = this.TfsRequestContext.GetService<IWorkItemTrackingProcessService>();
      ProcessDescriptor processDescriptor;
      service.TryGetProjectProcessDescriptor(this.TfsRequestContext, this.ProjectId, out processDescriptor);
      ProcessDescriptor descriptor;
      this.TfsRequestContext.GetService<ITeamFoundationProcessService>().TryGetProcessDescriptor(this.TfsRequestContext, newProcess.TypeId, out descriptor);
      if (descriptor == null)
        throw new VssPropertyValidationException(nameof (newProcess), ResourceStrings.ProcessNotFound((object) newProcess.TypeId));
      if (processDescriptor.IsCustom || descriptor.IsCustom)
        throw new VssPropertyValidationException(nameof (newProcess), ResourceStrings.XMLMigrationNotSupported());
      if (processDescriptor.TypeId == descriptor.TypeId)
        throw new VssPropertyValidationException(nameof (newProcess), ResourceStrings.ProjectUsesSameProcess());
      if (!((processDescriptor.IsDerived ? processDescriptor.Inherits : processDescriptor.TypeId) == (descriptor.IsDerived ? descriptor.Inherits : descriptor.TypeId)))
        throw new VssPropertyValidationException(nameof (newProcess), ResourceStrings.MigrationAcrossOOBTypesNotSupported());
      ProcessMigrationModel processMigrationModel = new ProcessMigrationModel()
      {
        NewProcessTypeId = descriptor.TypeId,
        ProjectId = this.ProjectId
      };
      IReadOnlyCollection<ProcessMigrationResult> processMigrationResults = service.MigrateProjectsProcess(this.TfsRequestContext, (IEnumerable<ProcessMigrationModel>) new ProcessMigrationModel[1]
      {
        processMigrationModel
      });
      if (processMigrationResults.Count != 0)
        return this.Request.CreateResponse<IReadOnlyCollection<ProcessMigrationResult>>(HttpStatusCode.BadRequest, processMigrationResults);
      return this.Request.CreateResponse<ProcessMigrationResultModel>(HttpStatusCode.OK, new ProcessMigrationResultModel()
      {
        ProjectId = this.ProjectId,
        ProcessId = descriptor.TypeId
      });
    }
  }
}
