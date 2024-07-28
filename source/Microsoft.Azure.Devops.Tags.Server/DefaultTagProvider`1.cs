// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.DefaultTagProvider`1
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Devops.Tags.Server
{
  public class DefaultTagProvider<T> : ITagProvider<T>, ITagProvider
  {
    public DefaultTagProvider(Guid artifactKind)
    {
      this.ArtifactKind = artifactKind;
      this.StorageProvider = (ITagStorageProvider<T>) new PropertyServiceTagStorageProvider<T>(artifactKind);
      this.SecurityProvider = (ITagSecurityProvider<T>) null;
    }

    public Guid ArtifactKind { get; private set; }

    public ITagStorageProvider<T> StorageProvider { get; private set; }

    public ITagSecurityProvider<T> SecurityProvider { get; private set; }

    public IEnumerable<Guid> DeleteAllTagAssociations(
      IVssRequestContext requestContext,
      IEnumerable<Guid> tagIds)
    {
      return this.StorageProvider.DeleteHistoryForTags(requestContext, tagIds);
    }

    public static IEnumerable<Guid> GetKnownArtifactKinds(IVssRequestContext requestContext) => PropertyServiceTagStorageProvider<object>.GetKnownArtifactKinds(requestContext);
  }
}
