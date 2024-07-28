// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TfsTestAgentCatalogService
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class TfsTestAgentCatalogService : ITfsTestAgentCatalogService, IVssFrameworkService
  {
    private IPropertyServiceHelper _propertyServiceHelper;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public TestAgent CreateAgent(TestExecutionRequestContext requestContext, TestAgent testAgent)
    {
      ArgumentUtility.CheckForNull<TestAgent>(testAgent, nameof (testAgent), requestContext.RequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(testAgent.Name, "testAgent.Name", requestContext.RequestContext.ServiceName);
      ArgumentUtility.CheckForNull<ShallowReference>(testAgent.DtlEnvironment, "testAgent.DtlEnvironment", requestContext.RequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(testAgent.DtlEnvironment.Url, "testAgent.DtlEnvironment.Url", requestContext.RequestContext.ServiceName);
      ArgumentUtility.CheckForNull<ShallowReference>(testAgent.DtlMachine, "testAgent.DtlMachine", requestContext.RequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(testAgent.DtlMachine.Name, "testAgent.DtlMachine.Name", requestContext.RequestContext.ServiceName);
      IVssRequestContext requestContext1 = requestContext.RequestContext;
      try
      {
        requestContext1.Trace(6200001, TraceLevel.Info, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, "Server got request from testAgent for Register. testAgent.Name : {0}", (object) testAgent.Name);
        requestContext1.Trace(6200002, TraceLevel.Info, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, "Environment validated. enivironment url : {0}", (object) testAgent.DtlEnvironment.Url);
        TeamProjectReference projectReference = TfsTestAgentCatalogService.GetTeamProjectReference(requestContext, testAgent.DtlEnvironment.Url);
        TeamProjectReference project = Utilities.HydrateProjectReference(requestContext, projectReference);
        string projectUri = Utilities.GetProjectUri(requestContext, project.Name);
        requestContext.SecurityManager.CheckPermission(requestContext, DTAPermissionType.AgentsCreate, projectUri);
        using (DtaAgentDatabase component = requestContext.RequestContext.CreateComponent<DtaAgentDatabase>())
          testAgent = component.RegisterAgent(testAgent);
        Dictionary<string, object> eventData = new Dictionary<string, object>()
        {
          {
            "Agent",
            (object) testAgent
          }
        };
        CILogger.Instance.PublishCI(requestContext, "AgentRegistered", eventData);
        requestContext1.Trace(6200004, TraceLevel.Info, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, "Test Agent is successfully registered to DB. AgentId : {0}", (object) testAgent.Id);
        this.StoreProjectReferenceForAgent(requestContext, testAgent.Id, project);
        requestContext1.Trace(6200008, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, "Test Agent is successfully registered to Tagging Service. AgentId : {0}", (object) testAgent.Id);
      }
      catch (Exception ex)
      {
        Dictionary<string, object> eventData = new Dictionary<string, object>()
        {
          {
            "DtlEnvironmentUrl",
            (object) testAgent.DtlEnvironment.Url
          },
          {
            "DtlMachineName",
            (object) testAgent.DtlMachine.Name
          },
          {
            "Exception",
            (object) ex
          }
        };
        CILogger.Instance.PublishCI(requestContext, "AgentRegistrationFailed", eventData);
        throw;
      }
      return testAgent;
    }

    private static TeamProjectReference GetTeamProjectReference(
      TestExecutionRequestContext requestContext,
      string dtaEnvironmentUrl)
    {
      string fromEnvironmentUrl = Utilities.GetProjectNameFromEnvironmentUrl(requestContext, dtaEnvironmentUrl);
      Guid result = Guid.Empty;
      if (!Guid.TryParse(fromEnvironmentUrl, out result))
        return new TeamProjectReference()
        {
          Name = fromEnvironmentUrl
        };
      return new TeamProjectReference() { Id = result };
    }

    private void ThrowInvalidDtlEnvironmentUrlExceptions(TestExecutionRequestContext requestContext)
    {
      requestContext.RequestContext.Trace(6200009, TraceLevel.Error, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, "Throwing invalid environment url Exception");
      throw new TestExecutionObjectNotFoundException(TestExecutionServiceResources.InvalidDtlEnvironmentUrl);
    }

    public void UnRegisterAgent(TestExecutionRequestContext requestContext, int testAgentId)
    {
      IVssRequestContext requestContext1 = requestContext.RequestContext;
      requestContext1.Trace(6200010, TraceLevel.Info, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, "Test Agent requested to Delete itself. testAgentId : {0}", (object) testAgentId);
      try
      {
        TeamProjectReference referenceForTestagent = TestRunPropertiesService.GetProjectReferenceForTestagent(requestContext, testAgentId);
        string projectUri = Utilities.GetProjectUri(requestContext, referenceForTestagent.Name);
        requestContext.SecurityManager.CheckPermission(requestContext, DTAPermissionType.AgentsDelete, projectUri);
        using (DtaAgentDatabase component = requestContext.RequestContext.CreateComponent<DtaAgentDatabase>())
          component.UnRegisterAgent(testAgentId);
        this.PropertyServiceHelper.Delete(requestContext, requestContext.TestAgentArtifactKindId, testAgentId);
        requestContext1.Trace(6200011, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, "Unregistered the test agent. testAgentId : {0}", (object) testAgentId);
        Dictionary<string, object> eventData = new Dictionary<string, object>()
        {
          {
            "AgentId",
            (object) testAgentId
          }
        };
        CILogger.Instance.PublishCI(requestContext, "AgentUnRegistered", eventData);
      }
      catch (TestExecutionObjectNotFoundException ex)
      {
        requestContext1.Trace(6200012, TraceLevel.Error, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, "Delete test agent failed with error : {0} for Agent : {1}.", (object) ex.Message, (object) testAgentId);
      }
    }

    public TestAgent GetAgent(TestExecutionRequestContext requestContext, int testAgentId)
    {
      IVssRequestContext requestContext1 = requestContext.RequestContext;
      requestContext1.Trace(6200013, TraceLevel.Info, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, "GetAgent() requested from test agent. testAgentId : {0}", (object) testAgentId);
      TeamProjectReference referenceForTestagent = TestRunPropertiesService.GetProjectReferenceForTestagent(requestContext, testAgentId);
      string projectUri = Utilities.GetProjectUri(requestContext, referenceForTestagent.Name);
      requestContext.SecurityManager.CheckPermission(requestContext, DTAPermissionType.AgentsGet, projectUri);
      TestAgent agent;
      using (DtaAgentDatabase component = requestContext.RequestContext.CreateComponent<DtaAgentDatabase>())
        agent = component.QueryTestAgent(testAgentId);
      requestContext1.Trace(6200014, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, "Test agent found. testAgentId : {0}", (object) testAgentId);
      return agent;
    }

    public List<TestAgent> GetAgentsForRun(
      TestExecutionRequestContext requestContext,
      int testRunId)
    {
      IVssRequestContext requestContext1 = requestContext.RequestContext;
      requestContext1.Trace(6200951, TraceLevel.Info, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, string.Format("GetAgentsForRun() requested from test agent. testRunId : {0}", (object) testRunId));
      List<TestAgent> list;
      using (DtaAgentDatabase component = requestContext.RequestContext.CreateComponent<DtaAgentDatabase>())
        list = component.QueryTestAgents(new TestAgentsQuery()
        {
          TestRunId = testRunId
        }).ToList<TestAgent>();
      requestContext1.Trace(6200952, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer, string.Format("Number of Test agent(s) found: {0}", (object) list.Count));
      return list;
    }

    public List<TestAgent> GetAgentsMarkedDownForRun(
      TestExecutionRequestContext requestContext,
      int testRunId)
    {
      List<TestAgent> agentsForRun = this.GetAgentsForRun(requestContext, testRunId);
      List<TestAgent> markedDownForRun = new List<TestAgent>();
      foreach (TestAgent testAgent in agentsForRun)
      {
        if (testAgent.LastHeartBeat != DateTime.MinValue && (DateTime.UtcNow - testAgent.LastHeartBeat).TotalMilliseconds > DtaConstants.DefaultAgentConnectionTimeOut.TotalMilliseconds)
          markedDownForRun.Add(testAgent);
      }
      return markedDownForRun;
    }

    private static string GetCapabilitiesAsSingleString(
      TestExecutionRequestContext requestContext,
      TestAgent testAgent)
    {
      List<string> values = new List<string>();
      if (testAgent.Capabilities != null)
        values.AddRange((IEnumerable<string>) testAgent.Capabilities);
      return string.Join(DtaConstants.TagSeparator, (IEnumerable<string>) values);
    }

    private void StoreProjectReferenceForAgent(
      TestExecutionRequestContext requestContext,
      int agentId,
      TeamProjectReference project)
    {
      this.PropertyServiceHelper.AddOrUpdate(requestContext, requestContext.TestAgentArtifactKindId, agentId, (IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          TestPropertiesConstants.AutomationRunProjectReference,
          (object) project.Id.ToString()
        }
      });
    }

    public IPropertyServiceHelper PropertyServiceHelper
    {
      get => this._propertyServiceHelper ?? (this._propertyServiceHelper = (IPropertyServiceHelper) new Microsoft.TeamFoundation.TestExecution.Server.PropertyServiceHelper());
      set => this._propertyServiceHelper = value;
    }
  }
}
