// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.CancellingTracer
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Pegasus.Common;
using Pegasus.Common.Tracing;
using System.Threading;

namespace Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs
{
  public class CancellingTracer : ITracer
  {
    private readonly CancellationToken token;
    private readonly ITracer innerTracer;

    public CancellingTracer(CancellationToken token, ITracer innerTracer = null)
    {
      this.token = token;
      this.innerTracer = innerTracer ?? (ITracer) NullTracer.Instance;
    }

    public void TraceCacheHit<T>(
      string ruleName,
      Cursor cursor,
      CacheKey cacheKey,
      IParseResult<T> parseResult)
    {
      this.innerTracer.TraceCacheHit<T>(ruleName, cursor, cacheKey, parseResult);
      this.token.ThrowIfCancellationRequested();
    }

    public void TraceCacheMiss(string ruleName, Cursor cursor, CacheKey cacheKey)
    {
      this.innerTracer.TraceCacheMiss(ruleName, cursor, cacheKey);
      this.token.ThrowIfCancellationRequested();
    }

    public void TraceInfo(string ruleName, Cursor cursor, string info)
    {
      this.innerTracer.TraceInfo(ruleName, cursor, info);
      this.token.ThrowIfCancellationRequested();
    }

    public void TraceRuleEnter(string ruleName, Cursor cursor)
    {
      this.innerTracer.TraceRuleEnter(ruleName, cursor);
      this.token.ThrowIfCancellationRequested();
    }

    public void TraceRuleExit<T>(string ruleName, Cursor cursor, IParseResult<T> parseResult)
    {
      this.innerTracer.TraceRuleExit<T>(ruleName, cursor, parseResult);
      this.token.ThrowIfCancellationRequested();
    }
  }
}
