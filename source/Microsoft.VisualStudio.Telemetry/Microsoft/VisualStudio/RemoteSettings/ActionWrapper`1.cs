// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.ActionWrapper`1
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

namespace Microsoft.VisualStudio.RemoteSettings
{
  public sealed class ActionWrapper<T>
  {
    public string ActionPath { get; internal set; }

    public T Action { get; internal set; }

    public int Precedence { get; internal set; }

    public string RuleId { get; internal set; }

    public string ActionType { get; internal set; }

    public string FlightName { get; internal set; }

    public ActionSubscriptionDetails Subscription { get; internal set; }

    internal string ActionJson { get; set; }
  }
}
