// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Common.XmlAnnotationInfo
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Csdl.Parsing.Common
{
  internal class XmlAnnotationInfo
  {
    internal XmlAnnotationInfo(
      CsdlLocation location,
      string namespaceName,
      string name,
      string value,
      bool isAttribute)
    {
      this.Location = location;
      this.NamespaceName = namespaceName;
      this.Name = name;
      this.Value = value;
      this.IsAttribute = isAttribute;
    }

    internal string NamespaceName { get; private set; }

    internal string Name { get; private set; }

    internal CsdlLocation Location { get; private set; }

    internal string Value { get; private set; }

    internal bool IsAttribute { get; private set; }
  }
}
