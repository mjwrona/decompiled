// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITraceRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public interface ITraceRequest
  {
    bool IsTracing(int tracepoint, TraceLevel level, string area, string layer, string[] tags = null);

    void TraceEnter(int tracepoint, string area, string layer, [CallerMemberName] string methodName = null);

    void TraceLeave(int tracepoint, string area, string layer, [CallerMemberName] string methodName = null);

    void TraceException(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Exception exception);

    void TraceException(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Exception exception,
      string format,
      params object[] args);

    void Trace(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string format);

    void Trace(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string format,
      object arg0);

    void Trace(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string format,
      object arg0,
      object arg1);

    void Trace(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string format,
      object arg0,
      object arg1,
      object arg2);

    void Trace(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string format,
      params object[] args);

    void TraceAlways(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string format,
      params object[] args);

    void TraceAlways(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string format,
      params object[] args);
  }
}
