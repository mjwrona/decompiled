// Decompiled with JetBrains decompiler
// Type: Nest.TemplateMapping
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;

namespace Nest
{
  public class TemplateMapping : ITemplateMapping
  {
    public IAliases Aliases { get; set; }

    public IReadOnlyCollection<string> IndexPatterns { get; set; } = EmptyReadOnly<string>.Collection;

    public ITypeMapping Mappings { get; set; }

    public int? Order { get; set; }

    public IIndexSettings Settings { get; set; }

    public int? Version { get; set; }
  }
}
