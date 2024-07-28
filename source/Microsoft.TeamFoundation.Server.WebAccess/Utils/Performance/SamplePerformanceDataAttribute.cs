// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Utils.Performance.SamplePerformanceDataAttribute
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Utils.Performance
{
  public class SamplePerformanceDataAttribute : ActionFilterAttribute, IActionFilter
  {
    private const int c_defaultSamplingProbability = 256;
    private const string c_defaultTelemetryArea = "WebAccess";
    private const string c_defaultTelemetryFeature = "PerformanceTracing";
    private const string c_ProbabilityModifierRegistryKey = "/Service/WebAccess/Settings/PerformanceSamplingProbabilityModifier";
    private int m_probability;
    private string[] m_parametersToInclude;
    private static bool s_configRead = false;
    private static float s_configuredModifier = 1f;

    public string Area { get; set; }

    public string Feature { get; set; }

    public SamplePerformanceDataAttribute(
      int samplingProbability,
      params string[] parametersToInclude)
    {
      this.m_probability = samplingProbability;
      this.m_parametersToInclude = parametersToInclude;
      this.Area = "WebAccess";
      this.Feature = "PerformanceTracing";
    }

    public SamplePerformanceDataAttribute(params string[] parametersToInclude)
      : this(256, parametersToInclude)
    {
    }

    public SamplePerformanceDataAttribute(int samplingProbability)
      : this(samplingProbability, (string[]) null)
    {
    }

    public SamplePerformanceDataAttribute()
      : this(256, (string[]) null)
    {
    }

    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
      base.OnActionExecuted(filterContext);
      IVssRequestContext vssRequestContext = (IVssRequestContext) null;
      try
      {
        if (!(filterContext.Controller is IPlatformController controller))
          return;
        vssRequestContext = controller.TfsRequestContext;
        if (!vssRequestContext.IsFeatureEnabled("WebAccess.Agile.Backlog.Safeguard.PerformanceSampling") || !this.ShouldCollectData(vssRequestContext))
          return;
        string str = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName + "." + filterContext.ActionDescriptor.ActionName;
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("ActionName", str);
        properties.Add("ActivityId", (object) vssRequestContext.ActivityId);
        properties.Add("Timings", vssRequestContext.GetTraceTimingAsString());
        if (this.m_parametersToInclude != null)
          properties.Add("Parameters", this.GetParameterValues(filterContext));
        vssRequestContext.GetService<CustomerIntelligenceService>().Publish(vssRequestContext, this.Area, this.Feature, properties);
      }
      catch (Exception ex)
      {
        if (vssRequestContext == null)
          return;
        vssRequestContext.TraceException(599999, "WebAccess", TfsTraceLayers.Framework, ex);
      }
    }

    private void ReadConfigurationFromRegistry(IVssRequestContext requestContext)
    {
      if (SamplePerformanceDataAttribute.s_configRead)
        return;
      try
      {
        SamplePerformanceDataAttribute.s_configuredModifier = requestContext.GetService<IVssRegistryService>().GetValue<float>(requestContext, (RegistryQuery) "/Service/WebAccess/Settings/PerformanceSamplingProbabilityModifier", true, 1f);
        SamplePerformanceDataAttribute.s_configRead = true;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(599999, "WebAccess", TfsTraceLayers.Framework, ex);
      }
    }

    private string GetParameterValues(ActionExecutedContext filterContext)
    {
      if (filterContext.Controller == null || filterContext.Controller.ValueProvider == null)
        return string.Empty;
      ParameterDescriptor[] parameters = filterContext.ActionDescriptor.GetParameters();
      StringBuilder stringBuilder = new StringBuilder();
      Func<ParameterDescriptor, bool> predicate = (Func<ParameterDescriptor, bool>) (x => ((IEnumerable<string>) this.m_parametersToInclude).Any<string>((Func<string, bool>) (p => p == x.ParameterName)));
      foreach (string key in ((IEnumerable<ParameterDescriptor>) parameters).Where<ParameterDescriptor>(predicate).Select<ParameterDescriptor, string>((Func<ParameterDescriptor, string>) (x => x.ParameterName)))
      {
        ValueProviderResult valueProviderResult = filterContext.Controller.ValueProvider.GetValue(key);
        if (valueProviderResult != null)
          stringBuilder.AppendFormat("{0}={1} ", (object) key, (object) valueProviderResult.AttemptedValue);
      }
      return stringBuilder.ToString();
    }

    private bool ShouldCollectData(IVssRequestContext requestContext)
    {
      this.ReadConfigurationFromRegistry(requestContext);
      return new Random(requestContext.ActivityId.GetHashCode()).Next(0, Math.Max(1, (int) ((double) this.m_probability * (double) SamplePerformanceDataAttribute.s_configuredModifier))) == 0;
    }
  }
}
