// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmCollectionValue
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmCollectionValue : EdmValue, IEdmCollectionValue, IEdmValue, IEdmElement
  {
    private readonly IEnumerable<IEdmDelayedValue> elements;

    public EdmCollectionValue(
      IEdmCollectionTypeReference type,
      IEnumerable<IEdmDelayedValue> elements)
      : base((IEdmTypeReference) type)
    {
      this.elements = elements;
    }

    public IEnumerable<IEdmDelayedValue> Elements => this.elements;

    public override EdmValueKind ValueKind => EdmValueKind.Collection;
  }
}
