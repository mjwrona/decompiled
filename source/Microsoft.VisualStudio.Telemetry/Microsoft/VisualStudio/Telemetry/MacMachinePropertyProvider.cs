// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.MacMachinePropertyProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class MacMachinePropertyProvider : IPropertyProvider
  {
    private static readonly long MbInBytes = 1048576;
    private const string NoneValue = "None";
    private const string UnknownValue = "Unknown";
    private const string MacAddressPropertyName = "VS.Core.MacAddressHash";
    private const string MachineIdPropertyName = "VS.Core.Machine.Id";
    private readonly Lazy<MacNativeMethods.SystemInfo> systemInformation;
    private readonly IMachineInformationProvider machineInformationProvider;
    private readonly IMACInformationProvider macInformationProvider;

    public MacMachinePropertyProvider(
      IMachineInformationProvider machineInformationProvider,
      IRegistryTools regTools,
      IMACInformationProvider macInformationProvider)
    {
      machineInformationProvider.RequiresArgumentNotNull<IMachineInformationProvider>(nameof (machineInformationProvider));
      regTools.RequiresArgumentNotNull<IRegistryTools>(nameof (regTools));
      macInformationProvider.RequiresArgumentNotNull<IMACInformationProvider>(nameof (macInformationProvider));
      this.machineInformationProvider = machineInformationProvider;
      this.macInformationProvider = macInformationProvider;
      this.systemInformation = new Lazy<MacNativeMethods.SystemInfo>((Func<MacNativeMethods.SystemInfo>) (() => this.InitializeSystemInformation()), false);
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
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.Machine.TotalRAM", (object) (this.systemInformation.Value.HardwareMemorySize / MacMachinePropertyProvider.MbInBytes));
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.Machine.Processor.Architecture", (object) this.systemInformation.Value.HardwareMachine);
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.Machine.Processor.Count", (object) this.systemInformation.Value.HardwarePhysicalCpuSize);
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.Machine.Processor.Description", (object) this.systemInformation.Value.MachineCpuBrandString);
      int num = token.IsCancellationRequested ? 1 : 0;
    }

    private MacNativeMethods.SystemInfo InitializeSystemInformation()
    {
      MacNativeMethods.SystemInfo info = new MacNativeMethods.SystemInfo();
      MacNativeMethods.GetSystemInfo(ref info);
      return info;
    }
  }
}
