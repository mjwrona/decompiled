// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Teams.Service.TfsTeamApiController
// Assembly: Microsoft.Azure.Devops.Teams.Service, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 643BB522-27FA-4CE7-8A1E-31D95F8AC409
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Teams.Service.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Net;

namespace Microsoft.Azure.Devops.Teams.Service
{
  [ResolveTfsProjectAndTeamFilter]
  public abstract class TfsTeamApiController : 
    TfsApiController,
    ITfsProjectApiController,
    ITfsTeamApiController
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      TfsProjectApiController.AddProjectExceptions(exceptionMap);
      exceptionMap.AddTranslation(typeof (TeamNotFoundException), typeof (Microsoft.TeamFoundation.Core.WebApi.TeamNotFoundException));
      exceptionMap.AddTranslation(typeof (DefaultTeamNotFoundException), typeof (Microsoft.TeamFoundation.Core.WebApi.DefaultTeamNotFoundException));
      exceptionMap.AddTranslation(typeof (IllegalIdentityException), typeof (InvalidTeamNameException));
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.Core.WebApi.TeamNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.Core.WebApi.DefaultTeamNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<InvalidTeamNameException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TeamNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<DefaultTeamNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<IllegalIdentityException>(HttpStatusCode.BadRequest);
    }

    public virtual ProjectInfo ProjectInfo { get; set; }

    public Guid ProjectId => this.ProjectInfo == null ? Guid.Empty : this.ProjectInfo.Id;

    public WebApiTeam Team { get; set; }

    public Guid TeamId => this.Team == null ? Guid.Empty : this.Team.Id;
  }
}
