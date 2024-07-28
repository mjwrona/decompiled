// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.DiagnosticMessage
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Text;


#nullable enable
namespace Tomlyn.Syntax
{
  public class DiagnosticMessage
  {
    public DiagnosticMessage(DiagnosticMessageKind kind, SourceSpan span, string message)
    {
      this.Kind = kind;
      this.Span = span;
      this.Message = message;
    }

    public DiagnosticMessageKind Kind { get; }

    public SourceSpan Span { get; }

    public string Message { get; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(this.Span.ToStringSimple());
      stringBuilder.Append(" : ");
      switch (this.Kind)
      {
        case DiagnosticMessageKind.Error:
          stringBuilder.Append("error");
          break;
        case DiagnosticMessageKind.Warning:
          stringBuilder.Append("warning");
          break;
        default:
          throw new InvalidOperationException(string.Format("Message type [{0}] not supported", (object) this.Kind));
      }
      stringBuilder.Append(" : ");
      if (this.Message != null)
        stringBuilder.Append(this.Message);
      return stringBuilder.ToString();
    }
  }
}
