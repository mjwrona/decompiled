// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlReferentialConstraint
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal class CsdlReferentialConstraint : CsdlElement
  {
    private readonly string propertyName;
    private readonly string referencedPropertyName;

    public CsdlReferentialConstraint(
      string propertyName,
      string referencedPropertyName,
      CsdlLocation location)
      : base(location)
    {
      this.propertyName = propertyName;
      this.referencedPropertyName = referencedPropertyName;
    }

    public string PropertyName => this.propertyName;

    public string ReferencedPropertyName => this.referencedPropertyName;
  }
}
