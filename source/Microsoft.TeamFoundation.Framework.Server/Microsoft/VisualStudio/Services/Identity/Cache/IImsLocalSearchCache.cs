// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.IImsLocalSearchCache
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  [DefaultServiceImplementation(typeof (ImsLocalSearchCache))]
  internal interface IImsLocalSearchCache : IVssFrameworkService
  {
    bool HasIndex<T>(IVssRequestContext context, Guid indexId) where T : ImsCacheObject<string, IdentityId>;

    bool IsIndexStale<T>(IVssRequestContext context, Guid indexId) where T : ImsCacheObject<string, IdentityId>;

    void CreateIndex<T>(
      IVssRequestContext context,
      Guid indexId,
      ICollection<T> objectsToIndex,
      DateTimeOffset creationTime)
      where T : ImsCacheObject<string, IdentityId>;

    void AddToIndex<T>(IVssRequestContext context, Guid indexId, ICollection<T> objects) where T : ImsCacheObject<string, IdentityId>;

    IDictionary<string, IEnumerable<IdentityId>> SearchIndex<T>(
      IVssRequestContext context,
      Guid indexId,
      ICollection<string> keys)
      where T : ImsCacheObject<string, IdentityId>;

    void InvalidateIndex<T>(IVssRequestContext context, Guid indexId) where T : ImsCacheObject<string, IdentityId>;
  }
}
