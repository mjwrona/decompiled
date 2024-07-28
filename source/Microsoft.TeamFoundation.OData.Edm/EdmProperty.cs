// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmProperty
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
  public abstract class EdmProperty : 
    EdmNamedElement,
    IEdmProperty,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable
  {
    private readonly IEdmStructuredType declaringType;
    private readonly IEdmTypeReference type;

    protected EdmProperty(IEdmStructuredType declaringType, string name, IEdmTypeReference type)
      : base(name)
    {
      EdmUtil.CheckArgumentNull<IEdmStructuredType>(declaringType, nameof (declaringType));
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      this.declaringType = declaringType;
      this.type = type;
    }

    public IEdmTypeReference Type => this.type;

    public IEdmStructuredType DeclaringType => this.declaringType;

    public abstract EdmPropertyKind PropertyKind { get; }
  }
}
