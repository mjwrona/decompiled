// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Common.XmlElementValue`1
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Csdl.Parsing.Common
{
  internal class XmlElementValue<TValue> : XmlElementValue
  {
    private readonly TValue value;
    private bool isUsed;

    internal XmlElementValue(string name, CsdlLocation location, TValue newValue)
      : base(name, location)
    {
      this.value = newValue;
    }

    internal override bool IsText => false;

    internal override bool IsUsed => this.isUsed;

    internal override object UntypedValue => (object) this.value;

    internal TValue Value
    {
      get
      {
        this.isUsed = true;
        return this.value;
      }
    }

    internal override T ValueAs<T>() => (object) this.Value as T;
  }
}
