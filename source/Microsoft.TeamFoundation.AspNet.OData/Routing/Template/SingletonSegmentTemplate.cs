// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.Template.SingletonSegmentTemplate
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData.UriParser;
using System.Collections.Generic;

namespace Microsoft.AspNet.OData.Routing.Template
{
  public class SingletonSegmentTemplate : ODataPathSegmentTemplate
  {
    public SingletonSegmentTemplate(SingletonSegment segment) => this.Segment = segment != null ? segment : throw Error.ArgumentNull(nameof (segment));

    public SingletonSegment Segment { get; private set; }

    public override bool TryMatch(ODataPathSegment pathSegment, IDictionary<string, object> values) => pathSegment is SingletonSegment singletonSegment && singletonSegment.Singleton == this.Segment.Singleton;
  }
}
