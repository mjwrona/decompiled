// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing.CoreEventSource
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing
{
  internal sealed class CoreEventSource
  {
    public static readonly ICoreEventSource Log;

    static CoreEventSource()
    {
      if (Platform.IsWindows)
        CoreEventSource.Log = CoreEventSource.CreateWindowsEventSource();
      else
        CoreEventSource.Log = CoreEventSource.CreateMonoEventSource();
    }

    private static ICoreEventSource CreateWindowsEventSource() => (ICoreEventSource) WindowsCoreEventSource.Log;

    private static ICoreEventSource CreateMonoEventSource() => (ICoreEventSource) new MonoEventSource();
  }
}
