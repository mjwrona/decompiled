// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.IIdentityInformationProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry
{
  internal interface IIdentityInformationProvider
  {
    bool AnyValueChanged { get; }

    bool PersistedIdWasInvalidated { get; }

    string PersistedIdInvalidationReason { get; }

    string PersistedSelectedMACAddress { get; }

    string MachineName { get; }

    string DNSDomain { get; }

    string BiosSerialNumber { get; }

    Guid BiosUUID { get; }

    string HardwareId { get; }

    DateTime? HardwareIdDate { get; }

    BiosFirmwareTableParserError BiosInformationError { get; }

    string SelectedMACAddress { get; }

    List<NetworkInterfaceCardInformation> PrioritizedNetworkInterfaces { get; }

    event EventHandler<EventArgs> HardwareIdCalculationCompleted;

    IEnumerable<KeyValuePair<string, object>> CollectIdentifiers(
      List<Exception> collectionExceptions);

    void SchedulePostPersistedSharedPropertyAndSendAnyFaults(
      TelemetrySession telemetrySession,
      ITelemetryScheduler scheduler);

    void GetHardwareIdWithCalculationCompletedEvent(Action<string> callback);

    TelemetryManifestMachineIdentityConfig MachineIdentityConfig { get; }

    void Initialize(
      TelemetryContext telemetryContext,
      ITelemetryScheduler contextScheduler,
      TelemetryManifestMachineIdentityConfig machineIdentityConfig);
  }
}
