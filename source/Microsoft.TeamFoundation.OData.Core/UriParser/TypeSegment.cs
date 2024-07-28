// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.TypeSegment
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public sealed class TypeSegment : ODataPathSegment
  {
    private readonly IEdmType edmType;
    private readonly IEdmType expectedType;
    private readonly IEdmNavigationSource navigationSource;

    public TypeSegment(IEdmType actualType, IEdmNavigationSource navigationSource)
      : this(actualType, navigationSource == null ? actualType : (IEdmType) navigationSource.EntityType(), navigationSource)
    {
    }

    public TypeSegment(
      IEdmType actualType,
      IEdmType expectedType,
      IEdmNavigationSource navigationSource)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmType>(actualType, nameof (actualType));
      ExceptionUtils.CheckArgumentNotNull<IEdmType>(expectedType, nameof (expectedType));
      this.edmType = actualType;
      this.navigationSource = navigationSource;
      this.expectedType = expectedType;
      this.TargetEdmType = expectedType;
      this.TargetEdmNavigationSource = navigationSource;
      if (navigationSource == null)
        return;
      ExceptionUtil.ThrowIfTypesUnrelated(actualType, expectedType, "TypeSegments");
    }

    public override IEdmType EdmType => this.edmType;

    public IEdmType ExpectedType => this.expectedType;

    public IEdmNavigationSource NavigationSource => this.navigationSource;

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
      return other is TypeSegment typeSegment && typeSegment.EdmType == this.EdmType;
    }
  }
}
