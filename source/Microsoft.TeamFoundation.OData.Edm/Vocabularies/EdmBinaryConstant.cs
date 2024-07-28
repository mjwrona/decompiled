// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmBinaryConstant
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmBinaryConstant : 
    EdmValue,
    IEdmBinaryConstantExpression,
    IEdmExpression,
    IEdmElement,
    IEdmBinaryValue,
    IEdmPrimitiveValue,
    IEdmValue
  {
    private readonly byte[] value;

    public EdmBinaryConstant(byte[] value)
      : this((IEdmBinaryTypeReference) null, value)
    {
    }

    public EdmBinaryConstant(IEdmBinaryTypeReference type, byte[] value)
      : base((IEdmTypeReference) type)
    {
      EdmUtil.CheckArgumentNull<byte[]>(value, nameof (value));
      this.value = value;
    }

    public byte[] Value => this.value;

    public EdmExpressionKind ExpressionKind => EdmExpressionKind.BinaryConstant;

    public override EdmValueKind ValueKind => EdmValueKind.Binary;
  }
}
