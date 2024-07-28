// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RateLimitTypeExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class RateLimitTypeExtensions
  {
    public static int ThrottlingWindowInSeconds(this RateLimitType rateLimitType)
    {
      if (rateLimitType == RateLimitType.Short)
        return 300;
      return rateLimitType == RateLimitType.Long ? 3600 : 0;
    }
  }
}
