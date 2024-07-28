// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.KeyLookupNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  internal sealed class KeyLookupNode : SingleEntityNode
  {
    private readonly CollectionResourceNode source;
    private readonly IEdmNavigationSource navigationSource;
    private readonly IEdmEntityTypeReference entityTypeReference;
    private readonly IEnumerable<KeyPropertyValue> keyPropertyValues;

    public KeyLookupNode(
      CollectionResourceNode source,
      IEnumerable<KeyPropertyValue> keyPropertyValues)
    {
      ExceptionUtils.CheckArgumentNotNull<CollectionResourceNode>(source, nameof (source));
      this.source = source;
      this.navigationSource = source.NavigationSource;
      this.entityTypeReference = source.ItemStructuredType as IEdmEntityTypeReference;
      this.keyPropertyValues = keyPropertyValues;
    }

    public CollectionResourceNode Source => this.source;

    public IEnumerable<KeyPropertyValue> KeyPropertyValues => this.keyPropertyValues;

    public override IEdmTypeReference TypeReference => (IEdmTypeReference) this.entityTypeReference;

    public override IEdmEntityTypeReference EntityTypeReference => this.entityTypeReference;

    public override IEdmNavigationSource NavigationSource => this.navigationSource;

    public override IEdmStructuredTypeReference StructuredTypeReference => (IEdmStructuredTypeReference) this.entityTypeReference;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.KeyLookup;
  }
}
