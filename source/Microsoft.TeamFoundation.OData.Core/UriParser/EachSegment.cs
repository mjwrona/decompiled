// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.EachSegment
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public sealed class EachSegment : ODataPathSegment
  {
    private readonly IEdmType edmType;

    public EachSegment(IEdmNavigationSource navigationSource, IEdmType targetEdmType)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmNavigationSource>(navigationSource, nameof (navigationSource));
      ExceptionUtils.CheckArgumentNotNull<IEdmType>(targetEdmType, nameof (targetEdmType));
      this.Identifier = "$each";
      this.SingleResult = false;
      this.TargetEdmNavigationSource = navigationSource;
      this.TargetEdmType = targetEdmType;
      this.TargetKind = targetEdmType.GetTargetKindFromType();
      this.edmType = navigationSource.Type;
    }

    public override IEdmType EdmType => this.edmType;

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
      return other is EachSegment eachSegment && eachSegment.TargetEdmNavigationSource == this.TargetEdmNavigationSource;
    }
  }
}
