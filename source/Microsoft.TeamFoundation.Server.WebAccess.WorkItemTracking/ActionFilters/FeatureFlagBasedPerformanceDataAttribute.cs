// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.ActionFilters.FeatureFlagBasedPerformanceDataAttribute
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 74AD14A4-225D-46D2-B154-945941A2D167
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.ActionFilters
{
  public class FeatureFlagBasedPerformanceDataAttribute : ActionFilterAttribute
  {
    public string Area { get; private set; }

    public string Feature { get; private set; }

    public string FeatureFlagName { get; private set; }

    public string ThresholdRegistryPath { get; private set; }

    public int DefaultThresholdMs { get; private set; }

    public FeatureFlagBasedPerformanceDataAttribute(
      string area,
      string feature,
      string featureFlagName,
      string thresholdRegistryPath,
      int defaultThresholdMs)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(area, nameof (area));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(feature, nameof (feature));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(featureFlagName, nameof (featureFlagName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(thresholdRegistryPath, nameof (thresholdRegistryPath));
      ArgumentUtility.CheckForOutOfRange(defaultThresholdMs, nameof (defaultThresholdMs), 1);
      this.Area = area;
      this.Feature = feature;
      this.FeatureFlagName = featureFlagName;
      this.ThresholdRegistryPath = thresholdRegistryPath;
      this.DefaultThresholdMs = defaultThresholdMs;
    }

    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
      base.OnActionExecuted(filterContext);
      if (!(filterContext.Controller is IPlatformController controller))
        return;
      IVssRequestContext tfsRequestContext = controller.TfsRequestContext;
      if (tfsRequestContext == null)
        return;
      try
      {
        this.OnActionExecutedInternal(tfsRequestContext, filterContext.ActionDescriptor.ControllerDescriptor.ControllerName + "." + filterContext.ActionDescriptor.ActionName);
      }
      catch (Exception ex)
      {
        tfsRequestContext.TraceException(599999, "WebAccess", TfsTraceLayers.Framework, ex);
      }
    }

    internal void OnActionExecutedInternal(IVssRequestContext requestContext, string actionName)
    {
      if (!this.ShouldCollectData(requestContext))
        return;
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("ActionName", actionName);
      properties.Add("ActivityId", (object) requestContext.ActivityId);
      properties.Add("Timings", (object) (requestContext.GetTraceTimingKeyValueList() ?? Enumerable.Empty<KeyValuePair<string, int>>()));
      IDictionary<string, IEnumerable<KeyValuePair<string, int>>> source;
      if (requestContext.TryGetItem<IDictionary<string, IEnumerable<KeyValuePair<string, int>>>>("WorkItemTracking.SqlTimings", out source) && source != null && source.Any<KeyValuePair<string, IEnumerable<KeyValuePair<string, int>>>>())
        properties.Add("SqlTimings", (object) source);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, this.Area, this.Feature, properties);
    }

    private bool ShouldCollectData(IVssRequestContext requestContext)
    {
      if (!requestContext.IsFeatureEnabled(this.FeatureFlagName))
        return false;
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) this.ThresholdRegistryPath, true, this.DefaultThresholdMs);
      return requestContext.ExecutionTime() / 1000L > (long) num;
    }
  }
}
