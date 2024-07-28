// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.PlanThrottleService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal class PlanThrottleService : IPlanThrottleService, IVssFrameworkService
  {
    private 
    #nullable disable
    IDictionary<string, PlanConcurrency> m_planConcurrencyLookup = (IDictionary<string, PlanConcurrency>) ImmutableDictionary<string, PlanConcurrency>.Empty;
    private int m_maxConcurrencyLimit;
    private int m_maxConcurrencyCountLimt;

    public int GetAvailableQueueSlots(IVssRequestContext requestContext)
    {
      using (TaskTrackingComponent component = requestContext.CreateComponent<TaskTrackingComponent>("DistributedTaskOrchestration"))
        return Math.Max(0, this.GetMaxQueuedPlans(requestContext) - component.GetPlanStartedCount(DateTime.UtcNow.AddMinutes(-1.0), DateTime.UtcNow));
    }

    public PlanConcurrency GetPlanConcurrency(
      IVssRequestContext requestContext,
      string hubName,
      Guid scopeIdentifier,
      TaskOrchestrationOwner definitionReference)
    {
      TaskHub taskHub = requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, hubName, false);
      if (taskHub == null)
        throw new TaskHubNotFoundException(TaskResources.HubNotFound((object) hubName));
      PlanConcurrency planConcurrency;
      return this.m_planConcurrencyLookup.TryGetValue(PlanConcurrency.GetIdentifier(taskHub.DataspaceCategory, scopeIdentifier, definitionReference?.Id), out planConcurrency) ? planConcurrency : (PlanConcurrency) null;
    }

    public IList<PlanConcurrency> GetPlanConcurrency(IVssRequestContext requestContext) => (IList<PlanConcurrency>) this.m_planConcurrencyLookup.Values.ToImmutableList<PlanConcurrency>();

    public IList<PlanConcurrency> SetPlanConcurrency(
      IVssRequestContext requestContext,
      string hubName,
      Guid scopeIdentifier,
      IList<int> definitionIds,
      int? concurrency,
      string incidentId = null)
    {
      if (concurrency.HasValue)
        ArgumentUtility.CheckBoundsInclusive(concurrency.Value, 0, this.m_maxConcurrencyLimit, nameof (concurrency));
      TaskHub taskHub = requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, hubName, false);
      if (taskHub == null)
        throw new TaskHubNotFoundException(TaskResources.HubNotFound((object) hubName));
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      ICollection<PlanConcurrency> source;
      IDictionary<string, PlanConcurrency> dictionary = !JsonUtilities.TryDeserialize<ICollection<PlanConcurrency>>(service.GetValue<string>(requestContext, (RegistryQuery) RegistryKeys.PlanConcurrencyConfigurationJson, false, (string) null), out source, true) ? (IDictionary<string, PlanConcurrency>) new Dictionary<string, PlanConcurrency>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (IDictionary<string, PlanConcurrency>) source.ToDictionary<PlanConcurrency, string, PlanConcurrency>((Func<PlanConcurrency, string>) (x => string.Format("{0}/{1}/{2}", (object) x.DataspaceCategory, (object) x.ScopeIdentifier, (object) x.DefinitionId)), (Func<PlanConcurrency, PlanConcurrency>) (x => x), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (int definitionId in (IEnumerable<int>) definitionIds)
      {
        string identifier = PlanConcurrency.GetIdentifier(taskHub.DataspaceCategory, scopeIdentifier, new int?(definitionId));
        string str = incidentId != null ? TaskResources.InternalCustomerThrottling((object) incidentId) : TaskResources.ExternalCustomerThrottling();
        if (concurrency.HasValue)
          dictionary[identifier] = new PlanConcurrency()
          {
            DataspaceCategory = taskHub.DataspaceCategory,
            ScopeIdentifier = scopeIdentifier,
            DefinitionId = definitionId,
            MaxConcurrency = concurrency.Value,
            Message = str,
            IncidentId = incidentId
          };
        else
          dictionary.Remove(identifier);
      }
      if (dictionary.Count > this.m_maxConcurrencyCountLimt)
        throw new ArgumentOutOfRangeException(nameof (definitionIds), string.Format("Only a maximum of {0} definitions can be throttled for this host. Please evaluate existing throttling on other definitions and try again.", (object) this.m_maxConcurrencyCountLimt));
      service.SetValue<string>(requestContext, RegistryKeys.PlanConcurrencyConfigurationJson, dictionary.Values.Serialize<ICollection<PlanConcurrency>>(true));
      return (IList<PlanConcurrency>) dictionary.Values.ToImmutableList<PlanConcurrency>();
    }

    public bool ShouldThrottleNewPlans(
      IVssRequestContext requestContext,
      TaskHub hub,
      Guid scopeIdentifier,
      TaskOrchestrationOwner definitionReference)
    {
      return this.m_planConcurrencyLookup.ContainsKey(PlanConcurrency.GetIdentifier(hub.DataspaceCategory, scopeIdentifier, definitionReference?.Id)) || this.GetAvailableQueueSlots(requestContext) <= 0;
    }

    public async Task StartThrottledPlan(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      TaskHub hub)
    {
      UpdatePlanResult updatePlanResult;
      using (TaskTrackingComponent trackingComponent = requestContext.CreateComponent<TaskTrackingComponent>(hub.DataspaceCategory))
      {
        updatePlanResult = await trackingComponent.UpdatePlanAsync(plan.ScopeIdentifier, plan.PlanId, new DateTime?(), new DateTime?(), new TaskOrchestrationPlanState?(TaskOrchestrationPlanState.Queued), new TaskResult?(), (string) null, (IOrchestrationEnvironment) null);
        if (updatePlanResult.Timeline != null)
          TaskHub.TraceTimelineRecordUpdates(requestContext, (IList<TimelineRecord>) null, (IList<TimelineRecord>) updatePlanResult.Timeline.Records, nameof (StartThrottledPlan));
      }
      try
      {
        hub.StartPlan(requestContext, (TaskAgentPoolReference) null, updatePlanResult.Plan);
      }
      catch (Exception ex)
      {
        requestContext.TraceError(10016170, nameof (PlanThrottleService), string.Format("Failed start plan '{0}' with exception: {1}", (object) plan.PlanId, (object) ex));
        throw;
      }
    }

    public IList<TaskOrchestrationPlan> GetThrottledPlans(
      IVssRequestContext requestContext,
      int maxPlans,
      string hubName = null)
    {
      if (this.m_planConcurrencyLookup.Count > 0)
      {
        using (TaskTrackingComponent component = requestContext.CreateComponent<TaskTrackingComponent>("DistributedTaskOrchestration"))
          return component.GetRunnablePlans(this.m_planConcurrencyLookup.Values, maxPlans);
      }
      else
      {
        using (TaskTrackingComponent component = requestContext.CreateComponent<TaskTrackingComponent>("DistributedTaskOrchestration"))
          return component.GetPlansByState(hubName, TaskOrchestrationPlanState.Throttled, new int?(maxPlans));
      }
    }

    public int GetMaxQueuedPlans(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int num1 = service.GetValue<int>(requestContext, in RegistryKeys.PipelinePlansPerParallelismMinute, true, 4);
      int val1 = service.GetValue<int>(requestContext, in RegistryKeys.PipelinesPlansPerMinuteMax, true, 200);
      int num2 = val1;
      int num3 = num2 <= 0 ? val1 : num2;
      return Math.Min(val1, num3 * num1);
    }

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChangedCallback), true, (RegistryQuery) (RegistryKeys.PlanConcurrencySettingsPath + "**"));
      this.LoadRegistrySettings(requestContext);
    }

    public void ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChangedCallback));

    private void OnRegistrySettingsChangedCallback(
      IVssRequestContext requestcontext,
      RegistryEntryCollection changedentries)
    {
      this.LoadRegistrySettings(requestcontext);
    }

    private void LoadRegistrySettings(IVssRequestContext requestContext)
    {
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) (RegistryKeys.PlanConcurrencySettingsPath + "**"));
      ICollection<PlanConcurrency> source;
      if (JsonUtilities.TryDeserialize<ICollection<PlanConcurrency>>(registryEntryCollection.GetValueFromPath<string>(RegistryKeys.PlanConcurrencyConfigurationJson, (string) null), out source, true))
        this.m_planConcurrencyLookup = (IDictionary<string, PlanConcurrency>) source.ToImmutableDictionary<PlanConcurrency, string, PlanConcurrency>((Func<PlanConcurrency, string>) (x => x.GetIdentifier()), (Func<PlanConcurrency, PlanConcurrency>) (x => x), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_maxConcurrencyLimit = registryEntryCollection.GetValueFromPath<int>(RegistryKeys.PlanConcurrencyMaxConcurrencyLimit, 50);
      this.m_maxConcurrencyCountLimt = registryEntryCollection.GetValueFromPath<int>(RegistryKeys.PlanConcurrencyMaxCountLimit, 10);
    }
  }
}
