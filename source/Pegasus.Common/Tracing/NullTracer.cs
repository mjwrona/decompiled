// Decompiled with JetBrains decompiler
// Type: Pegasus.Common.Tracing.NullTracer
// Assembly: Pegasus.Common, Version=4.0.14.0, Culture=neutral, PublicKeyToken=28c69b6c6d100f4a
// MVID: 081C50C0-D236-41F7-86F9-E3F2168B7118
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Pegasus.Common.dll

namespace Pegasus.Common.Tracing
{
  public class NullTracer : ITracer
  {
    private NullTracer()
    {
    }

    public static NullTracer Instance { get; } = new NullTracer();

    public void TraceCacheHit<T>(
      string ruleName,
      Cursor cursor,
      CacheKey cacheKey,
      IParseResult<T> parseResult)
    {
    }

    public void TraceCacheMiss(string ruleName, Cursor cursor, CacheKey cacheKey)
    {
    }

    public void TraceInfo(string ruleName, Cursor cursor, string info)
    {
    }

    public void TraceRuleEnter(string ruleName, Cursor cursor)
    {
    }

    public void TraceRuleExit<T>(string ruleName, Cursor cursor, IParseResult<T> parseResult)
    {
    }
  }
}
