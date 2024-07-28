// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.MacMACInformationProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class MacMACInformationProvider : IMACInformationProvider
  {
    private const string MacInformationProviderVersionKey = "mac.info.provider.version";
    private const string MacInformationProviderVersion = "1";
    private readonly IPersistentPropertyBag persistentStorage;
    private readonly ILegacyApi legacyApi;
    private readonly Lazy<string> persistedMAC;

    public MacMACInformationProvider(IPersistentPropertyBag persistentStorage, ILegacyApi legacyApi)
    {
      persistentStorage.RequiresArgumentNotNull<IPersistentPropertyBag>(nameof (persistentStorage));
      legacyApi.RequiresArgumentNotNull<ILegacyApi>(nameof (legacyApi));
      this.persistentStorage = persistentStorage;
      this.legacyApi = legacyApi;
      this.persistedMAC = new Lazy<string>((Func<string>) (() => this.CalculateMACAddressHash()), LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public event EventHandler<EventArgs> MACAddressHashCalculationCompleted;

    public string GetMACAddressHash() => this.persistedMAC.Value;

    public void RunProcessIfNecessary(Action<string> onComplete)
    {
    }

    private string CalculateMACAddressHash()
    {
      string persistedMacHash = this.GetPersistedMacHash();
      if (persistedMacHash != null)
      {
        this.OnMACAddressHashCalculationCompletedEvent(EventArgs.Empty);
        return persistedMacHash;
      }
      try
      {
        string macAddress;
        if (MacHardwareIdentification.TryGetFirstPrimaryMacAddress(out macAddress))
        {
          string macAddressHash = MACInformationProvider.HashMACAddress(macAddress);
          this.persistentStorage.SetProperty(MACInformationProvider.MacAddressKey, macAddressHash);
          this.persistentStorage.SetProperty("mac.info.provider.version", "1");
          this.OnMACAddressHashCalculationCompletedEvent(EventArgs.Empty);
          return macAddressHash;
        }
      }
      catch (Exception ex)
      {
      }
      this.OnMACAddressHashCalculationCompletedEvent(EventArgs.Empty);
      return MACInformationProvider.ZeroHash;
    }

    private string GetPersistedMacHash()
    {
      object property1 = this.persistentStorage.GetProperty("mac.info.provider.version");
      if (property1 == null || !(property1 is string) || string.IsNullOrEmpty((string) property1))
      {
        this.legacyApi.SetSharedMachineId(new Guid());
        return (string) null;
      }
      object property2 = this.persistentStorage.GetProperty(MACInformationProvider.MacAddressKey);
      if (property2 != null && property2 is string)
      {
        string input = (string) property2;
        if (Regex.IsMatch(input, MACInformationProvider.PersistRegex))
          return input;
      }
      return (string) null;
    }

    private void OnMACAddressHashCalculationCompletedEvent(EventArgs e)
    {
      EventHandler<EventArgs> calculationCompleted = this.MACAddressHashCalculationCompleted;
      if (calculationCompleted == null)
        return;
      calculationCompleted((object) this, e);
    }
  }
}
