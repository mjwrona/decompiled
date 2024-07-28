// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.EdmComplexObjectCollection
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData.Edm;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.AspNet.OData
{
  [NonValidatingParameterBinding]
  public class EdmComplexObjectCollection : Collection<IEdmComplexObject>, IEdmObject
  {
    private IEdmCollectionTypeReference _edmType;

    public EdmComplexObjectCollection(IEdmCollectionTypeReference edmType) => this.Initialize(edmType);

    public EdmComplexObjectCollection(
      IEdmCollectionTypeReference edmType,
      IList<IEdmComplexObject> list)
      : base(list)
    {
      this.Initialize(edmType);
    }

    public IEdmTypeReference GetEdmType() => (IEdmTypeReference) this._edmType;

    private void Initialize(IEdmCollectionTypeReference edmType)
    {
      if (edmType == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (edmType));
      this._edmType = edmType.ElementType().IsComplex() ? edmType : throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (edmType), SRResources.UnexpectedElementType, (object) edmType.ElementType().ToTraceString(), (object) edmType.ToTraceString(), (object) typeof (IEdmComplexType).Name);
    }
  }
}
