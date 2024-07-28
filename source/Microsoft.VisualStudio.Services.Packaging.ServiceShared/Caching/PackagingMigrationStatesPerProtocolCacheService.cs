// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Caching.PackagingMigrationStatesPerProtocolCacheService
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Caching
{
  public class PackagingMigrationStatesPerProtocolCacheService : 
    PackagingMemoryCacheService<IProtocol, EtagValue<IEnumerable<MigrationEntry>>>
  {
    private const int MaxElementsDefault = 10000;

    protected override Guid PackagingInvalidationNotificationGuid => new Guid("db699ee7-2d9d-4870-9f4f-9f16f068c9d8");

    public PackagingMigrationStatesPerProtocolCacheService()
      : base(MemoryCacheConfiguration<IProtocol, EtagValue<IEnumerable<MigrationEntry>>>.Default.WithExpiryInterval(TimeSpan.FromMinutes(1.0)).WithCleanupInterval(TimeSpan.FromSeconds(30.0)).WithMaxElements(10000))
    {
    }

    protected override IProtocol StringToKey(string keyString) => ProtocolRegistrar.Instance.GetProtocolOrDefault(keyString);
  }
}
