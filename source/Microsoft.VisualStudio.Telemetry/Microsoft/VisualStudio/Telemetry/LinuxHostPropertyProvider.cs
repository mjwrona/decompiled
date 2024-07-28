// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.LinuxHostPropertyProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class LinuxHostPropertyProvider : IPropertyProvider
  {
    private readonly IHostInformationProvider hostInfoProvider;
    private readonly Version hostVersionInfo;

    public LinuxHostPropertyProvider(IHostInformationProvider theHostInfoProvider)
    {
      theHostInfoProvider.RequiresArgumentNotNull<IHostInformationProvider>(nameof (theHostInfoProvider));
      this.hostInfoProvider = theHostInfoProvider;
      this.hostVersionInfo = !string.IsNullOrEmpty(this.hostInfoProvider.ProcessExeVersion) ? new Version(this.hostInfoProvider.ProcessExeVersion) : (Version) null;
    }

    public void AddSharedProperties(
      List<KeyValuePair<string, object>> sharedProperties,
      TelemetryContext telemetryContext)
    {
      if (!string.IsNullOrEmpty(this.hostInfoProvider.ProcessName))
        sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.ExeName", (object) this.hostInfoProvider.ProcessName.ToLowerInvariant()));
      if (this.hostVersionInfo != (Version) null)
      {
        sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.ExeVersion", (object) this.hostVersionInfo.ToString()));
        sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.BuildNumber", (object) this.hostVersionInfo.Build));
      }
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.ProcessId", (object) this.hostInfoProvider.ProcessId));
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.Is64BitProcess", (object) this.hostInfoProvider.Is64BitProcess));
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.OSBitness", (object) this.hostInfoProvider.OSBitness));
    }

    public void PostProperties(TelemetryContext telemetryContext, CancellationToken token)
    {
    }
  }
}
