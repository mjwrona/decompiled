// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectCatalog
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectCatalog
  {
    public static void Update(
      IVssRequestContext requestContext,
      ProjectUpdatedEvent projectUpdatedEvent)
    {
      ICommonStructureService service1 = requestContext.GetService<ICommonStructureService>();
      CatalogNode catalogNode = service1.QueryProjectCatalogNode(requestContext, projectUpdatedEvent.Uri);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      CatalogTransactionContext transactionContext = vssRequestContext.GetService<ITeamFoundationCatalogService>().CreateTransactionContext();
      Guid projectId = ProjectInfo.GetProjectId(projectUpdatedEvent.Uri);
      IProjectService service2 = requestContext.GetService<IProjectService>();
      ProjectInfo projectInfo;
      if (service2.TryGetProject(requestContext, projectId, out projectInfo) && projectInfo.Revision < projectUpdatedEvent.Revision)
        projectInfo = (service2.GetProjectHistory(requestContext, projectId, projectUpdatedEvent.Revision) ?? throw new ArgumentException(string.Format("Revision {0} is missing in the project history for project {1}.", (object) projectUpdatedEvent.Revision, (object) projectId))).Last<ProjectInfo>();
      if (projectInfo != null && !projectInfo.IsSoftDeleted)
      {
        if (catalogNode != null)
        {
          catalogNode.Resource.Properties["ProjectName"] = projectUpdatedEvent.Name;
          catalogNode.Resource.DisplayName = projectUpdatedEvent.Name;
          catalogNode.Resource.Properties["ProjectState"] = projectInfo.State.ToString();
          catalogNode.Resource.Description = projectInfo.Description;
          transactionContext.AttachNode(catalogNode);
          transactionContext.Save(vssRequestContext, false);
        }
        else
        {
          if (!projectUpdatedEvent.GetOperationProperty<bool>("IsProjectRecovery", false))
            return;
          service1.EnsureTeamProjectsExistsInCatalog(requestContext, (IEnumerable<CommonStructureProjectInfo>) new CommonStructureProjectInfo[1]
          {
            CommonStructureProjectInfo.ConvertProjectInfo(projectInfo)
          });
        }
      }
      else
      {
        if (catalogNode == null || !projectUpdatedEvent.GetOperationProperty<bool>("IsProjectSoftDelete", false))
          return;
        transactionContext.AttachDelete(catalogNode, true);
        transactionContext.Save(vssRequestContext, false);
      }
    }
  }
}
