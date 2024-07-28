// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlEntityContainer
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal class CsdlEntityContainer : CsdlNamedElement
  {
    private readonly string extends;
    private readonly List<CsdlEntitySet> entitySets;
    private readonly List<CsdlSingleton> singletons;
    private readonly List<CsdlOperationImport> operationImports;

    public CsdlEntityContainer(
      string name,
      string extends,
      IEnumerable<CsdlEntitySet> entitySets,
      IEnumerable<CsdlSingleton> singletons,
      IEnumerable<CsdlOperationImport> operationImports,
      CsdlLocation location)
      : base(name, location)
    {
      this.extends = extends;
      this.entitySets = new List<CsdlEntitySet>(entitySets);
      this.singletons = new List<CsdlSingleton>(singletons);
      this.operationImports = new List<CsdlOperationImport>(operationImports);
    }

    public string Extends => this.extends;

    public IEnumerable<CsdlEntitySet> EntitySets => (IEnumerable<CsdlEntitySet>) this.entitySets;

    public IEnumerable<CsdlSingleton> Singletons => (IEnumerable<CsdlSingleton>) this.singletons;

    public IEnumerable<CsdlOperationImport> OperationImports => (IEnumerable<CsdlOperationImport>) this.operationImports;
  }
}
