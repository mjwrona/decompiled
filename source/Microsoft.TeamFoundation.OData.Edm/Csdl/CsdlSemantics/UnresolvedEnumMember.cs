// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.UnresolvedEnumMember
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class UnresolvedEnumMember : 
    BadElement,
    IEdmEnumMember,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable
  {
    private readonly string name;
    private readonly IEdmEnumType declaringType;
    private readonly Cache<UnresolvedEnumMember, IEdmEnumMemberValue> value = new Cache<UnresolvedEnumMember, IEdmEnumMemberValue>();
    private static readonly Func<UnresolvedEnumMember, IEdmEnumMemberValue> ComputeValueFunc = (Func<UnresolvedEnumMember, IEdmEnumMemberValue>) (me => UnresolvedEnumMember.ComputeValue());

    public UnresolvedEnumMember(string name, IEdmEnumType declaringType, EdmLocation location)
      : base((IEnumerable<EdmError>) new EdmError[1]
      {
        new EdmError(location, EdmErrorCode.BadUnresolvedEnumMember, Strings.Bad_UnresolvedEnumMember((object) name))
      })
    {
      this.name = name ?? string.Empty;
      this.declaringType = declaringType;
    }

    public string Name => this.name;

    public IEdmEnumMemberValue Value => this.value.GetValue(this, UnresolvedEnumMember.ComputeValueFunc, (Func<UnresolvedEnumMember, IEdmEnumMemberValue>) null);

    public IEdmEnumType DeclaringType => this.declaringType;

    private static IEdmEnumMemberValue ComputeValue() => (IEdmEnumMemberValue) new EdmEnumMemberValue(0L);
  }
}
