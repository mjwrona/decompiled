// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.DocumentSyntax
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll


#nullable enable
namespace Tomlyn.Syntax
{
  public sealed class DocumentSyntax : SyntaxNode
  {
    public DocumentSyntax()
      : base(SyntaxKind.Document)
    {
      SyntaxList<KeyValueSyntax> syntaxList1 = new SyntaxList<KeyValueSyntax>();
      syntaxList1.Parent = (SyntaxNode) this;
      this.KeyValues = syntaxList1;
      SyntaxList<TableSyntaxBase> syntaxList2 = new SyntaxList<TableSyntaxBase>();
      syntaxList2.Parent = (SyntaxNode) this;
      this.Tables = syntaxList2;
      this.Diagnostics = new DiagnosticsBag();
    }

    public DiagnosticsBag Diagnostics { get; }

    public bool HasErrors => this.Diagnostics.HasErrors;

    public SyntaxList<KeyValueSyntax> KeyValues { get; }

    public SyntaxList<TableSyntaxBase> Tables { get; }

    public override void Accept(SyntaxVisitor visitor) => visitor.Visit(this);

    public override int ChildrenCount => 2;

    protected override SyntaxNode GetChildImpl(int index) => index != 0 ? (SyntaxNode) this.Tables : (SyntaxNode) this.KeyValues;
  }
}
