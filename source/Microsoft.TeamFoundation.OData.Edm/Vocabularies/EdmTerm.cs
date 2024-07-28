// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmTerm
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmTerm : 
    EdmNamedElement,
    IEdmTerm,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable,
    IEdmFullNamedElement
  {
    private readonly string namespaceName;
    private readonly string fullName;
    private readonly IEdmTypeReference type;
    private readonly string appliesTo;
    private readonly string defaultValue;

    public EdmTerm(string namespaceName, string name, EdmPrimitiveTypeKind type)
      : this(namespaceName, name, type, (string) null)
    {
    }

    public EdmTerm(string namespaceName, string name, EdmPrimitiveTypeKind type, string appliesTo)
      : this(namespaceName, name, (IEdmTypeReference) EdmCoreModel.Instance.GetPrimitive(type, true), appliesTo)
    {
    }

    public EdmTerm(string namespaceName, string name, IEdmTypeReference type)
      : this(namespaceName, name, type, (string) null)
    {
    }

    public EdmTerm(string namespaceName, string name, IEdmTypeReference type, string appliesTo)
      : this(namespaceName, name, type, appliesTo, (string) null)
    {
    }

    public EdmTerm(
      string namespaceName,
      string name,
      IEdmTypeReference type,
      string appliesTo,
      string defaultValue)
      : base(name)
    {
      EdmUtil.CheckArgumentNull<string>(namespaceName, nameof (namespaceName));
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      this.namespaceName = namespaceName;
      this.type = type;
      this.appliesTo = appliesTo;
      this.defaultValue = defaultValue;
      this.fullName = EdmUtil.GetFullNameForSchemaElement(this.namespaceName, this.Name);
    }

    public string Namespace => this.namespaceName;

    public string FullName => this.fullName;

    public IEdmTypeReference Type => this.type;

    public string AppliesTo => this.appliesTo;

    public string DefaultValue => this.defaultValue;

    public EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.Term;
  }
}
