// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmContainedEntitySet
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
{
  internal class EdmContainedEntitySet : 
    EdmEntitySetBase,
    IEdmContainedEntitySet,
    IEdmEntitySetBase,
    IEdmNavigationSource,
    IEdmNamedElement,
    IEdmElement
  {
    private readonly IEdmPathExpression navigationPath;
    private readonly IEdmNavigationSource parentNavigationSource;
    private readonly IEdmNavigationProperty navigationProperty;
    private IEdmPathExpression path;
    private string fullPath;

    public EdmContainedEntitySet(
      IEdmNavigationSource parentNavigationSource,
      IEdmNavigationProperty navigationProperty)
      : this(parentNavigationSource, navigationProperty, (IEdmPathExpression) new EdmPathExpression(navigationProperty.Name))
    {
    }

    public EdmContainedEntitySet(
      IEdmNavigationSource parentNavigationSource,
      IEdmNavigationProperty navigationProperty,
      IEdmPathExpression navigationPath)
      : base(navigationProperty.Name, navigationProperty.ToEntityType())
    {
      EdmUtil.CheckArgumentNull<IEdmNavigationSource>(parentNavigationSource, nameof (parentNavigationSource));
      EdmUtil.CheckArgumentNull<IEdmNavigationProperty>(navigationProperty, nameof (navigationProperty));
      this.parentNavigationSource = parentNavigationSource;
      this.navigationProperty = navigationProperty;
      this.navigationPath = navigationPath;
    }

    public override IEdmPathExpression Path => this.path ?? (this.path = this.ComputePath());

    public IEdmNavigationSource ParentNavigationSource => this.parentNavigationSource;

    public IEdmNavigationProperty NavigationProperty => this.navigationProperty;

    internal IEdmPathExpression NavigationPath => this.navigationPath;

    private string FullNavigationPath
    {
      get
      {
        if (this.fullPath == null)
        {
          List<string> pathSegments = new List<string>();
          for (EdmContainedEntitySet containedEntitySet = this; containedEntitySet != null; containedEntitySet = containedEntitySet.ParentNavigationSource as EdmContainedEntitySet)
            pathSegments.AddRange(containedEntitySet.NavigationPath.PathSegments);
          pathSegments.Reverse();
          this.fullPath = new EdmPathExpression((IEnumerable<string>) pathSegments).Path;
        }
        return this.fullPath;
      }
    }

    public override IEnumerable<IEdmNavigationPropertyBinding> FindNavigationPropertyBindings(
      IEdmNavigationProperty navigationProperty)
    {
      IEnumerable<IEdmNavigationPropertyBinding> first = base.FindNavigationPropertyBindings(navigationProperty);
      IEdmNavigationSource navigationSource;
      for (IEdmContainedEntitySet containedEntitySet = (IEdmContainedEntitySet) this; containedEntitySet != null; containedEntitySet = navigationSource as IEdmContainedEntitySet)
      {
        navigationSource = containedEntitySet.ParentNavigationSource;
        IEnumerable<IEdmNavigationPropertyBinding> propertyBindings = navigationSource.FindNavigationPropertyBindings(navigationProperty);
        if (propertyBindings != null)
          first = first == null ? propertyBindings : first.Concat<IEdmNavigationPropertyBinding>(propertyBindings);
      }
      return first;
    }

    public override IEdmNavigationSource FindNavigationTarget(
      IEdmNavigationProperty navigationProperty)
    {
      return this.FindNavigationTarget(navigationProperty, (IEdmPathExpression) new EdmPathExpression(navigationProperty.Name));
    }

    public override IEdmNavigationSource FindNavigationTarget(
      IEdmNavigationProperty navigationProperty,
      IEdmPathExpression bindingPath)
    {
      if (bindingPath != null && bindingPath.Path.Length > this.FullNavigationPath.Length && bindingPath.Path.StartsWith(this.FullNavigationPath, StringComparison.Ordinal))
        bindingPath = (IEdmPathExpression) new EdmPathExpression(bindingPath.Path.Substring(this.FullNavigationPath.Length + 1));
      IEdmNavigationSource navigationTarget = base.FindNavigationTarget(navigationProperty, bindingPath);
      if (!(navigationTarget is IEdmUnknownEntitySet))
        return navigationTarget;
      IEnumerable<string> second;
      if (bindingPath != null && !string.IsNullOrEmpty(bindingPath.Path))
        second = bindingPath.PathSegments;
      else
        second = (IEnumerable<string>) new string[1]
        {
          navigationProperty.Name
        };
      bindingPath = (IEdmPathExpression) new EdmPathExpression(this.NavigationPath.PathSegments.Concat<string>(second));
      return this.parentNavigationSource.FindNavigationTarget(navigationProperty, bindingPath);
    }

    private IEdmPathExpression ComputePath()
    {
      IEdmType edmType = this.navigationProperty.DeclaringType.AsElementType();
      List<string> stringList = new List<string>(this.parentNavigationSource.Path.PathSegments);
      if (!(edmType is IEdmComplexType) && !this.parentNavigationSource.Type.AsElementType().IsOrInheritsFrom(edmType))
        stringList.Add(edmType.FullTypeName());
      stringList.AddRange(this.NavigationPath.PathSegments);
      return (IEdmPathExpression) new EdmPathExpression(stringList.ToArray());
    }
  }
}
