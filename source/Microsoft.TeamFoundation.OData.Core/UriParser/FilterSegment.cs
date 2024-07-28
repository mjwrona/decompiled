// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.FilterSegment
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public sealed class FilterSegment : ODataPathSegment
  {
    private readonly SingleValueNode expression;
    private readonly RangeVariable rangeVariable;
    private readonly IEdmType bindingType;
    private readonly string literalText;

    public FilterSegment(
      SingleValueNode expression,
      RangeVariable rangeVariable,
      IEdmNavigationSource navigationSource)
    {
      ExceptionUtils.CheckArgumentNotNull<SingleValueNode>(expression, nameof (expression));
      ExceptionUtils.CheckArgumentNotNull<RangeVariable>(rangeVariable, nameof (rangeVariable));
      ExceptionUtils.CheckArgumentNotNull<IEdmNavigationSource>(navigationSource, nameof (navigationSource));
      this.Identifier = "$filter";
      this.SingleResult = false;
      this.TargetEdmNavigationSource = navigationSource;
      this.TargetKind = RequestTargetKind.Resource;
      this.TargetEdmType = rangeVariable.TypeReference.Definition;
      this.expression = expression;
      this.rangeVariable = rangeVariable;
      this.bindingType = navigationSource.Type;
      this.literalText = "$filter(" + new NodeToStringBuilder().TranslateNode((QueryNode) expression) + ")";
    }

    public SingleValueNode Expression => this.expression;

    public RangeVariable RangeVariable => this.rangeVariable;

    public IEdmTypeReference ItemType => this.RangeVariable.TypeReference;

    public override IEdmType EdmType => this.bindingType;

    public string LiteralText => this.literalText;

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
      return other is FilterSegment filterSegment && filterSegment.TargetEdmNavigationSource == this.TargetEdmNavigationSource && filterSegment.Expression == this.Expression && filterSegment.ItemType == this.ItemType && filterSegment.RangeVariable == this.RangeVariable;
    }
  }
}
