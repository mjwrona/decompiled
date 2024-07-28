// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ValueSegment
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public sealed class ValueSegment : ODataPathSegment
  {
    private readonly IEdmType edmType;

    public ValueSegment(IEdmType previousType)
    {
      this.Identifier = "$value";
      this.SingleResult = true;
      switch (previousType)
      {
        case IEdmCollectionType _:
          throw new ODataException(Microsoft.OData.Strings.PathParser_CannotUseValueOnCollection);
        case IEdmEntityType _:
          this.edmType = EdmCoreModel.Instance.GetStream(false).Definition;
          break;
        default:
          this.edmType = previousType;
          break;
      }
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

    internal override bool Equals(ODataPathSegment other) => other is ValueSegment valueSegment && valueSegment.EdmType == this.edmType;
  }
}
