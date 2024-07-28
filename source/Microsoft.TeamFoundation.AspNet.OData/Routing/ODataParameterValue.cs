// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.ODataParameterValue
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;

namespace Microsoft.AspNet.OData.Routing
{
  internal class ODataParameterValue
  {
    public const string ParameterValuePrefix = "DF908045-6922-46A0-82F2-2F6E7F43D1B1_";

    public ODataParameterValue(object paramValue, IEdmTypeReference paramType)
    {
      if (paramType == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (paramType));
      this.Value = paramValue;
      this.EdmType = paramType;
    }

    public IEdmTypeReference EdmType { get; private set; }

    public object Value { get; private set; }
  }
}
