// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.BaseModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Models
{
  internal abstract class BaseModel
  {
    private WebAccessWorkItemService m_workItemService;

    public BaseModel(
      IVssRequestContext requestContext,
      IAgileSettings agileSettings,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo projectInfo,
      WebApiTeam team,
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode requestIterationNode)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IAgileSettings>(agileSettings, nameof (agileSettings));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Core.WebApi.ProjectInfo>(projectInfo, nameof (projectInfo));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>(requestIterationNode, nameof (requestIterationNode));
      this.TfsRequestContext = requestContext;
      this.ProjectInfo = projectInfo;
      this.Team = team;
      this.RequestIterationNode = requestIterationNode;
      this.AgileSettings = agileSettings;
    }

    public BaseModel(TfsWebContext webContext, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode requestIterationNode)
    {
      ArgumentUtility.CheckForNull<TfsWebContext>(webContext, nameof (webContext));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>(requestIterationNode, nameof (requestIterationNode));
      this.TfsRequestContext = webContext.TfsRequestContext;
      this.ProjectInfo = webContext.Project;
      this.Team = webContext.Team;
      this.RequestIterationNode = requestIterationNode;
      this.AgileSettings = (IAgileSettings) new Microsoft.TeamFoundation.Agile.Server.AgileSettings(this.TfsRequestContext, CommonStructureProjectInfo.ConvertProjectInfo(this.ProjectInfo), this.Team);
    }

    protected WebAccessWorkItemService WitService
    {
      get
      {
        if (this.m_workItemService == null)
          this.m_workItemService = this.TfsRequestContext.GetService<WebAccessWorkItemService>();
        return this.m_workItemService;
      }
    }

    protected IAgileSettings AgileSettings { get; private set; }

    protected IVssRequestContext TfsRequestContext { get; private set; }

    protected Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode RequestIterationNode { get; private set; }

    protected Microsoft.TeamFoundation.Core.WebApi.ProjectInfo ProjectInfo { get; private set; }

    protected WebApiTeam Team { get; private set; }
  }
}
