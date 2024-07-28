// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ReadOnlySequenceToken
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens
{
  internal sealed class ReadOnlySequenceToken : 
    IReadOnlyTemplateToken,
    IReadOnlyList<IReadOnlyTemplateToken>,
    IReadOnlyCollection<IReadOnlyTemplateToken>,
    IEnumerable<IReadOnlyTemplateToken>,
    IEnumerable
  {
    private readonly SequenceToken m_sequence;

    internal ReadOnlySequenceToken(SequenceToken sequence) => this.m_sequence = sequence;

    public IReadOnlyTemplateToken this[int index] => this.m_sequence[index]?.ToReadOnly();

    public int? FileId => this.m_sequence.FileId;

    public int? Line => this.m_sequence.Line;

    public int? Column => this.m_sequence.Column;

    public int Count => this.m_sequence.Count;

    public IEnumerator<IReadOnlyTemplateToken> GetEnumerator() => this.CreateEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.CreateEnumerator();

    private IEnumerator<IReadOnlyTemplateToken> CreateEnumerator() => this.m_sequence.Select<TemplateToken, IReadOnlyTemplateToken>((Func<TemplateToken, IReadOnlyTemplateToken>) (x => x?.ToReadOnly())).GetEnumerator();
  }
}
