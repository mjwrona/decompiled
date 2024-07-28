// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Native.AsyncOperationResult
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using LibGit2Sharp;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Native
{
  internal class AsyncOperationResult
  {
    public bool Success { get; set; }

    public bool TimeOut { get; set; }

    public List<Sha1Id> Commits { get; } = new List<Sha1Id>();

    public IEnumerable<Conflict> Conflicts { get; set; }
  }
}
