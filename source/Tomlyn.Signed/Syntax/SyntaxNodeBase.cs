// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.SyntaxNodeBase
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System.Diagnostics;


#nullable enable
namespace Tomlyn.Syntax
{
  [DebuggerDisplay("{ToDebuggerDisplay(),nq}")]
  public abstract class SyntaxNodeBase
  {
    public SourceSpan Span;

    public abstract void Accept(SyntaxVisitor visitor);

    public SyntaxNode? Parent { get; internal set; }

    protected virtual string ToDebuggerDisplay() => this.GetType().Name ?? "";
  }
}
