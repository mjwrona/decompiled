// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlSchema
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal class CsdlSchema : CsdlElement
  {
    private readonly List<CsdlStructuredType> structuredTypes;
    private readonly List<CsdlEnumType> enumTypes;
    private readonly List<CsdlOperation> operations;
    private readonly List<CsdlTerm> terms;
    private readonly List<CsdlEntityContainer> entityContainers;
    private readonly List<CsdlAnnotations> outOfLineAnnotations;
    private readonly List<CsdlTypeDefinition> typeDefinitions;
    private readonly string alias;
    private readonly string namespaceName;
    private readonly Version version;

    public CsdlSchema(
      string namespaceName,
      string alias,
      Version version,
      IEnumerable<CsdlStructuredType> structuredTypes,
      IEnumerable<CsdlEnumType> enumTypes,
      IEnumerable<CsdlOperation> operations,
      IEnumerable<CsdlTerm> terms,
      IEnumerable<CsdlEntityContainer> entityContainers,
      IEnumerable<CsdlAnnotations> outOfLineAnnotations,
      IEnumerable<CsdlTypeDefinition> typeDefinitions,
      CsdlLocation location)
      : base(location)
    {
      this.alias = alias;
      this.namespaceName = namespaceName;
      this.version = version;
      this.structuredTypes = new List<CsdlStructuredType>(structuredTypes);
      this.enumTypes = new List<CsdlEnumType>(enumTypes);
      this.operations = new List<CsdlOperation>(operations);
      this.terms = new List<CsdlTerm>(terms);
      this.entityContainers = new List<CsdlEntityContainer>(entityContainers);
      this.outOfLineAnnotations = new List<CsdlAnnotations>(outOfLineAnnotations);
      this.typeDefinitions = new List<CsdlTypeDefinition>(typeDefinitions);
    }

    public IEnumerable<CsdlStructuredType> StructuredTypes => (IEnumerable<CsdlStructuredType>) this.structuredTypes;

    public IEnumerable<CsdlEnumType> EnumTypes => (IEnumerable<CsdlEnumType>) this.enumTypes;

    public IEnumerable<CsdlOperation> Operations => (IEnumerable<CsdlOperation>) this.operations;

    public IEnumerable<CsdlTerm> Terms => (IEnumerable<CsdlTerm>) this.terms;

    public IEnumerable<CsdlEntityContainer> EntityContainers => (IEnumerable<CsdlEntityContainer>) this.entityContainers;

    public IEnumerable<CsdlAnnotations> OutOfLineAnnotations => (IEnumerable<CsdlAnnotations>) this.outOfLineAnnotations;

    public IEnumerable<CsdlTypeDefinition> TypeDefinitions => (IEnumerable<CsdlTypeDefinition>) this.typeDefinitions;

    public string Alias => this.alias;

    public string Namespace => this.namespaceName;

    public Version Version => this.version;
  }
}
