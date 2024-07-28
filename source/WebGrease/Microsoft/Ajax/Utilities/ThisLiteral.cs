// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ThisLiteral
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

namespace Microsoft.Ajax.Utilities
{
  public sealed class ThisLiteral : Expression
  {
    public ThisLiteral(Context context)
      : base(context)
    {
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override bool IsEquivalentTo(AstNode otherNode) => otherNode is ThisLiteral;
  }
}
