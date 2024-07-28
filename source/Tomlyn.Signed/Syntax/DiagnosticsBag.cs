// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.DiagnosticsBag
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;


#nullable enable
namespace Tomlyn.Syntax
{
  [DebuggerDisplay("{Count} Errors: {HasErrors}")]
  public class DiagnosticsBag : IEnumerable<DiagnosticMessage>, IEnumerable
  {
    private readonly List<DiagnosticMessage> _messages;

    public DiagnosticsBag() => this._messages = new List<DiagnosticMessage>();

    public DiagnosticsBag(IEnumerable<DiagnosticMessage> messages)
      : this()
    {
      foreach (DiagnosticMessage message in messages)
        this.Add(message);
    }

    public int Count => this._messages.Count;

    public DiagnosticMessage this[int index] => this._messages[index];

    public bool HasErrors { get; private set; }

    public void Add(DiagnosticMessage message)
    {
      if (message == null)
        throw new ArgumentNullException(nameof (message));
      this._messages.Add(message);
      if (message.Kind != DiagnosticMessageKind.Error)
        return;
      this.HasErrors = true;
    }

    public void AddRange(IEnumerable<DiagnosticMessage> messages)
    {
      foreach (DiagnosticMessage message in messages)
        this.Add(message);
    }

    public void Clear()
    {
      this._messages.Clear();
      this.HasErrors = false;
    }

    public void Warning(SourceSpan span, string text) => this.Add(new DiagnosticMessage(DiagnosticMessageKind.Warning, span, text));

    public void Error(SourceSpan span, string text) => this.Add(new DiagnosticMessage(DiagnosticMessageKind.Error, span, text));

    public List<DiagnosticMessage>.Enumerator GetEnumerator() => this._messages.GetEnumerator();

    IEnumerator<DiagnosticMessage> IEnumerable<DiagnosticMessage>.GetEnumerator() => (IEnumerator<DiagnosticMessage>) this.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) this._messages).GetEnumerator();

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (DiagnosticMessage message in this._messages)
        stringBuilder.AppendLine(message.ToString());
      return stringBuilder.ToString();
    }
  }
}
