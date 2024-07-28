// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlStructuredType
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal abstract class CsdlStructuredType : CsdlElement
  {
    protected List<CsdlProperty> structuralProperties;
    protected List<CsdlNavigationProperty> navigationProperties;

    protected CsdlStructuredType(
      IEnumerable<CsdlProperty> structuralProperties,
      IEnumerable<CsdlNavigationProperty> navigationProperties,
      CsdlLocation location)
      : base(location)
    {
      this.structuralProperties = new List<CsdlProperty>(structuralProperties);
      this.navigationProperties = new List<CsdlNavigationProperty>(navigationProperties);
    }

    public IEnumerable<CsdlProperty> StructuralProperties => (IEnumerable<CsdlProperty>) this.structuralProperties;

    public IEnumerable<CsdlNavigationProperty> NavigationProperties => (IEnumerable<CsdlNavigationProperty>) this.navigationProperties;
  }
}
