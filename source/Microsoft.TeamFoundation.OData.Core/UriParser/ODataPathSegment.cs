// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ODataPathSegment
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public abstract class ODataPathSegment
  {
    internal ODataPathSegment(ODataPathSegment other) => this.CopyValuesFrom(other);

    protected ODataPathSegment()
    {
    }

    public abstract IEdmType EdmType { get; }

    public string Identifier { get; set; }

    internal bool SingleResult { get; set; }

    internal IEdmNavigationSource TargetEdmNavigationSource { get; set; }

    internal IEdmType TargetEdmType { get; set; }

    internal RequestTargetKind TargetKind { get; set; }

    public abstract T TranslateWith<T>(PathSegmentTranslator<T> translator);

    public abstract void HandleWith(PathSegmentHandler handler);

    internal virtual bool Equals(ODataPathSegment other) => this == other;

    internal void CopyValuesFrom(ODataPathSegment other)
    {
      this.Identifier = other.Identifier;
      this.SingleResult = other.SingleResult;
      this.TargetEdmNavigationSource = other.TargetEdmNavigationSource;
      this.TargetKind = other.TargetKind;
      this.TargetEdmType = other.TargetEdmType;
    }
  }
}
