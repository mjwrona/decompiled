// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.ArtifactTagBase`2
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Azure.Devops.Tags.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class ArtifactTagBase<T, Tag>
  {
    public ArtifactTagBase(VersionedTagArtifact<T> artifact, IEnumerable<Tag> tags)
    {
      this.Artifact = artifact;
      this.Tags = tags;
    }

    public VersionedTagArtifact<T> Artifact { get; private set; }

    public IEnumerable<Tag> Tags { get; private set; }
  }
}
