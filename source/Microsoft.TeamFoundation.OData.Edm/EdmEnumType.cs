// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmEnumType
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
  public class EdmEnumType : 
    EdmType,
    IEdmEnumType,
    IEdmSchemaType,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable,
    IEdmType,
    IEdmFullNamedElement
  {
    private readonly IEdmPrimitiveType underlyingType;
    private readonly string namespaceName;
    private readonly string name;
    private readonly string fullName;
    private readonly bool isFlags;
    private readonly List<IEdmEnumMember> members = new List<IEdmEnumMember>();

    public EdmEnumType(string namespaceName, string name)
      : this(namespaceName, name, false)
    {
    }

    public EdmEnumType(string namespaceName, string name, bool isFlags)
      : this(namespaceName, name, EdmPrimitiveTypeKind.Int32, isFlags)
    {
    }

    public EdmEnumType(
      string namespaceName,
      string name,
      EdmPrimitiveTypeKind underlyingType,
      bool isFlags)
      : this(namespaceName, name, EdmCoreModel.Instance.GetPrimitiveType(underlyingType), isFlags)
    {
    }

    public EdmEnumType(
      string namespaceName,
      string name,
      IEdmPrimitiveType underlyingType,
      bool isFlags)
    {
      EdmUtil.CheckArgumentNull<IEdmPrimitiveType>(underlyingType, nameof (underlyingType));
      EdmUtil.CheckArgumentNull<string>(namespaceName, nameof (namespaceName));
      EdmUtil.CheckArgumentNull<string>(name, nameof (name));
      this.underlyingType = underlyingType;
      this.name = name;
      this.namespaceName = namespaceName;
      this.isFlags = isFlags;
      this.fullName = EdmUtil.GetFullNameForSchemaElement(this.namespaceName, this.name);
    }

    public override EdmTypeKind TypeKind => EdmTypeKind.Enum;

    public EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.TypeDefinition;

    public string Namespace => this.namespaceName;

    public string Name => this.name;

    public string FullName => this.fullName;

    public IEdmPrimitiveType UnderlyingType => this.underlyingType;

    public virtual IEnumerable<IEdmEnumMember> Members => (IEnumerable<IEdmEnumMember>) this.members;

    public bool IsFlags => this.isFlags;

    public void AddMember(IEdmEnumMember member) => this.members.Add(member);

    public EdmEnumMember AddMember(string name, IEdmEnumMemberValue value)
    {
      EdmEnumMember member = new EdmEnumMember((IEdmEnumType) this, name, value);
      this.AddMember((IEdmEnumMember) member);
      return member;
    }
  }
}
