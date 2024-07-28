// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.IsCollectionTranslator
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;

namespace Microsoft.OData.UriParser
{
  internal sealed class IsCollectionTranslator : PathSegmentTranslator<bool>
  {
    public override bool Translate(NavigationPropertySegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<NavigationPropertySegment>(segment, nameof (segment));
      return segment.NavigationProperty.Type.IsCollection();
    }

    public override bool Translate(EntitySetSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<EntitySetSegment>(segment, nameof (segment));
      return true;
    }

    public override bool Translate(KeySegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<KeySegment>(segment, nameof (segment));
      return false;
    }

    public override bool Translate(PropertySegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<PropertySegment>(segment, nameof (segment));
      return false;
    }

    public override bool Translate(AnnotationSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<AnnotationSegment>(segment, nameof (segment));
      return false;
    }

    public override bool Translate(DynamicPathSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<DynamicPathSegment>(segment, nameof (segment));
      return false;
    }

    public override bool Translate(CountSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<CountSegment>(segment, nameof (segment));
      return false;
    }

    public override bool Translate(FilterSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<FilterSegment>(segment, nameof (segment));
      return true;
    }

    public override bool Translate(ReferenceSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<ReferenceSegment>(segment, nameof (segment));
      return !segment.SingleResult;
    }

    public override bool Translate(EachSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<EachSegment>(segment, nameof (segment));
      return false;
    }

    public override bool Translate(NavigationPropertyLinkSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<NavigationPropertyLinkSegment>(segment, nameof (segment));
      return false;
    }

    public override bool Translate(BatchSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<BatchSegment>(segment, nameof (segment));
      return false;
    }

    public override bool Translate(BatchReferenceSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<BatchReferenceSegment>(segment, nameof (segment));
      return false;
    }

    public override bool Translate(ValueSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<ValueSegment>(segment, nameof (segment));
      throw new NotImplementedException(segment.ToString());
    }

    public override bool Translate(MetadataSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<MetadataSegment>(segment, nameof (segment));
      return false;
    }

    public override bool Translate(PathTemplateSegment segment)
    {
      ExceptionUtils.CheckArgumentNotNull<PathTemplateSegment>(segment, nameof (segment));
      return !segment.SingleResult;
    }
  }
}
