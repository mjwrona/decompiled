// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.TracerExtension
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public static class TracerExtension
  {
    public static Action<TraceLevel, string> ToTraceAction(this Tracer tracer) => (Action<TraceLevel, string>) ((level, msg) =>
    {
      switch (level)
      {
        case TraceLevel.Error:
          tracer.TraceError(msg);
          break;
        case TraceLevel.Warning:
          tracer.TraceWarning(msg);
          break;
        case TraceLevel.Info:
          tracer.TraceInfo(msg);
          break;
      }
    });
  }
}
