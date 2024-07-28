// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.EntitySetSegment
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public sealed class EntitySetSegment : ODataPathSegment
  {
    private readonly IEdmEntitySet entitySet;
    private readonly IEdmType type;

    public EntitySetSegment(IEdmEntitySet entitySet)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmEntitySet>(entitySet, nameof (entitySet));
      this.entitySet = entitySet;
      this.type = (IEdmType) new EdmCollectionType((IEdmTypeReference) new EdmEntityTypeReference(this.entitySet.EntityType(), false));
      this.TargetEdmNavigationSource = (IEdmNavigationSource) entitySet;
      this.TargetEdmType = (IEdmType) entitySet.EntityType();
      this.TargetKind = RequestTargetKind.Resource;
      this.SingleResult = false;
    }

    public IEdmEntitySet EntitySet => this.entitySet;

    public override IEdmType EdmType => this.type;

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
      return other is EntitySetSegment entitySetSegment && entitySetSegment.EntitySet == this.EntitySet;
    }
  }
}
