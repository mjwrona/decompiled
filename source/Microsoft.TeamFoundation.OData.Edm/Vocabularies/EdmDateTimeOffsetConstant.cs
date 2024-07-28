// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmDateTimeOffsetConstant
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmDateTimeOffsetConstant : 
    EdmValue,
    IEdmDateTimeOffsetConstantExpression,
    IEdmExpression,
    IEdmElement,
    IEdmDateTimeOffsetValue,
    IEdmPrimitiveValue,
    IEdmValue
  {
    private readonly DateTimeOffset value;

    public EdmDateTimeOffsetConstant(DateTimeOffset value)
      : this((IEdmTemporalTypeReference) null, value)
    {
      this.value = value;
    }

    public EdmDateTimeOffsetConstant(IEdmTemporalTypeReference type, DateTimeOffset value)
      : base((IEdmTypeReference) type)
    {
      this.value = value;
    }

    public DateTimeOffset Value => this.value;

    public EdmExpressionKind ExpressionKind => EdmExpressionKind.DateTimeOffsetConstant;

    public override EdmValueKind ValueKind => EdmValueKind.DateTimeOffset;
  }
}
