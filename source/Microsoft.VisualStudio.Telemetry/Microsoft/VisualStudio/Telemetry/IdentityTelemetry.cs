// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.IdentityTelemetry
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class IdentityTelemetry : IIdentityTelemetry
  {
    public const string EvaluationValuesEventName = "VS/TelemetryApi/Identity/EvaluationValues";
    public static readonly string EvaluationValuesEventFaultName = "VS/TelemetryApi/Identity/EvaluationValues/Fault";
    public static readonly string ChangeDetectedPropertyName = "ChangeDetected";
    internal const string propertyPrefix = "VS.TelemetryApi.Identity.";
    private List<KeyValuePair<string, Exception>> exceptions = new List<KeyValuePair<string, Exception>>();

    private bool SendIdentityValuesEvent => this.IdentityInformationProvider.MachineIdentityConfig.SendValuesEvent;

    public IIdentityInformationProvider IdentityInformationProvider { get; }

    private ITelemetryScheduler Scheduler { get; }

    private CancellationToken CancellationToken { get; }

    public IdentityTelemetry(
      IIdentityInformationProvider identityInformationProvider,
      ITelemetryScheduler scheduler)
    {
      identityInformationProvider.RequiresArgumentNotNull<IIdentityInformationProvider>(nameof (identityInformationProvider));
      this.Scheduler = scheduler ?? (ITelemetryScheduler) new TelemetryScheduler();
      this.IdentityInformationProvider = identityInformationProvider;
    }

    public void PostIdentityTelemetryWhenSessionInitialized(TelemetrySession telemetrySession)
    {
      telemetrySession.RequiresArgumentNotNull<TelemetrySession>(nameof (telemetrySession));
      if (telemetrySession.IsSessionCloned)
        return;
      this.Scheduler.Schedule((Action) (() => this.CollectAndSendIdentityEvaluationValuesEvent(telemetrySession, telemetrySession.CancellationToken)), new CancellationToken?(telemetrySession.CancellationToken));
    }

    private IEnumerable<KeyValuePair<string, object>> GetIdentityProperties(
      CancellationToken cancellationToken)
    {
      List<Exception> collectionExceptions = new List<Exception>();
      foreach (KeyValuePair<string, object> collectIdentifier in this.IdentityInformationProvider.CollectIdentifiers(collectionExceptions))
      {
        cancellationToken.ThrowIfCancellationRequested();
        yield return new KeyValuePair<string, object>("VS.TelemetryApi.Identity." + collectIdentifier.Key, collectIdentifier.Value);
      }
      foreach (Exception exception in collectionExceptions)
        this.exceptions.Add(new KeyValuePair<string, Exception>("CollectIdentifiersException", exception));
      if (this.IdentityInformationProvider.AnyValueChanged)
        yield return new KeyValuePair<string, object>("VS.TelemetryApi.Identity.ChangeDetected", (object) 1);
      foreach (NetworkInterfaceCardInformation networkInterface in this.IdentityInformationProvider.PrioritizedNetworkInterfaces)
        yield return new KeyValuePair<string, object>(string.Format("{0}{1}.{2}", (object) "VS.TelemetryApi.Identity.", (object) "PrioritizedNetworkInterfaces", (object) networkInterface.SelectionRank), (object) networkInterface.Serialize());
      if (this.IdentityInformationProvider.BiosInformationError != BiosFirmwareTableParserError.Success)
        yield return new KeyValuePair<string, object>("VS.TelemetryApi.Identity.BiosInformationError", (object) this.IdentityInformationProvider.BiosInformationError);
      if (this.IdentityInformationProvider.PersistedIdWasInvalidated)
        yield return new KeyValuePair<string, object>("VS.TelemetryApi.Identity.PersistedIdWasInvalidated", (object) this.IdentityInformationProvider.PersistedIdInvalidationReason);
    }

    private void CollectAndSendIdentityEvaluationValuesEvent(
      TelemetrySession telemetrySession,
      CancellationToken cancellationToken)
    {
      try
      {
        KeyValuePair<string, object>[] array = this.GetIdentityProperties(cancellationToken).ToArray<KeyValuePair<string, object>>();
        if (!this.SendIdentityValuesEvent)
          return;
        TelemetryEvent telemetryEvent = new TelemetryEvent("VS/TelemetryApi/Identity/EvaluationValues");
        foreach (KeyValuePair<string, object> keyValuePair in array)
          telemetryEvent.Properties.Add(keyValuePair.Key, keyValuePair.Value);
        cancellationToken.ThrowIfCancellationRequested();
        telemetrySession.PostEvent(telemetryEvent);
      }
      catch (OperationCanceledException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        telemetrySession.PostFault(IdentityTelemetry.EvaluationValuesEventFaultName, "SendIdentityEvaluationValuesEvent", ex);
      }
      foreach (KeyValuePair<string, Exception> exception in this.exceptions)
        telemetrySession.PostFault(IdentityTelemetry.EvaluationValuesEventFaultName, exception.Key, exception.Value);
    }
  }
}
