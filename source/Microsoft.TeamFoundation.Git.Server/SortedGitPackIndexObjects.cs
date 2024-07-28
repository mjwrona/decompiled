// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.SortedGitPackIndexObjects
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class SortedGitPackIndexObjects
  {
    public SortedGitPackIndexObjects(IGitPackIndex index)
    {
      ArgumentUtility.CheckForNull<IGitPackIndex>(index, nameof (index));
      this.RawIndex = index;
      ISha1IdTwoWayReadOnlyList rawObjectIds = index.ObjectIds;
      this.SortedToRawObjectIntIds = new int[rawObjectIds.Count];
      for (int index1 = 0; index1 < this.SortedToRawObjectIntIds.Length; ++index1)
        this.SortedToRawObjectIntIds[index1] = index1;
      Array.Sort<int>(this.SortedToRawObjectIntIds, (Comparison<int>) ((x, y) => rawObjectIds[x].CompareTo(rawObjectIds[y])));
      for (int index2 = 1; index2 < this.SortedToRawObjectIntIds.Length; ++index2)
      {
        Sha1Id sha1Id = rawObjectIds[this.SortedToRawObjectIntIds[index2]];
        if (sha1Id == rawObjectIds[this.SortedToRawObjectIntIds[index2 - 1]])
          throw new InvalidGitIndexException((Exception) new InvalidDataException(FormattableString.Invariant(FormattableStringFactory.Create("Unexpected dupe object: {0}", (object) sha1Id))));
      }
      this.RawToSortedObjectIntIds = new int[rawObjectIds.Count];
      for (int index3 = 0; index3 < this.RawToSortedObjectIntIds.Length; ++index3)
        this.RawToSortedObjectIntIds[this.SortedToRawObjectIntIds[index3]] = index3;
      this.ObjectIds = (ISha1IdTwoWayReadOnlyList) new ReorderedObjectIdList(rawObjectIds, (IReadOnlyList<int>) this.SortedToRawObjectIntIds, (IReadOnlyList<int>) this.RawToSortedObjectIntIds);
      this.Entries = (IReadOnlyList<GitPackIndexEntry>) new ReorderedEntryList(index.Entries, (IReadOnlyList<int>) this.SortedToRawObjectIntIds);
    }

    public IGitPackIndex RawIndex { get; }

    public ISha1IdTwoWayReadOnlyList ObjectIds { get; }

    public IReadOnlyList<GitPackIndexEntry> Entries { get; }

    internal int[] SortedToRawObjectIntIds { get; }

    internal int[] RawToSortedObjectIntIds { get; }

    public void Dispose() => this.RawIndex.Dispose();
  }
}
