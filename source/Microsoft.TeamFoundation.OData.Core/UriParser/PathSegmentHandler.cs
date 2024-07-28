// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.PathSegmentHandler
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;

namespace Microsoft.OData.UriParser
{
  public abstract class PathSegmentHandler
  {
    public virtual void Handle(ODataPathSegment segment) => throw new NotImplementedException();

    public virtual void Handle(TypeSegment segment) => throw new NotImplementedException();

    public virtual void Handle(NavigationPropertySegment segment) => throw new NotImplementedException();

    public virtual void Handle(EntitySetSegment segment) => throw new NotImplementedException();

    public virtual void Handle(SingletonSegment segment) => throw new NotImplementedException();

    public virtual void Handle(KeySegment segment) => throw new NotImplementedException();

    public virtual void Handle(PropertySegment segment) => throw new NotImplementedException();

    public virtual void Handle(AnnotationSegment segment) => throw new NotImplementedException();

    public virtual void Handle(OperationImportSegment segment) => throw new NotImplementedException();

    public virtual void Handle(OperationSegment segment) => throw new NotImplementedException();

    public virtual void Handle(DynamicPathSegment segment) => throw new NotImplementedException();

    public virtual void Handle(CountSegment segment) => throw new NotImplementedException();

    public virtual void Handle(FilterSegment segment) => throw new NotImplementedException();

    public virtual void Handle(ReferenceSegment segment) => throw new NotImplementedException();

    public virtual void Handle(EachSegment segment) => throw new NotImplementedException();

    public virtual void Handle(NavigationPropertyLinkSegment segment) => throw new NotImplementedException();

    public virtual void Handle(ValueSegment segment) => throw new NotImplementedException();

    public virtual void Handle(BatchSegment segment) => throw new NotImplementedException();

    public virtual void Handle(BatchReferenceSegment segment) => throw new NotImplementedException();

    public virtual void Handle(MetadataSegment segment) => throw new NotImplementedException();

    public virtual void Handle(PathTemplateSegment segment) => throw new NotImplementedException();
  }
}
