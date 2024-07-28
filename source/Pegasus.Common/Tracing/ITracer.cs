// Decompiled with JetBrains decompiler
// Type: Pegasus.Common.Tracing.ITracer
// Assembly: Pegasus.Common, Version=4.0.14.0, Culture=neutral, PublicKeyToken=28c69b6c6d100f4a
// MVID: 081C50C0-D236-41F7-86F9-E3F2168B7118
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Pegasus.Common.dll

namespace Pegasus.Common.Tracing
{
  public interface ITracer
  {
    void TraceCacheHit<T>(
      string ruleName,
      Cursor cursor,
      CacheKey cacheKey,
      IParseResult<T> parseResult);

    void TraceCacheMiss(string ruleName, Cursor cursor, CacheKey cacheKey);

    void TraceInfo(string ruleName, Cursor cursor, string info);

    void TraceRuleEnter(string ruleName, Cursor cursor);

    void TraceRuleExit<T>(string ruleName, Cursor cursor, IParseResult<T> parseResult);
  }
}
