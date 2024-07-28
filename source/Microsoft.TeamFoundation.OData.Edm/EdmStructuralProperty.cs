// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmStructuralProperty
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData.Edm
{
  public class EdmStructuralProperty : 
    EdmProperty,
    IEdmStructuralProperty,
    IEdmProperty,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable
  {
    private readonly string defaultValueString;

    public EdmStructuralProperty(
      IEdmStructuredType declaringType,
      string name,
      IEdmTypeReference type)
      : this(declaringType, name, type, (string) null)
    {
    }

    [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "defaultValue might be confused for an IEdmValue.")]
    public EdmStructuralProperty(
      IEdmStructuredType declaringType,
      string name,
      IEdmTypeReference type,
      string defaultValueString)
      : base(declaringType, name, type)
    {
      this.defaultValueString = defaultValueString;
    }

    [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", Justification = "defaultValue might be confused for an IEdmValue.")]
    public string DefaultValueString => this.defaultValueString;

    public override EdmPropertyKind PropertyKind => EdmPropertyKind.Structural;
  }
}
