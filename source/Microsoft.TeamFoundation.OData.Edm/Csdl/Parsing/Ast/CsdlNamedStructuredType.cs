// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlNamedStructuredType
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal abstract class CsdlNamedStructuredType : CsdlStructuredType
  {
    protected string baseTypeName;
    protected bool isAbstract;
    protected bool isOpen;
    protected string name;

    protected CsdlNamedStructuredType(
      string name,
      string baseTypeName,
      bool isAbstract,
      bool isOpen,
      IEnumerable<CsdlProperty> structuralproperties,
      IEnumerable<CsdlNavigationProperty> navigationProperties,
      CsdlLocation location)
      : base(structuralproperties, navigationProperties, location)
    {
      this.isAbstract = isAbstract;
      this.isOpen = isOpen;
      this.name = name;
      this.baseTypeName = baseTypeName;
    }

    public string BaseTypeName => this.baseTypeName;

    public bool IsAbstract => this.isAbstract;

    public bool IsOpen => this.isOpen;

    public string Name => this.name;
  }
}
