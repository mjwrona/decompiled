// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ODataPathInfo
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  internal class ODataPathInfo
  {
    private readonly IEdmType targetEdmType;
    private readonly IEdmNavigationSource targetNavigationSource;
    private readonly IEnumerable<ODataPathSegment> segments;

    public ODataPathInfo(ODataPath odataPath)
    {
      ODataPathSegment odataPathSegment = odataPath.LastSegment;
      IEnumerator<ODataPathSegment> enumerator = odataPath.GetEnumerator();
      int num = 0;
      do
        ;
      while (++num < odataPath.Count && enumerator.MoveNext());
      ODataPathSegment current = enumerator.Current;
      switch (odataPathSegment)
      {
        case null:
          this.segments = (IEnumerable<ODataPathSegment>) odataPath;
          break;
        case KeySegment _:
        case CountSegment _:
          odataPathSegment = current;
          goto default;
        default:
          this.targetNavigationSource = odataPathSegment.TargetEdmNavigationSource;
          this.targetEdmType = odataPathSegment.TargetEdmType;
          if (this.targetEdmType != null && this.targetEdmType is IEdmCollectionType targetEdmType)
          {
            this.targetEdmType = targetEdmType.ElementType.Definition;
            goto case null;
          }
          else
            goto case null;
      }
    }

    public ODataPathInfo(IEdmType targetEdmType, IEdmNavigationSource targetNavigationSource)
    {
      this.targetEdmType = targetEdmType;
      this.targetNavigationSource = targetNavigationSource;
      this.segments = (IEnumerable<ODataPathSegment>) new List<ODataPathSegment>();
    }

    public IEdmType TargetEdmType => this.targetEdmType;

    public IEdmNavigationSource TargetNavigationSource => this.targetNavigationSource;

    public IEnumerable<ODataPathSegment> Segments => this.segments;

    public IEdmStructuredType TargetStructuredType => (IEdmStructuredType) this.targetEdmType;
  }
}
