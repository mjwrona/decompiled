// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UnknownEntitySet
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System.Collections.Generic;

namespace Microsoft.OData
{
  internal class UnknownEntitySet : 
    IEdmUnknownEntitySet,
    IEdmEntitySetBase,
    IEdmNavigationSource,
    IEdmNamedElement,
    IEdmElement
  {
    private readonly IEdmNavigationProperty navigationProperty;
    private readonly IEdmNavigationSource parentNavigationSource;
    private IEdmPathExpression path;

    public UnknownEntitySet(
      IEdmNavigationSource parentNavigationSource,
      IEdmNavigationProperty navigationProperty)
    {
      this.parentNavigationSource = parentNavigationSource;
      this.navigationProperty = navigationProperty;
    }

    public string Name => this.navigationProperty.Name;

    public IEnumerable<IEdmNavigationPropertyBinding> NavigationPropertyBindings => (IEnumerable<IEdmNavigationPropertyBinding>) null;

    public IEdmPathExpression Path => this.path ?? (this.path = this.ComputePath());

    public IEdmType Type => this.navigationProperty.Type.Definition;

    public IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty) => (IEdmNavigationSource) null;

    public IEdmNavigationSource FindNavigationTarget(
      IEdmNavigationProperty navigationProperty,
      IEdmPathExpression bindingPath)
    {
      return (IEdmNavigationSource) null;
    }

    public IEnumerable<IEdmNavigationPropertyBinding> FindNavigationPropertyBindings(
      IEdmNavigationProperty navigationProperty)
    {
      return (IEnumerable<IEdmNavigationPropertyBinding>) null;
    }

    private IEdmPathExpression ComputePath() => (IEdmPathExpression) new EdmPathExpression(new List<string>(this.parentNavigationSource.Path.PathSegments)
    {
      this.navigationProperty.Name
    }.ToArray());
  }
}
