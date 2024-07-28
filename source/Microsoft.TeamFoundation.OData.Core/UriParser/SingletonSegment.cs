// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SingletonSegment
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public sealed class SingletonSegment : ODataPathSegment
  {
    private readonly IEdmSingleton singleton;

    public SingletonSegment(IEdmSingleton singleton)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmSingleton>(singleton, nameof (singleton));
      this.singleton = singleton;
      this.TargetEdmNavigationSource = (IEdmNavigationSource) singleton;
      this.TargetEdmType = (IEdmType) singleton.EntityType();
      this.TargetKind = RequestTargetKind.Resource;
      this.SingleResult = true;
    }

    public IEdmSingleton Singleton => this.singleton;

    public override IEdmType EdmType => (IEdmType) this.singleton.EntityType();

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
      return other is SingletonSegment singletonSegment && singletonSegment.singleton == this.Singleton;
    }
  }
}
