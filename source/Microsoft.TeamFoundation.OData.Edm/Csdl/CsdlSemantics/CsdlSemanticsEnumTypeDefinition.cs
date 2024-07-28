// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsEnumTypeDefinition
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsEnumTypeDefinition : 
    CsdlSemanticsTypeDefinition,
    IEdmEnumType,
    IEdmSchemaType,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable,
    IEdmType,
    IEdmFullNamedElement
  {
    private readonly string fullName;
    private readonly CsdlEnumType enumeration;
    private readonly Cache<CsdlSemanticsEnumTypeDefinition, IEdmPrimitiveType> underlyingTypeCache = new Cache<CsdlSemanticsEnumTypeDefinition, IEdmPrimitiveType>();
    private static readonly Func<CsdlSemanticsEnumTypeDefinition, IEdmPrimitiveType> ComputeUnderlyingTypeFunc = (Func<CsdlSemanticsEnumTypeDefinition, IEdmPrimitiveType>) (me => me.ComputeUnderlyingType());
    private readonly Cache<CsdlSemanticsEnumTypeDefinition, IEnumerable<IEdmEnumMember>> membersCache = new Cache<CsdlSemanticsEnumTypeDefinition, IEnumerable<IEdmEnumMember>>();
    private static readonly Func<CsdlSemanticsEnumTypeDefinition, IEnumerable<IEdmEnumMember>> ComputeMembersFunc = (Func<CsdlSemanticsEnumTypeDefinition, IEnumerable<IEdmEnumMember>>) (me => me.ComputeMembers());

    public CsdlSemanticsEnumTypeDefinition(CsdlSemanticsSchema context, CsdlEnumType enumeration)
      : base((CsdlElement) enumeration)
    {
      this.Context = context;
      this.enumeration = enumeration;
      this.fullName = EdmUtil.GetFullNameForSchemaElement(this.Context?.Namespace, this.enumeration?.Name);
    }

    IEdmPrimitiveType IEdmEnumType.UnderlyingType => this.underlyingTypeCache.GetValue(this, CsdlSemanticsEnumTypeDefinition.ComputeUnderlyingTypeFunc, (Func<CsdlSemanticsEnumTypeDefinition, IEdmPrimitiveType>) null);

    public IEnumerable<IEdmEnumMember> Members => this.membersCache.GetValue(this, CsdlSemanticsEnumTypeDefinition.ComputeMembersFunc, (Func<CsdlSemanticsEnumTypeDefinition, IEnumerable<IEdmEnumMember>>) null);

    bool IEdmEnumType.IsFlags => this.enumeration.IsFlags;

    EdmSchemaElementKind IEdmSchemaElement.SchemaElementKind => EdmSchemaElementKind.TypeDefinition;

    public string Namespace => this.Context.Namespace;

    public string FullName => this.fullName;

    string IEdmNamedElement.Name => this.enumeration.Name;

    public override EdmTypeKind TypeKind => EdmTypeKind.Enum;

    public override CsdlSemanticsModel Model => this.Context.Model;

    public override CsdlElement Element => (CsdlElement) this.enumeration;

    public CsdlSemanticsSchema Context { get; private set; }

    protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations() => this.Model.WrapInlineVocabularyAnnotations((CsdlSemanticsElement) this, this.Context);

    private IEdmPrimitiveType ComputeUnderlyingType()
    {
      if (this.enumeration.UnderlyingTypeName == null)
        return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32);
      EdmPrimitiveTypeKind primitiveTypeKind = EdmCoreModel.Instance.GetPrimitiveTypeKind(this.enumeration.UnderlyingTypeName);
      return primitiveTypeKind == EdmPrimitiveTypeKind.None ? (IEdmPrimitiveType) new UnresolvedPrimitiveType(this.enumeration.UnderlyingTypeName, this.Location) : EdmCoreModel.Instance.GetPrimitiveType(primitiveTypeKind);
    }

    private IEnumerable<IEdmEnumMember> ComputeMembers()
    {
      List<IEdmEnumMember> members = new List<IEdmEnumMember>();
      long num = -1;
      foreach (CsdlEnumMember member1 in this.enumeration.Members)
      {
        long? nullable = new long?();
        IEdmEnumMember member2;
        if (!member1.Value.HasValue)
        {
          if (num < long.MaxValue)
          {
            nullable = new long?(num + 1L);
            num = nullable.Value;
            member1.Value = nullable;
            member2 = (IEdmEnumMember) new CsdlSemanticsEnumMember(this, member1);
          }
          else
            member2 = (IEdmEnumMember) new CsdlSemanticsEnumMember(this, member1);
          member2.SetIsValueExplicit((IEdmModel) this.Model, new bool?(false));
        }
        else
        {
          num = member1.Value.Value;
          member2 = (IEdmEnumMember) new CsdlSemanticsEnumMember(this, member1);
          member2.SetIsValueExplicit((IEdmModel) this.Model, new bool?(true));
        }
        members.Add(member2);
      }
      return (IEnumerable<IEdmEnumMember>) members;
    }
  }
}
