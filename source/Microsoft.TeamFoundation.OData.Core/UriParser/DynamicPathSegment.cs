// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.DynamicPathSegment
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public sealed class DynamicPathSegment : ODataPathSegment
  {
    public DynamicPathSegment(string identifier)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(identifier, nameof (identifier));
      this.Identifier = identifier;
      this.TargetEdmType = (IEdmType) null;
      this.TargetKind = RequestTargetKind.Dynamic;
      this.SingleResult = true;
    }

    public DynamicPathSegment(
      string identifier,
      IEdmType edmType,
      IEdmNavigationSource navigationSource,
      bool singleResult)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(identifier, nameof (identifier));
      this.Identifier = identifier;
      this.TargetEdmType = edmType;
      this.SingleResult = singleResult;
      this.TargetKind = edmType == null ? RequestTargetKind.Dynamic : edmType.GetTargetKindFromType();
      this.TargetEdmNavigationSource = navigationSource;
    }

    public override IEdmType EdmType => this.TargetEdmType;

    public override T TranslateWith<T>(PathSegmentTranslator<T> translator)
    {
      ExceptionUtils.CheckArgumentNotNull<PathSegmentTranslator<T>>(translator, nameof (translator));
      return translator.Translate(this);
    }

    public override void HandleWith(PathSegmentHandler handler)
    {
      ExceptionUtils.CheckArgumentNotNull<PathSegmentHandler>(handler, nameof (handler));
      handler.Handle(this);
    }

    internal override bool Equals(ODataPathSegment other)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataPathSegment>(other, nameof (other));
      return other is DynamicPathSegment dynamicPathSegment && dynamicPathSegment.Identifier == this.Identifier && dynamicPathSegment.EdmType == this.EdmType && dynamicPathSegment.TargetEdmNavigationSource == this.TargetEdmNavigationSource && dynamicPathSegment.SingleResult == this.SingleResult;
    }
  }
}
