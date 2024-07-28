// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.MACInformationProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry
{
  internal abstract class MACInformationProvider : IMACInformationProvider
  {
    internal static string ZeroHash = "0000000000000000000000000000000000000000000000000000000000000000";
    private readonly IProcessTools processTools;
    private readonly IPersistentPropertyBag persistentStorage;
    private readonly Lazy<string> persistedMAC;
    internal static string MacAddressKey = "mac.address";
    private const string MacRegex = "(?:[a-z0-9]{2}[:\\-]){5}[a-z0-9]{2}";
    private const string ZeroRegex = "(?:00[:\\-]){5}00";
    internal static string PersistRegex = "[a-f0-9]{64}";
    private readonly string command;
    private readonly string commandArgs;
    private bool needToRunProcess;
    private object needToRunProcessLock = new object();

    protected MACInformationProvider(
      IProcessTools processTools,
      IPersistentPropertyBag persistentStorage,
      string command,
      string commandArgs)
    {
      processTools.RequiresArgumentNotNull<IProcessTools>(nameof (processTools));
      persistentStorage.RequiresArgumentNotNull<IPersistentPropertyBag>(nameof (persistentStorage));
      this.processTools = processTools;
      this.persistentStorage = persistentStorage;
      this.command = command;
      this.commandArgs = commandArgs;
      this.persistedMAC = new Lazy<string>((Func<string>) (() => this.CalculateMACAddressHash()), LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public void RunProcessIfNecessary(Action<string> onComplete)
    {
      if (!this.needToRunProcess)
        return;
      lock (this.needToRunProcessLock)
      {
        if (!this.needToRunProcess)
          return;
        this.processTools.RunCommand(this.command, (Action<string>) (data =>
        {
          if (data != null)
          {
            string macAddress = this.ParseMACAddress(data);
            if (macAddress != null)
            {
              this.persistentStorage.SetProperty(MACInformationProvider.MacAddressKey, macAddress);
              onComplete(macAddress);
            }
          }
          this.OnMACAddressHashCalculationCompletedEvent(EventArgs.Empty);
        }), this.commandArgs);
        this.needToRunProcess = false;
      }
    }

    public string GetMACAddressHash() => this.persistedMAC.Value;

    public event EventHandler<EventArgs> MACAddressHashCalculationCompleted;

    private string CalculateMACAddressHash()
    {
      object property = this.persistentStorage.GetProperty(MACInformationProvider.MacAddressKey);
      if (property != null && property is string)
      {
        string input = (string) property;
        if (Regex.IsMatch(input, MACInformationProvider.PersistRegex))
        {
          this.OnMACAddressHashCalculationCompletedEvent(EventArgs.Empty);
          return input;
        }
      }
      this.needToRunProcess = true;
      return MACInformationProvider.ZeroHash;
    }

    private string ParseMACAddress(string data)
    {
      string macAddress = (string) null;
      foreach (Match match in Regex.Matches(data, "(?:[a-z0-9]{2}[:\\-]){5}[a-z0-9]{2}", RegexOptions.IgnoreCase))
      {
        if (!Regex.IsMatch(match.Value, "(?:00[:\\-]){5}00"))
        {
          macAddress = match.Value;
          break;
        }
      }
      if (macAddress != null)
        macAddress = MACInformationProvider.HashMACAddress(macAddress);
      return macAddress;
    }

    internal static string HashMACAddress(string macAddress)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(macAddress);
      return BitConverter.ToString(FipsCompliantSha.Sha256.Value.ComputeHash(bytes)).Replace("-", string.Empty).ToLowerInvariant();
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
