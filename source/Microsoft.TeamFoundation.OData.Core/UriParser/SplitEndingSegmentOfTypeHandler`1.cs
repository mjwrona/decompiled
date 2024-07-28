// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SplitEndingSegmentOfTypeHandler`1
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  internal sealed class SplitEndingSegmentOfTypeHandler<T> : PathSegmentHandler where T : ODataPathSegment
  {
    private readonly Queue<ODataPathSegment> first;
    private readonly Queue<ODataPathSegment> last;

    public SplitEndingSegmentOfTypeHandler()
    {
      this.first = new Queue<ODataPathSegment>();
      this.last = new Queue<ODataPathSegment>();
    }

    public ODataPath FirstPart => new ODataPath((IEnumerable<ODataPathSegment>) this.first);

    public ODataPath LastPart => new ODataPath((IEnumerable<ODataPathSegment>) this.last);

    public override void Handle(TypeSegment segment) => this.CommonHandler((ODataPathSegment) segment);

    public override void Handle(NavigationPropertySegment segment) => this.CommonHandler((ODataPathSegment) segment);

    public override void Handle(EntitySetSegment segment) => this.CommonHandler((ODataPathSegment) segment);

    public override void Handle(SingletonSegment segment) => this.CommonHandler((ODataPathSegment) segment);

    public override void Handle(KeySegment segment) => this.CommonHandler((ODataPathSegment) segment);

    public override void Handle(PropertySegment segment) => this.CommonHandler((ODataPathSegment) segment);

    public override void Handle(AnnotationSegment segment) => this.CommonHandler((ODataPathSegment) segment);

    public override void Handle(OperationImportSegment segment) => this.CommonHandler((ODataPathSegment) segment);

    public override void Handle(OperationSegment segment) => this.CommonHandler((ODataPathSegment) segment);

    public override void Handle(DynamicPathSegment segment) => this.CommonHandler((ODataPathSegment) segment);

    public override void Handle(CountSegment segment) => this.CommonHandler((ODataPathSegment) segment);

    public override void Handle(FilterSegment segment) => this.CommonHandler((ODataPathSegment) segment);

    public override void Handle(EachSegment segment) => this.CommonHandler((ODataPathSegment) segment);

    public override void Handle(ReferenceSegment segment) => this.CommonHandler((ODataPathSegment) segment);

    public override void Handle(PathTemplateSegment segment) => this.CommonHandler((ODataPathSegment) segment);

    public override void Handle(NavigationPropertyLinkSegment segment) => this.CommonHandler((ODataPathSegment) segment);

    public override void Handle(ValueSegment segment) => this.CommonHandler((ODataPathSegment) segment);

    public override void Handle(BatchSegment segment) => this.CommonHandler((ODataPathSegment) segment);

    public override void Handle(BatchReferenceSegment segment) => this.CommonHandler((ODataPathSegment) segment);

    public override void Handle(MetadataSegment segment) => this.CommonHandler((ODataPathSegment) segment);

    private void CommonHandler(ODataPathSegment segment)
    {
      if (segment is T)
      {
        this.last.Enqueue(segment);
      }
      else
      {
        while (this.last.Any<ODataPathSegment>())
          this.first.Enqueue(this.last.Dequeue());
        this.first.Enqueue(segment);
      }
    }
  }
}
