// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.Taskboard.TaskboardColumnController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Agile.Server.TaskBoard;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller.Taskboard
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "taskboardColumns")]
  [ControllerApiVersion(5.2)]
  [FeatureDisabled("WebAccess.Agile.Taskboard.DisableCustomColumn")]
  public class TaskboardColumnController : TfsTeamApiController
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<TaskboardColumnInvalidMappingException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskboardColumnInvalidException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskboardColumnMinColumnLimitException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskboardColumnDuplicateColumnIdException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskboardColumnMaxLimitExceededException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskboardColumnNameInvalidException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskboardColumnIdInvalidException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskboardColumnMappingEmptyException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskboardColumnNameDuplicateException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskboardColumnUpdateException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskboardColumnUpdateUserIsNotTeamAdminException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<TaskboardColumnNotFoundException>(HttpStatusCode.NotFound);
    }

    [HttpGet]
    public Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard.TaskboardColumns GetColumns() => this.TfsRequestContext.GetService<ITaskboardColumnService>().GetColumns(this.TfsRequestContext, this.ProjectInfo, this.Team).Convert();

    [HttpPut]
    public Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard.TaskboardColumns UpdateColumns(
      [FromBody] IEnumerable<UpdateTaskboardColumn> updateColumns)
    {
      return this.TfsRequestContext.GetService<ITaskboardColumnService>().UpdateColumns(this.TfsRequestContext, this.ProjectInfo, this.Team, (IReadOnlyCollection<UpdateTaskboardColumn>) updateColumns.ToList<UpdateTaskboardColumn>()).Convert();
    }
  }
}
