// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildDeploymentWebService
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClientService(ComponentName = "TeamBuild", RegistrationName = "Build", ServiceName = "BuildDeploymentService", CollectionServiceIdentifier = "3561F2FC-F755-481B-A74A-BE488B7179E3")]
  [WebService(Name = "BuildDeploymentService", Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class BuildDeploymentWebService : DeployWebServiceBase
  {
    [WebMethod]
    public DeploymentEnvironmentMetadata CreateDeploymentEnvironment(
      DeploymentEnvironmentCreationData deploymentEnvironmentCreationData)
    {
      try
      {
        new MethodInformation(nameof (CreateDeploymentEnvironment), MethodType.Normal, EstimatedMethodCost.Low).AddParameter(nameof (deploymentEnvironmentCreationData), (object) deploymentEnvironmentCreationData);
        return this.DeploymentService.CreateDeploymentEnvironment(this.RequestContext, deploymentEnvironmentCreationData);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "Build", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public List<DeploymentEnvironmentMetadata> QueryDeploymentEnvironments(string teamProject)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("GetDeploymentEnvironments", MethodType.Normal, EstimatedMethodCost.Low);
        return this.DeploymentService.QueryDeploymentEnvironments(this.RequestContext, teamProject);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "Build", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public DeploymentEnvironment GetDeploymentEnvironment(
      string environmentName,
      string teamProject)
    {
      try
      {
        new MethodInformation(nameof (GetDeploymentEnvironment), MethodType.Normal, EstimatedMethodCost.Low).AddParameter(nameof (environmentName), (object) environmentName);
        return this.DeploymentService.GetDeploymentEnvironment(this.RequestContext, environmentName, teamProject);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "Build", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void DeleteDeploymentEnvironment(string environmentName, string teamProject)
    {
      try
      {
        new MethodInformation(nameof (DeleteDeploymentEnvironment), MethodType.Normal, EstimatedMethodCost.Low).AddParameter(nameof (environmentName), (object) environmentName);
        this.DeploymentService.DeleteDeploymentEnvironment(this.RequestContext, environmentName, teamProject);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "Build", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void ConnectAzureWebsite(
      string teamProject,
      string subscriptionId,
      string webspace,
      string website)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ConnectAzureWebsite), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProject), (object) teamProject);
        methodInformation.AddParameter(nameof (subscriptionId), (object) subscriptionId);
        methodInformation.AddParameter(nameof (webspace), (object) webspace);
        methodInformation.AddParameter(nameof (website), (object) website);
        this.DeploymentService.ConnectAzureWebsite(this.RequestContext, teamProject, subscriptionId, webspace, website);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "Build", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [ClientServiceMethod(AsyncPattern = true, SyncPattern = true)]
    public BuildDeployment CreateBuildDeployment(
      [ClientType(typeof (Uri))] string deploymentUri,
      [ClientType(typeof (Uri))] string sourceUri,
      string environmentName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CreateBuildDeployment), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (deploymentUri), (object) deploymentUri);
        methodInformation.AddParameter(nameof (sourceUri), (object) sourceUri);
        return this.DeploymentService.CreateBuildDeployment(this.RequestContext, deploymentUri, sourceUri, environmentName);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "Build", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [ClientServiceMethod(AsyncPattern = true, SyncPattern = true)]
    public void AddBuildDeploymentProperty([ClientType(typeof (Uri))] string deploymentUri, string key, string value)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (AddBuildDeploymentProperty), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (deploymentUri), (object) deploymentUri);
        methodInformation.AddParameter(nameof (key), (object) key);
        methodInformation.AddParameter(nameof (value), (object) value);
        this.DeploymentService.AddBuildDeploymentProperty(this.RequestContext, deploymentUri, key, value);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "Build", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public List<BuildDeployment> QueryDeployments(BuildDeploymentSpec spec)
    {
      try
      {
        new MethodInformation(nameof (QueryDeployments), MethodType.Normal, EstimatedMethodCost.Moderate).AddParameter(nameof (spec), (object) spec);
        return this.DeploymentService.QueryDeployments(this.RequestContext, spec);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "Build", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public List<BuildDeployment> QueryDeploymentsByUri([ClientType(typeof (Uri[]))] string[] deploymentUris)
    {
      try
      {
        new MethodInformation("QueryDeployments", MethodType.Normal, EstimatedMethodCost.Moderate).AddArrayParameter<string>(nameof (deploymentUris), (IList<string>) deploymentUris);
        return this.DeploymentService.QueryDeploymentsByUri(this.RequestContext, (IList<string>) deploymentUris);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "Build", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public BuildQueueQueryResult Redeploy(
      [ClientType(typeof (Uri))] string deploymentUri,
      AzureDeploymentSlot deploymentSlot,
      bool updateSlot)
    {
      try
      {
        new MethodInformation(nameof (Redeploy), MethodType.Normal, EstimatedMethodCost.Moderate).AddParameter(nameof (deploymentUri), (object) deploymentUri);
        return this.DeploymentService.Redeploy(this.RequestContext, deploymentUri, deploymentSlot, updateSlot);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "Build", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void ConnectAzureCloudApp(
      string teamProject,
      string subscriptionId,
      string azurePublishProfile)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ConnectAzureCloudApp), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProject), (object) teamProject);
        methodInformation.AddParameter(nameof (subscriptionId), (object) subscriptionId);
        methodInformation.AddParameter(nameof (azurePublishProfile), (object) azurePublishProfile);
        this.DeploymentService.ConnectAzureCloudApp(this.RequestContext, teamProject, subscriptionId, azurePublishProfile);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "Build", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void DisconnectAzureCloudApp(string teamProject, string hostedServiceName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DisconnectAzureCloudApp), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProject), (object) teamProject);
        methodInformation.AddParameter(nameof (hostedServiceName), (object) hostedServiceName);
        this.DeploymentService.DisconnectAzureCloudApp(this.RequestContext, teamProject, hostedServiceName);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "Build", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void DisconnectAzureWebsite(string teamProject, string website)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DisconnectAzureWebsite), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (teamProject), (object) teamProject);
        methodInformation.AddParameter(nameof (website), (object) website);
        this.DeploymentService.DisconnectAzureWebsite(this.RequestContext, teamProject, website);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "Build", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }
  }
}
