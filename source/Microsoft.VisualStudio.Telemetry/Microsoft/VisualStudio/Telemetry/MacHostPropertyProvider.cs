// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.MacHostPropertyProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class MacHostPropertyProvider : BaseHostRealtimePropertyProvider
  {
    private readonly IHostInformationProvider hostInfoProvider;
    private readonly INsBundleInformationProvider nsBundleInformationProvider;
    private readonly Lazy<string> hostExeName;
    private readonly Lazy<Version> hostVersionInfo;

    public MacHostPropertyProvider(
      IHostInformationProvider theHostInfoProvider,
      INsBundleInformationProvider theNsBundleInformationProvider)
      : base(theHostInfoProvider)
    {
      theHostInfoProvider.RequiresArgumentNotNull<IHostInformationProvider>(nameof (theHostInfoProvider));
      this.hostInfoProvider = theHostInfoProvider;
      theNsBundleInformationProvider.RequiresArgumentNotNull<INsBundleInformationProvider>(nameof (theNsBundleInformationProvider));
      this.nsBundleInformationProvider = theNsBundleInformationProvider;
      this.hostExeName = new Lazy<string>((Func<string>) (() => this.InitializeHostExeName()), false);
      this.hostVersionInfo = new Lazy<Version>((Func<Version>) (() => this.InitializeHostVersionInfo()), false);
    }

    public override void AddSharedProperties(
      List<KeyValuePair<string, object>> sharedProperties,
      TelemetryContext telemetryContext)
    {
      if (this.hostExeName.Value != null)
        sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.ExeName", (object) this.hostExeName.Value.ToLowerInvariant()));
      else
        sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.ExeName", (object) this.hostInfoProvider.ProcessName));
      if (this.hostVersionInfo.Value != (Version) null)
      {
        Version version = this.hostVersionInfo.Value;
        sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.ExeVersion", (object) version.ToString()));
        sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.BuildNumber", (object) version.Build));
      }
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.ProcessId", (object) this.hostInfoProvider.ProcessId));
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.Is64BitProcess", (object) this.hostInfoProvider.Is64BitProcess));
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.OSBitness", (object) this.hostInfoProvider.OSBitness));
    }

    private string InitializeHostExeName() => this.nsBundleInformationProvider.GetName();

    private Version InitializeHostVersionInfo()
    {
      string version = this.nsBundleInformationProvider.GetVersion();
      return version != null ? new Version(version) : (Version) null;
    }
  }
}
