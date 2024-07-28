// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.SyntaxList
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System.Collections.Generic;


#nullable enable
namespace Tomlyn.Syntax
{
  public abstract class SyntaxList : SyntaxNode
  {
    protected readonly List<SyntaxNode> Children;

    internal SyntaxList()
      : base(SyntaxKind.List)
    {
      this.Children = new List<SyntaxNode>();
    }

    public override sealed int ChildrenCount => this.Children.Count;

    protected override SyntaxNode GetChildImpl(int index) => this.Children[index];

    public override void Accept(SyntaxVisitor visitor) => visitor.Visit(this);
  }
}
