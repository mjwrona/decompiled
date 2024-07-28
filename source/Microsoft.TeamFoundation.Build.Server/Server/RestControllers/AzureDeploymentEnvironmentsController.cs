// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.RestControllers.AzureDeploymentEnvironmentsController
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build.Server.RestControllers
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Build", ResourceName = "DeploymentEnvironments", ResourceVersion = 1)]
  public sealed class AzureDeploymentEnvironmentsController : TfsProjectApiController
  {
    private static Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();

    static AzureDeploymentEnvironmentsController() => AzureDeploymentEnvironmentsController.s_httpExceptions.Add(typeof (DeploymentEnvironmentNotFoundException), HttpStatusCode.NotFound);

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) AzureDeploymentEnvironmentsController.s_httpExceptions;

    [HttpGet]
    public IEnumerable<Microsoft.TeamFoundation.Build.Server.DeploymentEnvironmentMetadata> GetDeploymentEnvironments(
      string serviceName = "")
    {
      return (IEnumerable<Microsoft.TeamFoundation.Build.Server.DeploymentEnvironmentMetadata>) this.TfsRequestContext.GetService<TeamFoundationDeploymentService>().QueryDeploymentEnvironments(this.TfsRequestContext, this.ProjectInfo.Name, serviceName);
    }

    [HttpPut]
    public Microsoft.TeamFoundation.Build.Server.DeploymentEnvironmentMetadata CreateDeploymentEnvironments(
      DeploymentEnvironmentApiData deploymentEnvironmentApiData)
    {
      ArgumentUtility.CheckForNull<DeploymentEnvironmentApiData>(deploymentEnvironmentApiData, nameof (deploymentEnvironmentApiData));
      ArgumentUtility.CheckStringForNullOrEmpty(deploymentEnvironmentApiData.SubscriptionId, "deploymentEnvironmentApiData.SubscriptionId");
      ArgumentUtility.CheckStringForNullOrEmpty(deploymentEnvironmentApiData.DeploymentName, "deploymentEnvironmentApiData.DeploymentName");
      ArgumentUtility.CheckStringForNullOrEmpty(deploymentEnvironmentApiData.ProjectName, "deploymentEnvironmentApiData.ProjectName");
      ArgumentUtility.CheckStringForNullOrEmpty(deploymentEnvironmentApiData.SubscriptionName, "deploymentEnvironmentApiData.SubscriptionName");
      if (deploymentEnvironmentApiData.Cert.IsNullOrEmpty<char>())
      {
        ArgumentUtility.CheckStringForNullOrEmpty(deploymentEnvironmentApiData.UserName, "deploymentEnvironmentApiData.UserName");
        ArgumentUtility.CheckStringForNullOrEmpty(deploymentEnvironmentApiData.Password, "deploymentEnvironmentApiData.Password");
      }
      return this.TfsRequestContext.GetService<TeamFoundationDeploymentService>().CreateDeploymentEnvironmentForAzure(this.TfsRequestContext, deploymentEnvironmentApiData);
    }
  }
}
