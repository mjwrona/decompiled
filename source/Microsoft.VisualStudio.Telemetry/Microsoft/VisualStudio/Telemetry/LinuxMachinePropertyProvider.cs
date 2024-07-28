// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.LinuxMachinePropertyProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class LinuxMachinePropertyProvider : IPropertyProvider
  {
    private readonly IMachineInformationProvider machineInformationProvider;
    private readonly IMACInformationProvider macInformationProvider;
    private const string MacAddressPropertyName = "VS.Core.MacAddressHash";
    private const string MachineIdPropertyName = "VS.Core.Machine.Id";

    public LinuxMachinePropertyProvider(
      IMachineInformationProvider machineInformationProvider,
      IRegistryTools regTools,
      IMACInformationProvider macInformationProvider)
    {
      machineInformationProvider.RequiresArgumentNotNull<IMachineInformationProvider>(nameof (machineInformationProvider));
      regTools.RequiresArgumentNotNull<IRegistryTools>(nameof (regTools));
      macInformationProvider.RequiresArgumentNotNull<IMACInformationProvider>(nameof (macInformationProvider));
      this.machineInformationProvider = machineInformationProvider;
      this.macInformationProvider = macInformationProvider;
    }

    public void AddSharedProperties(
      List<KeyValuePair<string, object>> sharedProperties,
      TelemetryContext telemetryContext)
    {
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.MacAddressHash", (object) this.macInformationProvider.GetMACAddressHash()));
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.Machine.Id", (object) this.machineInformationProvider.MachineId));
      this.macInformationProvider.RunProcessIfNecessary((Action<string>) (macAddress => telemetryContext.SharedProperties["VS.Core.MacAddressHash"] = (object) macAddress));
    }

    public void PostProperties(TelemetryContext telemetryContext, CancellationToken token)
    {
    }
  }
}
