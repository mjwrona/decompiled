// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataResourceTypeContext
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;

namespace Microsoft.OData
{
  internal class ODataResourceTypeContext : IODataResourceTypeContext
  {
    protected IEdmStructuredType expectedResourceType;
    protected string expectedResourceTypeName;
    private readonly bool throwIfMissingTypeInfo;

    private ODataResourceTypeContext(bool throwIfMissingTypeInfo) => this.throwIfMissingTypeInfo = throwIfMissingTypeInfo;

    private ODataResourceTypeContext(
      IEdmStructuredType expectedResourceType,
      bool throwIfMissingTypeInfo)
    {
      this.expectedResourceType = expectedResourceType;
      this.throwIfMissingTypeInfo = throwIfMissingTypeInfo;
    }

    public virtual string NavigationSourceName => this.ValidateAndReturn<string>((string) null);

    public virtual string NavigationSourceEntityTypeName => this.ValidateAndReturn<string>((string) null);

    public virtual string NavigationSourceFullTypeName => this.ValidateAndReturn<string>((string) null);

    public virtual EdmNavigationSourceKind NavigationSourceKind => EdmNavigationSourceKind.None;

    public virtual string ExpectedResourceTypeName
    {
      get
      {
        if (this.expectedResourceTypeName == null)
          this.expectedResourceTypeName = this.expectedResourceType == null ? (string) null : this.expectedResourceType.FullTypeName();
        return this.expectedResourceTypeName;
      }
    }

    public virtual IEdmStructuredType ExpectedResourceType => this.expectedResourceType;

    public virtual bool IsFromCollection => false;

    public virtual bool IsMediaLinkEntry => false;

    internal static ODataResourceTypeContext Create(
      ODataResourceSerializationInfo serializationInfo,
      IEdmNavigationSource navigationSource,
      IEdmEntityType navigationSourceEntityType,
      IEdmStructuredType expectedResourceType,
      bool throwIfMissingTypeInfo)
    {
      if (serializationInfo != null)
        return (ODataResourceTypeContext) new ODataResourceTypeContext.ODataResourceTypeContextWithoutModel(serializationInfo);
      if (expectedResourceType != null && expectedResourceType.IsODataComplexTypeKind())
        return (ODataResourceTypeContext) new ODataResourceTypeContext.ODataResourceTypeContextWithModel((IEdmNavigationSource) null, (IEdmEntityType) null, expectedResourceType);
      return navigationSource != null && expectedResourceType != null ? (ODataResourceTypeContext) new ODataResourceTypeContext.ODataResourceTypeContextWithModel(navigationSource, navigationSourceEntityType, expectedResourceType) : new ODataResourceTypeContext(expectedResourceType, throwIfMissingTypeInfo);
    }

    private T ValidateAndReturn<T>(T value) where T : class => !this.throwIfMissingTypeInfo || (object) value != null ? value : throw new ODataException(Strings.ODataResourceTypeContext_MetadataOrSerializationInfoMissing);

    internal sealed class ODataResourceTypeContextWithoutModel : ODataResourceTypeContext
    {
      private readonly ODataResourceSerializationInfo serializationInfo;

      internal ODataResourceTypeContextWithoutModel(ODataResourceSerializationInfo serializationInfo)
        : base(false)
      {
        this.serializationInfo = serializationInfo;
      }

      public override string NavigationSourceName => this.serializationInfo.NavigationSourceName;

      public override string NavigationSourceEntityTypeName => this.serializationInfo.NavigationSourceEntityTypeName;

      public override string NavigationSourceFullTypeName => this.IsFromCollection ? EdmLibraryExtensions.GetCollectionTypeName(this.serializationInfo.NavigationSourceEntityTypeName) : this.serializationInfo.NavigationSourceEntityTypeName;

      public override EdmNavigationSourceKind NavigationSourceKind => this.serializationInfo.NavigationSourceKind;

      public override string ExpectedResourceTypeName => this.serializationInfo.ExpectedTypeName;

      public override IEdmStructuredType ExpectedResourceType => (IEdmStructuredType) null;

      public override bool IsMediaLinkEntry => false;

      public override bool IsFromCollection => this.serializationInfo.IsFromCollection;
    }

    internal sealed class ODataResourceTypeContextWithModel : ODataResourceTypeContext
    {
      private readonly IEdmNavigationSource navigationSource;
      private readonly IEdmEntityType navigationSourceEntityType;
      private readonly string navigationSourceName;
      private readonly bool isMediaLinkEntry;
      private readonly bool isFromCollection;
      private string navigationSourceFullTypeName;
      private string navigationSourceEntityTypeName;

      internal ODataResourceTypeContextWithModel(
        IEdmNavigationSource navigationSource,
        IEdmEntityType navigationSourceEntityType,
        IEdmStructuredType expectedResourceType)
        : base(expectedResourceType, false)
      {
        this.navigationSource = navigationSource;
        this.navigationSourceEntityType = navigationSourceEntityType;
        if (navigationSource is IEdmContainedEntitySet containedEntitySet && containedEntitySet.NavigationProperty.Type.TypeKind() == EdmTypeKind.Collection)
          this.isFromCollection = true;
        if (navigationSource is IEdmUnknownEntitySet unknownEntitySet && unknownEntitySet.Type.TypeKind == EdmTypeKind.Collection)
          this.isFromCollection = true;
        this.navigationSourceName = this.navigationSource == null ? (string) null : this.navigationSource.Name;
        this.isMediaLinkEntry = this.expectedResourceType is IEdmEntityType expectedResourceType1 && expectedResourceType1.HasStream;
      }

      public override string NavigationSourceName => this.navigationSourceName;

      public override string NavigationSourceEntityTypeName
      {
        get
        {
          if (this.navigationSourceEntityType != null)
            this.navigationSourceEntityTypeName = this.navigationSourceEntityType.FullName();
          return this.navigationSourceEntityTypeName;
        }
      }

      public override string NavigationSourceFullTypeName
      {
        get
        {
          if (this.navigationSource != null)
            this.navigationSourceFullTypeName = this.navigationSource.Type.FullTypeName();
          return this.navigationSourceFullTypeName;
        }
      }

      public override EdmNavigationSourceKind NavigationSourceKind => this.navigationSource.NavigationSourceKind();

      public override string ExpectedResourceTypeName => this.expectedResourceType.FullTypeName();

      public override IEdmStructuredType ExpectedResourceType => this.expectedResourceType;

      public override bool IsMediaLinkEntry => this.isMediaLinkEntry;

      public override bool IsFromCollection => this.isFromCollection;

      internal IEdmEntityType NavigationSourceEntityType => this.navigationSourceEntityType;
    }
  }
}
