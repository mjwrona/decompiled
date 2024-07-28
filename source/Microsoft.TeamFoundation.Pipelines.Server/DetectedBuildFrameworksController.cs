// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.DetectedBuildFrameworksController
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "pipelines", ResourceName = "detectedbuildframeworks")]
  public class DetectedBuildFrameworksController : TfsProjectApiController
  {
    [HttpGet]
    public IEnumerable<Microsoft.TeamFoundation.Pipelines.WebApi.DetectedBuildFramework> GetDetectedBuildFrameworks(
      [ClientQueryParameter] string repositoryType = null,
      [ClientQueryParameter] string repositoryId = null,
      [ClientQueryParameter] string branch = null,
      [ClientParameterType(typeof (BuildFrameworkDetectionType), false)] string detectionType = null,
      [ClientQueryParameter] Guid? serviceConnectionId = null)
    {
      PipelineHelper.VerifyRepositoryParameters(this.TfsRequestContext, repositoryType, repositoryId, branch, serviceConnectionId);
      BuildFrameworkDetectionType detectionType1 = BuildFrameworkDetectionType.Full;
      if (!string.IsNullOrEmpty(detectionType))
        detectionType1 = (BuildFrameworkDetectionType) System.Enum.Parse(typeof (BuildFrameworkDetectionType), detectionType, true);
      return this.TfsRequestContext.GetService<IBuildFrameworkDetectionService>().Detect(this.TfsRequestContext, this.ProjectId, repositoryType, repositoryId, serviceConnectionId, branch, detectionType1).Select<DetectedBuildFramework, Microsoft.TeamFoundation.Pipelines.WebApi.DetectedBuildFramework>((Func<DetectedBuildFramework, Microsoft.TeamFoundation.Pipelines.WebApi.DetectedBuildFramework>) (d => d.ToWebApi()));
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentNullException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BuildRepositoryTypeNotSupportedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<MissingEndpointInformationException>(HttpStatusCode.NotFound);
    }
  }
}
