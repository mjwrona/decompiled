// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsEntityReferenceTypeDefinition
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using System;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsEntityReferenceTypeDefinition : 
    CsdlSemanticsTypeDefinition,
    IEdmEntityReferenceType,
    IEdmType,
    IEdmElement
  {
    private readonly CsdlSemanticsSchema schema;
    private readonly Cache<CsdlSemanticsEntityReferenceTypeDefinition, IEdmEntityType> entityTypeCache = new Cache<CsdlSemanticsEntityReferenceTypeDefinition, IEdmEntityType>();
    private static readonly Func<CsdlSemanticsEntityReferenceTypeDefinition, IEdmEntityType> ComputeEntityTypeFunc = (Func<CsdlSemanticsEntityReferenceTypeDefinition, IEdmEntityType>) (me => me.ComputeEntityType());
    private readonly CsdlEntityReferenceType entityTypeReference;

    public CsdlSemanticsEntityReferenceTypeDefinition(
      CsdlSemanticsSchema schema,
      CsdlEntityReferenceType entityTypeReference)
      : base((CsdlElement) entityTypeReference)
    {
      this.schema = schema;
      this.entityTypeReference = entityTypeReference;
    }

    public override EdmTypeKind TypeKind => EdmTypeKind.EntityReference;

    public IEdmEntityType EntityType => this.entityTypeCache.GetValue(this, CsdlSemanticsEntityReferenceTypeDefinition.ComputeEntityTypeFunc, (Func<CsdlSemanticsEntityReferenceTypeDefinition, IEdmEntityType>) null);

    public override CsdlElement Element => (CsdlElement) this.entityTypeReference;

    public override CsdlSemanticsModel Model => this.schema.Model;

    private IEdmEntityType ComputeEntityType()
    {
      IEdmTypeReference type = CsdlSemanticsModel.WrapTypeReference(this.schema, this.entityTypeReference.EntityType);
      return type.TypeKind() != EdmTypeKind.Entity ? (IEdmEntityType) new UnresolvedEntityType(this.schema.UnresolvedName(type.FullName()), this.Location) : type.AsEntity().EntityDefinition();
    }
  }
}
