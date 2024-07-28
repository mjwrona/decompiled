// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsComplexTypeDefinition
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsComplexTypeDefinition : 
    CsdlSemanticsStructuredTypeDefinition,
    IEdmComplexType,
    IEdmStructuredType,
    IEdmType,
    IEdmElement,
    IEdmSchemaType,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmVocabularyAnnotatable,
    IEdmFullNamedElement
  {
    private readonly string fullName;
    private readonly CsdlComplexType complex;
    private readonly Cache<CsdlSemanticsComplexTypeDefinition, IEdmComplexType> baseTypeCache = new Cache<CsdlSemanticsComplexTypeDefinition, IEdmComplexType>();
    private static readonly Func<CsdlSemanticsComplexTypeDefinition, IEdmComplexType> ComputeBaseTypeFunc = (Func<CsdlSemanticsComplexTypeDefinition, IEdmComplexType>) (me => me.ComputeBaseType());
    private static readonly Func<CsdlSemanticsComplexTypeDefinition, IEdmComplexType> OnCycleBaseTypeFunc = (Func<CsdlSemanticsComplexTypeDefinition, IEdmComplexType>) (me => (IEdmComplexType) new CyclicComplexType(me.GetCyclicBaseTypeName(me.complex.BaseTypeName), me.Location));

    public CsdlSemanticsComplexTypeDefinition(CsdlSemanticsSchema context, CsdlComplexType complex)
      : base(context, (CsdlStructuredType) complex)
    {
      this.complex = complex;
      this.fullName = EdmUtil.GetFullNameForSchemaElement(context?.Namespace, this.complex?.Name);
    }

    public override IEdmStructuredType BaseType => (IEdmStructuredType) this.baseTypeCache.GetValue(this, CsdlSemanticsComplexTypeDefinition.ComputeBaseTypeFunc, CsdlSemanticsComplexTypeDefinition.OnCycleBaseTypeFunc);

    public override EdmTypeKind TypeKind => EdmTypeKind.Complex;

    public override bool IsAbstract => this.complex.IsAbstract;

    public override bool IsOpen => this.complex.IsOpen;

    public string Name => this.complex.Name;

    public string FullName => this.fullName;

    protected override CsdlStructuredType MyStructured => (CsdlStructuredType) this.complex;

    [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "baseType2", Justification = "Value assignment is required by compiler.")]
    private IEdmComplexType ComputeBaseType()
    {
      if (this.complex.BaseTypeName == null)
        return (IEdmComplexType) null;
      if (this.Context.FindType(this.complex.BaseTypeName) is IEdmComplexType type)
      {
        IEdmStructuredType baseType = type.BaseType;
      }
      return type ?? (IEdmComplexType) new UnresolvedComplexType(this.Context.UnresolvedName(this.complex.BaseTypeName), this.Location);
    }
  }
}
