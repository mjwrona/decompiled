// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmEnumMember
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
  public class EdmEnumMember : 
    EdmNamedElement,
    IEdmEnumMember,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable
  {
    private readonly IEdmEnumType declaringType;
    private IEdmEnumMemberValue value;

    public EdmEnumMember(IEdmEnumType declaringType, string name, IEdmEnumMemberValue value)
      : base(name)
    {
      EdmUtil.CheckArgumentNull<IEdmEnumType>(declaringType, nameof (declaringType));
      EdmUtil.CheckArgumentNull<IEdmEnumMemberValue>(value, nameof (value));
      this.declaringType = declaringType;
      this.value = value;
    }

    public IEdmEnumType DeclaringType => this.declaringType;

    public IEdmEnumMemberValue Value => this.value;
  }
}
