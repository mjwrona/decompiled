// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitKnownFilesBuilder
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class GitKnownFilesBuilder
  {
    private readonly Dictionary<string, KnownFile> m_createQueue;

    public GitKnownFilesBuilder() => this.m_createQueue = new Dictionary<string, KnownFile>();

    public void QueueExtant(string name, KnownFileType type, DateTime? actionDate = null) => this.m_createQueue[name] = new KnownFile(type, actionDate ?? DateTime.UtcNow);

    public IReadOnlyDictionary<string, KnownFile> GetCreates() => (IReadOnlyDictionary<string, KnownFile>) this.m_createQueue;
  }
}
