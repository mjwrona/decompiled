// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.ScaledAgileApiControllerBase
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Agile.Server.Services;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.Work.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Net;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [ApplyRequestLanguage]
  public abstract class ScaledAgileApiControllerBase : TfsTeamApiController
  {
    private IAgileSettings m_AgileSettings;

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentOutOfRangeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentNullException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<KeyNotFoundException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<SqlException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<SqlTypeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ViewRevisionMismatchException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<UnauthorizedAccessException>(HttpStatusCode.Forbidden);
    }

    public virtual IAgileSettings AgileSettings
    {
      get
      {
        if (this.m_AgileSettings == null)
        {
          ArgumentUtility.CheckForNull<WebApiTeam>(this.Team, "TfsTeamApiController.Team");
          this.m_AgileSettings = (IAgileSettings) new Microsoft.TeamFoundation.Agile.Server.AgileSettings(this.TfsRequestContext, this.CssProjectInfo, this.Team);
        }
        return this.m_AgileSettings;
      }
    }

    protected CommonStructureProjectInfo CssProjectInfo => CommonStructureProjectInfo.ConvertProjectInfo(this.ProjectInfo);

    protected virtual string GetPlanRestLink(Guid planId) => this.Url.RestLink(this.TfsRequestContext, ScaledAgileApiConstants.PlansLocationId, (object) new
    {
      id = planId
    });

    protected void CheckPlansLicense() => this.TfsRequestContext.GetService<IAgileProjectPermissionService>().GetPlansPermission(this.TfsRequestContext, this.ProjectId, true);
  }
}
