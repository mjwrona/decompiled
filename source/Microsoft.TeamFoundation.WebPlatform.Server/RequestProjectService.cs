// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WebPlatform.Server.RequestProjectService
// Assembly: Microsoft.TeamFoundation.WebPlatform.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BDF91478-A3ED-4B5B-AA51-9473C7AE6182
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WebPlatform.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;

namespace Microsoft.TeamFoundation.WebPlatform.Server
{
  internal class RequestProjectService : IRequestProjectService, IVssFrameworkService
  {
    internal const string c_requestProjectKey = "request-project";
    internal const string c_dataProviderScopeDataKey = "DataProviderQuery.Scope";
    internal const string c_projectScopeName = "project";

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public string GetRequestProjectIdentifier(IVssRequestContext requestContext) => requestContext.GetService<IContributionRoutingService>().GetRouteValue<string>(requestContext, "project");

    public ProjectInfo GetProject(IVssRequestContext requestContext)
    {
      string projectIdentifier = this.GetRequestProjectIdentifier(requestContext);
      if (string.IsNullOrEmpty(projectIdentifier))
        return (ProjectInfo) null;
      object projectFromRequestName;
      if (!requestContext.RootContext.Items.TryGetValue("request-project", out projectFromRequestName))
      {
        projectFromRequestName = (object) TfsProjectHelpers.GetProjectFromRequestName(requestContext, projectIdentifier);
        object obj;
        if (requestContext.RootContext.Items.TryGetValue("DataProviderQuery.Scope", out obj) && obj is IDataProviderScope dataProviderScope && string.Equals(dataProviderScope.Name, "project", StringComparison.OrdinalIgnoreCase))
        {
          ProjectInfo projectInfo = projectFromRequestName as ProjectInfo;
          if (!string.Equals(dataProviderScope.Value, projectInfo.Id.ToString(), StringComparison.OrdinalIgnoreCase) && !string.Equals(dataProviderScope.Value, projectInfo.Name, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Scoped project does not match the project in the resolved route values. " + string.Format("Scoped project: '{0}', project in the resolved route values: '{1}' - '{2}'", (object) dataProviderScope.Value, (object) projectInfo.Name, (object) projectInfo.Id));
        }
        requestContext.RootContext.Items["request-project"] = projectFromRequestName;
      }
      return projectFromRequestName as ProjectInfo;
    }

    public bool IsProjectSpecifiedById(IVssRequestContext requestContext)
    {
      string projectIdentifier = this.GetRequestProjectIdentifier(requestContext);
      return !string.IsNullOrEmpty(projectIdentifier) && Guid.TryParse(projectIdentifier, out Guid _);
    }
  }
}
