// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.IGitPackIndex
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal interface IGitPackIndex : IDisposable
  {
    Sha1Id? Id { get; }

    GitPackIndexVersion? Version { get; }

    IReadOnlyList<Sha1Id> BaseIndexIds { get; }

    IReadOnlyList<Sha1Id> PackIds { get; }

    IReadOnlyList<GitPackStates> PackStates { get; }

    IReadOnlyGitPackWatermarks PackWatermarks { get; }

    ISha1IdTwoWayReadOnlyList ObjectIds { get; }

    IReadOnlyList<GitPackIndexEntry> Entries { get; }

    Sha1Id? StableObjectOrderEpoch { get; }
  }
}
