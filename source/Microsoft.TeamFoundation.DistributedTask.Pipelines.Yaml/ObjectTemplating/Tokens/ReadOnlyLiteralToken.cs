// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ReadOnlyLiteralToken
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens
{
  internal sealed class ReadOnlyLiteralToken : IReadOnlyTemplateToken
  {
    private readonly LiteralToken m_literal;

    internal ReadOnlyLiteralToken(LiteralToken literal) => this.m_literal = literal;

    public int? FileId => this.m_literal.FileId;

    public int? Line => this.m_literal.Line;

    public int? Column => this.m_literal.Column;

    public string Value => this.m_literal.Value;
  }
}
