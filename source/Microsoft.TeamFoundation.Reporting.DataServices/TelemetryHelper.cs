// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.TelemetryHelper
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Reflection;

namespace Microsoft.TeamFoundation.Reporting.DataServices
{
  public static class TelemetryHelper
  {
    private static string s_featureArea = "LightWeightCharting";
    private static string s_chartTabulation = "ChartTabulation";
    private static string s_chartConfiguration = "ChartConfiguration";
    private static string s_chartCapabilities = "ChartCapabilities";
    private static string s_chartFailure = "ChartFailure";
    private static string s_filterProvider = "FeatureFilterProviderRequest";
    private static string s_scopeKeyword = "Scope";
    private static string s_RuntimeLoad_UserTime_seconds = "RuntimeLoad_UserTime_S";
    private static string s_contextId = "ContextId";

    public static void PublishTabulationCost(
      IVssRequestContext requestContext,
      string scope,
      TransformOptions transformOptions,
      ICountLoad loadCost)
    {
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      TelemetryHelper.AddCommonContext(requestContext, scope, intelligenceData);
      intelligenceData.AddOptionsInfo(transformOptions);
      intelligenceData.AddPerfInfo(loadCost);
      TelemetryHelper.Publish(requestContext, TelemetryHelper.s_chartTabulation, intelligenceData);
    }

    public static void PublishChartingRequestFailure(
      IVssRequestContext requestContext,
      string scope,
      string operation,
      Exception e)
    {
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      TelemetryHelper.AddCommonContext(requestContext, scope, intelligenceData);
      intelligenceData.Add("Operation", operation);
      intelligenceData.Add("Message", e.Message);
      if (e.TargetSite != (MethodBase) null)
      {
        intelligenceData.Add("TargetSite_Name", e.TargetSite.Name);
        if (e.TargetSite.DeclaringType != (Type) null)
          intelligenceData.Add("TargetSite_ClassName", e.TargetSite.DeclaringType.FullName);
      }
      TelemetryHelper.Publish(requestContext, TelemetryHelper.s_chartFailure, intelligenceData);
    }

    public static void PublishConfigRequest(
      IVssRequestContext requestContext,
      string scope,
      string operation)
    {
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      TelemetryHelper.AddCommonContext(requestContext, scope, intelligenceData);
      intelligenceData.Add("CRUD_Operation", operation);
      TelemetryHelper.Publish(requestContext, TelemetryHelper.s_chartConfiguration, intelligenceData);
    }

    public static void PublishCapabilitiesRequest(
      IVssRequestContext requestContext,
      string scope,
      TimeSpan duration)
    {
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      TelemetryHelper.AddCommonContext(requestContext, scope, intelligenceData);
      intelligenceData.Add(TelemetryHelper.s_RuntimeLoad_UserTime_seconds, duration.TotalSeconds);
      TelemetryHelper.Publish(requestContext, TelemetryHelper.s_chartCapabilities, intelligenceData);
    }

    public static void PublishFilterProviderRequestDetail(
      IVssRequestContext requestContext,
      string scope,
      string filterId,
      int historySampleCount)
    {
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      TelemetryHelper.AddCommonContext(requestContext, scope, intelligenceData);
      intelligenceData.Add("FilterId", filterId);
      intelligenceData.Add("HistorySampleCount", (double) historySampleCount);
      TelemetryHelper.Publish(requestContext, TelemetryHelper.s_filterProvider, intelligenceData);
    }

    private static void Publish(
      IVssRequestContext requestContext,
      string operation,
      CustomerIntelligenceData details)
    {
      try
      {
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, TelemetryHelper.s_featureArea, operation, details);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Reporting", "DataServicesTelemetry", ex);
      }
    }

    private static void AddOptionsInfo(
      this CustomerIntelligenceData ciData,
      TransformOptions options)
    {
      ciData.Add("Transform_TransformId", (object) options.TransformId);
      ciData.Add("Transform_Filter", options.Filter);
      ciData.Add("Transform_HistoryRange", options.HistoryRange);
    }

    private static void AddPerfInfo(this CustomerIntelligenceData ciData, ICountLoad countedLoad)
    {
      ciData.Add("RuntimeLoad_IterationCount", (double) countedLoad.CountedIterations);
      ciData.Add("RuntimeLoad_ProcessingTime", (object) countedLoad.ElapsedProcessingTime);
      ciData.Add("RuntimeLoad_UserTime", (object) countedLoad.ElapsedUserTime);
      ciData.Add("RuntimeLoad_ProcessingTime_S", countedLoad.ElapsedProcessingTime.TotalSeconds);
      ciData.Add(TelemetryHelper.s_RuntimeLoad_UserTime_seconds, countedLoad.ElapsedUserTime.TotalSeconds);
    }

    private static void AddCommonContext(
      IVssRequestContext requestContext,
      string scope,
      CustomerIntelligenceData payload)
    {
      if (scope == FeatureProviderScopes.WorkItemQueriesLegacy || string.IsNullOrWhiteSpace(scope))
        scope = FeatureProviderScopes.WorkItemQueries;
      payload.Add(TelemetryHelper.s_scopeKeyword, scope);
      payload.Add(TelemetryHelper.s_contextId, (double) requestContext.ContextId);
    }
  }
}
