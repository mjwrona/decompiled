// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ITfsGitContentDB`1
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal interface ITfsGitContentDB<TRawKey> : IContentDB, IDisposable
  {
    bool TryLookupObject(Sha1Id objectId, out GitPackObjectType packType, out TRawKey rawKey);

    Stream GetContent(TRawKey rawKey, Sha1Id objectId);

    Stream GetRawContent(TRawKey rawKey);

    IEnumerable<TRawKey> GetAllObjectLocations();
  }
}
