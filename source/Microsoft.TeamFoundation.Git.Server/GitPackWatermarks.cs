// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitPackWatermarks
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class GitPackWatermarks : 
    IReadOnlyGitPackWatermarks,
    IEnumerable<KeyValuePair<GitPackWatermark, ushort>>,
    IEnumerable
  {
    public static readonly IReadOnlyGitPackWatermarks Empty = (IReadOnlyGitPackWatermarks) new GitPackWatermarks();

    public ushort this[GitPackWatermark key] => this.Inner.GetValueOrDefault<GitPackWatermark, ushort>(key, (ushort) 0);

    public IEnumerable<KeyValuePair<GitPackWatermark, ushort>> NonZero => this.Inner.Where<KeyValuePair<GitPackWatermark, ushort>>((Func<KeyValuePair<GitPackWatermark, ushort>, bool>) (x => x.Value > (ushort) 0));

    public void Add(GitPackWatermark key, ushort value) => this.Inner.Add(key, value);

    IEnumerator<KeyValuePair<GitPackWatermark, ushort>> IEnumerable<KeyValuePair<GitPackWatermark, ushort>>.GetEnumerator() => (IEnumerator<KeyValuePair<GitPackWatermark, ushort>>) this.Inner.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.Inner.GetEnumerator();

    public Dictionary<GitPackWatermark, ushort> Inner { get; } = new Dictionary<GitPackWatermark, ushort>();
  }
}
