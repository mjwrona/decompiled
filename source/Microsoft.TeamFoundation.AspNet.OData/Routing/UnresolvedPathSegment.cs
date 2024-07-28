// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.UnresolvedPathSegment
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace Microsoft.AspNet.OData.Routing
{
  public class UnresolvedPathSegment : ODataPathSegment
  {
    public UnresolvedPathSegment(string segmentValue) => this.SegmentValue = segmentValue != null ? segmentValue : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segmentValue));

    public virtual string SegmentKind => "unresolved";

    public string SegmentValue { get; private set; }

    public override string ToString() => this.SegmentValue;

    public override T TranslateWith<T>(PathSegmentTranslator<T> translator) => typeof (T) == typeof (string) ? (T) this.SegmentValue : default (T);

    public override void HandleWith(PathSegmentHandler handler)
    {
    }

    public override IEdmType EdmType => (IEdmType) null;
  }
}
