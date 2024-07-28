// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.ODataPath
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.AspNet.OData.Routing
{
  [ODataPathParameterBinding]
  public class ODataPath
  {
    private readonly ReadOnlyCollection<ODataPathSegment> _segments;
    private readonly IEdmType _edmType;
    private readonly IEdmNavigationSource _navigationSource;
    private readonly string _pathTemplate;
    private readonly string _pathLiteral;

    public ODataPath(params ODataPathSegment[] segments)
      : this((IEnumerable<ODataPathSegment>) segments)
    {
    }

    public ODataPath(IEnumerable<ODataPathSegment> segments)
    {
      if (segments == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segments));
      if (!(segments is IList<ODataPathSegment> odataPathSegmentList1))
        odataPathSegmentList1 = (IList<ODataPathSegment>) segments.ToList<ODataPathSegment>();
      IList<ODataPathSegment> odataPathSegmentList2 = odataPathSegmentList1;
      this._edmType = odataPathSegmentList2.Any<ODataPathSegment>() ? odataPathSegmentList2.Last<ODataPathSegment>().EdmType : (IEdmType) null;
      this._segments = new ReadOnlyCollection<ODataPathSegment>(odataPathSegmentList2);
      ODataPathSegmentHandler handler = new ODataPathSegmentHandler();
      foreach (ODataPathSegment odataPathSegment in (IEnumerable<ODataPathSegment>) odataPathSegmentList2)
      {
        if (odataPathSegment is UnresolvedPathSegment segment)
          handler.Handle(segment);
        else
          odataPathSegment.HandleWith((PathSegmentHandler) handler);
      }
      this._navigationSource = handler.NavigationSource;
      this._pathTemplate = handler.PathTemplate;
      this._pathLiteral = handler.PathLiteral;
    }

    public IEdmType EdmType => this._edmType;

    public IEdmNavigationSource NavigationSource => this._navigationSource;

    public ReadOnlyCollection<ODataPathSegment> Segments => this._segments;

    public virtual string PathTemplate => this._pathTemplate;

    public override string ToString() => this._pathLiteral;

    public Microsoft.OData.UriParser.ODataPath Path { get; internal set; }
  }
}
