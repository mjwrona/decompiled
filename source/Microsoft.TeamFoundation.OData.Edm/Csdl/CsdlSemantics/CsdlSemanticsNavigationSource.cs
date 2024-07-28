// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsNavigationSource
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
  internal abstract class CsdlSemanticsNavigationSource : 
    CsdlSemanticsElement,
    IEdmNavigationSource,
    IEdmNamedElement,
    IEdmElement
  {
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1304:NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "protected field in internal class.")]
    protected readonly CsdlAbstractNavigationSource navigationSource;
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1304:NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "protected field in internal class.")]
    protected readonly CsdlSemanticsEntityContainer container;
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1304:NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "protected field in internal class.")]
    protected readonly IEdmPathExpression path;
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1304:NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "protected field in internal class.")]
    protected readonly Cache<CsdlSemanticsNavigationSource, IEdmEntityType> typeCache = new Cache<CsdlSemanticsNavigationSource, IEdmEntityType>();
    protected static readonly Func<CsdlSemanticsNavigationSource, IEdmEntityType> ComputeElementTypeFunc = (Func<CsdlSemanticsNavigationSource, IEdmEntityType>) (me => me.ComputeElementType());
    private readonly Cache<CsdlSemanticsNavigationSource, IEnumerable<IEdmNavigationPropertyBinding>> navigationTargetsCache = new Cache<CsdlSemanticsNavigationSource, IEnumerable<IEdmNavigationPropertyBinding>>();
    private static readonly Func<CsdlSemanticsNavigationSource, IEnumerable<IEdmNavigationPropertyBinding>> ComputeNavigationTargetsFunc = (Func<CsdlSemanticsNavigationSource, IEnumerable<IEdmNavigationPropertyBinding>>) (me => me.ComputeNavigationTargets());
    private readonly Dictionary<IEdmNavigationProperty, IEdmContainedEntitySet> containedNavigationPropertyCache = new Dictionary<IEdmNavigationProperty, IEdmContainedEntitySet>();
    private readonly Dictionary<IEdmNavigationProperty, IEdmUnknownEntitySet> unknownNavigationPropertyCache = new Dictionary<IEdmNavigationProperty, IEdmUnknownEntitySet>();

    public CsdlSemanticsNavigationSource(
      CsdlSemanticsEntityContainer container,
      CsdlAbstractNavigationSource navigationSource)
      : base((CsdlElement) navigationSource)
    {
      this.container = container;
      this.navigationSource = navigationSource;
      this.path = (IEdmPathExpression) new EdmPathExpression(this.navigationSource.Name);
    }

    public override CsdlSemanticsModel Model => this.container.Model;

    public IEdmEntityContainer Container => (IEdmEntityContainer) this.container;

    public override CsdlElement Element => (CsdlElement) this.navigationSource;

    public string Name => this.navigationSource.Name;

    public IEdmPathExpression Path => this.path;

    public abstract IEdmType Type { get; }

    public abstract EdmContainerElementKind ContainerElementKind { get; }

    public IEnumerable<IEdmNavigationPropertyBinding> NavigationPropertyBindings => this.navigationTargetsCache.GetValue(this, CsdlSemanticsNavigationSource.ComputeNavigationTargetsFunc, (Func<CsdlSemanticsNavigationSource, IEnumerable<IEdmNavigationPropertyBinding>>) null);

    public IEdmNavigationSource FindNavigationTarget(
      IEdmNavigationProperty property,
      IEdmPathExpression bindingPath)
    {
      EdmUtil.CheckArgumentNull<IEdmNavigationProperty>(property, nameof (property));
      if (!property.ContainsTarget && bindingPath != null)
      {
        foreach (IEdmNavigationPropertyBinding navigationPropertyBinding in this.NavigationPropertyBindings)
        {
          if (navigationPropertyBinding.NavigationProperty == property && navigationPropertyBinding.Path.Path == bindingPath.Path)
            return navigationPropertyBinding.Target;
        }
      }
      else if (property.ContainsTarget)
        return (IEdmNavigationSource) EdmUtil.DictionaryGetOrUpdate<IEdmNavigationProperty, IEdmContainedEntitySet>((IDictionary<IEdmNavigationProperty, IEdmContainedEntitySet>) this.containedNavigationPropertyCache, property, (Func<IEdmNavigationProperty, IEdmContainedEntitySet>) (navProperty => (IEdmContainedEntitySet) new EdmContainedEntitySet((IEdmNavigationSource) this, navProperty)));
      return (IEdmNavigationSource) EdmUtil.DictionaryGetOrUpdate<IEdmNavigationProperty, IEdmUnknownEntitySet>((IDictionary<IEdmNavigationProperty, IEdmUnknownEntitySet>) this.unknownNavigationPropertyCache, property, (Func<IEdmNavigationProperty, IEdmUnknownEntitySet>) (navProperty => (IEdmUnknownEntitySet) new EdmUnknownEntitySet((IEdmNavigationSource) this, navProperty)));
    }

    public IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty)
    {
      EdmPathExpression edmPathExpression;
      if (this.Type.AsElementType().IsOrInheritsFrom((IEdmType) navigationProperty.DeclaringType))
        edmPathExpression = new EdmPathExpression(navigationProperty.Name);
      else
        edmPathExpression = new EdmPathExpression(new string[2]
        {
          navigationProperty.DeclaringType.FullTypeName(),
          navigationProperty.Name
        });
      IEdmPathExpression bindingPath = (IEdmPathExpression) edmPathExpression;
      return this.FindNavigationTarget(navigationProperty, bindingPath);
    }

    public IEnumerable<IEdmNavigationPropertyBinding> FindNavigationPropertyBindings(
      IEdmNavigationProperty navigationProperty)
    {
      return !navigationProperty.ContainsTarget ? (IEnumerable<IEdmNavigationPropertyBinding>) this.NavigationPropertyBindings.Where<IEdmNavigationPropertyBinding>((Func<IEdmNavigationPropertyBinding, bool>) (targetMapping => targetMapping.NavigationProperty == navigationProperty)).ToList<IEdmNavigationPropertyBinding>() : (IEnumerable<IEdmNavigationPropertyBinding>) null;
    }

    protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations() => this.Model.WrapInlineVocabularyAnnotations((CsdlSemanticsElement) this, this.container.Context);

    protected abstract IEdmEntityType ComputeElementType();

    private IEnumerable<IEdmNavigationPropertyBinding> ComputeNavigationTargets() => (IEnumerable<IEdmNavigationPropertyBinding>) this.navigationSource.NavigationPropertyBindings.Select<CsdlNavigationPropertyBinding, IEdmNavigationPropertyBinding>(new Func<CsdlNavigationPropertyBinding, IEdmNavigationPropertyBinding>(this.CreateSemanticMappingForBinding)).ToList<IEdmNavigationPropertyBinding>();

    private IEdmNavigationPropertyBinding CreateSemanticMappingForBinding(
      CsdlNavigationPropertyBinding binding)
    {
      return (IEdmNavigationPropertyBinding) new EdmNavigationPropertyBinding(this.ResolveNavigationPropertyPathForBinding(binding), this.Container.FindNavigationSourceExtended(binding.Target) ?? (IEdmNavigationSource) this.Container.FindSingletonExtended(binding.Target) ?? (IEdmNavigationSource) new UnresolvedEntitySet(binding.Target, this.Container, binding.Location), (IEdmPathExpression) new EdmPathExpression(binding.Path));
    }

    private IEdmNavigationProperty ResolveNavigationPropertyPathForBinding(
      CsdlNavigationPropertyBinding binding)
    {
      string[] source = binding.Path.Split('/');
      edmStructuredType = (IEdmStructuredType) this.typeCache.GetValue(this, CsdlSemanticsNavigationSource.ComputeElementTypeFunc, (Func<CsdlSemanticsNavigationSource, IEdmEntityType>) null);
      for (int index = 0; index < source.Length - 1; ++index)
      {
        string name = source[index];
        if (name.IndexOf('.') < 0)
        {
          IEdmProperty property = edmStructuredType.FindProperty(name);
          if (property == null)
            return (IEdmNavigationProperty) new UnresolvedNavigationPropertyPath(edmStructuredType, binding.Path, binding.Location);
          if (property is IEdmNavigationProperty navigationProperty && !navigationProperty.ContainsTarget)
            return (IEdmNavigationProperty) new UnresolvedNavigationPropertyPath(edmStructuredType, binding.Path, binding.Location);
          if (!(property.Type.Definition.AsElementType() is IEdmStructuredType edmStructuredType))
            return (IEdmNavigationProperty) new UnresolvedNavigationPropertyPath(edmStructuredType, binding.Path, binding.Location);
        }
        else
        {
          if (!(this.container.Context.FindType(name) is IEdmStructuredType type) || !type.IsOrInheritsFrom((IEdmType) edmStructuredType))
            return (IEdmNavigationProperty) new UnresolvedNavigationPropertyPath(edmStructuredType, binding.Path, binding.Location);
          edmStructuredType = type;
        }
      }
      return edmStructuredType.FindProperty(((IEnumerable<string>) source).Last<string>()) is IEdmNavigationProperty property1 ? property1 : (IEdmNavigationProperty) new UnresolvedNavigationPropertyPath(edmStructuredType, binding.Path, binding.Location);
    }
  }
}
