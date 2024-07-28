// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.ArtifactTags`1
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Azure.Devops.Tags.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ArtifactTags<T> : ArtifactTagBase<T, TagDefinition>
  {
    public ArtifactTags(VersionedTagArtifact<T> artifactId, IEnumerable<TagDefinition> tags)
      : base(artifactId, tags)
    {
    }
  }
}
