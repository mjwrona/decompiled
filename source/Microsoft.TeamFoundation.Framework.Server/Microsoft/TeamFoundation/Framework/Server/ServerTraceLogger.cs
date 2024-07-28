// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServerTraceLogger
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ServerTraceLogger : TFLogger
  {
    public override void Info(string message) => TeamFoundationTracingService.TraceRaw(0, TraceLevel.Info, (string) null, (string) null, message);

    public override void Warning(string message) => TeamFoundationTracingService.TraceRaw(0, TraceLevel.Warning, (string) null, (string) null, message);

    public override void Error(string message) => TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, (string) null, (string) null, message);
  }
}
