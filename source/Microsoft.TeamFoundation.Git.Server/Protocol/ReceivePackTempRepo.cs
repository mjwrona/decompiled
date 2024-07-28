// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Protocol.ReceivePackTempRepo
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Storage;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Git.Server.Protocol
{
  internal class ReceivePackTempRepo : IDisposable
  {
    private readonly CachedGitObjectSet m_packStreamGitObjectSet;
    private bool m_disposed;
    private const string c_layer = "ReceivePackTempRepo";

    public ReceivePackTempRepo(
      IVssRequestContext rc,
      ITfsGitRepository baseRepo,
      GitReceivePackDeserializer packDeserializer,
      FileBufferedStreamBase packStream,
      IBufferStreamFactory bufferStreamFactory,
      GitObjectCoreCacheService rawContentCache,
      TfsGitContentCacheService gitContentCache,
      IReadOnlyList<TfsGitRefUpdateRequest> refUpdateRequests)
    {
      ArgumentUtility.CheckForNull<ITfsGitRepository>(baseRepo, nameof (baseRepo));
      ArgumentUtility.CheckForNull<GitReceivePackDeserializer>(packDeserializer, nameof (packDeserializer));
      this.BaseRepo = baseRepo;
      this.PackDeserializer = packDeserializer;
      ArgumentUtility.CheckForNull<FileBufferedStreamBase>(packStream, nameof (packStream));
      ArgumentUtility.CheckForNull<IBufferStreamFactory>(bufferStreamFactory, nameof (bufferStreamFactory));
      ArgumentUtility.CheckForNull<IReadOnlyList<TfsGitRefUpdateRequest>>(refUpdateRequests, nameof (refUpdateRequests));
      rc.Trace(1013094, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (ReceivePackTempRepo), "Creating a temporary repo based on pack stream.");
      CacheKeys.CrossHostOdbId crossHostOdbId = new CacheKeys.CrossHostOdbId(rc.ServiceHost.InstanceId, new OdbId(Guid.NewGuid()));
      GitObjectSet objectSet = GitServerUtils.GetOdb(this.BaseRepo).ObjectSet;
      PackStreamContentDB contentDB = new PackStreamContentDB(crossHostOdbId, packStream, bufferStreamFactory, gitContentCache, (IContentDB) objectSet.ContentDB, this.PackDeserializer.Splitter.PackSplitter);
      this.m_packStreamGitObjectSet = new CachedGitObjectSet(crossHostOdbId, (IContentDB) contentDB, rawContentCache);
      InMemoryObjectMetadata memoryObjectMetadata = new InMemoryObjectMetadata(this.PackDeserializer.ObjectSizeMap);
      this.ObjectSet = new UnionGitObjectSet(new List<ICachedGitObjectSet>()
      {
        (ICachedGitObjectSet) objectSet,
        (ICachedGitObjectSet) this.m_packStreamGitObjectSet
      });
      this.ObjectMetadata = new UnionObjectMetadata(new List<IObjectMetadata>()
      {
        this.BaseRepo.ObjectMetadata,
        (IObjectMetadata) memoryObjectMetadata
      });
      this.RefUpdateRequests = refUpdateRequests;
      this.BufferStreamFactory = bufferStreamFactory;
    }

    public ITfsGitRepository BaseRepo { get; }

    public IBufferStreamFactory BufferStreamFactory { get; }

    public GitReceivePackDeserializer PackDeserializer { get; }

    public UnionGitObjectSet ObjectSet { get; }

    public UnionObjectMetadata ObjectMetadata { get; }

    public IReadOnlyList<TfsGitRefUpdateRequest> RefUpdateRequests { get; }

    public void Dispose()
    {
      if (this.m_disposed)
        return;
      this.m_packStreamGitObjectSet?.Dispose();
      this.m_disposed = true;
    }
  }
}
