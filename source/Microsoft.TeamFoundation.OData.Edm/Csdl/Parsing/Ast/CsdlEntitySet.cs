// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlEntitySet
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal class CsdlEntitySet : CsdlAbstractNavigationSource
  {
    private readonly string elementType;

    public CsdlEntitySet(
      string name,
      string elementType,
      IEnumerable<CsdlNavigationPropertyBinding> navigationPropertyBindings,
      CsdlLocation location)
      : this(name, elementType, navigationPropertyBindings, location, true)
    {
    }

    public CsdlEntitySet(
      string name,
      string elementType,
      IEnumerable<CsdlNavigationPropertyBinding> navigationPropertyBindings,
      CsdlLocation location,
      bool includeInServiceDocument)
      : base(name, navigationPropertyBindings, location)
    {
      this.elementType = elementType;
      this.IncludeInServiceDocument = includeInServiceDocument;
    }

    public string ElementType => this.elementType;

    public bool IncludeInServiceDocument { get; private set; }
  }
}
