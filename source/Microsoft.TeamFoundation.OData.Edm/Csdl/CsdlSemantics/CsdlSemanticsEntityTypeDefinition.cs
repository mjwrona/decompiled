// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsEntityTypeDefinition
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsEntityTypeDefinition : 
    CsdlSemanticsStructuredTypeDefinition,
    IEdmEntityType,
    IEdmStructuredType,
    IEdmType,
    IEdmElement,
    IEdmSchemaType,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmVocabularyAnnotatable,
    IEdmFullNamedElement
  {
    private readonly CsdlEntityType entity;
    private readonly string fullName;
    private readonly Cache<CsdlSemanticsEntityTypeDefinition, IEdmEntityType> baseTypeCache = new Cache<CsdlSemanticsEntityTypeDefinition, IEdmEntityType>();
    private static readonly Func<CsdlSemanticsEntityTypeDefinition, IEdmEntityType> ComputeBaseTypeFunc = (Func<CsdlSemanticsEntityTypeDefinition, IEdmEntityType>) (me => me.ComputeBaseType());
    private static readonly Func<CsdlSemanticsEntityTypeDefinition, IEdmEntityType> OnCycleBaseTypeFunc = (Func<CsdlSemanticsEntityTypeDefinition, IEdmEntityType>) (me => (IEdmEntityType) new CyclicEntityType(me.GetCyclicBaseTypeName(me.entity.BaseTypeName), me.Location));
    private readonly Cache<CsdlSemanticsEntityTypeDefinition, IEnumerable<IEdmStructuralProperty>> declaredKeyCache = new Cache<CsdlSemanticsEntityTypeDefinition, IEnumerable<IEdmStructuralProperty>>();
    private static readonly Func<CsdlSemanticsEntityTypeDefinition, IEnumerable<IEdmStructuralProperty>> ComputeDeclaredKeyFunc = (Func<CsdlSemanticsEntityTypeDefinition, IEnumerable<IEdmStructuralProperty>>) (me => me.ComputeDeclaredKey());

    public CsdlSemanticsEntityTypeDefinition(CsdlSemanticsSchema context, CsdlEntityType entity)
      : base(context, (CsdlStructuredType) entity)
    {
      this.entity = entity;
      this.fullName = EdmUtil.GetFullNameForSchemaElement(context?.Namespace, this.entity?.Name);
    }

    public override IEdmStructuredType BaseType => (IEdmStructuredType) this.baseTypeCache.GetValue(this, CsdlSemanticsEntityTypeDefinition.ComputeBaseTypeFunc, CsdlSemanticsEntityTypeDefinition.OnCycleBaseTypeFunc);

    public override EdmTypeKind TypeKind => EdmTypeKind.Entity;

    public string Name => this.entity.Name;

    public string FullName => this.fullName;

    public override bool IsAbstract => this.entity.IsAbstract;

    public override bool IsOpen => this.entity.IsOpen;

    public bool HasStream => this.entity.HasStream;

    public IEnumerable<IEdmStructuralProperty> DeclaredKey => this.declaredKeyCache.GetValue(this, CsdlSemanticsEntityTypeDefinition.ComputeDeclaredKeyFunc, (Func<CsdlSemanticsEntityTypeDefinition, IEnumerable<IEdmStructuralProperty>>) null);

    protected override CsdlStructuredType MyStructured => (CsdlStructuredType) this.entity;

    [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "baseType2", Justification = "Value assignment is required by compiler.")]
    private IEdmEntityType ComputeBaseType()
    {
      if (this.entity.BaseTypeName == null)
        return (IEdmEntityType) null;
      if (this.Context.FindType(this.entity.BaseTypeName) is IEdmEntityType type)
      {
        IEdmStructuredType baseType = type.BaseType;
      }
      return type ?? (IEdmEntityType) new UnresolvedEntityType(this.Context.UnresolvedName(this.entity.BaseTypeName), this.Location);
    }

    private IEnumerable<IEdmStructuralProperty> ComputeDeclaredKey()
    {
      if (this.entity.Key == null)
        return (IEnumerable<IEdmStructuralProperty>) null;
      List<IEdmStructuralProperty> declaredKey = new List<IEdmStructuralProperty>();
      foreach (CsdlPropertyReference property1 in this.entity.Key.Properties)
      {
        CsdlPropertyReference keyProperty = property1;
        if (this.FindProperty(keyProperty.PropertyName) is IEdmStructuralProperty property2)
          declaredKey.Add(property2);
        else if (this.DeclaredProperties.FirstOrDefault<IEdmProperty>((Func<IEdmProperty, bool>) (p => p.Name == keyProperty.PropertyName)) is IEdmStructuralProperty structuralProperty)
          declaredKey.Add(structuralProperty);
        else
          declaredKey.Add((IEdmStructuralProperty) new UnresolvedProperty((IEdmStructuredType) this, keyProperty.PropertyName, this.Location));
      }
      return (IEnumerable<IEdmStructuralProperty>) declaredKey;
    }
  }
}
