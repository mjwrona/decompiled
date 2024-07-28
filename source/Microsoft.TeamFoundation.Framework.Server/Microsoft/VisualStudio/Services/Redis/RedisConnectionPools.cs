// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.RedisConnectionPools
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.VisualStudio.Services.Redis
{
  public static class RedisConnectionPools
  {
    public const string Default = "$Default";
    public const string Probation = "$Probation";
    public const string SignalR = "$SignalR";
    public static readonly string[] AllPools = new string[3]
    {
      "$Default",
      "$Probation",
      "$SignalR"
    };
  }
}
