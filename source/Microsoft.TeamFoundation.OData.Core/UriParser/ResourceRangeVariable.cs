// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ResourceRangeVariable
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public sealed class ResourceRangeVariable : RangeVariable
  {
    private readonly string name;
    private readonly CollectionResourceNode collectionResourceNode;
    private readonly IEdmNavigationSource navigationSource;
    private readonly IEdmStructuredTypeReference structuredTypeReference;

    public ResourceRangeVariable(
      string name,
      IEdmStructuredTypeReference structuredType,
      CollectionResourceNode collectionResourceNode)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(name, nameof (name));
      ExceptionUtils.CheckArgumentNotNull<IEdmStructuredTypeReference>(structuredType, nameof (structuredType));
      this.name = name;
      this.structuredTypeReference = structuredType;
      this.collectionResourceNode = collectionResourceNode;
      this.navigationSource = collectionResourceNode?.NavigationSource;
    }

    public ResourceRangeVariable(
      string name,
      IEdmStructuredTypeReference structuredType,
      IEdmNavigationSource navigationSource)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(name, nameof (name));
      ExceptionUtils.CheckArgumentNotNull<IEdmStructuredTypeReference>(structuredType, nameof (structuredType));
      this.name = name;
      this.structuredTypeReference = structuredType;
      this.collectionResourceNode = (CollectionResourceNode) null;
      this.navigationSource = navigationSource;
    }

    public override string Name => this.name;

    public CollectionResourceNode CollectionResourceNode => this.collectionResourceNode;

    public IEdmNavigationSource NavigationSource => this.navigationSource;

    public override IEdmTypeReference TypeReference => (IEdmTypeReference) this.structuredTypeReference;

    public IEdmStructuredTypeReference StructuredTypeReference => this.structuredTypeReference;

    public override int Kind => 0;
  }
}
