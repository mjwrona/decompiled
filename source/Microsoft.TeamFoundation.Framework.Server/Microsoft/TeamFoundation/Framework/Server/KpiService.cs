// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.KpiService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class KpiService : IKpiService, IVssFrameworkService
  {
    private const string s_Area = "KpiService";
    private const string s_Layer = "BusinessLogic";

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckDeploymentRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void DeleteKpiState(IVssRequestContext requestContext, KpiDefinition kpiDefinition) => this.DeleteKpiStates(requestContext, (IEnumerable<KpiDefinition>) new List<KpiDefinition>()
    {
      kpiDefinition
    });

    public void DeleteKpiStates(
      IVssRequestContext requestContext,
      IEnumerable<KpiDefinition> kpiDefinitions)
    {
      requestContext.TraceEnter(15164000, nameof (KpiService), "BusinessLogic", nameof (DeleteKpiStates));
      try
      {
        foreach (KpiDefinition kpiDefinition in kpiDefinitions)
        {
          ArgumentUtility.CheckStringForNullOrEmpty(kpiDefinition.Area, "KpiDefinition.Area");
          ArgumentUtility.CheckStringForNullOrEmpty(kpiDefinition.Name, "KpiDefinition.Name");
          using (KpiComponent component = requestContext.CreateComponent<KpiComponent>())
            component.DeleteKpiStates(kpiDefinition);
          this.RemoveDefinitionFromCache(requestContext, kpiDefinition.Area, kpiDefinition.Name, kpiDefinition.Scope);
        }
      }
      finally
      {
        requestContext.TraceLeave(15164019, nameof (KpiService), "BusinessLogic", nameof (DeleteKpiStates));
      }
    }

    public void EnsureKpiIsRegistered(
      IVssRequestContext requestContext,
      string area,
      string name,
      string scope,
      string displayName,
      string description = null)
    {
      requestContext.TraceEnter(15164020, nameof (KpiService), "BusinessLogic", nameof (EnsureKpiIsRegistered));
      try
      {
        if (this.GetKpiDefinition(requestContext, area, name, scope) != null)
          return;
        KpiDefinition kpiDefinition = new KpiDefinition()
        {
          Area = area,
          Name = name,
          Scope = scope,
          DisplayName = displayName,
          Description = description
        };
        this.SaveKpi(requestContext, kpiDefinition);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15164024, nameof (KpiService), "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(15164039, nameof (KpiService), "BusinessLogic", nameof (EnsureKpiIsRegistered));
      }
    }

    public KpiDefinition GetKpiDefinition(
      IVssRequestContext requestContext,
      string area,
      string name,
      string scope)
    {
      KpiDefinition definition = (KpiDefinition) null;
      KpiCacheService service = requestContext.GetService<KpiCacheService>();
      if (!service.TryGetValue(requestContext, area, name, scope, out definition))
      {
        definition = this.LoadDefinition(requestContext, area, name, scope);
        service.Set(requestContext, area, name, scope, definition);
      }
      return definition;
    }

    public void Publish(IVssRequestContext requestContext, string area, string name, double value) => this.Publish(requestContext, area, (string) null, name, value);

    public void Publish(
      IVssRequestContext requestContext,
      string area,
      string scope,
      string name,
      double value)
    {
      this.Publish(requestContext, area, Guid.Empty, scope, name, value);
    }

    public void Publish(
      IVssRequestContext requestContext,
      string area,
      Guid hostId,
      string name,
      double value)
    {
      this.Publish(requestContext, area, hostId, (string) null, name, value);
    }

    public void Publish(
      IVssRequestContext requestContext,
      string area,
      Guid hostId,
      string scope,
      string name,
      double value)
    {
      KpiMetric metric = new KpiMetric()
      {
        Name = name,
        Value = value
      };
      this.Publish(requestContext, area, hostId, scope, metric);
    }

    public void Publish(IVssRequestContext requestContext, string area, KpiMetric metric) => this.Publish(requestContext, area, (string) null, metric);

    public void Publish(
      IVssRequestContext requestContext,
      string area,
      string scope,
      KpiMetric metric)
    {
      this.Publish(requestContext, area, Guid.Empty, scope, metric);
    }

    public void Publish(
      IVssRequestContext requestContext,
      string area,
      Guid hostId,
      KpiMetric metric)
    {
      this.Publish(requestContext, area, hostId, (string) null, metric);
    }

    public void Publish(
      IVssRequestContext requestContext,
      string area,
      Guid hostId,
      string scope,
      KpiMetric metric)
    {
      this.Publish(requestContext, area, hostId, scope, new List<KpiMetric>()
      {
        metric
      });
    }

    public void Publish(IVssRequestContext requestContext, string area, List<KpiMetric> metrics) => this.Publish(requestContext, area, (string) null, metrics);

    public void Publish(
      IVssRequestContext requestContext,
      string area,
      string scope,
      List<KpiMetric> metrics)
    {
      this.Publish(requestContext, area, Guid.Empty, scope, metrics);
    }

    public void Publish(
      IVssRequestContext requestContext,
      string area,
      Guid hostId,
      List<KpiMetric> metrics)
    {
      this.Publish(requestContext, area, hostId, (string) null, metrics);
    }

    public void Publish(
      IVssRequestContext requestContext,
      string area,
      Guid hostId,
      string scope,
      List<KpiMetric> metrics)
    {
      if (!this.IsTracingEnabled(requestContext))
        return;
      try
      {
        List<KpiMetricDetail> details = this.ProcessMetrics(requestContext, area, scope, metrics);
        KpiService.NormalizeString(ref area);
        KpiService.NormalizeString(ref scope);
        string json = this.ToJson(requestContext.UniqueIdentifier, area, hostId, scope, details);
        requestContext.TracingService().TraceKpi(json);
        MdmService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<MdmService>();
        foreach (KpiMetricDetail kpiMetricDetail in details)
          service.PublishKpi(requestContext, kpiMetricDetail.Metric.TimeStamp, hostId, kpiMetricDetail.Area, kpiMetricDetail.Scope, kpiMetricDetail.DisplayName, kpiMetricDetail.Description, kpiMetricDetail.Metric.Name, (long) kpiMetricDetail.Metric.Value);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, nameof (KpiService), "BusinessLogic", ex);
      }
    }

    public void SaveKpi(IVssRequestContext requestContext, KpiDefinition kpiDefinition) => this.SaveKpis(requestContext, (IEnumerable<KpiDefinition>) new List<KpiDefinition>()
    {
      kpiDefinition
    });

    public void SaveKpis(
      IVssRequestContext requestContext,
      IEnumerable<KpiDefinition> kpiDefinitions)
    {
      requestContext.TraceEnter(15164040, nameof (KpiService), "BusinessLogic", "RegisterKpis");
      try
      {
        foreach (KpiDefinition kpiDefinition in kpiDefinitions)
        {
          ArgumentUtility.CheckStringForNullOrEmpty(kpiDefinition.Area, "KpiDefinition.Area");
          ArgumentUtility.CheckStringForNullOrEmpty(kpiDefinition.Name, "KpiDefinition.Name");
          ArgumentUtility.CheckStringForNullOrEmpty(kpiDefinition.DisplayName, "KpiDefinition.DisplayName");
          try
          {
            using (KpiComponent component = requestContext.CreateComponent<KpiComponent>())
              component.SaveKpi(kpiDefinition);
          }
          catch (SqlException ex)
          {
            bool flag = false;
            foreach (SqlError error in ex.Errors)
            {
              if (error.Number == 2601)
              {
                flag = true;
                break;
              }
            }
            if (flag)
              requestContext.Trace(15095044, TraceLevel.Error, nameof (KpiService), "BusinessLogic", "KPI definition (Area - {0}, Name - {1}, Scope - {2}) is already saved. SqlException: {3}", (object) kpiDefinition.Area, (object) kpiDefinition.Name, (object) kpiDefinition.Scope, (object) ex);
            else
              throw;
          }
          this.SetStates(requestContext, kpiDefinition.Area, kpiDefinition.Name, kpiDefinition.Scope, kpiDefinition.States);
          this.RemoveDefinitionFromCache(requestContext, kpiDefinition.Area, kpiDefinition.Name, kpiDefinition.Scope);
        }
      }
      finally
      {
        requestContext.TraceLeave(15164059, nameof (KpiService), "BusinessLogic", "RegisterKpis");
      }
    }

    public void SetStates(
      IVssRequestContext requestContext,
      string area,
      string name,
      List<KpiStateDefinition> states)
    {
      this.SetStates(requestContext, area, (string) null, states);
    }

    public void SetStates(
      IVssRequestContext requestContext,
      string area,
      string name,
      string scope,
      List<KpiStateDefinition> states)
    {
      requestContext.TraceEnter(15164060, nameof (KpiService), "BusinessLogic", nameof (SetStates));
      try
      {
        if (states.Count == 0)
          return;
        ArgumentUtility.CheckStringForNullOrEmpty(area, nameof (area));
        ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
        KpiDefinition kpiDefinition = this.GetKpiDefinition(requestContext, area, name, scope);
        List<KpiStateDefinition> combinedStates = kpiDefinition != null ? KpiService.MergeStates(states, kpiDefinition) : throw new KpiNotFoundException(area, name, scope);
        KpiService.VerifyStates(kpiDefinition, combinedStates);
        this.RemoveDefinitionFromCache(requestContext, area, name, scope);
        using (KpiComponent component = requestContext.CreateComponent<KpiComponent>())
          component.SetStates(area, name, scope, states);
      }
      finally
      {
        requestContext.TraceLeave(15164079, nameof (KpiService), "BusinessLogic", nameof (SetStates));
      }
    }

    private bool IsTracingEnabled(IVssRequestContext requestContext)
    {
      ITeamFoundationFeatureAvailabilityService service = requestContext.GetService<ITeamFoundationFeatureAvailabilityService>();
      return requestContext.ExecutionEnvironment.IsHostedDeployment && service.IsFeatureEnabled(requestContext, "VisualStudio.FrameworkService.KpiCollection");
    }

    private KpiDefinition LoadDefinition(
      IVssRequestContext requestContext,
      string area,
      string name,
      string scope)
    {
      KpiDefinition kpiDefinition = (KpiDefinition) null;
      using (KpiComponent component = requestContext.CreateComponent<KpiComponent>())
      {
        ResultCollection kpi = component.GetKpi(area, name, scope);
        List<KpiDefinition> items = kpi.GetCurrent<KpiDefinition>().Items;
        if (items != null)
        {
          if (items.Count == 1)
          {
            kpiDefinition = items[0];
            if (kpi.TryNextResult())
              kpiDefinition.States.AddRange((IEnumerable<KpiStateDefinition>) kpi.GetCurrent<KpiStateDefinition>().Items);
          }
        }
      }
      return kpiDefinition;
    }

    private List<KpiMetricDetail> ProcessMetrics(
      IVssRequestContext requestContext,
      string area,
      string scope,
      List<KpiMetric> metrics)
    {
      List<KpiMetricDetail> kpiMetricDetailList = new List<KpiMetricDetail>();
      foreach (KpiMetric metric in metrics)
      {
        KpiMetricDetail kpiMetricDetail = new KpiMetricDetail(area, scope, metric);
        KpiDefinition kpiDefinition = this.GetKpiDefinition(requestContext, area, metric.Name, scope);
        if (kpiDefinition != null)
        {
          kpiMetricDetail.Description = kpiDefinition.Description;
          kpiMetricDetail.DisplayName = kpiDefinition.DisplayName;
          if (kpiDefinition.States.Count > 0)
          {
            IOrderedEnumerable<KpiStateDefinition> orderedEnumerable = kpiDefinition.States.OrderBy<KpiStateDefinition, KpiState>((Func<KpiStateDefinition, KpiState>) (state => state.KpiState));
            KpiStateDefinition kpiStateDefinition1 = (KpiStateDefinition) null;
            foreach (KpiStateDefinition kpiStateDefinition2 in (IEnumerable<KpiStateDefinition>) orderedEnumerable)
            {
              kpiStateDefinition1 = kpiStateDefinition2;
              kpiMetricDetail.State = kpiStateDefinition2.KpiState;
              if (kpiDefinition.HigherIsBetter)
              {
                if (metric.Value >= kpiStateDefinition2.Limit)
                  break;
              }
              else if (metric.Value <= kpiStateDefinition2.Limit)
                break;
            }
            if (kpiStateDefinition1.EventId != 0)
              TeamFoundationEventLog.Default.Log(requestContext, kpiMetricDetail.GenerateMessage(), kpiStateDefinition1.EventId, KpiService.GetEventLogEntryType(kpiMetricDetail.State), (object) kpiMetricDetail.Area, (object) kpiMetricDetail.Metric.Name, (object) kpiMetricDetail.Scope, (object) kpiMetricDetail.Metric.Value, (object) kpiMetricDetail.State, (object) kpiMetricDetail.Metric.TimeStamp);
          }
        }
        kpiMetricDetailList.Add(kpiMetricDetail);
      }
      return kpiMetricDetailList;
    }

    public static EventLogEntryType GetEventLogEntryType(KpiState state)
    {
      switch (state)
      {
        case KpiState.Good:
          return EventLogEntryType.Information;
        case KpiState.Warning:
          return EventLogEntryType.Warning;
        case KpiState.Error:
          return EventLogEntryType.Error;
        case KpiState.Critical:
          return EventLogEntryType.Error;
        default:
          return EventLogEntryType.Information;
      }
    }

    private void RemoveDefinitionFromCache(
      IVssRequestContext requestContext,
      string area,
      string name,
      string scope)
    {
      requestContext.GetService<KpiCacheService>().Remove(requestContext, area, name, scope);
    }

    private static void VerifyStates(
      KpiDefinition definition,
      List<KpiStateDefinition> combinedStates)
    {
      IOrderedEnumerable<KpiStateDefinition> source = combinedStates.OrderBy<KpiStateDefinition, KpiState>((Func<KpiStateDefinition, KpiState>) (state => state.KpiState));
      KpiStateDefinition kpiStateDefinition1 = source.Last<KpiStateDefinition>();
      KpiStateDefinition kpiStateDefinition2 = (KpiStateDefinition) null;
      foreach (KpiStateDefinition kpiStateDefinition3 in (IEnumerable<KpiStateDefinition>) source)
      {
        if (kpiStateDefinition3 != kpiStateDefinition1 && kpiStateDefinition2 != null)
        {
          if (definition.HigherIsBetter)
          {
            if (kpiStateDefinition2.Limit <= kpiStateDefinition3.Limit)
              throw new InvalidKpiStateException(definition.Area, definition.Name, definition.Scope, kpiStateDefinition3.KpiState);
          }
          else if (kpiStateDefinition2.Limit >= kpiStateDefinition3.Limit)
            throw new InvalidKpiStateException(definition.Area, definition.Name, definition.Scope, kpiStateDefinition3.KpiState);
        }
        kpiStateDefinition2 = kpiStateDefinition3;
      }
    }

    private static List<KpiStateDefinition> MergeStates(
      List<KpiStateDefinition> states,
      KpiDefinition definition)
    {
      Dictionary<KpiState, KpiStateDefinition> dictionary1 = states.ToDictionary<KpiStateDefinition, KpiState>((Func<KpiStateDefinition, KpiState>) (l1 => l1.KpiState));
      Dictionary<KpiState, KpiStateDefinition> dictionary2 = definition.States.ToDictionary<KpiStateDefinition, KpiState>((Func<KpiStateDefinition, KpiState>) (l2 => l2.KpiState));
      List<KpiState> list = dictionary1.Keys.Union<KpiState>((IEnumerable<KpiState>) dictionary2.Keys).ToList<KpiState>();
      List<KpiStateDefinition> kpiStateDefinitionList = new List<KpiStateDefinition>();
      foreach (KpiState key in list)
      {
        KpiStateDefinition kpiStateDefinition;
        if (dictionary1.TryGetValue(key, out kpiStateDefinition))
          kpiStateDefinitionList.Add(kpiStateDefinition);
        else
          kpiStateDefinitionList.Add(dictionary2[key]);
      }
      return kpiStateDefinitionList;
    }

    private string ToJson(
      Guid uniqueIdentifier,
      string area,
      Guid hostId,
      string scope,
      List<KpiMetricDetail> details)
    {
      return JsonConvert.SerializeObject((object) new Dictionary<string, object>()
      {
        [nameof (uniqueIdentifier)] = (object) uniqueIdentifier,
        [nameof (area)] = (object) area,
        [nameof (hostId)] = (object) hostId,
        [nameof (scope)] = (object) scope,
        ["metrics"] = (details == null ? (object) (IEnumerable<Dictionary<string, object>>) null : (object) details.Select<KpiMetricDetail, Dictionary<string, object>>((Func<KpiMetricDetail, Dictionary<string, object>>) (x => x.ToJson())))
      });
    }

    private static void NormalizeString(ref string message)
    {
      if (message != null)
        return;
      message = string.Empty;
    }
  }
}
