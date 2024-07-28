// Decompiled with JetBrains decompiler
// Type: Pegasus.Common.Tracing.DiagnosticsTracer
// Assembly: Pegasus.Common, Version=4.0.14.0, Culture=neutral, PublicKeyToken=28c69b6c6d100f4a
// MVID: 081C50C0-D236-41F7-86F9-E3F2168B7118
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Pegasus.Common.dll

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Pegasus.Common.Tracing
{
  public class DiagnosticsTracer : ITracer
  {
    private DiagnosticsTracer()
    {
    }

    public static DiagnosticsTracer Instance { get; } = new DiagnosticsTracer();

    public void TraceCacheHit<T>(
      string ruleName,
      Cursor cursor,
      CacheKey cacheKey,
      IParseResult<T> parseResult)
    {
      this.TraceInfo(ruleName, cursor, "Cache hit.");
    }

    public void TraceCacheMiss(string ruleName, Cursor cursor, CacheKey cacheKey) => this.TraceInfo(ruleName, cursor, "Cache miss.");

    public void TraceInfo(string ruleName, Cursor cursor, string info) => Trace.WriteLine(info);

    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])", Justification = "This is fine.")]
    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validation excluded for performance reasons.")]
    public void TraceRuleEnter(string ruleName, Cursor cursor)
    {
      Trace.WriteLine(string.Format("Begin '{0}' at ({1},{2}) with state key {3}", (object) ruleName, (object) cursor.Line, (object) cursor.Column, (object) cursor.StateKey));
      Trace.Indent();
    }

    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])", Justification = "This is fine.")]
    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validation excluded for performance reasons.")]
    public void TraceRuleExit<T>(string ruleName, Cursor cursor, IParseResult<T> parseResult)
    {
      bool flag = parseResult != null;
      Trace.Unindent();
      Trace.WriteLine(string.Format("End '{0}' with {1} at ({2},{3}) with state key {4}", (object) ruleName, flag ? (object) "success" : (object) "failure", (object) cursor.Line, (object) cursor.Column, (object) cursor.StateKey));
    }
  }
}
