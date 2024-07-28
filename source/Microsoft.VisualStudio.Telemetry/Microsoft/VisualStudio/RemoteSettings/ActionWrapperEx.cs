// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.ActionWrapperEx
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal static class ActionWrapperEx
  {
    public static ActionWrapper<T> WithSubscriptionDetails<T>(
      this ActionWrapper<T> baseAction,
      ActionSubscriptionDetails details)
    {
      return new ActionWrapper<T>()
      {
        RuleId = baseAction.RuleId,
        FlightName = baseAction.FlightName,
        ActionPath = baseAction.ActionPath,
        Precedence = baseAction.Precedence,
        Action = baseAction.Action,
        Subscription = details
      };
    }
  }
}
