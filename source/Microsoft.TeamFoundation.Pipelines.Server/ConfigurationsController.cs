// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.ConfigurationsController
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "pipelines", ResourceName = "configurations")]
  public class ConfigurationsController : TfsProjectApiController
  {
    [HttpGet]
    public IEnumerable<Microsoft.TeamFoundation.Pipelines.WebApi.ConfigurationFile> GetConfigurations(
      [ClientQueryParameter] string repositoryType = null,
      [ClientQueryParameter] string repositoryId = null,
      [ClientQueryParameter] string branch = null,
      [ClientQueryParameter] Guid? serviceConnectionId = null)
    {
      PipelineHelper.VerifyRepositoryParameters(this.TfsRequestContext, repositoryType, repositoryId, branch, serviceConnectionId);
      ConfigurationFile configurationFile = this.TfsRequestContext.GetService<IRepositoryAnalysisService>().GetExistingConfigurationFile(this.TfsRequestContext, this.ProjectId, repositoryType, repositoryId, serviceConnectionId, branch, (string) null);
      if (configurationFile == null)
        return Enumerable.Empty<Microsoft.TeamFoundation.Pipelines.WebApi.ConfigurationFile>();
      return (IEnumerable<Microsoft.TeamFoundation.Pipelines.WebApi.ConfigurationFile>) new List<Microsoft.TeamFoundation.Pipelines.WebApi.ConfigurationFile>()
      {
        configurationFile.ToWebApiConfigurationFile()
      };
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentNullException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BuildRepositoryTypeNotSupportedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ExternalSourceProviderException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<VssServiceException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<GitRepositoryNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<MissingEndpointInformationException>(HttpStatusCode.NotFound);
    }
  }
}
