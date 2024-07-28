// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.WindowsHostPropertyProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class WindowsHostPropertyProvider : BaseHostRealtimePropertyProvider
  {
    private readonly IHostInformationProvider hostInfoProvider;
    private readonly IRegistryTools registryTools;
    private readonly Lazy<bool> isRDPSession;
    private const string TerminalServerRegistryPath = "SYSTEM\\\\CurrentControlSet\\\\Control\\\\Terminal Server\\\\";
    private const string GlassSessionIdRegistryKey = "GlassSessionId";

    public WindowsHostPropertyProvider(
      IHostInformationProvider theHostInfoProvider,
      IRegistryTools regTools)
      : base(theHostInfoProvider)
    {
      theHostInfoProvider.RequiresArgumentNotNull<IHostInformationProvider>(nameof (theHostInfoProvider));
      regTools.RequiresArgumentNotNull<IRegistryTools>(nameof (regTools));
      this.hostInfoProvider = theHostInfoProvider;
      this.registryTools = regTools;
      this.isRDPSession = new Lazy<bool>((Func<bool>) (() => this.InitializeIsRDPSession()), false);
    }

    public override void AddSharedProperties(
      List<KeyValuePair<string, object>> sharedProperties,
      TelemetryContext telemetryContext)
    {
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.ExeName", (object) this.hostInfoProvider.ProcessName));
      if (this.hostInfoProvider.ProcessExeVersion != null)
        sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.ExeVersion", (object) this.hostInfoProvider.ProcessExeVersion));
      if (this.hostInfoProvider.ProcessBuildNumber.HasValue)
        sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.BuildNumber", (object) this.hostInfoProvider.ProcessBuildNumber.Value));
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.ProcessId", (object) this.hostInfoProvider.ProcessId));
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.Is64BitProcess", (object) this.hostInfoProvider.Is64BitProcess));
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.OSBitness", (object) this.hostInfoProvider.OSBitness));
    }

    public override void PostProperties(TelemetryContext telemetryContext, CancellationToken token)
    {
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.IsRDPSession", (object) this.isRDPSession.Value);
      if (token.IsCancellationRequested)
        return;
      base.PostProperties(telemetryContext, token);
    }

    private bool InitializeIsRDPSession()
    {
      if (NativeMethods.GetSystemMetrics(4096) != 0)
        return true;
      object localMachineRoot = this.registryTools.GetRegistryValueFromLocalMachineRoot("SYSTEM\\\\CurrentControlSet\\\\Control\\\\Terminal Server\\\\", "GlassSessionId", true, (object) null);
      if (localMachineRoot != null)
      {
        uint uint32 = Convert.ToUInt32(localMachineRoot);
        uint pSessionId;
        if (NativeMethods.ProcessIdToSessionId(NativeMethods.GetCurrentProcessId(), out pSessionId))
          return (int) pSessionId != (int) uint32;
      }
      return false;
    }
  }
}
