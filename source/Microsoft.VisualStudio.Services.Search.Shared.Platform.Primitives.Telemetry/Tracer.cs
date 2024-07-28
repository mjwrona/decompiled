// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4AA9C920-1627-4C01-9F3D-849A7BC9C916
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebPlatform;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry
{
  public static class Tracer
  {
    private const string ContextDataName = "17dba268-4702-4154-a536-9fafb417624a";
    private const string Scope = "SearchService";
    private static bool s_isOnPremiseEnvironment;

    public static void SetLogicalContext(IDiagnosticContext traceContext)
    {
      Tracer.s_isOnPremiseEnvironment = traceContext.IsOnPremisesEnvironment;
      if (HttpContext.Current != null)
        HttpContext.Current.Items[(object) "17dba268-4702-4154-a536-9fafb417624a"] = (object) traceContext;
      else
        CallContext.LogicalSetData("17dba268-4702-4154-a536-9fafb417624a", (object) traceContext);
    }

    public static void ClearLogicalContext()
    {
      if (HttpContext.Current != null)
        HttpContext.Current.Items.Remove((object) "17dba268-4702-4154-a536-9fafb417624a");
      else
        CallContext.FreeNamedDataSlot("17dba268-4702-4154-a536-9fafb417624a");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IDiagnosticContext GetLogicalContext() => HttpContext.Current != null ? HttpContext.Current.Items[(object) "17dba268-4702-4154-a536-9fafb417624a"] as IDiagnosticContext : CallContext.LogicalGetData("17dba268-4702-4154-a536-9fafb417624a") as IDiagnosticContext;

    public static void TraceEnter(TraceMetaData traceMetadata, [CallerMemberName] string methodName = "") => Tracer.TraceEnter(traceMetadata.TracePoint, traceMetadata.TraceArea, traceMetadata.TraceLayer, methodName);

    public static void TraceEnter(
      int tracepoint,
      string tracearea,
      string tracelayer,
      [CallerMemberName] string methodName = "")
    {
      Tracer.GetLogicalContext().TraceEnter(tracepoint, tracearea, tracelayer, methodName);
    }

    public static void TraceError() => throw new NotImplementedException();

    public static void TraceLeave(TraceMetaData traceMetadata, [CallerMemberName] string methodName = "") => Tracer.TraceLeave(traceMetadata.TracePoint, traceMetadata.TraceArea, traceMetadata.TraceLayer, methodName);

    public static void TraceLeave(
      int tracepoint,
      string tracearea,
      string tracelayer,
      [CallerMemberName] string methodName = "")
    {
      Tracer.GetLogicalContext().TraceLeave(tracepoint, tracearea, tracelayer, methodName);
    }

    public static void TraceException(TraceMetaData traceMetaData, Exception e) => Tracer.TraceException(traceMetaData.TracePoint, traceMetaData.TraceArea, traceMetaData.TraceLayer, e);

    public static void TraceException(
      int tracepoint,
      string tracearea,
      string tracelayer,
      Exception e)
    {
      Tracer.GetLogicalContext().TraceException(tracepoint, tracearea, tracelayer, e);
    }

    public static void TraceInfo(TraceMetaData traceMetaData, [Localizable(false)] string message, string[] tags = null) => Tracer.TraceInfo(traceMetaData.TracePoint, traceMetaData.TraceArea, traceMetaData.TraceLayer, message, tags);

    public static void TraceInfoConditionally(TraceMetaData traceMetaData, Func<string> message) => Tracer.TraceInfoConditionally(traceMetaData.TracePoint, traceMetaData.TraceArea, traceMetaData.TraceLayer, message);

    public static void TraceInfo(
      int tracepoint,
      string tracearea,
      string tracelayer,
      [Localizable(false)] string message,
      string[] tags = null)
    {
      Tracer.TraceRaw(tracepoint, TraceLevel.Info, tracearea, tracelayer, message, tags);
    }

    public static void TraceInfoConditionally(
      int tracepoint,
      string tracearea,
      string tracelayer,
      Func<string> message)
    {
      Tracer.TraceRawConditionally(tracepoint, TraceLevel.Info, tracearea, tracelayer, message);
    }

    public static void TraceWarning(TraceMetaData traceMetaData, [Localizable(false)] string message, string[] tags = null) => Tracer.TraceWarning(traceMetaData.TracePoint, traceMetaData.TraceArea, traceMetaData.TraceLayer, message, tags);

    public static void TraceWarningConditionally(TraceMetaData traceMetaData, Func<string> message) => Tracer.TraceWarningConditionally(traceMetaData.TracePoint, traceMetaData.TraceArea, traceMetaData.TraceLayer, message);

    public static void TraceWarning(
      int tracepoint,
      string tracearea,
      string tracelayer,
      [Localizable(false)] string message,
      string[] tags = null)
    {
      Tracer.TraceRaw(tracepoint, TraceLevel.Warning, tracearea, tracelayer, message, tags);
    }

    public static void TraceWarningConditionally(
      int tracepoint,
      string tracearea,
      string tracelayer,
      Func<string> message)
    {
      Tracer.TraceRawConditionally(tracepoint, TraceLevel.Warning, tracearea, tracelayer, message);
    }

    public static void TraceError(TraceMetaData traceMetaData, [Localizable(false)] string message, string[] tags = null) => Tracer.TraceError(traceMetaData.TracePoint, traceMetaData.TraceArea, traceMetaData.TraceLayer, message, tags);

    public static void TraceErrorConditionally(TraceMetaData traceMetaData, Func<string> message) => Tracer.TraceErrorConditionally(traceMetaData.TracePoint, traceMetaData.TraceArea, traceMetaData.TraceLayer, message);

    public static void TraceError(
      int tracepoint,
      string tracearea,
      string tracelayer,
      [Localizable(false)] string message,
      string[] tags = null)
    {
      Tracer.TraceRaw(tracepoint, TraceLevel.Error, tracearea, tracelayer, message, tags);
    }

    public static void TraceErrorWithStackTrace(
      int tracepoint,
      string tracearea,
      string tracelayer,
      [Localizable(false)] string message,
      string[] tags = null)
    {
      message += FormattableString.Invariant(FormattableStringFactory.Create(" Stack trace: [{0}]", (object) EnvironmentWrapper.ToReadableStackTrace()));
      Tracer.TraceError(tracepoint, tracearea, tracelayer, message, tags);
    }

    public static void TraceErrorConditionally(
      int tracepoint,
      string tracearea,
      string tracelayer,
      Func<string> message)
    {
      Tracer.TraceRawConditionally(tracepoint, TraceLevel.Error, tracearea, tracelayer, message);
    }

    public static void TraceVerbose(TraceMetaData traceMetaData, [Localizable(false)] string message, string[] tags = null) => Tracer.TraceVerbose(traceMetaData.TracePoint, traceMetaData.TraceArea, traceMetaData.TraceLayer, message, tags);

    public static void TraceVerboseConditionally(TraceMetaData traceMetaData, Func<string> message) => Tracer.TraceVerboseConditionally(traceMetaData.TracePoint, traceMetaData.TraceArea, traceMetaData.TraceLayer, message);

    public static void TraceVerbose(
      int tracepoint,
      string tracearea,
      string tracelayer,
      [Localizable(false)] string message,
      string[] tags = null)
    {
      Tracer.TraceRaw(tracepoint, TraceLevel.Verbose, tracearea, tracelayer, message, tags);
    }

    public static void TraceVerboseConditionally(
      int tracepoint,
      string tracearea,
      string tracelayer,
      Func<string> message)
    {
      Tracer.TraceRawConditionally(tracepoint, TraceLevel.Verbose, tracearea, tracelayer, message);
    }

    public static void TraceAlways(
      int tracepoint,
      TraceLevel level,
      string tracearea,
      string tracelayer,
      [Localizable(false)] string message)
    {
      Tracer.GetLogicalContext().TraceAlways(tracepoint, level, tracearea, tracelayer, message);
    }

    public static void SmartAssert(bool condition, [Localizable(false)] string message = "")
    {
      if (condition)
        return;
      message = message + "\r\nStack trace: " + EnvironmentWrapper.ToReadableStackTrace();
      Tracer.TraceError(1081000, "Diagnostics", "Diagnostics", message);
    }

    public static void EnterMethod(MethodInformation methodInformation) => Tracer.GetLogicalContext().EnterMethod(methodInformation);

    public static void LeaveMethod() => Tracer.GetLogicalContext().LeaveMethod();

    public static void PublishKpi(
      string kpiArea,
      List<KpiMetric> kpiMetrics,
      bool requiredForOnPrem = false)
    {
      if (Tracer.s_isOnPremiseEnvironment)
      {
        if (!requiredForOnPrem)
          return;
        CustomerIntelligenceData ciData = new CustomerIntelligenceData();
        if (kpiMetrics == null || kpiMetrics.Count <= 0)
          return;
        kpiMetrics.ForEach((Action<KpiMetric>) (m => ciData.Add(m.Name, m.Value)));
        Tracer.PublishCi(kpiArea, kpiArea, ciData, true);
      }
      else
        Tracer.GetLogicalContext().LogKpi(kpiArea, "SearchService", kpiMetrics);
    }

    public static void PublishKpi(
      string kpiName,
      string kpiArea,
      double value,
      bool requiredForOnPrem = false)
    {
      if (Tracer.s_isOnPremiseEnvironment)
      {
        if (!requiredForOnPrem)
          return;
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add(kpiName, value);
        Tracer.PublishCi(kpiArea, kpiArea, properties, true);
      }
      else
        Tracer.GetLogicalContext().LogKpi(kpiArea, "SearchService", kpiName, value);
    }

    public static void PublishKpiAndCi(
      string kpiName,
      string kpiArea,
      double value,
      bool requiredForOnPrem = false)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(kpiName, value);
      Tracer.PublishCi(kpiArea, kpiArea, properties, requiredForOnPrem);
      Tracer.GetLogicalContext().LogKpi(kpiArea, "SearchService", kpiName, value);
    }

    public static void PublishCi(
      string area,
      string feature,
      CustomerIntelligenceData properties,
      bool requiredForOnPrem = false)
    {
      properties.Add("ActivityId", Tracer.GetLogicalContext().ActivityId.ToString());
      Tracer.GetLogicalContext().LogCiData(area, feature, properties);
      if (!requiredForOnPrem)
        return;
      Tracer.AddToOnPremiseTelemetryProperties(area, properties.GetData());
    }

    public static void PublishCi(
      string area,
      string feature,
      IDictionary<string, object> properties,
      bool requiredForOnPrem = false)
    {
      Tracer.PublishCi(area, feature, new CustomerIntelligenceData(properties), requiredForOnPrem);
    }

    public static void PublishClientTrace(
      string area,
      string feature,
      ClientTraceData properties,
      bool requiredForOnPrem = false)
    {
      properties.Add("ActivityId", (object) Tracer.GetLogicalContext().ActivityId.ToString());
      Tracer.GetLogicalContext().LogClientTraceData(area, feature, properties);
      if (!requiredForOnPrem)
        return;
      Tracer.AddToOnPremiseTelemetryProperties(area, properties.GetData());
    }

    public static void PublishClientTrace(
      string area,
      string feature,
      IDictionary<string, object> properties,
      bool requiredForOnPrem = false)
    {
      Tracer.PublishClientTrace(area, feature, new ClientTraceData(properties), requiredForOnPrem);
    }

    public static void PublishClientTrace(
      string area,
      string feature,
      string name,
      object value,
      bool requiredForOnPrem = false)
    {
      ClientTraceData properties = new ClientTraceData();
      properties.Add(name, value);
      Tracer.PublishClientTrace(area, feature, properties, requiredForOnPrem);
    }

    public static void PublishClientTraceMessage(
      string area,
      string feature,
      Level level,
      string message)
    {
      Tracer.GetLogicalContext().LogClientTraceData(area, feature, level, message);
    }

    public static void PublishClientTraceException(
      string area,
      string feature,
      Exception exception)
    {
      Tracer.GetLogicalContext().LogClientTraceData(area, feature, exception);
    }

    public static void PublishCi(string area, string feature, string property, bool value) => Tracer.GetLogicalContext().LogCiData(area, feature, property, value);

    public static void PublishCi(string area, string feature, string property, double value) => Tracer.GetLogicalContext().LogCiData(area, feature, property, value);

    public static void PublishCi(string area, string feature, string property, string value) => Tracer.GetLogicalContext().LogCiData(area, feature, property, value);

    public static void PublishOnPremiseIndicator(string eventName) => Tracer.GetLogicalContext().LogIndicator(eventName);

    public static void AddToOnPremiseTelemetryProperties(
      string traceArea,
      IDictionary<string, object> telemetryPropertyNameToValueMap)
    {
      if (!Tracer.s_isOnPremiseEnvironment)
        return;
      IDictionary<string, string> propertyToEventMap = SearchTelemetryEvents.GetPropertyToEventMap(traceArea);
      ICollection<string> additiveProperties = SearchTelemetryEvents.GetAdditiveProperties(traceArea);
      Dictionary<string, string> telemetryProperties = Tracer.GetLogicalContext().OnPremiseTelemetryProperties;
      foreach (string key1 in telemetryPropertyNameToValueMap.Keys.Intersect<string>((IEnumerable<string>) propertyToEventMap.Keys).ToList<string>())
      {
        string key2 = propertyToEventMap[key1];
        string s = telemetryPropertyNameToValueMap[key1].ToString();
        string empty;
        if (!telemetryProperties.TryGetValue(key2, out empty))
          empty = string.Empty;
        double result1;
        double result2;
        if (additiveProperties.Contains(key1) && double.TryParse(s, out result1) && double.TryParse(empty, out result2))
          s = (result1 + result2).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        telemetryProperties[key2] = s;
      }
    }

    public static IDictionary<string, string> GetOnPremiseTelemetryProperties()
    {
      Dictionary<string, string> telemetryProperties = new Dictionary<string, string>();
      foreach (KeyValuePair<string, string> telemetryProperty in Tracer.GetLogicalContext().OnPremiseTelemetryProperties)
        telemetryProperties[telemetryProperty.Key] = telemetryProperty.Value;
      return (IDictionary<string, string>) telemetryProperties;
    }

    private static void TraceRaw(
      int tracepoint,
      TraceLevel level,
      string tracearea,
      string tracelayer,
      string message,
      string[] tags = null)
    {
      Tracer.GetLogicalContext().Trace(tracepoint, level, tracearea, tracelayer, message, tags);
    }

    private static void TraceRawConditionally(
      int tracepoint,
      TraceLevel level,
      string tracearea,
      string tracelayer,
      Func<string> message)
    {
      Tracer.GetLogicalContext().TraceConditionally(tracepoint, level, tracearea, tracelayer, message);
    }
  }
}
