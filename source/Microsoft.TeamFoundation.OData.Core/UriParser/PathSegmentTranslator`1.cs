// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.PathSegmentTranslator`1
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;

namespace Microsoft.OData.UriParser
{
  public abstract class PathSegmentTranslator<T>
  {
    public virtual T Translate(TypeSegment segment) => throw new NotImplementedException();

    public virtual T Translate(NavigationPropertySegment segment) => throw new NotImplementedException();

    public virtual T Translate(EntitySetSegment segment) => throw new NotImplementedException();

    public virtual T Translate(SingletonSegment segment) => throw new NotImplementedException();

    public virtual T Translate(KeySegment segment) => throw new NotImplementedException();

    public virtual T Translate(PropertySegment segment) => throw new NotImplementedException();

    public virtual T Translate(AnnotationSegment segment) => throw new NotImplementedException();

    public virtual T Translate(OperationImportSegment segment) => throw new NotImplementedException();

    public virtual T Translate(OperationSegment segment) => throw new NotImplementedException();

    public virtual T Translate(DynamicPathSegment segment) => throw new NotImplementedException();

    public virtual T Translate(CountSegment segment) => throw new NotImplementedException();

    public virtual T Translate(FilterSegment segment) => throw new NotImplementedException();

    public virtual T Translate(ReferenceSegment segment) => throw new NotImplementedException();

    public virtual T Translate(EachSegment segment) => throw new NotImplementedException();

    public virtual T Translate(NavigationPropertyLinkSegment segment) => throw new NotImplementedException();

    public virtual T Translate(ValueSegment segment) => throw new NotImplementedException();

    public virtual T Translate(BatchSegment segment) => throw new NotImplementedException();

    public virtual T Translate(BatchReferenceSegment segment) => throw new NotImplementedException();

    public virtual T Translate(MetadataSegment segment) => throw new NotImplementedException();

    public virtual T Translate(PathTemplateSegment segment) => throw new NotImplementedException();
  }
}
