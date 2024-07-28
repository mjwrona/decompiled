// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.AdministrationWebService
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClientService(ComponentName = "TeamBuild", RegistrationName = "Build", ServiceName = "AdministrationService", CollectionServiceIdentifier = "d1e9471d-7e69-4210-ad4c-3c941b245e2f")]
  [WebService]
  public sealed class AdministrationWebService : BuildWebServiceBase
  {
    [WebMethod]
    public List<BuildAgent2010> AddBuildAgents(List<BuildAgent2010> agents)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (AddBuildAgents), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildAgent2010>(nameof (agents), (IList<BuildAgent2010>) agents);
        this.EnterMethod(methodInformation);
        return RosarioHelper.AddBuildAgents(this.RequestContext, (IList<BuildAgent2010>) agents);
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
    public List<BuildAgentQueryResult2010> QueryBuildAgents(BuildAgentSpec2010[] agentSpecs)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildAgents), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildAgentSpec2010>(nameof (agentSpecs), (IList<BuildAgentSpec2010>) agentSpecs);
        this.EnterMethod(methodInformation);
        return RosarioHelper.QueryBuildAgents(this.RequestContext, (IList<BuildAgentSpec2010>) agentSpecs);
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
    public BuildAgentQueryResult2010 QueryBuildAgentsByUri([ClientType(typeof (Uri[]))] string[] agentUris)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildAgentsByUri), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (agentUris), (IList<string>) agentUris);
        this.EnterMethod(methodInformation);
        return RosarioHelper.QueryBuildAgentsByUri(this.RequestContext, (IList<string>) agentUris);
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
    public void UpdateBuildAgents(List<BuildAgentUpdateOptions2010> updates)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateBuildAgents), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildAgentUpdateOptions2010>(nameof (updates), (IList<BuildAgentUpdateOptions2010>) updates);
        this.EnterMethod(methodInformation);
        RosarioHelper.UpdateBuildAgents(this.RequestContext, (IList<BuildAgentUpdateOptions2010>) updates);
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
    public BuildAgentQueryResult2010 TestBuildAgentConnection([ClientType(typeof (Uri))] string agentUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (TestBuildAgentConnection), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (agentUri), (object) agentUri);
        this.EnterMethod(methodInformation);
        return RosarioHelper.TestBuildAgentConnection(this.RequestContext, agentUri);
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
    public List<BuildController2010> AddBuildControllers(List<BuildController2010> controllers)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("AddBuildController", MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildController2010>(nameof (controllers), (IList<BuildController2010>) controllers);
        this.EnterMethod(methodInformation);
        return RosarioHelper.AddBuildControllers(this.RequestContext, (IList<BuildController2010>) controllers);
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
    public List<BuildControllerQueryResult2010> QueryBuildControllers(
      BuildControllerSpec2010[] controllerSpecs)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildControllers), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildControllerSpec2010>(nameof (controllerSpecs), (IList<BuildControllerSpec2010>) controllerSpecs);
        this.EnterMethod(methodInformation);
        return RosarioHelper.QueryBuildControllers(this.RequestContext, (IList<BuildControllerSpec2010>) controllerSpecs);
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
    public BuildControllerQueryResult2010 QueryBuildControllersByUri(
      [ClientType(typeof (Uri[]))] string[] controllerUris,
      bool includeAgents)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildControllersByUri), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (controllerUris), (IList<string>) controllerUris);
        this.EnterMethod(methodInformation);
        return RosarioHelper.QueryBuildControllersByUri(this.RequestContext, (IList<string>) controllerUris, includeAgents);
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
    public void UpdateBuildControllers(List<BuildControllerUpdateOptions2010> updates)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateBuildControllers), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<BuildControllerUpdateOptions2010>(nameof (updates), (IList<BuildControllerUpdateOptions2010>) updates);
        this.EnterMethod(methodInformation);
        RosarioHelper.UpdateBuildControllers(this.RequestContext, (IList<BuildControllerUpdateOptions2010>) updates);
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
    public BuildControllerQueryResult2010 TestBuildControllerConnection([ClientType(typeof (Uri))] string controllerUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (TestBuildControllerConnection), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (controllerUri), (object) controllerUri);
        this.EnterMethod(methodInformation);
        return RosarioHelper.TestBuildControllerConnection(this.RequestContext, controllerUri);
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
    public BuildServiceHost2010 AddBuildServiceHost(BuildServiceHost2010 serviceHost)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (AddBuildServiceHost), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (serviceHost), (object) serviceHost);
        this.EnterMethod(methodInformation);
        return RosarioHelper.AddBuildServiceHost(this.RequestContext, serviceHost);
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
    public BuildServiceHostQueryResult2010 QueryBuildServiceHosts(string computer)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildServiceHosts), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (computer), (object) computer);
        this.EnterMethod(methodInformation);
        return RosarioHelper.QueryBuildServiceHosts(this.RequestContext, computer);
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
    public BuildServiceHostQueryResult2010 QueryBuildServiceHostsByUri([ClientType(typeof (Uri[]))] string[] serviceHostUris)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBuildServiceHostsByUri), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (serviceHostUris), (IList<string>) serviceHostUris);
        this.EnterMethod(methodInformation);
        return RosarioHelper.QueryBuildServiceHostsByUri(this.RequestContext, (IList<string>) serviceHostUris);
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
    public void UpdateBuildServiceHost(BuildServiceHostUpdateOptions2010 update)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateBuildServiceHost), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (update), (object) update);
        this.EnterMethod(methodInformation);
        RosarioHelper.UpdateBuildServiceHost(this.RequestContext, update);
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
    public BuildServiceHostQueryResult2010 TestBuildServiceHostConnections([ClientType(typeof (Uri))] string hostUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (TestBuildServiceHostConnections), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (hostUri), (object) hostUri);
        this.EnterMethod(methodInformation);
        return RosarioHelper.TestBuildServiceHostConnections(this.RequestContext, hostUri);
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
