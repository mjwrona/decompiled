// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.BadCollectionType
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Validation;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
  internal class BadCollectionType : BadType, IEdmCollectionType, IEdmType, IEdmElement
  {
    private readonly IEdmTypeReference elementType;

    public BadCollectionType(IEnumerable<EdmError> errors)
      : base(errors)
    {
      this.elementType = (IEdmTypeReference) new BadTypeReference(new BadType(errors), true);
    }

    public override EdmTypeKind TypeKind => EdmTypeKind.Collection;

    public IEdmTypeReference ElementType => this.elementType;
  }
}
