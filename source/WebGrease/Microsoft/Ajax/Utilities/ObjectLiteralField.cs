// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ObjectLiteralField
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

namespace Microsoft.Ajax.Utilities
{
  public class ObjectLiteralField : ConstantWrapper, INameDeclaration
  {
    public bool IsIdentifier { get; set; }

    public Context ColonContext { get; set; }

    public ObjectLiteralField(object value, PrimitiveType primitiveType, Context context)
      : base(value, primitiveType, context)
    {
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public string Name => this.ToString();

    public AstNode Initializer => (AstNode) null;

    public bool IsParameter => false;

    public bool RenameNotAllowed => true;

    public JSVariableField VariableField { get; set; }
  }
}
