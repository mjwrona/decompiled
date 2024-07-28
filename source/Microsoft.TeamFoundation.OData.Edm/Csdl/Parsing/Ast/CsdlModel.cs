// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlModel
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal class CsdlModel
  {
    private readonly List<CsdlSchema> schemata = new List<CsdlSchema>();
    private readonly List<IEdmReference> currentModelReferences = new List<IEdmReference>();
    private readonly List<IEdmReference> parentModelReferences = new List<IEdmReference>();

    public IEnumerable<IEdmReference> CurrentModelReferences => (IEnumerable<IEdmReference>) this.currentModelReferences;

    public IEnumerable<IEdmReference> ParentModelReferences => (IEnumerable<IEdmReference>) this.parentModelReferences;

    public IEnumerable<CsdlSchema> Schemata => (IEnumerable<CsdlSchema>) this.schemata;

    public void AddSchema(CsdlSchema schema) => this.schemata.Add(schema);

    public void AddCurrentModelReferences(IEnumerable<IEdmReference> referencesToAdd) => this.currentModelReferences.AddRange(referencesToAdd);

    public void AddParentModelReferences(IEdmReference referenceToAdd) => this.parentModelReferences.Add(referenceToAdd);
  }
}
