// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.BatchReferenceSegment
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public sealed class BatchReferenceSegment : ODataPathSegment
  {
    private readonly IEdmType edmType;
    private readonly IEdmEntitySetBase entitySet;
    private readonly string contentId;

    public BatchReferenceSegment(string contentId, IEdmType edmType, IEdmEntitySetBase entitySet)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmType>(edmType, nameof (edmType));
      ExceptionUtils.CheckArgumentNotNull<string>(contentId, nameof (contentId));
      if (!ODataPathParser.ContentIdRegex.IsMatch(contentId))
        throw new ODataException(Microsoft.OData.Strings.BatchReferenceSegment_InvalidContentID((object) contentId));
      this.edmType = edmType;
      this.entitySet = entitySet;
      this.contentId = contentId;
      this.Identifier = this.ContentId;
      this.TargetEdmType = edmType;
      this.TargetEdmNavigationSource = (IEdmNavigationSource) this.EntitySet;
      this.SingleResult = true;
      this.TargetKind = RequestTargetKind.Resource;
      if (entitySet == null)
        return;
      ExceptionUtil.ThrowIfTypesUnrelated(edmType, (IEdmType) entitySet.EntityType(), "BatchReferenceSegments");
    }

    public override IEdmType EdmType => this.edmType;

    public IEdmEntitySetBase EntitySet => this.entitySet;

    public string ContentId => this.contentId;

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

    internal override bool Equals(ODataPathSegment other) => other is BatchReferenceSegment referenceSegment && referenceSegment.EdmType == this.edmType && referenceSegment.EntitySet == this.entitySet && referenceSegment.ContentId == this.contentId;
  }
}
