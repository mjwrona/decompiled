// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing.ThreadResourceLock
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing
{
  internal class ThreadResourceLock : IDisposable
  {
    [ThreadStatic]
    private static object syncObject;

    public ThreadResourceLock() => ThreadResourceLock.syncObject = new object();

    public static bool IsResourceLocked => ThreadResourceLock.syncObject != null;

    public void Dispose() => ThreadResourceLock.syncObject = (object) null;
  }
}
