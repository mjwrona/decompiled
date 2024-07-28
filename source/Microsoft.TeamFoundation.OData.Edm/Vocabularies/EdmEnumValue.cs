// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmEnumValue
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmEnumValue : EdmValue, IEdmEnumValue, IEdmPrimitiveValue, IEdmValue, IEdmElement
  {
    private readonly IEdmEnumMemberValue value;

    public EdmEnumValue(IEdmEnumTypeReference type, IEdmEnumMember member)
      : this(type, member.Value)
    {
    }

    public EdmEnumValue(IEdmEnumTypeReference type, IEdmEnumMemberValue value)
      : base((IEdmTypeReference) type)
    {
      this.value = value;
    }

    public IEdmEnumMemberValue Value => this.value;

    public override EdmValueKind ValueKind => EdmValueKind.Enum;
  }
}
