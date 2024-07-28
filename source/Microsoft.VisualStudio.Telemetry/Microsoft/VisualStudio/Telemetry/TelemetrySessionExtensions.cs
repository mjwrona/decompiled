// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetrySessionExtensions
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry
{
  public static class TelemetrySessionExtensions
  {
    public static TelemetryEventCorrelation PostUserTask(
      this TelemetrySession session,
      string eventName,
      TelemetryResult result,
      string resultSummary = null,
      TelemetryEventCorrelation[] correlatedWith = null)
    {
      session.RequiresArgumentNotNull<TelemetrySession>(nameof (session));
      return session.PostOperationHelper<UserTaskEvent>((Func<UserTaskEvent>) (() => new UserTaskEvent(eventName, result, resultSummary)), correlatedWith);
    }

    public static TelemetryEventCorrelation PostOperation(
      this TelemetrySession session,
      string eventName,
      TelemetryResult result,
      string resultSummary = null,
      TelemetryEventCorrelation[] correlatedWith = null)
    {
      session.RequiresArgumentNotNull<TelemetrySession>(nameof (session));
      return session.PostOperationHelper<OperationEvent>((Func<OperationEvent>) (() => new OperationEvent(eventName, result, resultSummary)), correlatedWith);
    }

    public static TelemetryScope<UserTaskEvent> StartUserTask(
      this TelemetrySession session,
      string eventName)
    {
      return session.StartUserTask(eventName, TelemetrySeverity.High, (IDictionary<string, object>) null, (TelemetryEventCorrelation[]) null);
    }

    public static TelemetryScope<UserTaskEvent> StartUserTask(
      this TelemetrySession session,
      string eventName,
      TelemetrySeverity severity)
    {
      return session.StartUserTask(eventName, severity, (IDictionary<string, object>) null, (TelemetryEventCorrelation[]) null);
    }

    public static TelemetryScope<UserTaskEvent> StartUserTask(
      this TelemetrySession session,
      string eventName,
      TelemetrySeverity severity,
      IDictionary<string, object> startEventProperties)
    {
      return session.StartUserTask(eventName, severity, startEventProperties, (TelemetryEventCorrelation[]) null);
    }

    public static TelemetryScope<UserTaskEvent> StartUserTask(
      this TelemetrySession session,
      string eventName,
      TelemetrySeverity severity,
      IDictionary<string, object> startEventProperties,
      TelemetryEventCorrelation[] correlations)
    {
      TelemetryScopeSettings settings = new TelemetryScopeSettings()
      {
        Severity = severity,
        StartEventProperties = startEventProperties,
        Correlations = correlations
      };
      return session.StartUserTask(eventName, settings);
    }

    public static TelemetryScope<UserTaskEvent> StartUserTask(
      this TelemetrySession session,
      string eventName,
      TelemetryScopeSettings settings)
    {
      session.RequiresArgumentNotNull<TelemetrySession>(nameof (session));
      settings.RequiresArgumentNotNull<TelemetryScopeSettings>(nameof (settings));
      return new TelemetryScope<UserTaskEvent>(session, eventName, (TelemetryScope<UserTaskEvent>.CreateNewEvent) (stageType => new UserTaskEvent(eventName, stageType, TelemetryResult.None)), settings);
    }

    public static TelemetryScope<OperationEvent> StartOperation(
      this TelemetrySession session,
      string eventName)
    {
      return session.StartOperation(eventName, TelemetrySeverity.Normal, (IDictionary<string, object>) null, (TelemetryEventCorrelation[]) null);
    }

    public static TelemetryScope<OperationEvent> StartOperation(
      this TelemetrySession session,
      string eventName,
      TelemetrySeverity severity)
    {
      return session.StartOperation(eventName, severity, (IDictionary<string, object>) null, (TelemetryEventCorrelation[]) null);
    }

    public static TelemetryScope<OperationEvent> StartOperation(
      this TelemetrySession session,
      string eventName,
      TelemetrySeverity severity,
      IDictionary<string, object> startEventProperties)
    {
      return session.StartOperation(eventName, severity, startEventProperties, (TelemetryEventCorrelation[]) null);
    }

    public static TelemetryScope<OperationEvent> StartOperation(
      this TelemetrySession session,
      string eventName,
      TelemetrySeverity severity,
      IDictionary<string, object> startEventProperties,
      TelemetryEventCorrelation[] correlations)
    {
      TelemetryScopeSettings settings = new TelemetryScopeSettings()
      {
        Severity = severity,
        StartEventProperties = startEventProperties,
        Correlations = correlations
      };
      return session.StartOperation(eventName, settings);
    }

    public static TelemetryScope<OperationEvent> StartOperation(
      this TelemetrySession session,
      string eventName,
      TelemetryScopeSettings settings)
    {
      session.RequiresArgumentNotNull<TelemetrySession>(nameof (session));
      settings.RequiresArgumentNotNull<TelemetryScopeSettings>(nameof (settings));
      return new TelemetryScope<OperationEvent>(session, eventName, (TelemetryScope<OperationEvent>.CreateNewEvent) (stageType => new OperationEvent(eventName, stageType, TelemetryResult.None)), settings);
    }

    public static TelemetryEventCorrelation PostFault(
      this TelemetrySession telemetrySession,
      string eventName,
      string description)
    {
      return telemetrySession.PostFault(eventName, description, FaultSeverity.Uncategorized);
    }

    public static TelemetryEventCorrelation PostFault(
      this TelemetrySession telemetrySession,
      string eventName,
      string description,
      FaultSeverity faultSeverity)
    {
      return telemetrySession.PostFault(eventName, description, faultSeverity, (Exception) null, (Func<IFaultUtility, int>) null);
    }

    public static TelemetryEventCorrelation PostFault(
      this TelemetrySession telemetrySession,
      string eventName,
      string description,
      Exception exceptionObject)
    {
      return telemetrySession.PostFault(eventName, description, FaultSeverity.Uncategorized, exceptionObject);
    }

    public static TelemetryEventCorrelation PostFault(
      this TelemetrySession telemetrySession,
      string eventName,
      string description,
      FaultSeverity faultSeverity,
      Exception exceptionObject)
    {
      return telemetrySession.PostFault(eventName, description, faultSeverity, exceptionObject, (Func<IFaultUtility, int>) null);
    }

    public static TelemetryEventCorrelation PostFault(
      this TelemetrySession telemetrySession,
      string eventName,
      string description,
      Exception exceptionObject,
      Func<IFaultUtility, int> gatherEventDetails)
    {
      return telemetrySession.PostFault(eventName, description, FaultSeverity.Uncategorized, exceptionObject, gatherEventDetails);
    }

    public static TelemetryEventCorrelation PostFault(
      this TelemetrySession telemetrySession,
      string eventName,
      string description,
      FaultSeverity faultSeverity,
      Exception exceptionObject,
      Func<IFaultUtility, int> gatherEventDetails)
    {
      return telemetrySession.PostFault(eventName, description, faultSeverity, exceptionObject, gatherEventDetails, (TelemetryEventCorrelation[]) null);
    }

    public static TelemetryEventCorrelation PostFault(
      this TelemetrySession telemetrySession,
      string eventName,
      string description,
      Exception exceptionObject,
      Func<IFaultUtility, int> gatherEventDetails,
      TelemetryEventCorrelation[] correlatedWith)
    {
      return telemetrySession.PostFault(eventName, description, FaultSeverity.Uncategorized, exceptionObject, gatherEventDetails, correlatedWith);
    }

    public static TelemetryEventCorrelation PostFault(
      this TelemetrySession telemetrySession,
      string eventName,
      string description,
      FaultSeverity faultSeverity,
      Exception exceptionObject,
      Func<IFaultUtility, int> gatherEventDetails,
      TelemetryEventCorrelation[] correlatedWith)
    {
      FaultEvent faultEvent = new FaultEvent(eventName, description, faultSeverity, exceptionObject, gatherEventDetails);
      faultEvent.Correlate(correlatedWith);
      telemetrySession.PostEvent((TelemetryEvent) faultEvent);
      return faultEvent.Correlation;
    }

    public static TelemetryEventCorrelation PostAsset(
      this TelemetrySession telemetrySession,
      string eventName,
      string assetId,
      int assetEventVersion,
      IDictionary<string, object> properties,
      TelemetryEventCorrelation[] correlatedWith = null)
    {
      properties.RequiresArgumentNotNull<IDictionary<string, object>>(nameof (properties));
      AssetEvent assetEvent = new AssetEvent(eventName, assetId, assetEventVersion);
      assetEvent.Properties.AddRange<string, object>(properties);
      assetEvent.Correlate(correlatedWith);
      telemetrySession.PostEvent((TelemetryEvent) assetEvent);
      return assetEvent.Correlation;
    }

    private static TelemetryEventCorrelation PostOperationHelper<T>(
      this TelemetrySession session,
      Func<T> createEvent,
      TelemetryEventCorrelation[] correlatedWith)
      where T : OperationEvent
    {
      T obj = createEvent();
      obj.Correlate(correlatedWith);
      session.PostEvent((TelemetryEvent) obj);
      return obj.Correlation;
    }
  }
}
