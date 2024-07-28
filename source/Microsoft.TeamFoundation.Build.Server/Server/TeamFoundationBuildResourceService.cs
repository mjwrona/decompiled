// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.TeamFoundationBuildResourceService
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server.Compatibility;
using Microsoft.TeamFoundation.Build.Server.DataAccess;
using Microsoft.TeamFoundation.Build.Server.ServiceProxies;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Channels;

namespace Microsoft.TeamFoundation.Build.Server
{
  public sealed class TeamFoundationBuildResourceService : IVssFrameworkService
  {
    private const string c_serviceUpdateLock = "tfs://Build/ServiceUpdate";
    private TeamFoundationBuildHost m_buildHost;
    private IVssServiceHost m_serviceHost;
    private IVssDeploymentServiceHost m_deploymentHost;
    private InputQueue<TeamFoundationBuildResourceService.DeleteArguments> m_deleteQueue;
    private ArtifactKind m_controllerArtifactKind;
    private ArtifactKind m_agentArtifactKind;

    public List<BuildAgent> AddBuildAgents(
      IVssRequestContext requestContext,
      IList<BuildAgent> agents)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (AddBuildAgents));
      TeamFoundationBuildService.CheckXamlEnabled(requestContext);
      Validation.CheckValidatable<BuildAgent>(requestContext, nameof (agents), agents, false, ValidationContext.Add);
      this.m_buildHost.SecurityManager.CheckPrivilege(requestContext, AdministrationPermissions.ManageBuildResources, false);
      List<BuildAgent> buildAgentList = new List<BuildAgent>();
      List<StartBuildData> startBuildData = new List<StartBuildData>();
      List<AgentReservationData> reservations = new List<AgentReservationData>();
      Dictionary<string, BuildServiceHost> dictionary;
      using (new TeamFoundationBuildResourceService.ServiceLockToken(requestContext, (ICollection<string>) null))
      {
        using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
        {
          HashSet<string> source = new HashSet<string>();
          using (ResultCollection resultCollection = component.AddBuildAgents(agents))
          {
            foreach (BuildAgent buildAgent in resultCollection.GetCurrent<BuildAgent>().Items)
            {
              buildAgentList.Add(buildAgent);
              source.Add(buildAgent.ServiceHostUri);
              requestContext.Trace(0, TraceLevel.Info, "BuildAdministration", "Service", "Added agent '{0}'", (object) buildAgent.Uri);
            }
            resultCollection.NextResult();
            startBuildData.AddRange((IEnumerable<StartBuildData>) resultCollection.GetCurrent<StartBuildData>().Items);
            resultCollection.NextResult();
            reservations.AddRange((IEnumerable<AgentReservationData>) resultCollection.GetCurrent<AgentReservationData>().Items);
          }
          using (ResultCollection resultCollection = component.QueryBuildServiceHostsByUri((IList<string>) source.ToList<string>()))
            dictionary = resultCollection.GetCurrent<BuildServiceHost>().Items.ToDictionary<BuildServiceHost, string>((Func<BuildServiceHost, string>) (x => x.Uri), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        }
      }
      BuildController.StartBuilds(requestContext, (IList<StartBuildData>) startBuildData);
      BuildController.NotifyAgentsAvailable(requestContext, (IEnumerable<AgentReservationData>) reservations);
      TeamFoundationEventService service = requestContext.GetService<TeamFoundationEventService>();
      for (int index = 0; index < buildAgentList.Count; ++index)
      {
        agents[index].Uri = buildAgentList[index].Uri;
        requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Publishing notification for agent '{0}'", (object) agents[index].Uri);
        BuildResourceChangedEvent notificationEvent = new BuildResourceChangedEvent(requestContext, ChangedType.Added, agents[index]);
        service.PublishNotification(requestContext, (object) notificationEvent);
        BuildServiceHost buildServiceHost;
        if (dictionary.TryGetValue(agents[index].ServiceHostUri, out buildServiceHost))
        {
          requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Notifying service host '{0}' for agent '{1}'", (object) buildServiceHost.Uri, (object) agents[index].Uri);
          buildServiceHost.AgentUpdated(requestContext.Elevate(), agents[index], ServiceAction.Add);
        }
      }
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (AddBuildAgents));
      return agents.ToList<BuildAgent>();
    }

    public List<BuildController> AddBuildControllers(
      IVssRequestContext requestContext,
      IList<BuildController> controllers)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (AddBuildControllers));
      TeamFoundationBuildService.CheckXamlEnabled(requestContext);
      Validation.CheckValidatable<BuildController>(requestContext, nameof (controllers), controllers, false, ValidationContext.Add);
      this.m_buildHost.SecurityManager.CheckPrivilege(requestContext, AdministrationPermissions.ManageBuildResources, false);
      List<BuildController> buildControllerList = new List<BuildController>();
      Dictionary<string, BuildServiceHost> dictionary = this.QueryBuildServiceHostsByUri(requestContext.Elevate(), (IList<string>) controllers.Select<BuildController, string>((Func<BuildController, string>) (x => x.ServiceHostUri)).ToList<string>()).ServiceHosts.ToDictionary<BuildServiceHost, string>((Func<BuildServiceHost, string>) (x => x.Uri), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      using (new TeamFoundationBuildResourceService.ServiceLockToken(requestContext, (ICollection<string>) null))
      {
        using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
        {
          HashSet<string> source = new HashSet<string>();
          using (ResultCollection resultCollection = component.AddBuildControllers(controllers))
          {
            foreach (BuildController buildController in resultCollection.GetCurrent<BuildController>().Items)
            {
              buildControllerList.Add(buildController);
              source.Add(buildController.ServiceHostUri);
              requestContext.Trace(0, TraceLevel.Info, "BuildAdministration", "Service", "Added controller '{0}'", (object) buildController.Uri);
            }
          }
          using (ResultCollection resultCollection = component.QueryBuildServiceHostsByUri((IList<string>) source.ToList<string>()))
            dictionary = resultCollection.GetCurrent<BuildServiceHost>().Items.ToDictionary<BuildServiceHost, string>((Func<BuildServiceHost, string>) (x => x.Uri), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        }
      }
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      vssRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>().SetFeatureState(vssRequestContext, "Build.XamlHub", FeatureAvailabilityState.On);
      TeamFoundationEventService service = requestContext.GetService<TeamFoundationEventService>();
      for (int index = 0; index < buildControllerList.Count; ++index)
      {
        controllers[index].Uri = buildControllerList[index].Uri;
        requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Publishing notification for controller '{0}'", (object) controllers[index].Uri);
        BuildResourceChangedEvent notificationEvent = new BuildResourceChangedEvent(requestContext, ChangedType.Added, controllers[index]);
        service.PublishNotification(requestContext, (object) notificationEvent);
        BuildServiceHost buildServiceHost;
        if (dictionary.TryGetValue(controllers[index].ServiceHostUri, out buildServiceHost))
        {
          requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Notifying service host '{0}' for controller '{1}'", (object) buildServiceHost.Uri, (object) controllers[index].Uri);
          buildServiceHost.ControllerUpdated(requestContext.Elevate(), controllers[index], ServiceAction.Add);
        }
      }
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (AddBuildControllers));
      return controllers.ToList<BuildController>();
    }

    public BuildServiceHost AddBuildServiceHost(
      IVssRequestContext requestContext,
      BuildServiceHost serviceHost)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (AddBuildServiceHost));
      TeamFoundationBuildService.CheckXamlEnabled(requestContext);
      Validation.CheckValidatable(requestContext, nameof (serviceHost), (IValidatable) serviceHost, false, ValidationContext.Add);
      this.m_buildHost.SecurityManager.CheckPrivilege(requestContext, AdministrationPermissions.ManageBuildResources, false);
      BuildServiceHost buildServiceHost;
      using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
        buildServiceHost = component.AddBuildServiceHost(serviceHost);
      if (serviceHost.Properties != null && serviceHost.Properties.Count > 0)
        requestContext.GetService<TeamFoundationPropertyService>().SetProperties(requestContext.Elevate(), ArtifactHelper.CreateArtifactSpec(buildServiceHost.Uri), serviceHost.Properties.Select<KeyValuePair<string, object>, PropertyValue>((Func<KeyValuePair<string, object>, PropertyValue>) (x => new PropertyValue(x.Key, x.Value))));
      Uri uri = new Uri(buildServiceHost.MessageQueueUrl);
      TeamFoundationMessageQueueService service = requestContext.GetService<TeamFoundationMessageQueueService>();
      requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Creating queue '{0}'", (object) uri);
      IVssRequestContext requestContext1 = requestContext.Elevate();
      string host = uri.Host;
      string description = ResourceStrings.MessageQueueDescription((object) buildServiceHost.Name);
      service.CreateQueue(requestContext1, host, description);
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (AddBuildServiceHost));
      return buildServiceHost;
    }

    public void DeleteBuildAgents(
      IVssRequestContext requestContext,
      IList<string> agentUris,
      bool canDeleteElastic = false)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (DeleteBuildAgents));
      ArgumentValidation.CheckUriArray(nameof (agentUris), agentUris, "Agent", false, (string) null);
      this.m_buildHost.SecurityManager.CheckPrivilege(requestContext, AdministrationPermissions.ManageBuildResources, false);
      List<BuildAgent> buildAgentList = new List<BuildAgent>();
      Dictionary<string, BuildServiceHost> dictionary = (Dictionary<string, BuildServiceHost>) null;
      using (new TeamFoundationBuildResourceService.ServiceLockToken(requestContext, (ICollection<string>) null))
      {
        BuildAgentQueryResult agentQueryResult = this.QueryBuildAgentsByUri(requestContext, agentUris, (IList<string>) BuildConstants.AllPropertyNames);
        dictionary = agentQueryResult.ServiceHosts.ToDictionary<BuildServiceHost, string>((Func<BuildServiceHost, string>) (x => x.Uri), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        buildAgentList.AddRange(agentQueryResult.Agents.Where<BuildAgent>((Func<BuildAgent, bool>) (x => x != null)));
        if (!canDeleteElastic)
        {
          foreach (BuildAgent buildAgent in buildAgentList)
          {
            BuildServiceHost buildServiceHost;
            if (dictionary.TryGetValue(buildAgent.ServiceHostUri, out buildServiceHost) && buildServiceHost != null && buildServiceHost.IsVirtual)
            {
              requestContext.Trace(0, TraceLevel.Error, "BuildAdministration", "Service", "Build agent '{0}' cannot be deleted because it is virtual.", (object) buildAgent.Name);
              throw new BuildAgentDeletionException(buildAgent.Name);
            }
          }
        }
        using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
          component.DeleteBuildAgents(agentUris, this.m_agentArtifactKind);
      }
      TeamFoundationEventService service = requestContext.GetService<TeamFoundationEventService>();
      foreach (BuildAgent agent in buildAgentList)
      {
        requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Publishing notification for agent '{0}'", (object) agent.Uri);
        service.PublishNotification(requestContext, (object) new BuildResourceChangedEvent(requestContext, ChangedType.Deleted, agent));
        BuildServiceHost buildServiceHost;
        if (dictionary.TryGetValue(agent.ServiceHostUri, out buildServiceHost))
        {
          requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Notifying service host '{0}' for agent '{1}'", (object) buildServiceHost.Uri, (object) agent.Uri);
          buildServiceHost.AgentUpdated(requestContext.Elevate(), agent, ServiceAction.Delete);
        }
      }
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (DeleteBuildAgents));
    }

    public void DeleteBuildControllers(
      IVssRequestContext requestContext,
      IList<string> controllerUris,
      bool canDeleteElastic = false)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (DeleteBuildControllers));
      ArgumentValidation.CheckUriArray(nameof (controllerUris), controllerUris, "Controller", false, (string) null);
      this.m_buildHost.SecurityManager.CheckPrivilege(requestContext, AdministrationPermissions.ManageBuildResources, false);
      List<BuildController> buildControllerList = new List<BuildController>();
      Dictionary<string, BuildServiceHost> dictionary = (Dictionary<string, BuildServiceHost>) null;
      using (new TeamFoundationBuildResourceService.ServiceLockToken(requestContext, (ICollection<string>) null))
      {
        BuildControllerQueryResult controllerQueryResult = this.QueryBuildControllersByUri(requestContext, (IList<string>) controllerUris.ToArray<string>(), (IList<string>) BuildConstants.AllPropertyNames, false);
        dictionary = controllerQueryResult.ServiceHosts.ToDictionary<BuildServiceHost, string>((Func<BuildServiceHost, string>) (x => x.Uri), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        buildControllerList.AddRange(controllerQueryResult.Controllers.Where<BuildController>((Func<BuildController, bool>) (x => x != null)));
        if (!canDeleteElastic)
        {
          foreach (BuildController buildController in buildControllerList)
          {
            BuildServiceHost buildServiceHost;
            if (dictionary.TryGetValue(buildController.ServiceHostUri, out buildServiceHost) && buildServiceHost != null && buildServiceHost.IsVirtual)
            {
              requestContext.Trace(0, TraceLevel.Error, "BuildAdministration", "Service", "Build controller '{0}' cannot be deleted because it is virtual.", (object) buildController.Name);
              throw new BuildControllerDeletionException(buildController.Name);
            }
          }
        }
        using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
          component.DeleteBuildControllers(controllerUris, this.m_controllerArtifactKind);
      }
      TeamFoundationEventService service = requestContext.GetService<TeamFoundationEventService>();
      foreach (BuildController controller in buildControllerList)
      {
        requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Publishing notification for controller '{0}'", (object) controller.Uri);
        service.PublishNotification(requestContext, (object) new BuildResourceChangedEvent(requestContext, ChangedType.Deleted, controller));
        BuildServiceHost buildServiceHost;
        if (dictionary.TryGetValue(controller.ServiceHostUri, out buildServiceHost))
        {
          requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Notifying service host '{0}' for controller '{1}'", (object) buildServiceHost.Uri, (object) controller.Uri);
          buildServiceHost.ControllerUpdated(requestContext.Elevate(), controller, ServiceAction.Delete);
        }
      }
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (DeleteBuildControllers));
    }

    public void DeleteBuildServiceHost(
      IVssRequestContext requestContext,
      string serviceHostUri,
      bool canDeleteElastic = false)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (DeleteBuildServiceHost));
      ArgumentValidation.CheckUri(nameof (serviceHostUri), serviceHostUri, "ServiceHost", false, (string) null);
      this.m_buildHost.SecurityManager.CheckPrivilege(requestContext, AdministrationPermissions.ManageBuildResources, false);
      Uri uri = (Uri) null;
      using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
      {
        using (ResultCollection resultCollection = component.QueryBuildServiceHostsByUri((IList<string>) new string[1]
        {
          serviceHostUri
        }))
        {
          ObjectBinder<BuildServiceHost> current1 = resultCollection.GetCurrent<BuildServiceHost>();
          if (!current1.MoveNext())
          {
            requestContext.Trace(0, TraceLevel.Warning, "BuildAdministration", "Service", "Build service host '{0}' not found.", (object) serviceHostUri);
            return;
          }
          BuildServiceHost current2 = current1.Current;
          if (current2 != null)
          {
            if (!canDeleteElastic && current2.IsVirtual)
            {
              requestContext.Trace(0, TraceLevel.Error, "BuildAdministration", "Service", "Build service host '{0}' cannot be deleted because it is virtual.", (object) current2.Name);
              throw new BuildServiceHostDeletionException(current2.Name);
            }
            uri = new Uri(current2.MessageQueueUrl);
          }
        }
        component.DeleteBuildServiceHost(serviceHostUri, this.m_agentArtifactKind, this.m_controllerArtifactKind);
      }
      try
      {
        requestContext.GetService<TeamFoundationPropertyService>().DeleteArtifacts(requestContext.Elevate(), (IEnumerable<ArtifactSpec>) new ArtifactSpec[1]
        {
          ArtifactHelper.CreateArtifactSpec(serviceHostUri)
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "BuildAdministration", "Service", ex);
      }
      if (uri != (Uri) null)
      {
        TeamFoundationMessageQueueService service = requestContext.GetService<TeamFoundationMessageQueueService>();
        requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Deleting queue '{0}'", (object) uri);
        try
        {
          service.DeleteQueue(requestContext.Elevate(), uri.Host);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(0, "BuildAdministration", "Service", ex);
        }
      }
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (DeleteBuildServiceHost));
    }

    public List<BuildAgentQueryResult> QueryBuildAgents(
      IVssRequestContext requestContext,
      IList<BuildAgentSpec> agentSpecs)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (QueryBuildAgents));
      Validation.CheckValidatable<BuildAgentSpec>(requestContext, nameof (agentSpecs), agentSpecs, false, ValidationContext.Query);
      List<BuildAgentQueryResult> agentQueryResultList = new List<BuildAgentQueryResult>();
      if (!this.m_buildHost.SecurityManager.HasPrivilege(requestContext, AdministrationPermissions.ViewBuildResources, false))
      {
        foreach (BuildAgentSpec agentSpec in (IEnumerable<BuildAgentSpec>) agentSpecs)
          agentQueryResultList.Add(new BuildAgentQueryResult());
        requestContext.Trace(0, TraceLevel.Warning, "BuildAdministration", "Command", "Exiting due to access denied: ViewBuildResources");
        return agentQueryResultList;
      }
      foreach (BuildAgentSpec agentSpec in (IEnumerable<BuildAgentSpec>) agentSpecs)
      {
        BuildAgentQueryResult agentQueryResult = new BuildAgentQueryResult();
        List<BuildAgent> items;
        using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
        {
          ResultCollection resultCollection = component.QueryBuildAgents(agentSpec);
          items = resultCollection.GetCurrent<BuildAgent>().Items;
          resultCollection.NextResult();
          agentQueryResult.Controllers.AddRange((IEnumerable<BuildController>) resultCollection.GetCurrent<BuildController>().Items);
          resultCollection.NextResult();
          agentQueryResult.ServiceHosts.AddRange((IEnumerable<BuildServiceHost>) resultCollection.GetCurrent<BuildServiceHost>().Items);
        }
        using (IDisposableReadOnlyList<IBuildQueueExtension> extensions = requestContext.GetExtensions<IBuildQueueExtension>())
        {
          foreach (IBuildQueueExtension buildQueueExtension in (IEnumerable<IBuildQueueExtension>) extensions)
          {
            try
            {
              buildQueueExtension.PostProcessBuildAgentQueryResult(requestContext, (IEnumerable<BuildServiceHost>) agentQueryResult.ServiceHosts, (IEnumerable<BuildAgent>) items);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(0, "Build", "Service", ex);
            }
          }
        }
        if (agentSpec.PropertyNameFilters != null && agentSpec.PropertyNameFilters.Any<string>())
        {
          TeamFoundationPropertyService service = requestContext.GetService<TeamFoundationPropertyService>();
          IEnumerable<ArtifactSpec> artifactSpecs1 = items.Where<BuildAgent>((Func<BuildAgent, bool>) (x => x != null)).Select<BuildAgent, ArtifactSpec>((Func<BuildAgent, ArtifactSpec>) (x => ArtifactHelper.CreateArtifactSpec(x.Uri)));
          IVssRequestContext requestContext1 = requestContext;
          IEnumerable<ArtifactSpec> artifactSpecs2 = artifactSpecs1;
          List<string> propertyNameFilters = agentSpec.PropertyNameFilters;
          using (TeamFoundationDataReader properties = service.GetProperties(requestContext1, artifactSpecs2, (IEnumerable<string>) propertyNameFilters))
            ArtifactHelper.MatchProperties<BuildAgent>(properties, (IList<BuildAgent>) items.Where<BuildAgent>((Func<BuildAgent, bool>) (x => x != null)).ToList<BuildAgent>(), (Func<BuildAgent, int>) (x => ArtifactHelper.GetArtifactId(x.Uri)), (Action<BuildAgent, List<PropertyValue>>) ((x, y) => x.Properties.AddRange((IEnumerable<PropertyValue>) y)));
        }
        agentQueryResult.Agents.AddRange((IEnumerable<BuildAgent>) items);
        agentQueryResultList.Add(agentQueryResult);
      }
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (QueryBuildAgents));
      return agentQueryResultList;
    }

    public BuildAgentQueryResult QueryBuildAgentsByUri(
      IVssRequestContext requestContext,
      IList<string> agentUris,
      IList<string> propertyNameFilters)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (QueryBuildAgentsByUri));
      ArgumentValidation.CheckUriArray(nameof (agentUris), agentUris, "Agent", false, (string) null);
      if (!this.m_buildHost.SecurityManager.HasPrivilege(requestContext, AdministrationPermissions.ViewBuildResources, false))
      {
        BuildAgentQueryResult agentQueryResult = new BuildAgentQueryResult();
        foreach (string agentUri in (IEnumerable<string>) agentUris)
          agentQueryResult.Agents.Add((BuildAgent) null);
        requestContext.Trace(0, TraceLevel.Warning, "BuildAdministration", "Command", "Exiting due to access denied: ViewBuildResources");
        return agentQueryResult;
      }
      BuildAgentQueryResult agentQueryResult1 = new BuildAgentQueryResult();
      List<BuildAgent> items;
      using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
      {
        ResultCollection resultCollection = component.QueryBuildAgentsByUri(agentUris);
        items = resultCollection.GetCurrent<BuildAgent>().Items;
        resultCollection.NextResult();
        agentQueryResult1.Controllers.AddRange((IEnumerable<BuildController>) resultCollection.GetCurrent<BuildController>().Items);
        resultCollection.NextResult();
        agentQueryResult1.ServiceHosts.AddRange((IEnumerable<BuildServiceHost>) resultCollection.GetCurrent<BuildServiceHost>().Items);
      }
      using (IDisposableReadOnlyList<IBuildQueueExtension> extensions = requestContext.GetExtensions<IBuildQueueExtension>())
      {
        foreach (IBuildQueueExtension buildQueueExtension in (IEnumerable<IBuildQueueExtension>) extensions)
        {
          try
          {
            buildQueueExtension.PostProcessBuildAgentQueryResult(requestContext, (IEnumerable<BuildServiceHost>) agentQueryResult1.ServiceHosts, (IEnumerable<BuildAgent>) items);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(0, "Build", "Service", ex);
          }
        }
      }
      if (propertyNameFilters != null && propertyNameFilters.Any<string>())
      {
        TeamFoundationPropertyService service = requestContext.GetService<TeamFoundationPropertyService>();
        IEnumerable<ArtifactSpec> artifactSpecs1 = items.Where<BuildAgent>((Func<BuildAgent, bool>) (x => x != null)).Select<BuildAgent, ArtifactSpec>((Func<BuildAgent, ArtifactSpec>) (x => ArtifactHelper.CreateArtifactSpec(x.Uri)));
        IVssRequestContext requestContext1 = requestContext;
        IEnumerable<ArtifactSpec> artifactSpecs2 = artifactSpecs1;
        IList<string> propertyNameFilters1 = propertyNameFilters;
        using (TeamFoundationDataReader properties = service.GetProperties(requestContext1, artifactSpecs2, (IEnumerable<string>) propertyNameFilters1))
          ArtifactHelper.MatchProperties<BuildAgent>(properties, (IList<BuildAgent>) items.Where<BuildAgent>((Func<BuildAgent, bool>) (x => x != null)).ToList<BuildAgent>(), (Func<BuildAgent, int>) (x => ArtifactHelper.GetArtifactId(x.Uri)), (Action<BuildAgent, List<PropertyValue>>) ((x, y) => x.Properties.AddRange((IEnumerable<PropertyValue>) y)));
      }
      BuildAgentDictionary buildAgentDictionary = new BuildAgentDictionary((IEnumerable<BuildAgent>) items);
      for (int index = 0; index < agentUris.Count; ++index)
      {
        BuildAgent buildAgent;
        if (buildAgentDictionary.TryGetValue(agentUris[index], out buildAgent))
        {
          agentQueryResult1.Agents.Add(buildAgent);
          requestContext.Trace(0, TraceLevel.Info, "BuildAdministration", "Service", "Inserted build agent '{0}' to position '{1}'", (object) buildAgent.Uri, (object) index);
        }
        else
        {
          agentQueryResult1.Agents.Add((BuildAgent) null);
          requestContext.Trace(0, TraceLevel.Warning, "BuildAdministration", "Service", "No build agent found for '{0}'", (object) agentUris[index]);
        }
      }
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (QueryBuildAgentsByUri));
      return agentQueryResult1;
    }

    public BuildControllerQueryResult QueryBuildControllers(
      IVssRequestContext requestContext,
      BuildControllerSpec controllerSpec)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (QueryBuildControllers));
      if (!this.m_buildHost.SecurityManager.HasPrivilege(requestContext, AdministrationPermissions.ViewBuildResources, false))
      {
        BuildControllerQueryResult controllerQueryResult = new BuildControllerQueryResult();
        requestContext.Trace(0, TraceLevel.Warning, "BuildAdministration", "Command", "Exiting due to access denied: ViewBuildResources");
        return controllerQueryResult;
      }
      BuildControllerQueryResult controllerQueryResult1 = new BuildControllerQueryResult();
      using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
      {
        ResultCollection resultCollection = component.QueryBuildControllers(controllerSpec);
        controllerQueryResult1.Controllers.AddRange((IEnumerable<BuildController>) resultCollection.GetCurrent<BuildController>().Items);
        resultCollection.NextResult();
        controllerQueryResult1.Agents.AddRange((IEnumerable<BuildAgent>) resultCollection.GetCurrent<BuildAgent>().Items);
        resultCollection.NextResult();
        controllerQueryResult1.ServiceHosts.AddRange((IEnumerable<BuildServiceHost>) resultCollection.GetCurrent<BuildServiceHost>().Items);
      }
      if (controllerSpec.IncludeAgents)
      {
        using (IDisposableReadOnlyList<IBuildQueueExtension> extensions = requestContext.GetExtensions<IBuildQueueExtension>())
        {
          foreach (IBuildQueueExtension buildQueueExtension in (IEnumerable<IBuildQueueExtension>) extensions)
          {
            try
            {
              buildQueueExtension.PostProcessBuildAgentQueryResult(requestContext, (IEnumerable<BuildServiceHost>) controllerQueryResult1.ServiceHosts, (IEnumerable<BuildAgent>) controllerQueryResult1.Agents);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(0, "Build", "Service", ex);
            }
          }
        }
      }
      if (controllerSpec.PropertyNameFilters != null && controllerSpec.PropertyNameFilters.Any<string>())
      {
        TeamFoundationPropertyService service = requestContext.GetService<TeamFoundationPropertyService>();
        IEnumerable<ArtifactSpec> artifactSpecs1 = controllerQueryResult1.Controllers.Where<BuildController>((Func<BuildController, bool>) (x => x != null)).Select<BuildController, ArtifactSpec>((Func<BuildController, ArtifactSpec>) (x => ArtifactHelper.CreateArtifactSpec(x.Uri)));
        IVssRequestContext requestContext1 = requestContext;
        IEnumerable<ArtifactSpec> artifactSpecs2 = artifactSpecs1;
        List<string> propertyNameFilters = controllerSpec.PropertyNameFilters;
        using (TeamFoundationDataReader properties = service.GetProperties(requestContext1, artifactSpecs2, (IEnumerable<string>) propertyNameFilters))
          ArtifactHelper.MatchProperties<BuildController>(properties, (IList<BuildController>) controllerQueryResult1.Controllers.Where<BuildController>((Func<BuildController, bool>) (x => x != null)).ToList<BuildController>(), (Func<BuildController, int>) (x => ArtifactHelper.GetArtifactId(x.Uri)), (Action<BuildController, List<PropertyValue>>) ((x, y) => x.Properties.AddRange((IEnumerable<PropertyValue>) y)));
      }
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (QueryBuildControllers));
      return controllerQueryResult1;
    }

    public List<BuildControllerQueryResult> QueryBuildControllers(
      IVssRequestContext requestContext,
      IList<BuildControllerSpec> controllerSpecs)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (QueryBuildControllers));
      List<BuildControllerQueryResult> controllerQueryResultList = new List<BuildControllerQueryResult>();
      foreach (BuildControllerSpec controllerSpec in (IEnumerable<BuildControllerSpec>) controllerSpecs)
        controllerQueryResultList.Add(this.QueryBuildControllers(requestContext, controllerSpec));
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (QueryBuildControllers));
      return controllerQueryResultList;
    }

    public BuildControllerQueryResult QueryBuildControllersByUri(
      IVssRequestContext requestContext,
      IList<string> controllerUris,
      IList<string> propertyNames,
      bool includeAgents)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (QueryBuildControllersByUri));
      ArgumentValidation.CheckUriArray(nameof (controllerUris), controllerUris, "Controller", false, (string) null);
      if (!this.m_buildHost.SecurityManager.HasPrivilege(requestContext, AdministrationPermissions.ViewBuildResources, false))
      {
        BuildControllerQueryResult controllerQueryResult = new BuildControllerQueryResult();
        for (int index = 0; index < controllerUris.Count; ++index)
          controllerQueryResult.Controllers.Add((BuildController) null);
        requestContext.Trace(0, TraceLevel.Warning, "BuildAdministration", "Command", "Exiting due to access denied: ViewBuildResources");
        return controllerQueryResult;
      }
      BuildControllerQueryResult controllerQueryResult1 = new BuildControllerQueryResult();
      List<BuildController> items;
      using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
      {
        ResultCollection resultCollection = component.QueryBuildControllersByUri(controllerUris, includeAgents);
        items = resultCollection.GetCurrent<BuildController>().Items;
        resultCollection.NextResult();
        controllerQueryResult1.Agents.AddRange((IEnumerable<BuildAgent>) resultCollection.GetCurrent<BuildAgent>().Items);
        resultCollection.NextResult();
        controllerQueryResult1.ServiceHosts.AddRange((IEnumerable<BuildServiceHost>) resultCollection.GetCurrent<BuildServiceHost>().Items);
      }
      if (includeAgents)
      {
        using (IDisposableReadOnlyList<IBuildQueueExtension> extensions = requestContext.GetExtensions<IBuildQueueExtension>())
        {
          foreach (IBuildQueueExtension buildQueueExtension in (IEnumerable<IBuildQueueExtension>) extensions)
          {
            try
            {
              buildQueueExtension.PostProcessBuildAgentQueryResult(requestContext, (IEnumerable<BuildServiceHost>) controllerQueryResult1.ServiceHosts, (IEnumerable<BuildAgent>) controllerQueryResult1.Agents);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(0, "Build", "Service", ex);
            }
          }
        }
      }
      if (propertyNames != null && propertyNames.Any<string>())
      {
        TeamFoundationPropertyService service = requestContext.GetService<TeamFoundationPropertyService>();
        IEnumerable<ArtifactSpec> artifactSpecs1 = items.Where<BuildController>((Func<BuildController, bool>) (x => x != null)).Select<BuildController, ArtifactSpec>((Func<BuildController, ArtifactSpec>) (x => ArtifactHelper.CreateArtifactSpec(x.Uri)));
        IVssRequestContext requestContext1 = requestContext;
        IEnumerable<ArtifactSpec> artifactSpecs2 = artifactSpecs1;
        IList<string> propertyNameFilters = propertyNames;
        using (TeamFoundationDataReader properties = service.GetProperties(requestContext1, artifactSpecs2, (IEnumerable<string>) propertyNameFilters))
          ArtifactHelper.MatchProperties<BuildController>(properties, (IList<BuildController>) items.Where<BuildController>((Func<BuildController, bool>) (x => x != null)).ToList<BuildController>(), (Func<BuildController, int>) (x => ArtifactHelper.GetArtifactId(x.Uri)), (Action<BuildController, List<PropertyValue>>) ((x, y) => x.Properties.AddRange((IEnumerable<PropertyValue>) y)));
      }
      BuildControllerDictionary controllerDictionary = new BuildControllerDictionary((IEnumerable<BuildController>) items);
      for (int index = 0; index < controllerUris.Count; ++index)
      {
        BuildController buildController;
        if (controllerDictionary.TryGetValue(controllerUris[index], out buildController))
        {
          controllerQueryResult1.Controllers.Add(buildController);
          requestContext.Trace(0, TraceLevel.Info, "BuildAdministration", "Service", "Inserted build controller '{0}' to position '{1}'", (object) buildController.Uri, (object) index);
        }
        else
        {
          controllerQueryResult1.Controllers.Add((BuildController) null);
          requestContext.Trace(0, TraceLevel.Warning, "BuildAdministration", "Service", "No build controller found for '{0}'", (object) controllerUris[index]);
        }
      }
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (QueryBuildControllersByUri));
      return controllerQueryResult1;
    }

    public BuildServiceHostQueryResult QueryBuildServiceHosts(
      IVssRequestContext requestContext,
      string computer)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (QueryBuildServiceHosts));
      if (!this.m_buildHost.SecurityManager.HasPrivilege(requestContext, AdministrationPermissions.ViewBuildResources, false))
      {
        requestContext.Trace(0, TraceLevel.Warning, "BuildAdministration", "Service", "Exiting due to lack of permissions.");
        return new BuildServiceHostQueryResult();
      }
      BuildServiceHostQueryResult serviceHostQueryResult = new BuildServiceHostQueryResult();
      using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
      {
        ResultCollection resultCollection = component.QueryBuildServiceHosts(computer);
        serviceHostQueryResult.ServiceHosts.AddRange((IEnumerable<BuildServiceHost>) resultCollection.GetCurrent<BuildServiceHost>().Items);
        resultCollection.NextResult();
        serviceHostQueryResult.Controllers.AddRange((IEnumerable<BuildController>) resultCollection.GetCurrent<BuildController>().Items);
        resultCollection.NextResult();
        serviceHostQueryResult.Agents.AddRange((IEnumerable<BuildAgent>) resultCollection.GetCurrent<BuildAgent>().Items);
      }
      using (IDisposableReadOnlyList<IBuildQueueExtension> extensions = requestContext.GetExtensions<IBuildQueueExtension>())
      {
        foreach (IBuildQueueExtension buildQueueExtension in (IEnumerable<IBuildQueueExtension>) extensions)
        {
          try
          {
            buildQueueExtension.PostProcessBuildAgentQueryResult(requestContext, (IEnumerable<BuildServiceHost>) serviceHostQueryResult.ServiceHosts, (IEnumerable<BuildAgent>) serviceHostQueryResult.Agents);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(0, "Build", "Service", ex);
          }
        }
      }
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (QueryBuildServiceHosts));
      return serviceHostQueryResult;
    }

    public BuildServiceHostQueryResult QueryBuildServiceHostsByUri(
      IVssRequestContext requestContext,
      IList<string> serviceHostUris)
    {
      return this.QueryBuildServiceHostsByUri(requestContext, serviceHostUris, (IEnumerable<string>) null);
    }

    public void UpdateBuildAgents(
      IVssRequestContext requestContext,
      IList<BuildAgentUpdateOptions> updates,
      bool canUpdateElastic = false,
      bool fromDev10Client = false)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (UpdateBuildAgents));
      Validation.CheckValidatable<BuildAgentUpdateOptions>(requestContext, nameof (updates), updates, false, ValidationContext.Update);
      this.m_buildHost.SecurityManager.CheckPrivilege(requestContext, AdministrationPermissions.ManageBuildResources, false);
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      TeamFoundationIdentity foundationIdentity = requestContext.GetService<TeamFoundationIdentityService>().ReadRequestIdentity(requestContext);
      foreach (BuildAgentUpdateOptions update in (IEnumerable<BuildAgentUpdateOptions>) updates)
      {
        stringSet.Add(update.Uri);
        if ((update.Fields & BuildAgentUpdate.Status) == BuildAgentUpdate.Status && (update.Fields & BuildAgentUpdate.StatusMessage) != BuildAgentUpdate.StatusMessage)
        {
          update.Fields |= BuildAgentUpdate.StatusMessage;
          update.StatusMessage = AdministrationResources.BuildAgentStatusChangedByUser((object) foundationIdentity.DisplayName);
          requestContext.Trace(0, TraceLevel.Info, "BuildAdministration", "Service", "Updated status message for agent '{0}': '{1}'", (object) update.Uri, (object) update.StatusMessage);
        }
      }
      List<StartBuildData> startBuildData = new List<StartBuildData>();
      List<AgentReservationData> reservations = new List<AgentReservationData>();
      Dictionary<string, BuildAgent> dictionary1;
      Dictionary<string, BuildServiceHost> dictionary2;
      using (new TeamFoundationBuildResourceService.ServiceLockToken(requestContext, (ICollection<string>) stringSet))
      {
        BuildAgentQueryResult agentQueryResult = this.QueryBuildAgentsByUri(requestContext.Elevate(), (IList<string>) stringSet.ToArray<string>(), (IList<string>) BuildConstants.AllPropertyNames);
        dictionary1 = agentQueryResult.Agents.Where<BuildAgent>((Func<BuildAgent, bool>) (x => x != null)).ToDictionary<BuildAgent, string>((Func<BuildAgent, string>) (x => x.Uri), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        dictionary2 = agentQueryResult.ServiceHosts.ToDictionary<BuildServiceHost, string>((Func<BuildServiceHost, string>) (x => x.Uri), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        if (!canUpdateElastic | fromDev10Client)
        {
          foreach (BuildAgentUpdateOptions update in (IEnumerable<BuildAgentUpdateOptions>) updates)
          {
            BuildAgent buildAgent;
            if (!dictionary1.TryGetValue(update.Uri, out buildAgent))
            {
              requestContext.Trace(0, TraceLevel.Error, "BuildAdministration", "Service", "Agent '{0}' not found.", (object) update.Uri);
            }
            else
            {
              BuildServiceHost buildServiceHost;
              if (!dictionary2.TryGetValue(buildAgent.ServiceHostUri, out buildServiceHost))
              {
                requestContext.Trace(0, TraceLevel.Error, "BuildAdministration", "Service", "Service host'{0}' not found.", (object) buildAgent.ServiceHostUri);
              }
              else
              {
                if (buildServiceHost.IsVirtual && !canUpdateElastic)
                {
                  if (update.Fields.HasFlag((Enum) BuildAgentUpdate.Name))
                    throw new BuildAgentUpdateException("Name", buildAgent.Name);
                  if (update.Fields.HasFlag((Enum) BuildAgentUpdate.BuildDirectory))
                    throw new BuildAgentUpdateException("BuildDirectory", buildAgent.Name);
                }
                if (fromDev10Client && buildServiceHost.Version >= 400)
                  update.Fields &= ~(BuildAgentUpdate.Status | BuildAgentUpdate.StatusMessage);
              }
            }
          }
        }
        using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
        {
          ResultCollection resultCollection = component.UpdateBuildAgents(updates);
          startBuildData.AddRange((IEnumerable<StartBuildData>) resultCollection.GetCurrent<StartBuildData>().Items);
          resultCollection.NextResult();
          reservations.AddRange((IEnumerable<AgentReservationData>) resultCollection.GetCurrent<AgentReservationData>().Items);
        }
      }
      BuildController.StartBuilds(requestContext, (IList<StartBuildData>) startBuildData);
      BuildController.NotifyAgentsAvailable(requestContext, (IEnumerable<AgentReservationData>) reservations);
      TeamFoundationEventService service = requestContext.GetService<TeamFoundationEventService>();
      foreach (BuildAgentUpdateOptions update in (IEnumerable<BuildAgentUpdateOptions>) updates)
      {
        BuildAgent buildAgent;
        if (dictionary1.TryGetValue(update.Uri, out buildAgent))
        {
          if (update.Fields != BuildAgentUpdate.None)
          {
            requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Publishing notification for agent '{0}'", (object) update.Uri);
            service.PublishNotification(requestContext, (object) new BuildResourceChangedEvent(requestContext, buildAgent, update));
          }
          BuildServiceHost buildServiceHost;
          if ((update.Fields & (BuildAgentUpdate.ControllerUri | BuildAgentUpdate.BuildDirectory)) != BuildAgentUpdate.None && dictionary2.TryGetValue(buildAgent.ServiceHostUri, out buildServiceHost))
          {
            requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Notifying service host '{0}' for agent '{1}'", (object) buildServiceHost.Uri, (object) update.Uri);
            buildServiceHost.AgentUpdated(requestContext.Elevate(), buildAgent, ServiceAction.Update);
          }
          if ((update.Fields & BuildAgentUpdate.Enabled) == BuildAgentUpdate.Enabled)
            this.NotifyServiceHostChanged(requestContext, dictionary2, buildAgent.ServiceHostUri);
        }
      }
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (UpdateBuildAgents));
    }

    public void UpdateBuildControllers(
      IVssRequestContext requestContext,
      IList<BuildControllerUpdateOptions> updates,
      bool canUpdateElastic = false,
      bool fromDev10Client = false)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (UpdateBuildControllers));
      Validation.CheckValidatable<BuildControllerUpdateOptions>(requestContext, nameof (updates), updates, false, ValidationContext.Update);
      this.m_buildHost.SecurityManager.CheckPrivilege(requestContext, AdministrationPermissions.ManageBuildResources, false);
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      TeamFoundationIdentity foundationIdentity = requestContext.GetService<TeamFoundationIdentityService>().ReadRequestIdentity(requestContext);
      foreach (BuildControllerUpdateOptions update in (IEnumerable<BuildControllerUpdateOptions>) updates)
      {
        stringSet.Add(update.Uri);
        if ((update.Fields & BuildControllerUpdate.Status) == BuildControllerUpdate.Status && (update.Fields & BuildControllerUpdate.StatusMessage) != BuildControllerUpdate.StatusMessage)
        {
          update.Fields |= BuildControllerUpdate.StatusMessage;
          update.StatusMessage = AdministrationResources.BuildControllerStatusChangedByUser((object) foundationIdentity.DisplayName);
          requestContext.Trace(0, TraceLevel.Info, "BuildAdministration", "Service", "Updated status message for controller '{0}': '{1}'", (object) update.Uri, (object) update.StatusMessage);
        }
      }
      List<StartBuildData> startBuildData = new List<StartBuildData>();
      Dictionary<string, BuildController> dictionary1;
      Dictionary<string, List<BuildAgent>> dictionary2;
      Dictionary<string, BuildServiceHost> dictionary3;
      using (new TeamFoundationBuildResourceService.ServiceLockToken(requestContext, (ICollection<string>) stringSet))
      {
        BuildControllerQueryResult controllerQueryResult = this.QueryBuildControllersByUri(requestContext.Elevate(), (IList<string>) stringSet.ToArray<string>(), (IList<string>) BuildConstants.AllPropertyNames, true);
        dictionary1 = controllerQueryResult.Controllers.Where<BuildController>((Func<BuildController, bool>) (x => x != null)).ToDictionary<BuildController, string>((Func<BuildController, string>) (x => x.Uri), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        dictionary2 = controllerQueryResult.Agents.GroupBy<BuildAgent, string>((Func<BuildAgent, string>) (x => x.ControllerUri), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToDictionary<IGrouping<string, BuildAgent>, string, List<BuildAgent>>((Func<IGrouping<string, BuildAgent>, string>) (x => x.Key), (Func<IGrouping<string, BuildAgent>, List<BuildAgent>>) (x => x.ToList<BuildAgent>()), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        dictionary3 = controllerQueryResult.ServiceHosts.ToDictionary<BuildServiceHost, string>((Func<BuildServiceHost, string>) (x => x.Uri), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        if (!canUpdateElastic | fromDev10Client)
        {
          foreach (BuildControllerUpdateOptions update in (IEnumerable<BuildControllerUpdateOptions>) updates)
          {
            BuildController buildController;
            if (!dictionary1.TryGetValue(update.Uri, out buildController))
            {
              requestContext.Trace(0, TraceLevel.Error, "BuildAdministration", "Service", "Controller '{0}' not found.", (object) update.Uri);
            }
            else
            {
              BuildServiceHost buildServiceHost;
              if (!dictionary3.TryGetValue(buildController.ServiceHostUri, out buildServiceHost))
              {
                requestContext.Trace(0, TraceLevel.Error, "BuildAdministration", "Service", "Service host'{0}' not found.", (object) buildController.ServiceHostUri);
              }
              else
              {
                if (!canUpdateElastic && buildServiceHost.IsVirtual)
                {
                  if (update.Fields.HasFlag((Enum) BuildControllerUpdate.Name))
                    throw new BuildControllerUpdateException("Name", buildController.Name);
                  if (update.Fields.HasFlag((Enum) BuildControllerUpdate.MaxConcurrentBuilds))
                    throw new BuildControllerUpdateException("MaxConcurrentBuilds", buildController.Name);
                }
                if (fromDev10Client && buildServiceHost.Version >= 400)
                  update.Fields &= ~(BuildControllerUpdate.Status | BuildControllerUpdate.StatusMessage);
              }
            }
          }
        }
        using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
        {
          ResultCollection resultCollection = component.UpdateBuildControllers(updates);
          startBuildData.AddRange((IEnumerable<StartBuildData>) resultCollection.GetCurrent<StartBuildData>().Items);
        }
      }
      BuildController.StartBuilds(requestContext, (IList<StartBuildData>) startBuildData);
      TeamFoundationEventService service = requestContext.GetService<TeamFoundationEventService>();
      foreach (BuildControllerUpdateOptions update in (IEnumerable<BuildControllerUpdateOptions>) updates)
      {
        BuildController buildController;
        if (dictionary1.TryGetValue(update.Uri, out buildController))
        {
          if (update.Fields != BuildControllerUpdate.None)
          {
            requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Publishing notification for controller '{0}'", (object) update.Uri);
            service.PublishNotification(requestContext, (object) new BuildResourceChangedEvent(requestContext, buildController, update));
          }
          if ((update.Fields & BuildControllerUpdate.CustomAssemblyPath) == BuildControllerUpdate.CustomAssemblyPath)
          {
            BuildServiceHost buildServiceHost;
            if (dictionary3.TryGetValue(buildController.ServiceHostUri, out buildServiceHost))
            {
              requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Notifying service host '{0}' for controller '{1}'", (object) buildServiceHost.Uri, (object) update.Uri);
              buildServiceHost.ControllerUpdated(requestContext.Elevate(), buildController, ServiceAction.Update);
            }
            List<BuildAgent> buildAgentList;
            if (dictionary2.TryGetValue(buildController.Uri, out buildAgentList))
            {
              foreach (BuildAgent agent in buildAgentList)
              {
                if (dictionary3.TryGetValue(agent.ServiceHostUri, out buildServiceHost))
                {
                  requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Notifying service host '{0}' for controller '{1}'", (object) buildServiceHost.Uri, (object) agent.Uri);
                  buildServiceHost.AgentUpdated(requestContext.Elevate(), agent, ServiceAction.Update);
                }
              }
            }
          }
          if ((update.Fields & BuildControllerUpdate.Enabled) == BuildControllerUpdate.Enabled)
            this.NotifyServiceHostChanged(requestContext, dictionary3, buildController.ServiceHostUri);
        }
      }
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (UpdateBuildControllers));
    }

    public void UpdateBuildServiceHost(
      IVssRequestContext requestContext,
      BuildServiceHostUpdateOptions update,
      bool canUpdateElastic = false)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (UpdateBuildServiceHost));
      Validation.CheckValidatable(requestContext, nameof (update), (IValidatable) update, false, ValidationContext.Update);
      this.m_buildHost.SecurityManager.CheckPrivilege(requestContext, AdministrationPermissions.ManageBuildResources, false);
      if (!canUpdateElastic)
      {
        BuildServiceHost serviceHost = this.QueryBuildServiceHostsByUri(requestContext, (IList<string>) new string[1]
        {
          update.Uri
        }).ServiceHosts[0];
        if (serviceHost != null && serviceHost.IsVirtual)
        {
          if (update.Fields.HasFlag((Enum) BuildServiceHostUpdate.Name))
            throw new BuildServiceHostUpdateException("Name", serviceHost.Name);
          if (update.Fields.HasFlag((Enum) BuildServiceHostUpdate.BaseUrl))
            throw new BuildServiceHostUpdateException("BaseUrl", serviceHost.Name);
        }
      }
      using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
        component.UpdateBuildServiceHost(update);
      if (update.Properties != null && update.Properties.Count > 0)
        requestContext.GetService<TeamFoundationPropertyService>().SetProperties(requestContext.Elevate(), ArtifactHelper.CreateArtifactSpec(update.Uri), update.Properties.Select<KeyValuePair<string, object>, PropertyValue>((Func<KeyValuePair<string, object>, PropertyValue>) (x => new PropertyValue(x.Key, x.Value))));
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (UpdateBuildServiceHost));
    }

    private void NotifyServiceHostChanged(
      IVssRequestContext requestContext,
      Dictionary<string, BuildServiceHost> serviceHosts,
      string serviceHostUri)
    {
      BuildServiceHost serviceHost;
      if (serviceHosts.TryGetValue(serviceHostUri, out serviceHost) && serviceHost.IsVirtual)
      {
        using (IDisposableReadOnlyList<IBuildQueueExtension> extensions = requestContext.GetExtensions<IBuildQueueExtension>())
        {
          foreach (IBuildQueueExtension buildQueueExtension in (IEnumerable<IBuildQueueExtension>) extensions)
          {
            try
            {
              buildQueueExtension.ServiceHostChanged(requestContext, serviceHost);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(0, "Build", "Service", ex);
            }
          }
        }
      }
      else
        requestContext.Trace(0, TraceLevel.Error, "BuildAdministration", "Service", "Service host'{0}' not found.", (object) serviceHostUri);
    }

    internal TeamFoundationBuildResourceService()
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(0, "BuildAdministration", "Service", "ServiceStart");
      try
      {
        this.m_serviceHost = systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? systemRequestContext.ServiceHost : throw new UnexpectedHostTypeException(TeamFoundationHostType.ProjectCollection);
        this.m_buildHost = systemRequestContext.GetService<TeamFoundationBuildHost>();
        this.m_deploymentHost = systemRequestContext.To(TeamFoundationHostType.Deployment).ServiceHost.DeploymentServiceHost;
        this.m_agentArtifactKind = this.m_buildHost.GetBuildAgentArtifactKind(systemRequestContext);
        this.m_controllerArtifactKind = this.m_buildHost.GetBuildControllerArtifactKind(systemRequestContext);
        this.m_deleteQueue = new InputQueue<TeamFoundationBuildResourceService.DeleteArguments>();
        this.m_deleteQueue.BeginDequeue(TimeSpan.MaxValue, new AsyncCallback(this.DeleteCallback), (object) null);
        systemRequestContext.GetService<TeamFoundationMessageQueueService>();
      }
      finally
      {
        systemRequestContext.TraceLeave(0, "BuildAdministration", "Service", "ServiceStart");
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_deleteQueue == null)
        return;
      this.m_deleteQueue.Close();
    }

    internal void AcquireServiceHost(
      IVssRequestContext requestContext,
      Guid ownerId,
      string serviceHostUri,
      string ownerName)
    {
      ArgumentUtility.CheckForEmptyGuid(ownerId, nameof (ownerId));
      ArgumentValidation.CheckUri(nameof (serviceHostUri), serviceHostUri, "ServiceHost", false, (string) null);
      ArgumentValidation.CheckBuildMachine(nameof (ownerName), ownerName, true);
      this.m_buildHost.SecurityManager.CheckPrivilege(requestContext, AdministrationPermissions.ManageBuildResources, false);
      bool flag = false;
      BuildController controller = (BuildController) null;
      BuildServiceHost serviceHost = (BuildServiceHost) null;
      List<WorkflowCancellationData> notificationData1 = (List<WorkflowCancellationData>) null;
      List<WorkflowCancellationData> notificationData2 = (List<WorkflowCancellationData>) null;
      using (requestContext.GetService<ITeamFoundationLockingService>().AcquireLock(requestContext, TeamFoundationLockMode.Exclusive, serviceHostUri))
      {
        try
        {
          using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
          {
            ResultCollection resultCollection = component.UpdateBuildServiceHostStatus(serviceHostUri, ServiceHostStatus.Online, new Guid?(ownerId), ownerName, new int?(), false);
            serviceHost = resultCollection.GetCurrent<BuildServiceHost>().FirstOrDefault<BuildServiceHost>();
            if (serviceHost != null)
            {
              resultCollection.NextResult();
              controller = resultCollection.GetCurrent<BuildController>().FirstOrDefault<BuildController>();
              resultCollection.NextResult();
              KeyValuePair<Guid, Guid> keyValuePair = resultCollection.GetCurrent<KeyValuePair<Guid, Guid>>().FirstOrDefault<KeyValuePair<Guid, Guid>>();
              if (keyValuePair.Key != Guid.Empty)
              {
                if (keyValuePair.Value != keyValuePair.Key)
                {
                  resultCollection.TryNextResult();
                  resultCollection.TryNextResult();
                  if (resultCollection.TryNextResult())
                    notificationData2 = resultCollection.GetCurrent<WorkflowCancellationData>().Items;
                  if (resultCollection.TryNextResult())
                    notificationData1 = resultCollection.GetCurrent<WorkflowCancellationData>().Items;
                  flag = true;
                }
              }
            }
          }
        }
        catch (BuildServiceHostOwnershipException ex)
        {
          BuildServiceHost buildServiceHost = (BuildServiceHost) null;
          using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>())
            buildServiceHost = component.QueryBuildServiceHostsByUri((IList<string>) new string[1]
            {
              serviceHostUri
            }).GetCurrent<BuildServiceHost>().FirstOrDefault<BuildServiceHost>();
          if (buildServiceHost != null)
          {
            DateTime lastConnectedOn = DateTime.MinValue;
            int connectionStatus = (int) requestContext.GetService<TeamFoundationMessageQueueService>().GetQueueConnectionStatus(requestContext, new Uri(buildServiceHost.MessageQueueUrl).Host, out lastConnectedOn);
            TimeSpan timeSpan = DateTime.UtcNow - lastConnectedOn;
            if (connectionStatus == 1 && timeSpan.TotalMinutes > 5.0 && buildServiceHost != null)
              this.ReleaseServiceHost(requestContext, buildServiceHost.OwnerSessionId, serviceHostUri, true);
          }
          throw;
        }
      }
      if (notificationData2 != null && notificationData2.Count > 0)
        this.NotifyWorkflowsCompleted(requestContext, notificationData2);
      if (notificationData1 != null && notificationData1.Count > 0)
        this.StopAgentWorkflows(requestContext, notificationData1);
      if (controller != null)
        this.StopAllBuilds(requestContext.Elevate(), serviceHost, controller, true);
      if (serviceHost == null)
        return;
      using (IDisposableReadOnlyList<IBuildQueueExtension> extensions = requestContext.GetExtensions<IBuildQueueExtension>())
      {
        if (!flag)
          return;
        foreach (IBuildQueueExtension buildQueueExtension in (IEnumerable<IBuildQueueExtension>) extensions)
        {
          try
          {
            buildQueueExtension.ServiceHostChanged(requestContext, serviceHost);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(0, "Build", "Service", ex);
          }
        }
      }
    }

    internal void ReleaseServiceHost(
      IVssRequestContext requestContext,
      Guid ownerId,
      string serviceHostUri,
      bool retryBuilds)
    {
      ArgumentUtility.CheckForEmptyGuid(ownerId, nameof (ownerId));
      ArgumentValidation.CheckUri(nameof (serviceHostUri), serviceHostUri, "ServiceHost", false, (string) null);
      this.m_buildHost.SecurityManager.CheckPrivilege(requestContext, AdministrationPermissions.ManageBuildResources, false);
      BuildController controller = (BuildController) null;
      BuildServiceHost serviceHost = (BuildServiceHost) null;
      List<WorkflowCancellationData> notificationData1 = (List<WorkflowCancellationData>) null;
      List<WorkflowCancellationData> notificationData2 = (List<WorkflowCancellationData>) null;
      using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
      {
        ResultCollection resultCollection = component.UpdateBuildServiceHostStatus(serviceHostUri, ServiceHostStatus.Offline, new Guid?(ownerId), (string) null, new int?(), true, true);
        serviceHost = resultCollection.GetCurrent<BuildServiceHost>().FirstOrDefault<BuildServiceHost>();
        if (serviceHost != null)
        {
          resultCollection.NextResult();
          controller = resultCollection.GetCurrent<BuildController>().FirstOrDefault<BuildController>();
          resultCollection.NextResult();
          resultCollection.TryNextResult();
          resultCollection.TryNextResult();
          if (resultCollection.TryNextResult())
            notificationData2 = resultCollection.GetCurrent<WorkflowCancellationData>().Items;
          if (resultCollection.TryNextResult())
            notificationData1 = resultCollection.GetCurrent<WorkflowCancellationData>().Items;
        }
      }
      if (serviceHost != null)
      {
        requestContext.GetService<TeamFoundationMessageQueueService>().SetQueueOffline(requestContext, new Uri(serviceHost.MessageQueueUrl).Host);
        using (IDisposableReadOnlyList<IBuildQueueExtension> extensions = requestContext.GetExtensions<IBuildQueueExtension>())
        {
          foreach (IBuildQueueExtension buildQueueExtension in (IEnumerable<IBuildQueueExtension>) extensions)
          {
            try
            {
              buildQueueExtension.ServiceHostChanged(requestContext, serviceHost);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(0, "Build", "Service", ex);
            }
          }
        }
      }
      if (notificationData2 != null && notificationData2.Count > 0)
        this.NotifyWorkflowsCompleted(requestContext, notificationData2);
      if (notificationData1 != null && notificationData1.Count > 0)
        this.StopAgentWorkflows(requestContext, notificationData1);
      if (controller == null)
        return;
      this.StopAllBuilds(requestContext.Elevate(), serviceHost, controller, retryBuilds);
    }

    private void StopAgentWorkflows(
      IVssRequestContext requestContext,
      List<WorkflowCancellationData> notificationData)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (StopAgentWorkflows));
      foreach (WorkflowCancellationData cancellationData in notificationData)
      {
        requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Stopping workflow with reservation ID {0} on build agent {1}", (object) cancellationData.ReservationId, (object) cancellationData.AgentDisplayName);
        Message message = (Message) null;
        try
        {
          message = BuildAgentService.StopWorkflow(cancellationData.ReservationId);
          message.Headers.To = new Uri(cancellationData.MessageQueueUrl);
          this.QueueMessage(requestContext, message, cancellationData.EndpointUrl);
        }
        catch (MessageQueueNotFoundException ex)
        {
        }
        finally
        {
          message?.Close();
        }
      }
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (StopAgentWorkflows));
    }

    private void NotifyWorkflowsCompleted(
      IVssRequestContext requestContext,
      List<WorkflowCancellationData> notificationData)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (NotifyWorkflowsCompleted));
      foreach (WorkflowCancellationData cancellationData in notificationData)
      {
        requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Notifying build controller the workflow with reservation ID {0} on build agent {1} has completed", (object) cancellationData.ReservationId, (object) cancellationData.AgentDisplayName);
        Message message = (Message) null;
        try
        {
          message = BuildControllerService.NotifyWorkflowCompleted(cancellationData.ReservationId, "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/ServiceHost/HostShutdownFault", ResourceStrings.AgentHostShutdownForcefullyError((object) cancellationData.AgentDisplayName), new Uri(DBHelper.CreateArtifactUri("Agent", cancellationData.ReservedAgentId)));
          message.Headers.To = new Uri(cancellationData.MessageQueueUrl);
          this.QueueMessage(requestContext, message, cancellationData.EndpointUrl);
        }
        catch (MessageQueueNotFoundException ex)
        {
        }
        finally
        {
          message?.Close();
        }
      }
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (NotifyWorkflowsCompleted));
    }

    internal void StopAllBuilds(
      IVssRequestContext requestContext,
      BuildServiceHost serviceHost,
      BuildController controller,
      bool retryBuilds)
    {
      BuildQueueSpec buildQueueSpec = new BuildQueueSpec();
      buildQueueSpec.ControllerSpec = new BuildControllerSpec(controller.Name, serviceHost.Name, false);
      buildQueueSpec.DefinitionFilter = (object) new BuildDefinitionSpec("*\\*");
      buildQueueSpec.InformationTypes.Add(InformationTypes.ActivityTracking);
      buildQueueSpec.InformationTypes.Add(InformationTypes.AgentScopeActivityTracking);
      buildQueueSpec.Status = QueueStatus.InProgress;
      buildQueueSpec.QueryOptions = QueryOptions.None;
      List<QueuedBuildUpdateOptions> updates1 = new List<QueuedBuildUpdateOptions>();
      List<QueuedBuildUpdateOptions> updates2 = new List<QueuedBuildUpdateOptions>();
      TeamFoundationBuildService service = requestContext.GetService<TeamFoundationBuildService>();
      using (TeamFoundationDataReader foundationDataReader = service.QueryQueuedBuilds(requestContext, (IList<BuildQueueSpec>) new BuildQueueSpec[1]
      {
        buildQueueSpec
      }, new Guid()))
      {
        StreamingCollection<BuildQueueQueryResult> streamingCollection = foundationDataReader.Current<StreamingCollection<BuildQueueQueryResult>>();
        if (!streamingCollection.MoveNext())
          return;
        BuildQueueQueryResult current = streamingCollection.Current;
        if (retryBuilds)
        {
          while (current.QueuedBuilds.MoveNext())
          {
            if (current.QueuedBuilds.Current.Reason != BuildReason.CheckInShelveset)
              updates1.Add(new QueuedBuildUpdateOptions()
              {
                BatchId = Guid.Empty,
                Fields = QueuedBuildUpdate.BatchId | QueuedBuildUpdate.Requeue,
                QueueId = current.QueuedBuilds.Current.Id,
                Retry = true,
                RetryOption = QueuedBuildRetryOption.CompletedBuild
              });
            else
              updates2.Add(new QueuedBuildUpdateOptions()
              {
                BatchId = Guid.Empty,
                Fields = QueuedBuildUpdate.BatchId | QueuedBuildUpdate.Requeue,
                QueueId = current.QueuedBuilds.Current.Id,
                Retry = true,
                RetryOption = QueuedBuildRetryOption.InProgressBuild
              });
          }
        }
        if (updates2.Count > 0)
          service.UpdateQueuedBuilds(requestContext, (IList<QueuedBuildUpdateOptions>) updates2, new Guid());
        TeamFoundationIdentity requestedFor = requestContext.GetService<TeamFoundationIdentityService>().ReadRequestIdentity(requestContext);
        string errorMessage = retryBuilds ? AdministrationResources.BuildStoppedMachineConnectionLost() : AdministrationResources.BuildStoppedMachineConnectionLostNoRetry();
        while (current.Builds.MoveNext())
        {
          try
          {
            BuildController.StopBuildForcefully(requestContext, current.Builds.Current, requestedFor, errorMessage, retryBuilds);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(0, "Build", "Service", ex);
          }
        }
        if (updates1.Count <= 0)
          return;
        service.UpdateQueuedBuilds(requestContext, (IList<QueuedBuildUpdateOptions>) updates1, false);
      }
    }

    internal BuildServiceHostQueryResult QueryBuildServiceHostsByUri(
      IVssRequestContext requestContext,
      IList<string> serviceHostUris,
      IEnumerable<string> propertyNameFilters)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (QueryBuildServiceHostsByUri));
      ArgumentValidation.CheckUriArray(nameof (serviceHostUris), serviceHostUris, "ServiceHost", false, (string) null);
      if (!this.m_buildHost.SecurityManager.HasPrivilege(requestContext, AdministrationPermissions.ViewBuildResources, false))
      {
        BuildServiceHostQueryResult serviceHostQueryResult = new BuildServiceHostQueryResult();
        foreach (string serviceHostUri in (IEnumerable<string>) serviceHostUris)
          serviceHostQueryResult.ServiceHosts.Add((BuildServiceHost) null);
        requestContext.Trace(0, TraceLevel.Warning, "BuildAdministration", "Service", "Exiting due to lack of permissions.");
        return serviceHostQueryResult;
      }
      BuildServiceHostQueryResult serviceHostQueryResult1 = new BuildServiceHostQueryResult();
      List<BuildServiceHost> items;
      using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
      {
        ResultCollection resultCollection = component.QueryBuildServiceHostsByUri(serviceHostUris);
        items = resultCollection.GetCurrent<BuildServiceHost>().Items;
        resultCollection.NextResult();
        serviceHostQueryResult1.Controllers.AddRange((IEnumerable<BuildController>) resultCollection.GetCurrent<BuildController>().Items);
        resultCollection.NextResult();
        serviceHostQueryResult1.Agents.AddRange((IEnumerable<BuildAgent>) resultCollection.GetCurrent<BuildAgent>().Items);
      }
      BuildServiceHostDictionary serviceHostDictionary = new BuildServiceHostDictionary((IEnumerable<BuildServiceHost>) items);
      serviceHostQueryResult1.ServiceHosts.Capacity = serviceHostUris.Count;
      for (int index = 0; index < serviceHostUris.Count; ++index)
      {
        BuildServiceHost buildServiceHost;
        if (serviceHostDictionary.TryGetValue(serviceHostUris[index], out buildServiceHost))
        {
          serviceHostQueryResult1.ServiceHosts.Insert(index, buildServiceHost);
          requestContext.Trace(0, TraceLevel.Info, "BuildAdministration", "Service", "Inserted build service host '{0}' to position '{1}'", (object) buildServiceHost.Uri, (object) index);
        }
        else
        {
          serviceHostQueryResult1.ServiceHosts.Insert(index, (BuildServiceHost) null);
          requestContext.Trace(0, TraceLevel.Warning, "BuildAdministration", "Service", "No build service host found for '{0}'", (object) serviceHostUris[index]);
        }
      }
      if (propertyNameFilters != null && propertyNameFilters.Any<string>())
      {
        TeamFoundationPropertyService service = requestContext.GetService<TeamFoundationPropertyService>();
        IEnumerable<ArtifactSpec> artifactSpecs1 = serviceHostQueryResult1.ServiceHosts.Where<BuildServiceHost>((Func<BuildServiceHost, bool>) (x => x != null)).Select<BuildServiceHost, ArtifactSpec>((Func<BuildServiceHost, ArtifactSpec>) (x => ArtifactHelper.CreateArtifactSpec(x.Uri)));
        IVssRequestContext requestContext1 = requestContext;
        IEnumerable<ArtifactSpec> artifactSpecs2 = artifactSpecs1;
        IEnumerable<string> propertyNameFilters1 = propertyNameFilters;
        using (TeamFoundationDataReader properties = service.GetProperties(requestContext1, artifactSpecs2, propertyNameFilters1))
          ArtifactHelper.MatchProperties<BuildServiceHost>(properties, (IList<BuildServiceHost>) serviceHostQueryResult1.ServiceHosts.Where<BuildServiceHost>((Func<BuildServiceHost, bool>) (x => x != null)).ToList<BuildServiceHost>(), (Func<BuildServiceHost, int>) (x => ArtifactHelper.GetArtifactId(x.Uri)), (Action<BuildServiceHost, List<PropertyValue>>) ((x, y) => x.Properties = (IDictionary<string, object>) y.ToDictionary<PropertyValue, string, object>((Func<PropertyValue, string>) (p => p.PropertyName), (Func<PropertyValue, object>) (p => p.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)));
      }
      using (IDisposableReadOnlyList<IBuildQueueExtension> extensions = requestContext.GetExtensions<IBuildQueueExtension>())
      {
        foreach (IBuildQueueExtension buildQueueExtension in (IEnumerable<IBuildQueueExtension>) extensions)
        {
          try
          {
            buildQueueExtension.PostProcessBuildAgentQueryResult(requestContext, (IEnumerable<BuildServiceHost>) serviceHostQueryResult1.ServiceHosts, (IEnumerable<BuildAgent>) serviceHostQueryResult1.Agents);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(0, "Build", "Service", ex);
          }
        }
      }
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (QueryBuildServiceHostsByUri));
      return serviceHostQueryResult1;
    }

    internal List<SharedResource> QuerySharedResources(
      IVssRequestContext requestContext,
      IList<string> resourceNames)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (QuerySharedResources));
      ArgumentValidation.CheckArray<string>(nameof (resourceNames), resourceNames, (Validate<string>) ((arg, val, allowNull, msg) => ArgumentValidation.CheckSharedResourceName(arg, val)), false, (string) null);
      this.m_buildHost.SecurityManager.CheckPrivilege(requestContext, AdministrationPermissions.UseBuildResources, false);
      using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
      {
        ResultCollection resultCollection = component.QuerySharedResources(resourceNames);
        List<SharedResource> items = resultCollection.GetCurrent<SharedResource>().Items;
        Dictionary<int, SharedResource> dictionary = items.ToDictionary<SharedResource, int, SharedResource>((Func<SharedResource, int>) (x => x.Id), (Func<SharedResource, SharedResource>) (x => x));
        resultCollection.NextResult();
        foreach (SharedResourceRequest sharedResourceRequest in resultCollection.GetCurrent<SharedResourceRequest>().Items)
          dictionary[sharedResourceRequest.ResourceId].Requests.Add(sharedResourceRequest);
        requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (QuerySharedResources));
        return items;
      }
    }

    internal void ReleaseAllSharedResourceLocks(
      IVssRequestContext requestContext,
      string requestedBy)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (ReleaseAllSharedResourceLocks));
      ArgumentValidation.Check(nameof (requestedBy), (object) requestedBy, false);
      this.m_buildHost.SecurityManager.CheckPrivilege(requestContext, AdministrationPermissions.UseBuildResources, false);
      using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
      {
        ResultCollection resultCollection = component.ReleaseSharedResourceLock((string) null, (string) null, requestedBy);
        TeamFoundationBuildResourceService.NotifySharedResourcesAvailable(requestContext, (IEnumerable<SharedResourceData>) resultCollection.GetCurrent<SharedResourceData>().Items);
      }
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (ReleaseAllSharedResourceLocks));
    }

    internal void ReleaseBuildAgent(IVssRequestContext requestContext, int reservationId)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (ReleaseBuildAgent));
      this.m_buildHost.SecurityManager.CheckPrivilege(requestContext, AdministrationPermissions.UseBuildResources, false);
      List<AgentReservationData> items;
      using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
        items = component.ReleaseBuildAgent(reservationId).GetCurrent<AgentReservationData>().Items;
      BuildController.NotifyAgentsAvailable(requestContext, (IEnumerable<AgentReservationData>) items);
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (ReleaseBuildAgent));
    }

    internal void ReleaseSharedResourceLock(
      IVssRequestContext requestContext,
      string resourceName,
      string instanceId,
      string requestedBy)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (ReleaseSharedResourceLock));
      ArgumentValidation.CheckSharedResourceName(nameof (resourceName), resourceName);
      ArgumentValidation.Check(nameof (instanceId), instanceId, false, (string) null);
      ArgumentValidation.Check(nameof (requestedBy), requestedBy, false, (string) null);
      this.m_buildHost.SecurityManager.CheckPrivilege(requestContext, AdministrationPermissions.UseBuildResources, false);
      using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
      {
        ResultCollection resultCollection = component.ReleaseSharedResourceLock(resourceName, instanceId, requestedBy);
        TeamFoundationBuildResourceService.NotifySharedResourcesAvailable(requestContext, (IEnumerable<SharedResourceData>) resultCollection.GetCurrent<SharedResourceData>().Items);
      }
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (ReleaseSharedResourceLock));
    }

    internal void RequestSharedResourceLock(
      IVssRequestContext requestContext,
      string resourceName,
      string instanceId,
      string requestedBy,
      string buildUri,
      Guid buildProjectId,
      string requestBuildUri,
      Guid requestBuildProjectId)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (RequestSharedResourceLock));
      ArgumentValidation.CheckSharedResourceName(nameof (resourceName), resourceName);
      ArgumentValidation.Check(nameof (instanceId), instanceId, false, (string) null);
      ArgumentValidation.CheckUri(nameof (requestedBy), requestedBy, false, (string) null);
      ArgumentValidation.CheckUri(nameof (buildUri), buildUri, "Build", true, (string) null);
      ArgumentValidation.CheckUri(nameof (requestBuildUri), requestBuildUri, "Build", true, (string) null);
      ArtifactId artifactId = LinkingUtilities.DecodeUri(requestedBy);
      if (!VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, "Agent") && !VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, "Controller"))
      {
        requestContext.Trace(0, TraceLevel.Error, "BuildAdministration", "Service", "Invalid shared resource requested by '{0}'", (object) requestedBy);
        throw new ArgumentException(ResourceStrings.InvalidSharedResourceRequestedBy((object) requestedBy), nameof (requestedBy));
      }
      this.m_buildHost.SecurityManager.CheckPrivilege(requestContext, AdministrationPermissions.UseBuildResources, false);
      if (!string.IsNullOrEmpty(buildUri) && buildProjectId == Guid.Empty && !this.TryGetProjectIdFromBuildUri(requestContext, buildUri, out buildProjectId))
        throw new InvalidBuildUriException(buildUri);
      if (!string.IsNullOrEmpty(requestBuildUri))
      {
        if (requestBuildProjectId == Guid.Empty && !requestBuildUri.Equals(buildUri))
        {
          if (!this.TryGetProjectIdFromBuildUri(requestContext, requestBuildUri, out requestBuildProjectId))
            throw new InvalidBuildUriException(requestBuildUri);
        }
        else if (requestBuildUri.Equals(buildUri))
          requestBuildProjectId = buildProjectId;
      }
      List<SharedResourceData> items;
      using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
        items = component.RequestSharedResourceLock(resourceName, instanceId, requestedBy, buildUri, buildProjectId, requestBuildUri, requestBuildProjectId).GetCurrent<SharedResourceData>().Items;
      TeamFoundationBuildResourceService.NotifySharedResourcesAvailable(requestContext, (IEnumerable<SharedResourceData>) items);
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (RequestSharedResourceLock));
    }

    internal bool TryRequestSharedResourceLock(
      IVssRequestContext requestContext,
      string resourceName,
      string instanceId,
      string requestedBy,
      string buildUri,
      Guid buildProjectId)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (TryRequestSharedResourceLock));
      ArgumentValidation.CheckSharedResourceName(nameof (resourceName), resourceName);
      ArgumentValidation.Check(nameof (instanceId), instanceId, false, (string) null);
      ArgumentValidation.Check(nameof (requestedBy), requestedBy, false, (string) null);
      ArgumentValidation.CheckUri(nameof (buildUri), buildUri, "Build", true, (string) null);
      this.m_buildHost.SecurityManager.CheckPrivilege(requestContext, AdministrationPermissions.UseBuildResources, false);
      if (!string.IsNullOrEmpty(buildUri) && buildProjectId == Guid.Empty && !this.TryGetProjectIdFromBuildUri(requestContext, buildUri, out buildProjectId))
        throw new InvalidBuildUriException(buildUri);
      using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
      {
        requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (TryRequestSharedResourceLock));
        return component.TryRequestSharedResourceLock(resourceName, instanceId, requestedBy, buildUri, buildProjectId);
      }
    }

    internal AgentReservation ReserveBuildAgent(
      IVssRequestContext requestContext,
      Guid buildProjectId,
      string buildUri,
      string name,
      IList<string> requiredTags,
      TagComparison tagComparison)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (ReserveBuildAgent));
      ArgumentValidation.CheckUri(nameof (buildUri), buildUri, false, (string) null);
      if (requiredTags != null)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ArgumentValidation.CheckArray<string>(nameof (requiredTags), requiredTags, TeamFoundationBuildResourceService.\u003C\u003EO.\u003C0\u003E__CheckBuildAgentTag ?? (TeamFoundationBuildResourceService.\u003C\u003EO.\u003C0\u003E__CheckBuildAgentTag = new Validate<string>(Validation.CheckBuildAgentTag)), false, (string) null);
      }
      this.m_buildHost.SecurityManager.CheckPrivilege(requestContext, AdministrationPermissions.UseBuildResources, false);
      if (buildProjectId == Guid.Empty && !this.TryGetProjectIdFromBuildUri(requestContext, buildUri, out buildProjectId))
        throw new InvalidBuildUriException(buildUri);
      AgentReservation current1;
      List<AgentReservationData> items;
      using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
      {
        ResultCollection resultCollection = component.ReserveBuildAgent(buildUri, buildProjectId, name, requiredTags, tagComparison);
        ObjectBinder<AgentReservation> current2 = resultCollection.GetCurrent<AgentReservation>();
        current2.MoveNext();
        current1 = current2.Current;
        resultCollection.NextResult();
        items = resultCollection.GetCurrent<AgentReservationData>().Items;
      }
      BuildController.NotifyAgentsAvailable(requestContext, (IEnumerable<AgentReservationData>) items);
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (ReserveBuildAgent));
      return current1;
    }

    internal void QueueMessage(
      IVssRequestContext requestContext,
      Message message,
      string endpointUrl)
    {
      BuildServiceHost serviceHost = (BuildServiceHost) null;
      BuildController controller = (BuildController) null;
      string host = message.Headers.To.Host;
      int num1 = 0;
      if (!string.IsNullOrEmpty(endpointUrl))
        num1 = BuildServiceHost.GetVersion(endpointUrl);
      if (num1 < 400)
      {
        int num2 = host.IndexOf("-");
        string str = "vstfs:///Build/ServiceHost/" + host.Substring(num2 + 1, host.Length - num2 - 1);
        BuildServiceHostQueryResult serviceHostQueryResult = this.QueryBuildServiceHostsByUri(requestContext, (IList<string>) ((IEnumerable<string>) new string[1]
        {
          str
        }).ToList<string>());
        serviceHost = serviceHostQueryResult.ServiceHosts.FirstOrDefault<BuildServiceHost>();
        controller = serviceHostQueryResult.Controllers.FirstOrDefault<BuildController>();
        num1 = BuildServiceHost.GetVersion(serviceHost.BaseUrl);
      }
      if (num1 < 400)
      {
        string action = message.Headers.Action;
        if (action == "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/DeleteBuildDrop" || action == "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/DeleteBuildSymbols")
          this.m_deleteQueue.Enqueue(new TeamFoundationBuildResourceService.DeleteArguments()
          {
            ServiceHost = serviceHost,
            Controller = controller,
            Message = message
          }, false);
        else
          BuildServiceHostMessageTranslator.QueueMessage(requestContext, message, serviceHost, controller);
      }
      else
        requestContext.GetService<TeamFoundationMessageQueueService>().EnqueueMessage(requestContext, host, message);
    }

    private void DeleteCallback(IAsyncResult result)
    {
      TeamFoundationBuildResourceService.DeleteArguments deleteArguments;
      if (this.m_deleteQueue.EndDequeue(result, out deleteArguments) && deleteArguments == null)
        return;
      bool flag1 = false;
      try
      {
        using (IVssRequestContext systemContext = this.m_deploymentHost.CreateSystemContext())
        {
          bool flag2;
          do
          {
            using (IVssRequestContext requestContext = systemContext.GetService<TeamFoundationHostManagementService>().BeginRequest(systemContext, this.m_serviceHost.InstanceId, RequestContextType.SystemContext, true, true))
              BuildServiceHostMessageTranslator.DeleteBuildOutput(requestContext, deleteArguments.ServiceHost, deleteArguments.Controller, deleteArguments.Message);
            flag2 = this.m_deleteQueue.Dequeue(TimeSpan.Zero, out deleteArguments);
            if (systemContext.IsCanceled || flag2 && deleteArguments == null)
            {
              flag1 = true;
              break;
            }
          }
          while (flag2);
        }
      }
      catch (HostShutdownException ex)
      {
        flag1 = true;
        TeamFoundationTracingService.TraceExceptionRaw(0, "Build", "Service", (Exception) ex);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, "Build", "Service", ex);
      }
      finally
      {
        if (!flag1)
          this.m_deleteQueue.BeginDequeue(TimeSpan.MaxValue, new AsyncCallback(this.DeleteCallback), (object) null);
      }
    }

    private bool TryGetProjectIdFromBuildUri(
      IVssRequestContext requestContext,
      string uri,
      out Guid projectId)
    {
      TeamFoundationBuildService service = requestContext.GetService<TeamFoundationBuildService>();
      IVssRequestContext requestContext1 = requestContext.Elevate();
      List<string> uris = new List<string>();
      uris.Add(uri);
      Guid projectId1 = new Guid();
      using (TeamFoundationDataReader foundationDataReader = service.QueryBuildsByUri(requestContext1, (IList<string>) uris, (IList<string>) null, QueryOptions.Definitions, QueryDeletedOption.ExcludeDeleted, projectId1, false))
      {
        BuildQueryResult buildQueryResult = foundationDataReader.Current<BuildQueryResult>();
        if (buildQueryResult.Definitions.Count > 1 || buildQueryResult.Definitions.Count == 0)
        {
          projectId = Guid.Empty;
          return false;
        }
        projectId = buildQueryResult.Definitions[0].TeamProject.Id;
        return true;
      }
    }

    internal static void NotifySharedResourcesAvailable(
      IVssRequestContext requestContext,
      IEnumerable<SharedResourceData> resources)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (NotifySharedResourcesAvailable));
      TeamFoundationBuildResourceService service = requestContext.GetService<TeamFoundationBuildResourceService>();
      foreach (SharedResourceData resource in resources)
      {
        Message message = (Message) null;
        try
        {
          message = SharedResourceNotifyService.NotifySharedResourceAcquired(resource.ResourceName, resource.InstanceId, resource.BuildUri, resource.LockedBy);
          message.Headers.To = new Uri(resource.MessageQueueUrl);
          requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Enqueuing message '{0}'", (object) message);
          service.QueueMessage(requestContext.Elevate(), message, resource.EndpointUrl);
        }
        catch (MessageQueueNotFoundException ex)
        {
          requestContext.TraceException(0, "BuildAdministration", "Service", (Exception) ex);
        }
        finally
        {
          if (message != null)
          {
            requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Closing message '{0}'", (object) message);
            message.Close();
          }
        }
      }
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (NotifySharedResourcesAvailable));
    }

    private sealed class ServiceLockToken : IDisposable
    {
      private TeamFoundationLock m_globalLock;
      private TeamFoundationLock m_objectLock;
      private IVssRequestContext m_requestContext;

      public ServiceLockToken(IVssRequestContext requestContext, ICollection<string> uris)
      {
        this.m_requestContext = requestContext;
        ITeamFoundationLockingService service = requestContext.GetService<ITeamFoundationLockingService>();
        if (uris != null && uris.Count == 1)
        {
          this.m_globalLock = service.AcquireLock(requestContext, TeamFoundationLockMode.Shared, "tfs://Build/ServiceUpdate");
          requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Acquired lock '{0}' in shared mode", (object) this.m_globalLock.Resources[0]);
          this.m_objectLock = service.AcquireLock(requestContext, TeamFoundationLockMode.Exclusive, uris.First<string>());
          requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Acquired lock '{0}' in exclusive mode", (object) this.m_objectLock.Resources[0]);
        }
        else
        {
          this.m_globalLock = service.AcquireLock(requestContext, TeamFoundationLockMode.Exclusive, "tfs://Build/ServiceUpdate");
          requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Acquired lock '{0}' in exclusive mode", (object) this.m_globalLock.Resources[0]);
        }
      }

      public void Dispose()
      {
        if (this.m_objectLock != null)
        {
          this.m_requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Releasing lock '{0}'", (object) this.m_objectLock.Resources[0]);
          this.m_objectLock.Dispose();
        }
        if (this.m_globalLock == null)
          return;
        this.m_requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Releasing lock '{0}'", (object) this.m_globalLock.Resources[0]);
        this.m_globalLock.Dispose();
      }
    }

    private sealed class DeleteArguments
    {
      public BuildServiceHost ServiceHost { get; set; }

      public BuildController Controller { get; set; }

      public Message Message { get; set; }
    }
  }
}
