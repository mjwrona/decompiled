// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TraceRequestExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class TraceRequestExtensions
  {
    public static void TraceConditionally(
      this ITraceRequest tracer,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Func<string> message)
    {
      tracer.TraceConditionally(tracepoint, level, area, layer, (string[]) null, message);
    }

    public static void TraceConditionally(
      this ITraceRequest tracer,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      Func<string> message)
    {
      if (level != TraceLevel.Error && !tracer.IsTracing(tracepoint, level, area, layer, tags))
        return;
      string format;
      try
      {
        format = message();
      }
      catch (Exception ex)
      {
        format = "Exception thrown while generating trace message: " + ex.ToReadableStackTrace();
      }
      tracer.Trace(tracepoint, level, area, layer, tags, format);
    }

    public static void TraceSerializedConditionally(
      this ITraceRequest tracer,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      bool includeStackTrace,
      string format,
      params object[] args)
    {
      if (level != TraceLevel.Error && !tracer.IsTracing(tracepoint, level, area, layer))
        return;
      string format1 = TraceRequestExtensions.FormatWithSafeSerialization(format, args, includeStackTrace);
      tracer.Trace(tracepoint, level, area, layer, format1);
    }

    public static void TraceSerializedConditionally(
      this ITraceRequest tracer,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string format,
      params object[] args)
    {
      tracer.TraceSerializedConditionally(tracepoint, level, area, layer, true, format, args);
    }

    public static void TraceDataConditionally(
      this ITraceRequest tracer,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string traceMessage,
      Func<object> traceData = null,
      [CallerMemberName] string methodName = null)
    {
      tracer.TraceConditionally(tracepoint, level, area, layer, (Func<string>) (() => JsonUtilities.Serialize(traceData != null ? (object) new
      {
        method = methodName,
        message = traceMessage,
        data = traceData()
      } : (object) new
      {
        method = methodName,
        message = traceMessage
      })));
    }

    public static void Trace(
      this ITraceRequest tracer,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string format,
      params object[] args)
    {
      tracer.Trace(tracepoint, level, area, layer, (string[]) null, format, args);
    }

    private static string FormatWithSafeSerialization(
      string format,
      object[] args,
      bool includeStackTrace)
    {
      int length = args == null ? 0 : args.Length;
      string[] strArray = new string[length];
      for (int index = 0; index < length; ++index)
      {
        try
        {
          strArray[index] = JsonConvert.SerializeObject(args[index]);
        }
        catch (Exception ex)
        {
          strArray[index] = string.Format("[Exception thrown while serializing argument {0}: {1}]", (object) index, (object) ex.ToReadableStackTrace());
        }
      }
      string str;
      try
      {
        str = string.Format(format, (object[]) strArray);
        if (includeStackTrace)
          str = str + "\nStack trace: " + EnvironmentWrapper.ToReadableStackTrace();
      }
      catch (Exception ex)
      {
        str = "Exception thrown while generating formatted message: " + ex.ToReadableStackTrace();
      }
      return str;
    }
  }
}
