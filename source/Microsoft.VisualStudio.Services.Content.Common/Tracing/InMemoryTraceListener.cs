// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Tracing.InMemoryTraceListener
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Content.Common.Tracing
{
  public class InMemoryTraceListener : TraceListener
  {
    private InMemoryLog log;

    internal InMemoryTraceListener(InMemoryLog log) => this.log = log != null ? log : throw new ArgumentNullException(nameof (log));

    public override void Write(string message) => this.WriteInternal(message);

    public override void WriteLine(string message) => this.WriteInternal(message);

    private void WriteInternal(string message) => this.log.Log(new InMemoryLogMessage(message));
  }
}
