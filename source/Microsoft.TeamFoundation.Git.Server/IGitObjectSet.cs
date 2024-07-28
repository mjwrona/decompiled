// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.IGitObjectSet
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  public interface IGitObjectSet : IDisposable
  {
    bool TryGetContent(Sha1Id objectId, out Stream content, out GitObjectType objectType);

    GitObjectType TryLookupObjectType(Sha1Id objectId);

    IEnumerable<TGitObject> FindObjectsBetween<TGitObject>(Sha1Id fromId, Sha1Id toId) where TGitObject : TfsGitObject;
  }
}
