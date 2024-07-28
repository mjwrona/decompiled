// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlDirectValueAnnotation
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal class CsdlDirectValueAnnotation : CsdlElement
  {
    private readonly string namespaceName;
    private readonly string name;
    private readonly string value;
    private readonly bool isAttribute;

    public CsdlDirectValueAnnotation(
      string namespaceName,
      string name,
      string value,
      bool isAttribute,
      CsdlLocation location)
      : base(location)
    {
      this.namespaceName = namespaceName;
      this.name = name;
      this.value = value;
      this.isAttribute = isAttribute;
    }

    public string NamespaceName => this.namespaceName;

    public string Name => this.name;

    public string Value => this.value;

    public bool IsAttribute => this.isAttribute;
  }
}
