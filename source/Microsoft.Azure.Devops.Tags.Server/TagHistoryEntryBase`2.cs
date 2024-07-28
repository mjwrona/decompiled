// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.TagHistoryEntryBase`2
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Azure.Devops.Tags.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class TagHistoryEntryBase<T, Tag>
  {
    public TagHistoryEntryBase(
      VersionedTagArtifact<T> artifactVersion,
      IEnumerable<Tag> tags,
      DateTime changedDate,
      Guid changedBy)
    {
      this.ArtifactVersion = artifactVersion;
      this.Tags = tags;
      this.ChangedDate = changedDate;
      this.ChangedBy = changedBy;
    }

    public VersionedTagArtifact<T> ArtifactVersion { get; private set; }

    public IEnumerable<Tag> Tags { get; private set; }

    public DateTime ChangedDate { get; private set; }

    public Guid ChangedBy { get; private set; }
  }
}
