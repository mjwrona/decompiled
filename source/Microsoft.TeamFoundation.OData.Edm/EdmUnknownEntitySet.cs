// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmUnknownEntitySet
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
  internal class EdmUnknownEntitySet : 
    EdmEntitySetBase,
    IEdmUnknownEntitySet,
    IEdmEntitySetBase,
    IEdmNavigationSource,
    IEdmNamedElement,
    IEdmElement
  {
    private readonly IEdmNavigationProperty navigationProperty;
    private readonly IEdmNavigationSource parentNavigationSource;
    private IEdmPathExpression path;

    public EdmUnknownEntitySet(
      IEdmNavigationSource parentNavigationSource,
      IEdmNavigationProperty navigationProperty)
      : base(navigationProperty.Name, navigationProperty.ToEntityType())
    {
      EdmUtil.CheckArgumentNull<IEdmNavigationSource>(parentNavigationSource, nameof (parentNavigationSource));
      EdmUtil.CheckArgumentNull<IEdmNavigationProperty>(navigationProperty, nameof (navigationProperty));
      this.parentNavigationSource = parentNavigationSource;
      this.navigationProperty = navigationProperty;
    }

    public override IEdmPathExpression Path => this.path ?? (this.path = this.ComputePath());

    public override IEdmType Type => this.navigationProperty.Type.Definition;

    public override IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty property) => (IEdmNavigationSource) null;

    private IEdmPathExpression ComputePath() => (IEdmPathExpression) new EdmPathExpression(new List<string>(this.parentNavigationSource.Path.PathSegments)
    {
      this.navigationProperty.Name
    }.ToArray());
  }
}
