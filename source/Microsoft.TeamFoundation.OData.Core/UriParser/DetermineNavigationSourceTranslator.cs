// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.DetermineNavigationSourceTranslator
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  internal sealed class DetermineNavigationSourceTranslator : 
    PathSegmentTranslator<IEdmNavigationSource>
  {
    public override IEdmNavigationSource Translate(NavigationPropertyLinkSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<NavigationPropertyLinkSegment>(segment, nameof (segment));
      return segment.NavigationSource;
    }

    public override IEdmNavigationSource Translate(TypeSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<TypeSegment>(segment, nameof (segment));
      return segment.NavigationSource;
    }

    public override IEdmNavigationSource Translate(NavigationPropertySegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<NavigationPropertySegment>(segment, nameof (segment));
      return segment.NavigationSource;
    }

    public override IEdmNavigationSource Translate(EntitySetSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<EntitySetSegment>(segment, nameof (segment));
      return (IEdmNavigationSource) segment.EntitySet;
    }

    public override IEdmNavigationSource Translate(SingletonSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<SingletonSegment>(segment, nameof (segment));
      return (IEdmNavigationSource) segment.Singleton;
    }

    public override IEdmNavigationSource Translate(KeySegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<KeySegment>(segment, nameof (segment));
      return segment.NavigationSource;
    }

    public override IEdmNavigationSource Translate(PropertySegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<PropertySegment>(segment, nameof (segment));
      return segment.EdmType.AsElementType() is IEdmComplexType ? segment.TargetEdmNavigationSource : (IEdmNavigationSource) null;
    }

    public override IEdmNavigationSource Translate(OperationImportSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<OperationImportSegment>(segment, nameof (segment));
      return (IEdmNavigationSource) segment.EntitySet;
    }

    public override IEdmNavigationSource Translate(OperationSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<OperationSegment>(segment, nameof (segment));
      return (IEdmNavigationSource) segment.EntitySet;
    }

    public override IEdmNavigationSource Translate(CountSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<CountSegment>(segment, nameof (segment));
      return (IEdmNavigationSource) null;
    }

    public override IEdmNavigationSource Translate(FilterSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<FilterSegment>(segment, nameof (segment));
      return segment.TargetEdmNavigationSource;
    }

    public override IEdmNavigationSource Translate(ReferenceSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<ReferenceSegment>(segment, nameof (segment));
      return segment.TargetEdmNavigationSource;
    }

    public override IEdmNavigationSource Translate(EachSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<EachSegment>(segment, nameof (segment));
      return segment.TargetEdmNavigationSource;
    }

    public override IEdmNavigationSource Translate(DynamicPathSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<DynamicPathSegment>(segment, nameof (segment));
      return (IEdmNavigationSource) null;
    }

    public override IEdmNavigationSource Translate(ValueSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<ValueSegment>(segment, nameof (segment));
      return (IEdmNavigationSource) null;
    }

    public override IEdmNavigationSource Translate(BatchSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<BatchSegment>(segment, nameof (segment));
      return (IEdmNavigationSource) null;
    }

    public override IEdmNavigationSource Translate(BatchReferenceSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<BatchReferenceSegment>(segment, nameof (segment));
      return (IEdmNavigationSource) segment.EntitySet;
    }

    public override IEdmNavigationSource Translate(MetadataSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<MetadataSegment>(segment, nameof (segment));
      return (IEdmNavigationSource) null;
    }

    public override IEdmNavigationSource Translate(PathTemplateSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<PathTemplateSegment>(segment, nameof (segment));
      return (IEdmNavigationSource) null;
    }
  }
}
