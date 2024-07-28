// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Tracing.InMemoryLogMessage
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Content.Common.Tracing
{
  internal struct InMemoryLogMessage
  {
    internal readonly DateTime Date;
    internal readonly int ThreadId;
    internal readonly string Message;

    internal InMemoryLogMessage(string message)
    {
      this.Date = DateTime.Now;
      this.ThreadId = Environment.CurrentManagedThreadId;
      this.Message = message;
    }

    internal string DisplayMessage => SafeStringFormat.FormatSafe(string.Format("{0}, {1}, {2}", (object) this.Date.ToStringSafe("o"), (object) this.ThreadId, (object) this.Message));

    public override string ToString() => this.DisplayMessage;
  }
}
