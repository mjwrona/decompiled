// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SqlCommandTimeout
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

namespace Microsoft.TeamFoundation
{
  public static class SqlCommandTimeout
  {
    public const int Infinite = 0;
    public const int FourHours = 14400;
    public const int OneHour = 3600;
    public const int ThirtyMinutes = 1800;
    public const int FifteenMinutes = 900;
    public const int TenMinutes = 600;
    public const int FiveMinutes = 300;
    public const int OneMinute = 60;
    public const int DefaultAnonymousTimeout = 10;

    public static int Max(int timeout1, int timeout2)
    {
      if (timeout1 == 0 || timeout2 == 0)
        return 0;
      return timeout1 <= timeout2 ? timeout2 : timeout1;
    }
  }
}
