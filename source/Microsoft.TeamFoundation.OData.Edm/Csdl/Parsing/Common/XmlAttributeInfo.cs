// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Common.XmlAttributeInfo
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Csdl.Parsing.Common
{
  internal class XmlAttributeInfo
  {
    internal static readonly XmlAttributeInfo Missing = new XmlAttributeInfo();
    private readonly string name;
    private readonly string attributeValue;
    private readonly CsdlLocation location;

    internal XmlAttributeInfo(string attrName, string attrValue, CsdlLocation attrLocation)
    {
      this.name = attrName;
      this.attributeValue = attrValue;
      this.location = attrLocation;
    }

    private XmlAttributeInfo()
    {
    }

    internal bool IsMissing => XmlAttributeInfo.Missing == this;

    internal bool IsUsed { get; set; }

    internal CsdlLocation Location => this.location;

    internal string Name => this.name;

    internal string Value => this.attributeValue;
  }
}
