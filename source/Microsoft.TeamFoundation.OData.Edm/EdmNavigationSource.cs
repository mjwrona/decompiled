// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmNavigationSource
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
{
  public abstract class EdmNavigationSource : 
    EdmNamedElement,
    IEdmNavigationSource,
    IEdmNamedElement,
    IEdmElement
  {
    private readonly Dictionary<IEdmNavigationProperty, Dictionary<string, IEdmNavigationPropertyBinding>> navigationPropertyMappings = new Dictionary<IEdmNavigationProperty, Dictionary<string, IEdmNavigationPropertyBinding>>();
    private readonly Dictionary<IEdmNavigationProperty, IEdmUnknownEntitySet> unknownNavigationPropertyCache = new Dictionary<IEdmNavigationProperty, IEdmUnknownEntitySet>();
    private readonly Cache<EdmNavigationSource, IEnumerable<IEdmNavigationPropertyBinding>> navigationTargetsCache = new Cache<EdmNavigationSource, IEnumerable<IEdmNavigationPropertyBinding>>();
    private static readonly Func<EdmNavigationSource, IEnumerable<IEdmNavigationPropertyBinding>> ComputeNavigationTargetsFunc = (Func<EdmNavigationSource, IEnumerable<IEdmNavigationPropertyBinding>>) (me => me.ComputeNavigationTargets());

    protected EdmNavigationSource(string name)
      : base(name)
    {
    }

    public IEnumerable<IEdmNavigationPropertyBinding> NavigationPropertyBindings => this.navigationTargetsCache.GetValue(this, EdmNavigationSource.ComputeNavigationTargetsFunc, (Func<EdmNavigationSource, IEnumerable<IEdmNavigationPropertyBinding>>) null);

    public abstract IEdmType Type { get; }

    public abstract IEdmPathExpression Path { get; }

    public void AddNavigationTarget(
      IEdmNavigationProperty navigationProperty,
      IEdmNavigationSource target)
    {
      EdmUtil.CheckArgumentNull<IEdmNavigationProperty>(navigationProperty, nameof (navigationProperty));
      EdmUtil.CheckArgumentNull<IEdmNavigationSource>(target, "navigation target");
      if (navigationProperty.ContainsTarget)
        return;
      string path = navigationProperty.Name;
      if (!this.Type.AsElementType().IsOrInheritsFrom((IEdmType) navigationProperty.DeclaringType))
        path = navigationProperty.DeclaringType.FullTypeName() + "/" + path;
      this.AddNavigationPropertyBinding(navigationProperty, target, (IEdmPathExpression) new EdmPathExpression(path));
      this.navigationTargetsCache.Clear((Func<EdmNavigationSource, IEnumerable<IEdmNavigationPropertyBinding>>) null);
    }

    public void AddNavigationTarget(
      IEdmNavigationProperty navigationProperty,
      IEdmNavigationSource target,
      IEdmPathExpression bindingPath)
    {
      EdmUtil.CheckArgumentNull<IEdmNavigationProperty>(navigationProperty, nameof (navigationProperty));
      EdmUtil.CheckArgumentNull<IEdmNavigationSource>(target, "navigation target");
      EdmUtil.CheckArgumentNull<IEdmPathExpression>(bindingPath, "binding path");
      if (navigationProperty.ContainsTarget)
        return;
      if (navigationProperty.Name != bindingPath.PathSegments.Last<string>())
        throw new ArgumentException(Strings.NavigationPropertyBinding_PathIsNotValid);
      this.AddNavigationPropertyBinding(navigationProperty, target, bindingPath);
      this.navigationTargetsCache.Clear((Func<EdmNavigationSource, IEnumerable<IEdmNavigationPropertyBinding>>) null);
    }

    public virtual IEnumerable<IEdmNavigationPropertyBinding> FindNavigationPropertyBindings(
      IEdmNavigationProperty navigationProperty)
    {
      EdmUtil.CheckArgumentNull<IEdmNavigationProperty>(navigationProperty, nameof (navigationProperty));
      IDictionary<string, IEdmNavigationPropertyBinding> source = (IDictionary<string, IEdmNavigationPropertyBinding>) EdmUtil.DictionarySafeGet<IEdmNavigationProperty, Dictionary<string, IEdmNavigationPropertyBinding>>((IDictionary<IEdmNavigationProperty, Dictionary<string, IEdmNavigationPropertyBinding>>) this.navigationPropertyMappings, navigationProperty);
      return source != null ? source.Select<KeyValuePair<string, IEdmNavigationPropertyBinding>, IEdmNavigationPropertyBinding>((Func<KeyValuePair<string, IEdmNavigationPropertyBinding>, IEdmNavigationPropertyBinding>) (item => item.Value)) : (IEnumerable<IEdmNavigationPropertyBinding>) null;
    }

    public virtual IEdmNavigationSource FindNavigationTarget(
      IEdmNavigationProperty navigationProperty)
    {
      EdmUtil.CheckArgumentNull<IEdmNavigationProperty>(navigationProperty, "property");
      EdmPathExpression edmPathExpression;
      if (navigationProperty.DeclaringType.AsElementType() is IEdmComplexType || this.Type.AsElementType().IsOrInheritsFrom((IEdmType) navigationProperty.DeclaringType))
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

    public virtual IEdmNavigationSource FindNavigationTarget(
      IEdmNavigationProperty navigationProperty,
      IEdmPathExpression bindingPath)
    {
      EdmUtil.CheckArgumentNull<IEdmNavigationProperty>(navigationProperty, nameof (navigationProperty));
      bindingPath = bindingPath ?? (IEdmPathExpression) new EdmPathExpression(navigationProperty.Name);
      IDictionary<string, IEdmNavigationPropertyBinding> dictionary = (IDictionary<string, IEdmNavigationPropertyBinding>) EdmUtil.DictionarySafeGet<IEdmNavigationProperty, Dictionary<string, IEdmNavigationPropertyBinding>>((IDictionary<IEdmNavigationProperty, Dictionary<string, IEdmNavigationPropertyBinding>>) this.navigationPropertyMappings, navigationProperty);
      if (dictionary != null)
      {
        IEdmNavigationPropertyBinding navigationPropertyBinding = EdmUtil.DictionarySafeGet<string, IEdmNavigationPropertyBinding>(dictionary, bindingPath.Path);
        if (navigationPropertyBinding != null)
          return navigationPropertyBinding.Target;
      }
      return navigationProperty.ContainsTarget ? this.AddNavigationPropertyBinding(navigationProperty, (IEdmNavigationSource) new EdmContainedEntitySet((IEdmNavigationSource) this, navigationProperty, bindingPath), bindingPath).Target : (IEdmNavigationSource) EdmUtil.DictionaryGetOrUpdate<IEdmNavigationProperty, IEdmUnknownEntitySet>((IDictionary<IEdmNavigationProperty, IEdmUnknownEntitySet>) this.unknownNavigationPropertyCache, navigationProperty, (Func<IEdmNavigationProperty, IEdmUnknownEntitySet>) (navProperty => (IEdmUnknownEntitySet) new EdmUnknownEntitySet((IEdmNavigationSource) this, navProperty)));
    }

    private IEdmNavigationPropertyBinding AddNavigationPropertyBinding(
      IEdmNavigationProperty navigationProperty,
      IEdmNavigationSource target,
      IEdmPathExpression bindingPath)
    {
      return EdmUtil.DictionaryGetOrUpdate<string, IEdmNavigationPropertyBinding>((IDictionary<string, IEdmNavigationPropertyBinding>) EdmUtil.DictionaryGetOrUpdate<IEdmNavigationProperty, Dictionary<string, IEdmNavigationPropertyBinding>>((IDictionary<IEdmNavigationProperty, Dictionary<string, IEdmNavigationPropertyBinding>>) this.navigationPropertyMappings, navigationProperty, (Func<IEdmNavigationProperty, Dictionary<string, IEdmNavigationPropertyBinding>>) (navProperty => new Dictionary<string, IEdmNavigationPropertyBinding>())), bindingPath.Path, (Func<string, IEdmNavigationPropertyBinding>) (path => (IEdmNavigationPropertyBinding) new EdmNavigationPropertyBinding(navigationProperty, target, (IEdmPathExpression) new EdmPathExpression(path))));
    }

    private IEnumerable<IEdmNavigationPropertyBinding> ComputeNavigationTargets()
    {
      List<IEdmNavigationPropertyBinding> navigationTargets = new List<IEdmNavigationPropertyBinding>();
      lock (this.navigationPropertyMappings)
      {
        foreach (KeyValuePair<IEdmNavigationProperty, Dictionary<string, IEdmNavigationPropertyBinding>> navigationPropertyMapping in this.navigationPropertyMappings)
        {
          if (!navigationPropertyMapping.Key.ContainsTarget)
          {
            foreach (KeyValuePair<string, IEdmNavigationPropertyBinding> keyValuePair in navigationPropertyMapping.Value)
              navigationTargets.Add(keyValuePair.Value);
          }
        }
      }
      return (IEnumerable<IEdmNavigationPropertyBinding>) navigationTargets;
    }
  }
}
