// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.WindowsMachinePropertyProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.Win32;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class WindowsMachinePropertyProvider : IPropertyProvider
  {
    internal const string ProcessArchPropertyName = "VS.Core.Process.Architecture";
    private static readonly ulong MbInBytes = 1048576;
    private const string AzureVMImageNameKey = "AzureVMImageName";
    private const string HardwareDescriptionRegistryPath = "HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0";
    private const string HardwareProcessNameRegistryKey = "ProcessorNameString";
    private const string HardwareCPUSpeedRegistryKey = "~MHz";
    private const string NoneValue = "None";
    private const string TelemetryLocalMachineRegistryPath = "SOFTWARE\\Microsoft\\VisualStudio\\Telemetry";
    private const string UnknownValue = "Unknown";
    private const string MacAddressPropertyName = "VS.Core.MacAddressHash";
    private const string MachineIdPropertyName = "VS.Core.Machine.Id";
    private const string MachineArchPropertyName = "VS.Core.Machine.Architecture";
    private const string IsVirtualMachinePropertyName = "VS.Core.Machine.IsVM";
    private const string VirtualMachineTypePropertyName = "VS.Core.Machine.VirtualMachineType";
    private const uint VMType_Unknown = 9999;
    private const string SystemInformationRegistryPath = "SYSTEM\\ControlSet001\\Control\\SystemInformation";
    private const string SkuNameRegistryKey = "SystemProductName";
    private const string Win365RegistryPath = "SOFTWARE\\Microsoft\\Windows365";
    private const string Win365PartnerIdRegistryKey = "PartnerId";
    private const string DevBoxPartnerId = "e3171dd9-9a5f-e5be-b36c-cc7c4f3f3bcf";
    private const string DevBoxPropertyName = "IsDevBox";
    private const string Win365PartnerIdPropertyName = "VS.Core.Win365.PartnerId";
    private const string Win365SkuNamePropertyName = "VS.Core.Win365.SkuName";
    private readonly Lazy<NativeMethods.MemoryStatus> memoryInformation;
    private readonly Lazy<NativeMethods.SystemInfo> systemInformation;
    private readonly Lazy<string> processorDescription;
    private readonly Lazy<int?> processorFrequency;
    private readonly Lazy<string> azureVMImageName;
    private readonly Lazy<bool> isVirtualMachine;
    private readonly Lazy<string> virtualMachineType;
    private readonly Lazy<uint> virtualMachineTypeValue;
    private readonly Lazy<string> win365PartnerId;
    private readonly Lazy<string> win365SkuName;
    private readonly Lazy<bool> isDevBox;
    private readonly IMachineInformationProvider machineInformationProvider;
    private readonly IRegistryTools registryTools;
    private readonly IMACInformationProvider macInformationProvider;
    private readonly IPersistentPropertyBag persistentPropertyBag;

    private string ProcessArchitecture { get; }

    private string MachineArchitecture { get; }

    public WindowsMachinePropertyProvider(
      IMachineInformationProvider machineInformationProvider,
      IRegistryTools regTools,
      IMACInformationProvider macInformationProvider,
      IPersistentPropertyBag persistentPropertyBag)
    {
      WindowsMachinePropertyProvider propertyProvider = this;
      machineInformationProvider.RequiresArgumentNotNull<IMachineInformationProvider>(nameof (machineInformationProvider));
      regTools.RequiresArgumentNotNull<IRegistryTools>(nameof (regTools));
      macInformationProvider.RequiresArgumentNotNull<IMACInformationProvider>(nameof (macInformationProvider));
      this.machineInformationProvider = machineInformationProvider;
      this.registryTools = regTools;
      this.macInformationProvider = macInformationProvider;
      this.persistentPropertyBag = persistentPropertyBag;
      this.memoryInformation = new Lazy<NativeMethods.MemoryStatus>((Func<NativeMethods.MemoryStatus>) (() => propertyProvider.InitializeOSMemoryInformation()), false);
      this.systemInformation = new Lazy<NativeMethods.SystemInfo>((Func<NativeMethods.SystemInfo>) (() => propertyProvider.InitializeSystemInformation()), false);
      this.processorDescription = new Lazy<string>((Func<string>) (() => propertyProvider.InitializeProcessorDescription()), false);
      this.processorFrequency = new Lazy<int?>((Func<int?>) (() => propertyProvider.registryTools.GetRegistryIntValueFromLocalMachineRoot("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0", "~MHz")), false);
      this.azureVMImageName = new Lazy<string>((Func<string>) (() => propertyProvider.InitializeAzureVMImageName()), false);
      this.isVirtualMachine = new Lazy<bool>((Func<bool>) (() => propertyProvider.IsVirtualMachine()), false);
      this.virtualMachineType = new Lazy<string>((Func<string>) (() => propertyProvider.InitializeVirtualMachineType()), false);
      this.virtualMachineTypeValue = new Lazy<uint>((Func<uint>) (() => propertyProvider.InitializeVirtualMachineTypeValue()), false);
      this.win365PartnerId = new Lazy<string>((Func<string>) (() => WindowsMachinePropertyProvider.GetWin365PartnerId(regTools)), false);
      this.win365SkuName = new Lazy<string>((Func<string>) (() => propertyProvider.InitializeWin365SkuName()), false);
      this.isDevBox = new Lazy<bool>((Func<bool>) (() => propertyProvider.InitializeIsDevBox()), false);
      if (!Platform.IsWindows)
        return;
      string processArch;
      string nativeMachineArch;
      ArchitectureTools.GetImageFileMachineArchitectures(out processArch, out nativeMachineArch);
      this.ProcessArchitecture = processArch;
      this.MachineArchitecture = nativeMachineArch;
    }

    public void AddSharedProperties(
      List<KeyValuePair<string, object>> sharedProperties,
      TelemetryContext telemetryContext)
    {
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.Machine.Id", (object) this.machineInformationProvider.MachineId));
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.MacAddressHash", (object) this.macInformationProvider.GetMACAddressHash()));
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.Machine.Architecture", (object) this.MachineArchitecture));
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.Process.Architecture", (object) this.ProcessArchitecture));
      if (this.isDevBox.Value)
        sharedProperties.Add(new KeyValuePair<string, object>("IsDevBox", (object) this.isDevBox.Value));
      this.macInformationProvider.RunProcessIfNecessary((Action<string>) (macAddress => telemetryContext.SharedProperties["VS.Core.MacAddressHash"] = (object) macAddress));
    }

    public void PostProperties(TelemetryContext telemetryContext, CancellationToken token)
    {
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.Machine.TotalRAM", (object) (this.memoryInformation.Value.TotalPhys / WindowsMachinePropertyProvider.MbInBytes));
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.Machine.Processor.Architecture", (object) this.MachineArchitecture);
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.Machine.Processor.Count", (object) this.systemInformation.Value.NumberOfProcessors);
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.Machine.Processor.Description", (object) this.processorDescription.Value);
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.Machine.Processor.Family", (object) this.systemInformation.Value.ProcessorLevel);
      if (token.IsCancellationRequested)
        return;
      if (this.processorFrequency.Value.HasValue)
      {
        telemetryContext.PostProperty("VS.Core.Machine.Processor.Frequency", (object) this.processorFrequency.Value);
        if (token.IsCancellationRequested)
          return;
      }
      telemetryContext.PostProperty("VS.Core.Machine.Processor.Model", (object) ((int) this.systemInformation.Value.ProcessorRevision >> 8));
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.Machine.Processor.Stepping", (object) ((int) this.systemInformation.Value.ProcessorRevision & (int) byte.MaxValue));
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.Machine.VM.AzureImage", (object) this.azureVMImageName.Value);
      if (token.IsCancellationRequested)
        return;
      if (!string.IsNullOrWhiteSpace(this.win365PartnerId.Value))
        telemetryContext.PostProperty("VS.Core.Win365.PartnerId", (object) this.win365PartnerId.Value);
      if (token.IsCancellationRequested)
        return;
      if (!string.IsNullOrWhiteSpace(this.win365SkuName.Value))
        telemetryContext.PostProperty("VS.Core.Win365.SkuName", (object) this.win365SkuName.Value);
      if (token.IsCancellationRequested)
        return;
      this.PostVirtualMachineTypeTelemetry(telemetryContext);
    }

    private void PostVirtualMachineTypeTelemetry(TelemetryContext telemetryContext)
    {
      telemetryContext.PostProperty("VS.Core.Machine.IsVM", (object) this.isVirtualMachine.Value);
      if (!this.isVirtualMachine.Value)
        return;
      telemetryContext.PostProperty("VS.Core.Machine.VirtualMachineType", (object) this.virtualMachineType.Value);
    }

    private NativeMethods.MemoryStatus InitializeOSMemoryInformation()
    {
      NativeMethods.MemoryStatus bufferPointer = new NativeMethods.MemoryStatus()
      {
        Length = (uint) Marshal.SizeOf(typeof (NativeMethods.MemoryStatus))
      };
      NativeMethods.GlobalMemoryStatusEx(ref bufferPointer);
      return bufferPointer;
    }

    private NativeMethods.SystemInfo InitializeSystemInformation()
    {
      NativeMethods.SystemInfo systemInfo = new NativeMethods.SystemInfo();
      NativeMethods.GetNativeSystemInfo(ref systemInfo);
      return systemInfo;
    }

    private string InitializeProcessorDescription()
    {
      object localMachineRoot = this.registryTools.GetRegistryValueFromLocalMachineRoot("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0", "ProcessorNameString");
      return localMachineRoot != null && localMachineRoot is string ? (string) localMachineRoot : "Unknown";
    }

    private string InitializeAzureVMImageName()
    {
      object localMachineRoot = this.registryTools.GetRegistryValueFromLocalMachineRoot("SOFTWARE\\Microsoft\\VisualStudio\\Telemetry", "AzureVMImageName");
      return localMachineRoot != null && localMachineRoot is string str ? str : "None";
    }

    internal bool IsVirtualMachine() => this.virtualMachineTypeValue.Value != 0U && this.virtualMachineTypeValue.Value != 9999U;

    private string InitializeVirtualMachineType()
    {
      switch (this.virtualMachineTypeValue.Value)
      {
        case 0:
          return "NotVirtual";
        case 1:
          return "Hyper-V";
        case 2:
          return "Virtual PC";
        case 3:
          return "VMware";
        case 4:
          return "Xen";
        case 5:
          return "Parallels";
        case 6:
          return "VirtualBox";
        default:
          return "Unknown";
      }
    }

    internal static string GetWin365PartnerId(IRegistryTools registryTools) => registryTools.GetRegistryValueFromLocalMachineRoot("SOFTWARE\\Microsoft\\Windows365", "PartnerId", true, (object) string.Empty) as string;

    private string InitializeWin365SkuName() => !string.IsNullOrWhiteSpace(this.win365PartnerId.Value) ? this.registryTools.GetRegistryValueFromLocalMachineRoot("SYSTEM\\ControlSet001\\Control\\SystemInformation", "SystemProductName", true, (object) string.Empty) as string : string.Empty;

    private bool InitializeIsDevBox()
    {
      Guid result;
      if (!string.IsNullOrWhiteSpace(this.win365PartnerId.Value) && Guid.TryParse(this.win365PartnerId.Value, out result))
      {
        Guid g = new Guid("e3171dd9-9a5f-e5be-b36c-cc7c4f3f3bcf");
        if (result.Equals(g))
        {
          this.persistentPropertyBag.SetProperty("IsDevBox", "true");
          return true;
        }
      }
      return false;
    }

    private uint InitializeVirtualMachineTypeValue()
    {
      int result;
      if (int.TryParse(this.registryTools.GetRegistryValueFromCurrentUserRoot("Software\\Microsoft\\VisualStudio\\Telemetry", "VS.Core.Machine.VirtualMachineType", (object) -1) as string, out result) && result != 9999)
        return (uint) result;
      uint machineTypeValue = this.GetVirtualMachineTypeValue();
      this.registryTools.SetRegistryFromCurrentUserRoot("Software\\Microsoft\\VisualStudio\\Telemetry", "VS.Core.Machine.VirtualMachineType", (object) machineTypeValue);
      return machineTypeValue;
    }

    private uint GetVirtualMachineTypeValue()
    {
      uint machineTypeValue = 9999;
      if (Environment.HasShutdownStarted)
        return 9999;
      try
      {
        string str = PInvoke.GetCommandLine().ToString();
        if (!string.IsNullOrEmpty(str) && str.IndexOf("updateconfiguration", StringComparison.OrdinalIgnoreCase) != -1)
          return 9999;
        foreach (ManagementBaseObject collection in new ManagementObjectSearcher("SELECT Version,SerialNumber from Win32_BIOS").Get())
        {
          if (collection != null)
          {
            string lowerInvariant1 = WindowsMachinePropertyProvider.GetValue(collection, "Version").ToLowerInvariant();
            string lowerInvariant2 = WindowsMachinePropertyProvider.GetValue(collection, "SerialNumber").ToLowerInvariant();
            if (lowerInvariant1.Trim().StartsWith("vrtual", StringComparison.OrdinalIgnoreCase))
              machineTypeValue = 1U;
            else if (lowerInvariant2.StartsWith("vmware-", StringComparison.OrdinalIgnoreCase) || lowerInvariant2.StartsWith("vmw", StringComparison.OrdinalIgnoreCase))
              machineTypeValue = 3U;
            else if (lowerInvariant1.Equals("xen", StringComparison.Ordinal))
              machineTypeValue = 4U;
            else if (lowerInvariant2.Trim().StartsWith("parallels-", StringComparison.OrdinalIgnoreCase))
              machineTypeValue = 5U;
            else if (lowerInvariant1.Equals("version", StringComparison.Ordinal))
              machineTypeValue = 6U;
          }
        }
        if (machineTypeValue == 9999U)
        {
          foreach (ManagementBaseObject collection in new ManagementObjectSearcher("SELECT Product from Win32_BaseBoard").Get())
          {
            if (collection != null && WindowsMachinePropertyProvider.GetValue(collection, "Product").ToLowerInvariant().Equals("virtual machine", StringComparison.Ordinal))
              machineTypeValue = 2U;
          }
        }
        if (machineTypeValue == 9999U)
          machineTypeValue = 0U;
      }
      catch (Exception ex)
      {
      }
      return machineTypeValue;
    }

    private static string GetValue(ManagementBaseObject collection, string value)
    {
      string empty = string.Empty;
      try
      {
        if (collection != null)
        {
          if (collection[value] != null)
            empty = collection[value].ToString();
        }
      }
      catch (Exception ex)
      {
      }
      return empty;
    }
  }
}
