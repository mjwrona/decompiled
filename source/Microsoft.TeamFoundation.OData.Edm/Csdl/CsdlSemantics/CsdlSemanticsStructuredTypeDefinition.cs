// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsStructuredTypeDefinition
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal abstract class CsdlSemanticsStructuredTypeDefinition : 
    CsdlSemanticsTypeDefinition,
    IEdmStructuredType,
    IEdmType,
    IEdmElement
  {
    private readonly CsdlSemanticsSchema context;
    private readonly Cache<CsdlSemanticsStructuredTypeDefinition, List<IEdmProperty>> declaredPropertiesCache = new Cache<CsdlSemanticsStructuredTypeDefinition, List<IEdmProperty>>();
    private static readonly Func<CsdlSemanticsStructuredTypeDefinition, List<IEdmProperty>> ComputeDeclaredPropertiesFunc = (Func<CsdlSemanticsStructuredTypeDefinition, List<IEdmProperty>>) (me => me.ComputeDeclaredProperties());
    private readonly Cache<CsdlSemanticsStructuredTypeDefinition, IDictionary<string, IEdmProperty>> propertiesDictionaryCache = new Cache<CsdlSemanticsStructuredTypeDefinition, IDictionary<string, IEdmProperty>>();
    private static readonly Func<CsdlSemanticsStructuredTypeDefinition, IDictionary<string, IEdmProperty>> ComputePropertiesDictionaryFunc = (Func<CsdlSemanticsStructuredTypeDefinition, IDictionary<string, IEdmProperty>>) (me => me.ComputePropertiesDictionary());

    protected CsdlSemanticsStructuredTypeDefinition(
      CsdlSemanticsSchema context,
      CsdlStructuredType type)
      : base((CsdlElement) type)
    {
      this.context = context;
    }

    public virtual bool IsAbstract => false;

    public virtual bool IsOpen => false;

    public abstract IEdmStructuredType BaseType { get; }

    public override CsdlElement Element => (CsdlElement) this.MyStructured;

    public override CsdlSemanticsModel Model => this.context.Model;

    public string Namespace => this.context.Namespace;

    public CsdlSemanticsSchema Context => this.context;

    public IEnumerable<IEdmProperty> DeclaredProperties => (IEnumerable<IEdmProperty>) this.declaredPropertiesCache.GetValue(this, CsdlSemanticsStructuredTypeDefinition.ComputeDeclaredPropertiesFunc, (Func<CsdlSemanticsStructuredTypeDefinition, List<IEdmProperty>>) null);

    protected abstract CsdlStructuredType MyStructured { get; }

    private IDictionary<string, IEdmProperty> PropertiesDictionary => this.propertiesDictionaryCache.GetValue(this, CsdlSemanticsStructuredTypeDefinition.ComputePropertiesDictionaryFunc, (Func<CsdlSemanticsStructuredTypeDefinition, IDictionary<string, IEdmProperty>>) null);

    public IEdmProperty FindProperty(string name)
    {
      IEdmProperty property;
      this.PropertiesDictionary.TryGetValue(name, out property);
      return property;
    }

    protected List<IEdmProperty> ComputeDeclaredProperties()
    {
      List<IEdmProperty> declaredProperties = new List<IEdmProperty>();
      foreach (CsdlProperty structuralProperty in this.MyStructured.StructuralProperties)
        declaredProperties.Add((IEdmProperty) new CsdlSemanticsProperty(this, structuralProperty));
      foreach (CsdlNavigationProperty navigationProperty in this.MyStructured.NavigationProperties)
        declaredProperties.Add((IEdmProperty) new CsdlSemanticsNavigationProperty(this, navigationProperty));
      return declaredProperties;
    }

    protected string GetCyclicBaseTypeName(string baseTypeName)
    {
      IEdmSchemaType type = this.context.FindType(baseTypeName);
      return type == null ? baseTypeName : type.FullName();
    }

    protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations() => this.Model.WrapInlineVocabularyAnnotations((CsdlSemanticsElement) this, this.context);

    private IDictionary<string, IEdmProperty> ComputePropertiesDictionary()
    {
      Dictionary<string, IEdmProperty> dictionary = new Dictionary<string, IEdmProperty>();
      foreach (IEdmProperty property in this.Properties())
        RegistrationHelper.RegisterProperty(property, property.Name, dictionary);
      return (IDictionary<string, IEdmProperty>) dictionary;
    }
  }
}
