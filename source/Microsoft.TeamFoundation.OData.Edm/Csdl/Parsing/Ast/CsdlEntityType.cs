// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlEntityType
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal class CsdlEntityType : CsdlNamedStructuredType
  {
    private readonly CsdlKey key;
    private readonly bool hasStream;

    public CsdlEntityType(
      string name,
      string baseTypeName,
      bool isAbstract,
      bool isOpen,
      bool hasStream,
      CsdlKey key,
      IEnumerable<CsdlProperty> structualProperties,
      IEnumerable<CsdlNavigationProperty> navigationProperties,
      CsdlLocation location)
      : base(name, baseTypeName, isAbstract, isOpen, structualProperties, navigationProperties, location)
    {
      this.key = key;
      this.hasStream = hasStream;
    }

    public CsdlKey Key => this.key;

    public bool HasStream => this.hasStream;
  }
}
