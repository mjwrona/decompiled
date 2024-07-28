// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.AdministrationWebService4
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClientService(ComponentName = "TeamBuild", RegistrationName = "Build", ServiceName = "AdministrationService4", CollectionServiceIdentifier = "FB42B129-9E9B-4CF4-BA4F-F87859C2DB1C")]
  [WebService(Name = "AdministrationService", Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class AdministrationWebService4 : BuildWebServiceBase
  {
    [WebMethod]
    public List<BuildAgent> AddBuildAgents(List<BuildAgent> agents)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (AddBuildAgents), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildAgent>(nameof (agents), (IList<BuildAgent>) agents);
        this.EnterMethod(methodInformation);
        return this.BuildResourceService.AddBuildAgents(this.RequestContext, (IList<BuildAgent>) agents);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void DeleteBuildAgents([ClientType(typeof (Uri[]))] string[] agentUris)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteBuildAgents), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (agentUris), (IList<string>) agentUris);
        this.EnterMethod(methodInformation);
        this.BuildResourceService.DeleteBuildAgents(this.RequestContext, (IList<string>) agentUris);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public List<BuildAgentQueryResult> QueryBuildAgents(BuildAgentSpec[] agentSpecs)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildAgents), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildAgentSpec>(nameof (agentSpecs), (IList<BuildAgentSpec>) agentSpecs);
        this.EnterMethod(methodInformation);
        return this.BuildResourceService.QueryBuildAgents(this.RequestContext, (IList<BuildAgentSpec>) agentSpecs);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public BuildAgentQueryResult QueryBuildAgentsByUri(
      [ClientType(typeof (Uri[]))] string[] agentUris,
      string[] propertyNameFilters)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildAgentsByUri), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (agentUris), (IList<string>) agentUris);
        this.EnterMethod(methodInformation);
        return this.BuildResourceService.QueryBuildAgentsByUri(this.RequestContext, (IList<string>) agentUris, (IList<string>) propertyNameFilters);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [ClientServiceMethod(AsyncPattern = true)]
    public void UpdateBuildAgents(List<BuildAgentUpdateOptions> updates)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateBuildAgents), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildAgentUpdateOptions>(nameof (updates), (IList<BuildAgentUpdateOptions>) updates);
        this.EnterMethod(methodInformation);
        this.BuildResourceService.UpdateBuildAgents(this.RequestContext, (IList<BuildAgentUpdateOptions>) updates);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public List<BuildController> AddBuildControllers(List<BuildController> controllers)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("AddBuildController", MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildController>(nameof (controllers), (IList<BuildController>) controllers);
        this.EnterMethod(methodInformation);
        return this.BuildResourceService.AddBuildControllers(this.RequestContext, (IList<BuildController>) controllers);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void DeleteBuildControllers([ClientType(typeof (Uri[]))] string[] controllerUris)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteBuildControllers), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (controllerUris), (IList<string>) controllerUris);
        this.EnterMethod(methodInformation);
        this.BuildResourceService.DeleteBuildControllers(this.RequestContext, (IList<string>) controllerUris);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public List<BuildControllerQueryResult> QueryBuildControllers(
      BuildControllerSpec[] controllerSpecs)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildControllers), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildControllerSpec>(nameof (controllerSpecs), (IList<BuildControllerSpec>) controllerSpecs);
        this.EnterMethod(methodInformation);
        return this.BuildResourceService.QueryBuildControllers(this.RequestContext, (IList<BuildControllerSpec>) controllerSpecs);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public BuildControllerQueryResult QueryBuildControllersByUri(
      [ClientType(typeof (Uri[]))] string[] controllerUris,
      string[] propertyNameFilters,
      bool includeAgents)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildControllersByUri), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (controllerUris), (IList<string>) controllerUris);
        this.EnterMethod(methodInformation);
        return this.BuildResourceService.QueryBuildControllersByUri(this.RequestContext, (IList<string>) controllerUris, (IList<string>) propertyNameFilters, includeAgents);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [ClientServiceMethod(AsyncPattern = true)]
    public void UpdateBuildControllers(List<BuildControllerUpdateOptions> updates)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateBuildControllers), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildControllerUpdateOptions>(nameof (updates), (IList<BuildControllerUpdateOptions>) updates);
        this.EnterMethod(methodInformation);
        this.BuildResourceService.UpdateBuildControllers(this.RequestContext, (IList<BuildControllerUpdateOptions>) updates);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public BuildServiceHost AddBuildServiceHost(BuildServiceHost serviceHost)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (AddBuildServiceHost), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (serviceHost), (object) serviceHost);
        this.EnterMethod(methodInformation);
        return this.BuildResourceService.AddBuildServiceHost(this.RequestContext, serviceHost);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void DeleteBuildServiceHost([ClientType(typeof (Uri))] string serviceHostUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteBuildServiceHost), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (serviceHostUri), (object) serviceHostUri);
        this.EnterMethod(methodInformation);
        this.BuildResourceService.DeleteBuildServiceHost(this.RequestContext, serviceHostUri);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public BuildServiceHostQueryResult QueryBuildServiceHosts(string computer)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildServiceHosts), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (computer), (object) computer);
        this.EnterMethod(methodInformation);
        return this.BuildResourceService.QueryBuildServiceHosts(this.RequestContext, computer);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public BuildServiceHostQueryResult QueryBuildServiceHostsByUri([ClientType(typeof (Uri[]))] string[] serviceHostUris)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildServiceHostsByUri), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (serviceHostUris), (IList<string>) serviceHostUris);
        this.EnterMethod(methodInformation);
        return this.BuildResourceService.QueryBuildServiceHostsByUri(this.RequestContext, (IList<string>) serviceHostUris);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void UpdateBuildServiceHost(BuildServiceHostUpdateOptions update)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateBuildServiceHost), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (update), (object) update);
        this.EnterMethod(methodInformation);
        this.BuildResourceService.UpdateBuildServiceHost(this.RequestContext, update);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void AcquireServiceHost([ClientType(typeof (Uri))] string serviceHostUri, string ownerName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (AcquireServiceHost), MethodType.ReadWrite, EstimatedMethodCost.VeryLow);
        methodInformation.AddParameter(nameof (serviceHostUri), (object) serviceHostUri);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        this.EnterMethod(methodInformation);
        this.BuildResourceService.AcquireServiceHost(this.RequestContext, this.RequestContext.UniqueIdentifier, serviceHostUri, ownerName);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void ReleaseServiceHost([ClientType(typeof (Uri))] string serviceHostUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ReleaseServiceHost), MethodType.ReadWrite, EstimatedMethodCost.VeryLow);
        methodInformation.AddParameter(nameof (serviceHostUri), (object) serviceHostUri);
        this.EnterMethod(methodInformation);
        this.BuildResourceService.ReleaseServiceHost(this.RequestContext, this.RequestContext.UniqueIdentifier, serviceHostUri, false);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }
  }
}
