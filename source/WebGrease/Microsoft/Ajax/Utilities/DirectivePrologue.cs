// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.DirectivePrologue
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

namespace Microsoft.Ajax.Utilities
{
  public class DirectivePrologue : ConstantWrapper
  {
    public DirectivePrologue(string value, Context context)
      : base((object) value, PrimitiveType.String, context)
    {
      this.UseStrict = string.CompareOrdinal(this.Context.Code, 1, "use strict", 0, 10) == 0;
    }

    public bool UseStrict { get; private set; }

    public bool IsRedundant { get; set; }

    public override bool IsExpression => false;

    public override bool IsConstant => false;

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);
  }
}
