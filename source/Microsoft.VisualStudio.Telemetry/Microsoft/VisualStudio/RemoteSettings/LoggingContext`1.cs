// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.LoggingContext`1
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal class LoggingContext<T>
  {
    public string Context { get; set; }

    public T Value { get; set; }

    public LoggingContext(string context, T value)
    {
      this.Context = context;
      this.Value = value;
    }
  }
}
