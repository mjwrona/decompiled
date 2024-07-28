// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ODataPath
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "ODataPathCollection just doesn't sound right")]
  public class ODataPath : IEnumerable<ODataPathSegment>, IEnumerable
  {
    private readonly IList<ODataPathSegment> segments;

    public ODataPath(IEnumerable<ODataPathSegment> segments)
    {
      ExceptionUtils.CheckArgumentNotNull<IEnumerable<ODataPathSegment>>(segments, nameof (segments));
      this.segments = (IList<ODataPathSegment>) segments.ToList<ODataPathSegment>();
      if (this.segments.Any<ODataPathSegment>((Func<ODataPathSegment, bool>) (s => s == null)))
        throw Error.ArgumentNull(nameof (segments));
    }

    public ODataPath(params ODataPathSegment[] segments)
      : this((IEnumerable<ODataPathSegment>) segments)
    {
    }

    public ODataPathSegment FirstSegment => this.segments.Count != 0 ? this.segments[0] : (ODataPathSegment) null;

    public ODataPathSegment LastSegment => this.segments.Count != 0 ? this.segments[this.segments.Count - 1] : (ODataPathSegment) null;

    public int Count => this.segments.Count;

    public IEnumerator<ODataPathSegment> GetEnumerator() => this.segments.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "We would rather ship the PathSegmentTranslator so that its more extensible later")]
    public IEnumerable<T> WalkWith<T>(PathSegmentTranslator<T> translator) => this.segments.Select<ODataPathSegment, T>((Func<ODataPathSegment, T>) (segment => segment.TranslateWith<T>(translator)));

    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "We would rather ship the PathSegmentHandler so that its more extensible later")]
    public void WalkWith(PathSegmentHandler handler)
    {
      foreach (ODataPathSegment segment in (IEnumerable<ODataPathSegment>) this.segments)
        segment.HandleWith(handler);
    }

    internal bool Equals(ODataPath other)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataPath>(other, nameof (other));
      return this.segments.Count == other.segments.Count && !this.segments.Where<ODataPathSegment>((Func<ODataPathSegment, int, bool>) ((t, i) => !t.Equals(other.segments[i]))).Any<ODataPathSegment>();
    }

    internal void Add(ODataPathSegment newSegment)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataPathSegment>(newSegment, nameof (newSegment));
      this.segments.Add(newSegment);
    }
  }
}
