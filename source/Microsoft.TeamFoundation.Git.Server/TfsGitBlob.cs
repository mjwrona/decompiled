// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitBlob
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public sealed class TfsGitBlob : TfsGitObject
  {
    internal TfsGitBlob(ICachedGitObjectSet objectSet, Sha1Id objectId)
      : base(objectSet, objectId)
    {
    }

    public override GitObjectType ObjectType => GitObjectType.Blob;

    internal override GitPackObjectType PackType => GitPackObjectType.Blob;

    internal override IEnumerable<Sha1Id> ReferencedObjectIds => (IEnumerable<Sha1Id>) Array.Empty<Sha1Id>();
  }
}
