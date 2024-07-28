// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmRecordExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmRecordExpression : EdmElement, IEdmRecordExpression, IEdmExpression, IEdmElement
  {
    private readonly IEdmStructuredTypeReference declaredType;
    private readonly IEnumerable<IEdmPropertyConstructor> properties;

    public EdmRecordExpression(params IEdmPropertyConstructor[] properties)
      : this((IEnumerable<IEdmPropertyConstructor>) properties)
    {
    }

    public EdmRecordExpression(
      IEdmStructuredTypeReference declaredType,
      params IEdmPropertyConstructor[] properties)
      : this(declaredType, (IEnumerable<IEdmPropertyConstructor>) properties)
    {
    }

    public EdmRecordExpression(IEnumerable<IEdmPropertyConstructor> properties)
      : this((IEdmStructuredTypeReference) null, properties)
    {
    }

    public EdmRecordExpression(
      IEdmStructuredTypeReference declaredType,
      IEnumerable<IEdmPropertyConstructor> properties)
    {
      EdmUtil.CheckArgumentNull<IEnumerable<IEdmPropertyConstructor>>(properties, nameof (properties));
      this.declaredType = declaredType;
      this.properties = properties;
    }

    public IEdmStructuredTypeReference DeclaredType => this.declaredType;

    public IEnumerable<IEdmPropertyConstructor> Properties => this.properties;

    public EdmExpressionKind ExpressionKind => EdmExpressionKind.Record;
  }
}
