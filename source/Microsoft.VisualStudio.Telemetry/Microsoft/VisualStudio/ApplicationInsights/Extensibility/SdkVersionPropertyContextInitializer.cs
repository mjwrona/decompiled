// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.SdkVersionPropertyContextInitializer
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.DataContracts;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility
{
  internal sealed class SdkVersionPropertyContextInitializer : IContextInitializer
  {
    private const string SDKVersion = "SDKVersion";
    private string sdkVersion;

    public void Initialize(TelemetryContext context)
    {
      string str = LazyInitializer.EnsureInitialized<string>(ref this.sdkVersion, new Func<string>(this.GetAssemblyVersion));
      if (!string.IsNullOrEmpty(context.Internal.SdkVersion))
        return;
      context.Internal.SdkVersion = str;
    }

    private string GetAssemblyVersion() => typeof (SdkVersionPropertyContextInitializer).Assembly.GetCustomAttributes(false).OfType<AssemblyFileVersionAttribute>().First<AssemblyFileVersionAttribute>().Version;
  }
}
