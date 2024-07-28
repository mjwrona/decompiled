// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.NavigationPropertySegment
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public sealed class NavigationPropertySegment : ODataPathSegment
  {
    private readonly IEdmNavigationProperty navigationProperty;

    public NavigationPropertySegment(
      IEdmNavigationProperty navigationProperty,
      IEdmNavigationSource navigationSource)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmNavigationProperty>(navigationProperty, nameof (navigationProperty));
      this.navigationProperty = navigationProperty;
      this.TargetEdmNavigationSource = navigationSource;
      this.Identifier = navigationProperty.Name;
      this.TargetEdmType = navigationProperty.Type.Definition;
      this.SingleResult = !navigationProperty.Type.IsCollection();
      this.TargetKind = RequestTargetKind.Resource;
    }

    public IEdmNavigationProperty NavigationProperty => this.navigationProperty;

    public IEdmNavigationSource NavigationSource => this.TargetEdmNavigationSource;

    public override IEdmType EdmType => this.navigationProperty.Type.Definition;

    public override T TranslateWith<T>(PathSegmentTranslator<T> translator)
    {
      ExceptionUtils.CheckArgumentNotNull<PathSegmentTranslator<T>>(translator, nameof (translator));
      return translator.Translate(this);
    }

    public override void HandleWith(PathSegmentHandler handler)
    {
      ExceptionUtils.CheckArgumentNotNull<PathSegmentHandler>(handler, nameof (handler));
      handler.Handle(this);
    }

    internal override bool Equals(ODataPathSegment other)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataPathSegment>(other, nameof (other));
      return other is NavigationPropertySegment navigationPropertySegment && navigationPropertySegment.NavigationProperty == this.NavigationProperty;
    }
  }
}
