// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsEnumMember
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsEnumMember : 
    CsdlSemanticsElement,
    IEdmEnumMember,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable
  {
    private readonly CsdlEnumMember member;
    private readonly CsdlSemanticsEnumTypeDefinition declaringType;
    private readonly Cache<CsdlSemanticsEnumMember, IEdmEnumMemberValue> valueCache = new Cache<CsdlSemanticsEnumMember, IEdmEnumMemberValue>();
    private static readonly Func<CsdlSemanticsEnumMember, IEdmEnumMemberValue> ComputeValueFunc = (Func<CsdlSemanticsEnumMember, IEdmEnumMemberValue>) (me => me.ComputeValue());

    public CsdlSemanticsEnumMember(
      CsdlSemanticsEnumTypeDefinition declaringType,
      CsdlEnumMember member)
      : base((CsdlElement) member)
    {
      this.member = member;
      this.declaringType = declaringType;
    }

    public string Name => this.member.Name;

    public IEdmEnumType DeclaringType => (IEdmEnumType) this.declaringType;

    public IEdmEnumMemberValue Value => this.valueCache.GetValue(this, CsdlSemanticsEnumMember.ComputeValueFunc, (Func<CsdlSemanticsEnumMember, IEdmEnumMemberValue>) null);

    public override CsdlSemanticsModel Model => this.declaringType.Model;

    public override CsdlElement Element => (CsdlElement) this.member;

    protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations() => this.Model.WrapInlineVocabularyAnnotations((CsdlSemanticsElement) this, this.declaringType.Context);

    private IEdmEnumMemberValue ComputeValue()
    {
      if (this.member.Value.HasValue)
        return (IEdmEnumMemberValue) new EdmEnumMemberValue(this.member.Value.Value);
      return (IEdmEnumMemberValue) new BadEdmEnumMemberValue((IEnumerable<EdmError>) new EdmError[1]
      {
        new EdmError(this.member.Location ?? this.Location, EdmErrorCode.EnumMemberMustHaveValue, Strings.CsdlSemantics_EnumMemberMustHaveValue)
      });
    }
  }
}
