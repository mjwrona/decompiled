// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.EdmEnumObjectCollection
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData.Edm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.AspNet.OData
{
  [NonValidatingParameterBinding]
  public class EdmEnumObjectCollection : Collection<IEdmEnumObject>, IEdmObject
  {
    private IEdmCollectionTypeReference _edmType;

    public EdmEnumObjectCollection(IEdmCollectionTypeReference edmType)
      : this(edmType, (IList<IEdmEnumObject>) Enumerable.Empty<IEdmEnumObject>().ToList<IEdmEnumObject>())
    {
    }

    public EdmEnumObjectCollection(IEdmCollectionTypeReference edmType, IList<IEdmEnumObject> list)
      : base(list)
    {
      this.Initialize(edmType);
    }

    public IEdmTypeReference GetEdmType() => (IEdmTypeReference) this._edmType;

    private void Initialize(IEdmCollectionTypeReference edmType)
    {
      if (edmType == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (edmType));
      this._edmType = edmType.ElementType().IsEnum() ? edmType : throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (edmType), SRResources.UnexpectedElementType, (object) edmType.ElementType().ToTraceString(), (object) edmType.ToTraceString(), (object) typeof (IEdmEnumType).Name);
    }
  }
}
