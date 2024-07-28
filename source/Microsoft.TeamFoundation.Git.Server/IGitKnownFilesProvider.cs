// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.IGitKnownFilesProvider
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal interface IGitKnownFilesProvider
  {
    IReadOnlyDictionary<string, KnownFile> Read();

    IReadOnlyDictionary<string, KnownFileIntervalData> ReadInterval(
      DateTime? minCreatedDate = null,
      int? minIntervals = null);

    void ResetIntervals(OdbId odbId);

    void Update(IReadOnlyDictionary<string, KnownFile> creates);

    void UpdateIntervals(ISet<string> increments, ISet<string> resets);

    void Delete(IEnumerable<string> filesToDelete);
  }
}
