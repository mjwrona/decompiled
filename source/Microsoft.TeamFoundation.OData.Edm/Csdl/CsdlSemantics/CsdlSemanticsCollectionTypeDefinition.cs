// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsCollectionTypeDefinition
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using System;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsCollectionTypeDefinition : 
    CsdlSemanticsTypeDefinition,
    IEdmCollectionType,
    IEdmType,
    IEdmElement
  {
    private readonly CsdlSemanticsSchema schema;
    private readonly CsdlCollectionType collection;
    private readonly Cache<CsdlSemanticsCollectionTypeDefinition, IEdmTypeReference> elementTypeCache = new Cache<CsdlSemanticsCollectionTypeDefinition, IEdmTypeReference>();
    private static readonly Func<CsdlSemanticsCollectionTypeDefinition, IEdmTypeReference> ComputeElementTypeFunc = (Func<CsdlSemanticsCollectionTypeDefinition, IEdmTypeReference>) (me => me.ComputeElementType());

    public CsdlSemanticsCollectionTypeDefinition(
      CsdlSemanticsSchema schema,
      CsdlCollectionType collection)
      : base((CsdlElement) collection)
    {
      this.collection = collection;
      this.schema = schema;
    }

    public override EdmTypeKind TypeKind => EdmTypeKind.Collection;

    public IEdmTypeReference ElementType => this.elementTypeCache.GetValue(this, CsdlSemanticsCollectionTypeDefinition.ComputeElementTypeFunc, (Func<CsdlSemanticsCollectionTypeDefinition, IEdmTypeReference>) null);

    public override CsdlSemanticsModel Model => this.schema.Model;

    public override CsdlElement Element => (CsdlElement) this.collection;

    private IEdmTypeReference ComputeElementType() => CsdlSemanticsModel.WrapTypeReference(this.schema, this.collection.ElementType);
  }
}
