// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataEnumValue
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData
{
  public sealed class ODataEnumValue : ODataValue
  {
    public ODataEnumValue(string value)
    {
      this.Value = value;
      this.TypeName = (string) null;
    }

    public ODataEnumValue(string value, string typeName)
    {
      this.Value = value;
      this.TypeName = typeName;
    }

    public string Value { get; private set; }

    public string TypeName { get; private set; }
  }
}
