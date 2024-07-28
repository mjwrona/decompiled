// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint.StorageEndpointCollectionService
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Search.Platform.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint
{
  public class StorageEndpointCollectionService : 
    IStorageEndpointCollectionService,
    IVssFrameworkService
  {
    private IDisposableReadOnlyList<IStorageEndpointCollection> m_backingStoreCollections;

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_backingStoreCollections = SearchPlatformHelper.GetExtensions<IStorageEndpointCollection>(systemRequestContext);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_backingStoreCollections == null)
        return;
      this.m_backingStoreCollections.Dispose();
      this.m_backingStoreCollections = (IDisposableReadOnlyList<IStorageEndpointCollection>) null;
    }

    public IStorageEndpointCollection CreateBackingStoreCollection(
      IVssRequestContext requestContext,
      IEntityType entityType,
      string storePath,
      params object[] collectionParams)
    {
      if (collectionParams == null)
        throw new ArgumentNullException(nameof (collectionParams));
      IStorageEndpointCollection endpointCollection = (IStorageEndpointCollection) null;
      foreach (IStorageEndpointCollection backingStoreCollection in (IEnumerable<IStorageEndpointCollection>) this.m_backingStoreCollections)
      {
        if (backingStoreCollection.SupportedEntityTypes.Contains<IEntityType>(entityType, (IEqualityComparer<IEntityType>) new EntityTypeComparer()))
        {
          object[] objArray = new object[collectionParams.Length + 1];
          objArray[0] = (object) storePath;
          for (int index = 0; index < collectionParams.Length; ++index)
            objArray[index + 1] = collectionParams[index];
          endpointCollection = (IStorageEndpointCollection) Activator.CreateInstance(backingStoreCollection.GetType(), objArray);
          break;
        }
      }
      return endpointCollection != null ? endpointCollection : throw new NotImplementedException(FormattableString.Invariant(FormattableStringFactory.Create("The current entity does not have an applicable {0}", (object) "IStorageEndpointCollection")));
    }
  }
}
