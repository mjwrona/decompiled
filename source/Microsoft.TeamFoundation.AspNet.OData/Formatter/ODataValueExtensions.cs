// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.ODataValueExtensions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData;

namespace Microsoft.AspNet.OData.Formatter
{
  internal static class ODataValueExtensions
  {
    public static object GetInnerValue(this ODataValue odataValue)
    {
      switch (odataValue)
      {
        case ODataNullValue _:
          return (object) null;
        case ODataPrimitiveValue odataPrimitiveValue:
          return odataPrimitiveValue.Value;
        default:
          return (object) odataValue;
      }
    }
  }
}
