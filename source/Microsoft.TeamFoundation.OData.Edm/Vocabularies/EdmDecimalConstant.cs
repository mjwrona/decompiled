// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmDecimalConstant
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmDecimalConstant : 
    EdmValue,
    IEdmDecimalConstantExpression,
    IEdmExpression,
    IEdmElement,
    IEdmDecimalValue,
    IEdmPrimitiveValue,
    IEdmValue
  {
    private readonly Decimal value;

    public EdmDecimalConstant(Decimal value)
      : this((IEdmDecimalTypeReference) null, value)
    {
    }

    public EdmDecimalConstant(IEdmDecimalTypeReference type, Decimal value)
      : base((IEdmTypeReference) type)
    {
      this.value = value;
    }

    public Decimal Value => this.value;

    public EdmExpressionKind ExpressionKind => EdmExpressionKind.DecimalConstant;

    public override EdmValueKind ValueKind => EdmValueKind.Decimal;
  }
}
