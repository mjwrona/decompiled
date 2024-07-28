// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.Template.ODataPathTemplate
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.AspNet.OData.Routing.Template
{
  public class ODataPathTemplate
  {
    private ReadOnlyCollection<ODataPathSegmentTemplate> _segments;

    public ODataPathTemplate(params ODataPathSegmentTemplate[] segments)
      : this((IList<ODataPathSegmentTemplate>) segments)
    {
    }

    public ODataPathTemplate(IEnumerable<ODataPathSegmentTemplate> segments)
      : this((IList<ODataPathSegmentTemplate>) segments.AsList<ODataPathSegmentTemplate>())
    {
    }

    public ODataPathTemplate(IList<ODataPathSegmentTemplate> segments) => this._segments = segments != null ? new ReadOnlyCollection<ODataPathSegmentTemplate>(segments) : throw Error.ArgumentNull(nameof (segments));

    public ReadOnlyCollection<ODataPathSegmentTemplate> Segments => this._segments;

    public bool TryMatch(ODataPath path, IDictionary<string, object> values)
    {
      if (path.Segments.Count != this.Segments.Count)
        return false;
      for (int index = 0; index < this.Segments.Count; ++index)
      {
        if (!this.Segments[index].TryMatch(path.Segments[index], values))
          return false;
      }
      return true;
    }
  }
}
