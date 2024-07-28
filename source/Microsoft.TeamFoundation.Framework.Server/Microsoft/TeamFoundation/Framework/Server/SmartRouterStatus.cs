// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SmartRouterStatus
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll


#nullable enable
namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class SmartRouterStatus
  {
    public const string None = "None";
    public const string ProxyReceived = "ProxyReceived";
    public const string NotRoutable = "NotRoutable";
    public const string Routable = "Routable";
    public const string NotRouted = "NotRouted";
    public const string Routed = "Routed";
    public const string Exception = "Exception";
  }
}
