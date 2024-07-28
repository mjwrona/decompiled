// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Tracing.InMemoryLog
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Content.Common.Tracing
{
  internal class InMemoryLog
  {
    private readonly ConcurrentQueue<InMemoryLogMessage> logs;

    internal InMemoryLog() => this.logs = new ConcurrentQueue<InMemoryLogMessage>();

    internal void Log(InMemoryLogMessage message) => this.logs.Enqueue(message);

    internal IEnumerable<InMemoryLogMessage> GetSnapshot() => (IEnumerable<InMemoryLogMessage>) this.logs.ToArray();
  }
}
