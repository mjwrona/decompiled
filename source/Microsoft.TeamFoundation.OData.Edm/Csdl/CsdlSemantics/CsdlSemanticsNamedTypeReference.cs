// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsNamedTypeReference
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using System;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsNamedTypeReference : 
    CsdlSemanticsElement,
    IEdmTypeReference,
    IEdmElement
  {
    private readonly CsdlSemanticsSchema schema;
    private readonly CsdlNamedTypeReference reference;
    private readonly Cache<CsdlSemanticsNamedTypeReference, IEdmType> definitionCache = new Cache<CsdlSemanticsNamedTypeReference, IEdmType>();
    private static readonly Func<CsdlSemanticsNamedTypeReference, IEdmType> ComputeDefinitionFunc = (Func<CsdlSemanticsNamedTypeReference, IEdmType>) (me => me.ComputeDefinition());

    public CsdlSemanticsNamedTypeReference(
      CsdlSemanticsSchema schema,
      CsdlNamedTypeReference reference)
      : base((CsdlElement) reference)
    {
      this.schema = schema;
      this.reference = reference;
    }

    public IEdmType Definition => this.definitionCache.GetValue(this, CsdlSemanticsNamedTypeReference.ComputeDefinitionFunc, (Func<CsdlSemanticsNamedTypeReference, IEdmType>) null);

    public bool IsNullable => this.reference.IsNullable;

    public override CsdlSemanticsModel Model => this.schema.Model;

    public override CsdlElement Element => (CsdlElement) this.reference;

    public override string ToString() => this.ToTraceString();

    private IEdmType ComputeDefinition() => (IEdmType) this.schema.FindType(this.reference.FullName) ?? (IEdmType) new UnresolvedType(this.schema.ReplaceAlias(this.reference.FullName), this.Location);
  }
}
