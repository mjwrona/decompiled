// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.PlatformDistributedTaskLibraryService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class PlatformDistributedTaskLibraryService : 
    IDistributedTaskLibraryService,
    IVssFrameworkService
  {
    public void DeleteTeamProject(IVssRequestContext systemRequestContext, Guid projectId)
    {
      systemRequestContext.CheckSystemRequestContext();
      systemRequestContext.GetService<IServiceEndpointService2>().DeleteTeamProject(systemRequestContext, projectId);
      systemRequestContext.GetService<MetaTaskService>().DeleteTeamProject(systemRequestContext, projectId);
      systemRequestContext.GetService<PlatformVariableGroupService>().DeleteTeamProject(systemRequestContext, projectId);
      systemRequestContext.GetService<PlatformSecureFileService>().DeleteTeamProject(systemRequestContext, projectId);
      using (DistributedTaskLibraryComponent component = systemRequestContext.CreateComponent<DistributedTaskLibraryComponent>())
        component.DeleteTeamProject(projectId);
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }
  }
}
