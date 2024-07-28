// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.UnresolvedVocabularyTerm
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class UnresolvedVocabularyTerm : 
    EdmElement,
    IEdmTerm,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable,
    IUnresolvedElement,
    IEdmFullNamedElement
  {
    private readonly UnresolvedVocabularyTerm.UnresolvedTermTypeReference type = new UnresolvedVocabularyTerm.UnresolvedTermTypeReference();
    private readonly string namespaceName;
    private readonly string name;
    private readonly string fullName;
    private readonly string appliesTo;
    private readonly string defaultValue;

    public UnresolvedVocabularyTerm(string qualifiedName)
    {
      qualifiedName = qualifiedName ?? string.Empty;
      EdmUtil.TryGetNamespaceNameFromQualifiedName(qualifiedName, out this.namespaceName, out this.name, out this.fullName);
    }

    public string Namespace => this.namespaceName;

    public string Name => this.name;

    public string FullName => this.fullName;

    public EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.Term;

    public IEdmTypeReference Type => (IEdmTypeReference) this.type;

    public string AppliesTo => this.appliesTo;

    public string DefaultValue => this.defaultValue;

    private class UnresolvedTermTypeReference : IEdmTypeReference, IEdmElement
    {
      private readonly UnresolvedVocabularyTerm.UnresolvedTermTypeReference.UnresolvedTermType definition = new UnresolvedVocabularyTerm.UnresolvedTermTypeReference.UnresolvedTermType();

      public bool IsNullable => false;

      public IEdmType Definition => (IEdmType) this.definition;

      private class UnresolvedTermType : IEdmType, IEdmElement
      {
        public EdmTypeKind TypeKind => EdmTypeKind.None;
      }
    }
  }
}
