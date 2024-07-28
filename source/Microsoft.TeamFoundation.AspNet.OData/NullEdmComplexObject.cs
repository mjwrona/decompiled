// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.NullEdmComplexObject
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData.Edm;

namespace Microsoft.AspNet.OData
{
  public class NullEdmComplexObject : IEdmComplexObject, IEdmStructuredObject, IEdmObject
  {
    private IEdmComplexTypeReference _edmType;

    public NullEdmComplexObject(IEdmComplexTypeReference edmType) => this._edmType = edmType != null ? edmType : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (edmType));

    public bool TryGetPropertyValue(string propertyName, out object value) => throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.EdmComplexObjectNullRef, (object) propertyName, (object) this._edmType.ToTraceString());

    public IEdmTypeReference GetEdmType() => (IEdmTypeReference) this._edmType;

    public void SetModel(IEdmModel model)
    {
    }
  }
}
