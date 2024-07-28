// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryVsOptinStatusReader
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class TelemetryVsOptinStatusReader : ITelemetryOptinStatusReader
  {
    private const string GlobalPolicyOptedInRegistryPath = "Software\\Policies\\Microsoft\\VisualStudio\\SQM";
    private const string LocalOptedInRegistryPath = "Software\\Microsoft\\VSCommon\\{0}\\SQM";
    private const string LocalOptedInRootRegistryPath = "Software\\Microsoft\\VSCommon";
    private const string OptedInRegistryKeyName = "OptIn";
    private const int UserIsOptedInValue = 1;
    private readonly IRegistryTools2 registryTools;

    public TelemetryVsOptinStatusReader(IRegistryTools2 registryTools)
    {
      registryTools.RequiresArgumentNotNull<IRegistryTools2>(nameof (registryTools));
      this.registryTools = registryTools;
    }

    public bool ReadIsOptedInStatus(string productVersion)
    {
      productVersion.RequiresArgumentNotNullAndNotEmpty(nameof (productVersion));
      bool flag = false;
      bool optedIn;
      if (this.TryGlobalPolicyOptedInStatus(out optedIn))
      {
        flag = optedIn;
      }
      else
      {
        int? localMachineRoot = this.registryTools.GetRegistryIntValueFromLocalMachineRoot(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Software\\Microsoft\\VSCommon\\{0}\\SQM", new object[1]
        {
          (object) productVersion
        }), "OptIn");
        if (localMachineRoot.HasValue)
          flag = localMachineRoot.Value == 1;
      }
      return flag;
    }

    public bool ReadIsOptedInStatus(TelemetrySession session)
    {
      session.RequiresArgumentNotNull<TelemetrySession>(nameof (session));
      TelemetryVsOptinStatusReader.OptinStatus optinStatus = TelemetryVsOptinStatusReader.OptinStatus.Undefined;
      bool optedIn;
      bool flag1;
      if (this.TryGlobalPolicyOptedInStatus(out optedIn))
      {
        flag1 = optedIn;
        optinStatus = TelemetryVsOptinStatusReader.OptinStatus.ReadFromGlobalPolicy;
      }
      else
      {
        bool flag2 = false;
        bool flag3 = false;
        string[] localMachineRoot1 = this.registryTools.GetRegistrySubKeyNamesFromLocalMachineRoot("Software\\Microsoft\\VSCommon");
        if (localMachineRoot1 != null && localMachineRoot1.Length != 0)
        {
          foreach (string key in localMachineRoot1)
          {
            if (this.KeyMatchesSqmFormat(key))
            {
              int? localMachineRoot2 = this.registryTools.GetRegistryIntValueFromLocalMachineRoot(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Software\\Microsoft\\VSCommon\\{0}\\SQM", new object[1]
              {
                (object) key
              }), "OptIn");
              if (localMachineRoot2.HasValue)
              {
                if (localMachineRoot2.Value == 1)
                  flag2 = true;
                else
                  flag3 = true;
              }
            }
          }
        }
        flag1 = flag2 && !flag3;
        if (flag2 && !flag3)
          optinStatus = TelemetryVsOptinStatusReader.OptinStatus.OptedInForAll;
        else if (flag2 & flag3)
          optinStatus = TelemetryVsOptinStatusReader.OptinStatus.OptedInForSomeOptedOutForSome;
        else if (!flag2 & flag3)
          optinStatus = TelemetryVsOptinStatusReader.OptinStatus.OptedOutForAll;
      }
      session.PostProperty("vs.core.usevsisoptedinstatus", (object) optinStatus.ToString());
      return flag1;
    }

    private bool KeyMatchesSqmFormat(string key) => Version.TryParse(key, out Version _);

    private bool TryGlobalPolicyOptedInStatus(out bool optedIn)
    {
      optedIn = false;
      int? localMachineRoot = this.registryTools.GetRegistryIntValueFromLocalMachineRoot("Software\\Policies\\Microsoft\\VisualStudio\\SQM", "OptIn");
      if (!localMachineRoot.HasValue)
        return false;
      optedIn = localMachineRoot.Value == 1;
      return true;
    }

    private enum OptinStatus
    {
      ReadFromGlobalPolicy,
      OptedInForAll,
      OptedOutForAll,
      OptedInForSomeOptedOutForSome,
      Undefined,
    }
  }
}
