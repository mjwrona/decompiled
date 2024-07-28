// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.ProjectResourcePermissionsControllerBase`1
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net;

namespace Microsoft.TeamFoundation.Server.Types
{
  [ResolveTfsProjectFilter]
  public abstract class ProjectResourcePermissionsControllerBase<TResourceIdentifier> : 
    ResourcePermissionsControllerBase<TResourceIdentifier>,
    ITfsProjectApiController
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      ProjectResourcePermissionsControllerBase<TResourceIdentifier>.AddProjectExceptions(exceptionMap);
    }

    public static void AddProjectExceptions(ApiExceptionMapping exceptionMap)
    {
      exceptionMap.AddStatusCode<ProjectDoesNotExistWithNameException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<ProjectDoesNotExistException>(HttpStatusCode.NotFound);
    }

    public ProjectInfo ProjectInfo { get; set; }

    public Guid ProjectId => this.ProjectInfo != null ? this.ProjectInfo.Id : Guid.Empty;

    public string ProjectName => this.ProjectInfo != null ? this.ProjectInfo.Name : string.Empty;
  }
}
