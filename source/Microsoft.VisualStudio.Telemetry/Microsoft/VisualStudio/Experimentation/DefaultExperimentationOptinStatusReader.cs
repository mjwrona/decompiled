// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Experimentation.DefaultExperimentationOptinStatusReader
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry;
using Microsoft.VisualStudio.Utilities.Internal;
using System;

namespace Microsoft.VisualStudio.Experimentation
{
  internal sealed class DefaultExperimentationOptinStatusReader : IExperimentationOptinStatusReader
  {
    private readonly TelemetrySession telemetrySession;
    private readonly IRegistryTools registryTools;
    private readonly Lazy<bool> isOptedIn;

    public DefaultExperimentationOptinStatusReader(
      TelemetrySession telemetrySession,
      IRegistryTools registryTools)
    {
      telemetrySession.RequiresArgumentNotNull<TelemetrySession>(nameof (telemetrySession));
      registryTools.RequiresArgumentNotNull<IRegistryTools>(nameof (registryTools));
      this.telemetrySession = telemetrySession;
      this.registryTools = registryTools;
      this.isOptedIn = new Lazy<bool>((Func<bool>) (() => this.GetIsOptedIn()));
    }

    public bool IsOptedIn => this.isOptedIn.Value;

    private bool GetIsOptedIn()
    {
      int result;
      TypeTools.TryConvertToInt(this.registryTools.GetRegistryValueFromCurrentUserRoot("Software\\Microsoft\\VisualStudio\\Telemetry", "TurnOffSwitch", (object) 0), out result);
      bool flag = result == 1;
      return this.telemetrySession.IsOptedIn && !flag;
    }
  }
}
