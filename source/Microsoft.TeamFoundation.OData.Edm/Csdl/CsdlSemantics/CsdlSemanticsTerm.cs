// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsTerm
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  [DebuggerDisplay("CsdlSemanticsTerm({Name})")]
  internal class CsdlSemanticsTerm : 
    CsdlSemanticsElement,
    IEdmTerm,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable,
    IEdmFullNamedElement
  {
    protected readonly CsdlSemanticsSchema Context;
    private readonly string fullName;
    protected CsdlTerm term;
    private readonly Cache<CsdlSemanticsTerm, IEdmTypeReference> typeCache = new Cache<CsdlSemanticsTerm, IEdmTypeReference>();
    private static readonly Func<CsdlSemanticsTerm, IEdmTypeReference> ComputeTypeFunc = (Func<CsdlSemanticsTerm, IEdmTypeReference>) (me => me.ComputeType());

    public CsdlSemanticsTerm(CsdlSemanticsSchema context, CsdlTerm valueTerm)
      : base((CsdlElement) valueTerm)
    {
      this.Context = context;
      this.term = valueTerm;
      this.fullName = EdmUtil.GetFullNameForSchemaElement(this.Context?.Namespace, this.term?.Name);
    }

    public string Name => this.term.Name;

    public string Namespace => this.Context.Namespace;

    public string FullName => this.fullName;

    public EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.Term;

    public IEdmTypeReference Type => this.typeCache.GetValue(this, CsdlSemanticsTerm.ComputeTypeFunc, (Func<CsdlSemanticsTerm, IEdmTypeReference>) null);

    public string AppliesTo => this.term.AppliesTo;

    public string DefaultValue => this.term.DefaultValue;

    public override CsdlSemanticsModel Model => this.Context.Model;

    public override CsdlElement Element => (CsdlElement) this.term;

    protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations() => this.Model.WrapInlineVocabularyAnnotations((CsdlSemanticsElement) this, this.Context);

    private IEdmTypeReference ComputeType() => CsdlSemanticsModel.WrapTypeReference(this.Context, this.term.Type);
  }
}
