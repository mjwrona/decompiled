// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ReadOnlyMappingToken
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens
{
  internal sealed class ReadOnlyMappingToken : 
    IReadOnlyList<KeyValuePair<ReadOnlyLiteralToken, IReadOnlyTemplateToken>>,
    IReadOnlyCollection<KeyValuePair<ReadOnlyLiteralToken, IReadOnlyTemplateToken>>,
    IEnumerable<KeyValuePair<ReadOnlyLiteralToken, IReadOnlyTemplateToken>>,
    IEnumerable,
    IReadOnlyTemplateToken
  {
    private readonly MappingToken m_mapping;

    internal ReadOnlyMappingToken(MappingToken mapping) => this.m_mapping = mapping;

    public KeyValuePair<ReadOnlyLiteralToken, IReadOnlyTemplateToken> this[int index] => this.CreateReadOnlyPair(this.m_mapping[index]);

    public int? FileId => this.m_mapping.FileId;

    public int? Line => this.m_mapping.Line;

    public int? Column => this.m_mapping.Column;

    public int Count => this.m_mapping.Count;

    public IEnumerator<KeyValuePair<ReadOnlyLiteralToken, IReadOnlyTemplateToken>> GetEnumerator() => this.CreateEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.CreateEnumerator();

    private IEnumerator<KeyValuePair<ReadOnlyLiteralToken, IReadOnlyTemplateToken>> CreateEnumerator() => this.m_mapping.Select<KeyValuePair<ScalarToken, TemplateToken>, KeyValuePair<ReadOnlyLiteralToken, IReadOnlyTemplateToken>>((Func<KeyValuePair<ScalarToken, TemplateToken>, KeyValuePair<ReadOnlyLiteralToken, IReadOnlyTemplateToken>>) (x => this.CreateReadOnlyPair(x))).GetEnumerator();

    private KeyValuePair<ReadOnlyLiteralToken, IReadOnlyTemplateToken> CreateReadOnlyPair(
      KeyValuePair<ScalarToken, TemplateToken> pair)
    {
      return new KeyValuePair<ReadOnlyLiteralToken, IReadOnlyTemplateToken>(pair.Key?.ToReadOnly() as ReadOnlyLiteralToken, pair.Value?.ToReadOnly());
    }
  }
}
