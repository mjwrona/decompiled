// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsTypeDefinitionDefinition
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsTypeDefinitionDefinition : 
    CsdlSemanticsTypeDefinition,
    IEdmTypeDefinition,
    IEdmSchemaType,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable,
    IEdmType,
    IEdmFullNamedElement
  {
    private readonly CsdlSemanticsSchema context;
    private readonly CsdlTypeDefinition typeDefinition;
    private readonly string fullName;
    private readonly Cache<CsdlSemanticsTypeDefinitionDefinition, IEdmPrimitiveType> underlyingTypeCache = new Cache<CsdlSemanticsTypeDefinitionDefinition, IEdmPrimitiveType>();
    private static readonly Func<CsdlSemanticsTypeDefinitionDefinition, IEdmPrimitiveType> ComputeUnderlyingTypeFunc = (Func<CsdlSemanticsTypeDefinitionDefinition, IEdmPrimitiveType>) (me => me.ComputeUnderlyingType());

    public CsdlSemanticsTypeDefinitionDefinition(
      CsdlSemanticsSchema context,
      CsdlTypeDefinition typeDefinition)
      : base((CsdlElement) typeDefinition)
    {
      this.context = context;
      this.typeDefinition = typeDefinition;
      this.fullName = EdmUtil.GetFullNameForSchemaElement(this.context?.Namespace, this.typeDefinition?.Name);
    }

    IEdmPrimitiveType IEdmTypeDefinition.UnderlyingType => this.underlyingTypeCache.GetValue(this, CsdlSemanticsTypeDefinitionDefinition.ComputeUnderlyingTypeFunc, (Func<CsdlSemanticsTypeDefinitionDefinition, IEdmPrimitiveType>) null);

    EdmSchemaElementKind IEdmSchemaElement.SchemaElementKind => EdmSchemaElementKind.TypeDefinition;

    public string Namespace => this.context.Namespace;

    string IEdmNamedElement.Name => this.typeDefinition.Name;

    public string FullName => this.fullName;

    public override EdmTypeKind TypeKind => EdmTypeKind.TypeDefinition;

    public override CsdlSemanticsModel Model => this.context.Model;

    public override CsdlElement Element => (CsdlElement) this.typeDefinition;

    protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations() => this.Model.WrapInlineVocabularyAnnotations((CsdlSemanticsElement) this, this.context);

    private IEdmPrimitiveType ComputeUnderlyingType()
    {
      if (this.typeDefinition.UnderlyingTypeName == null)
        return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32);
      EdmPrimitiveTypeKind primitiveTypeKind = EdmCoreModel.Instance.GetPrimitiveTypeKind(this.typeDefinition.UnderlyingTypeName);
      return primitiveTypeKind == EdmPrimitiveTypeKind.None ? (IEdmPrimitiveType) new UnresolvedPrimitiveType(this.typeDefinition.UnderlyingTypeName, this.Location) : EdmCoreModel.Instance.GetPrimitiveType(primitiveTypeKind);
    }
  }
}
