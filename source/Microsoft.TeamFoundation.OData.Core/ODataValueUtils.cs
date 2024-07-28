// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataValueUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData
{
  internal static class ODataValueUtils
  {
    internal static ODataValue ToODataValue(this object objectToConvert)
    {
      if (objectToConvert == null)
        return (ODataValue) new ODataNullValue();
      if (objectToConvert is ODataValue odataValue)
        return odataValue;
      return objectToConvert.GetType().IsEnum() ? (ODataValue) new ODataEnumValue(objectToConvert.ToString().Replace(", ", ",")) : (ODataValue) new ODataPrimitiveValue(objectToConvert);
    }

    internal static object FromODataValue(this ODataValue odataValue)
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
