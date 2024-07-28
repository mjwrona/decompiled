// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.ServiceEndpointShareService
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  public class ServiceEndpointShareService : IVssFrameworkService
  {
    private const string c_layer = "ServiceEndpointShareService";

    public ServiceEndpointShareService() => this.ServiceEndpointSecurity = new ServiceEndpointSecurity();

    protected ServiceEndpointSecurity ServiceEndpointSecurity { get; set; }

    public void ShareEndpointWithProject(
      IVssRequestContext requestContext,
      Guid endpointId,
      string fromProject,
      string withProject)
    {
      ProjectInfo projectInfo1 = requestContext.IsFeatureEnabled("WebAccess.ServiceEndpoints.ShareAcrossProjects") ? requestContext.GetProjectInfo(fromProject) : throw new ServiceEndpointException(ServiceEndpointResources.SharingServiceConnectinNotAllowed());
      ProjectInfo projectInfo2 = requestContext.GetProjectInfo(withProject);
      if (projectInfo1.Id.Equals(projectInfo2.Id))
        throw new ServiceEndpointException(ServiceEndpointResources.TryingToShareEndpointWithSameProject());
      if (requestContext.IsFeatureEnabled("ServiceEndpoints.NewServiceEndpointAPIs"))
        this.ServiceEndpointSecurity.CheckPermission(requestContext, ServiceEndpointSecurity.Collection, endpointId.ToString("D"), 2, false, (Func<IVssRequestContext, string>) (context => ServiceEndpointResources.ServiceEndpointAccessDeniedForCollectionAdminOperation()));
      else
        this.ServiceEndpointSecurity.CheckPermission(requestContext, projectInfo1.Id.ToString("D"), endpointId.ToString("D"), 2, false, (Func<IVssRequestContext, string>) (context => ServiceEndpointResources.EndpointAccessDeniedForAdminOperation()));
      requestContext.GetService<PlatformServiceEndpointService>().CheckCreatePermissions(requestContext, projectInfo2.Id);
      this.ServiceEndpointSecurity.InitializeServiceEndpointSecurity(requestContext, endpointId, projectInfo2.Id, requestContext.GetUserIdentity());
      using (new MethodScope(requestContext, nameof (ServiceEndpointShareService), nameof (ShareEndpointWithProject)))
      {
        using (ServiceEndpointComponent component = requestContext.CreateComponent<ServiceEndpointComponent>())
          component.ShareServiceEndpoint(endpointId, projectInfo1.Id, projectInfo2.Id);
      }
    }

    public List<ProjectReference> QuerySharedProjects(
      IVssRequestContext requestContext,
      Guid endpointId,
      string project)
    {
      Guid projectId = requestContext.GetProjectInfo(project).Id;
      this.ServiceEndpointSecurity.CheckPermission(requestContext, projectId.ToString("D"), endpointId.ToString("D"), 1, true, (Func<IVssRequestContext, string>) (context => ServiceEndpointResources.EndpointAccessDeniedForUseOperation()));
      List<Guid> source = requestContext.RunSynchronously<List<Guid>>((Func<Task<List<Guid>>>) (() => ServiceEndpointShareService.QuerySharedProjectsInternal(requestContext, endpointId, projectId)));
      IVssRequestContext systemContext = requestContext.Elevate();
      Func<Guid, ProjectInfo> selector = (Func<Guid, ProjectInfo>) (x => systemContext.GetProjectInfo(x.ToString("D")));
      return source.Select<Guid, ProjectInfo>(selector).Select<ProjectInfo, ProjectReference>((Func<ProjectInfo, ProjectReference>) (x => new ProjectReference()
      {
        Id = x.Id,
        Name = x.Name
      })).ToList<ProjectReference>();
    }

    private static async Task<List<Guid>> QuerySharedProjectsInternal(
      IVssRequestContext requestContext,
      Guid endpointId,
      Guid projectId)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (ServiceEndpointShareService), nameof (QuerySharedProjectsInternal));
      List<ServiceEndpointProjectReferenceResult> source;
      try
      {
        using (ServiceEndpointComponent scs = requestContext.CreateComponent<ServiceEndpointComponent>())
        {
          ServiceEndpointComponent endpointComponent = scs;
          List<Guid> endpointIds = new List<Guid>();
          endpointIds.Add(endpointId);
          Guid project = projectId;
          source = await endpointComponent.QueryServiceEndpointSharedProjectsAsync((IEnumerable<Guid>) endpointIds, project);
        }
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
      return source.Select<ServiceEndpointProjectReferenceResult, Guid>((Func<ServiceEndpointProjectReferenceResult, Guid>) (x => x.ProjectId)).ToList<Guid>();
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckProjectCollectionRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
