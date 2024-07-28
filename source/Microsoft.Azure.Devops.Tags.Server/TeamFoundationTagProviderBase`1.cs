// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.TeamFoundationTagProviderBase`1
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Devops.Tags.Server
{
  public abstract class TeamFoundationTagProviderBase<T> : ITagProvider<T>, ITagProvider
  {
    private Guid m_artifactKind;

    protected TeamFoundationTagProviderBase() => this.Initialize();

    public virtual Guid ArtifactKind => this.m_artifactKind;

    public IEnumerable<Guid> DeleteAllTagAssociations(
      IVssRequestContext requestContext,
      IEnumerable<Guid> tagIds)
    {
      return (this.StorageProvider ?? new DefaultTagProvider<T>(this.m_artifactKind).StorageProvider).DeleteHistoryForTags(requestContext, tagIds);
    }

    public abstract ITagStorageProvider<T> StorageProvider { get; }

    public abstract ITagSecurityProvider<T> SecurityProvider { get; }

    private void Initialize()
    {
      object[] customAttributes = this.GetType().GetCustomAttributes(typeof (TagProviderAttribute), true);
      this.m_artifactKind = customAttributes.Length != 0 ? ((TagProviderAttribute) customAttributes[0]).ArtifactKind : throw new InvalidOperationException("TagProvider attribute is missing.");
    }
  }
}
