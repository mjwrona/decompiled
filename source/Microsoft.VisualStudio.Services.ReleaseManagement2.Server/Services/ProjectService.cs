// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ProjectService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Project.Plugins;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class ProjectService : 
    ReleaseManagement2ServiceBase,
    IReleaseManagementProjectService,
    IVssFrameworkService
  {
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    public void DeleteProject(IVssRequestContext requestContext, string projectUri)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (projectUri == null)
        throw new ArgumentNullException(nameof (projectUri));
      ProjectService.CheckPermissions(requestContext, projectUri);
      Guid guid = new Guid(LinkingUtilities.DecodeUri(projectUri).ToolSpecificId);
      if (!requestContext.IsReleaseManagementDataspaceAvailable(guid))
        return;
      requestContext.GetService<ReleaseDefinitionsService>().SoftDeleteReleaseDefinitions(requestContext, guid, Resources.DeletingProjectData);
      requestContext.GetService<DefinitionEnvironmentTemplatesService>().SoftDeleteDefinitionEnvironmentTemplate(requestContext, guid);
      requestContext.Trace(1980005, TraceLevel.Info, "ReleaseManagementService", "Service", "Service: DeleteProjectResources: projectId: {0}", (object) guid);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf requires it to be non-static")]
    public IEnumerable<ProjectIdData> QueryProjects(IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "ProjectService.QueryProjects", 1980007))
      {
        Func<ProjectSqlComponent, IEnumerable<ProjectIdData>> action = (Func<ProjectSqlComponent, IEnumerable<ProjectIdData>>) (component => component.QueryProjects());
        return requestContext.ExecuteWithinUsingWithComponent<ProjectSqlComponent, IEnumerable<ProjectIdData>>(action);
      }
    }

    private static void CheckPermissions(IVssRequestContext requestContext, string projectUri)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
      string securityToken = requestContext.GetService<IProjectService>().GetSecurityToken(requestContext, projectUri);
      IVssRequestContext requestContext1 = requestContext;
      string token = securityToken;
      int delete = TeamProjectPermissions.Delete;
      securityNamespace.CheckPermission(requestContext1, token, delete);
    }
  }
}
