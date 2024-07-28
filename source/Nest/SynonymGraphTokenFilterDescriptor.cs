// Decompiled with JetBrains decompiler
// Type: Nest.SynonymGraphTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class SynonymGraphTokenFilterDescriptor : 
    TokenFilterDescriptorBase<SynonymGraphTokenFilterDescriptor, ISynonymGraphTokenFilter>,
    ISynonymGraphTokenFilter,
    ITokenFilter
  {
    protected override string Type => "synonym_graph";

    bool? ISynonymGraphTokenFilter.Expand { get; set; }

    SynonymFormat? ISynonymGraphTokenFilter.Format { get; set; }

    bool? ISynonymGraphTokenFilter.Lenient { get; set; }

    IEnumerable<string> ISynonymGraphTokenFilter.Synonyms { get; set; }

    string ISynonymGraphTokenFilter.SynonymsPath { get; set; }

    string ISynonymGraphTokenFilter.Tokenizer { get; set; }

    bool? ISynonymGraphTokenFilter.Updateable { get; set; }

    public SynonymGraphTokenFilterDescriptor Expand(bool? expand = true) => this.Assign<bool?>(expand, (Action<ISynonymGraphTokenFilter, bool?>) ((a, v) => a.Expand = v));

    public SynonymGraphTokenFilterDescriptor Lenient(bool? lenient = true) => this.Assign<bool?>(lenient, (Action<ISynonymGraphTokenFilter, bool?>) ((a, v) => a.Lenient = v));

    public SynonymGraphTokenFilterDescriptor Tokenizer(string tokenizer) => this.Assign<string>(tokenizer, (Action<ISynonymGraphTokenFilter, string>) ((a, v) => a.Tokenizer = v));

    public SynonymGraphTokenFilterDescriptor SynonymsPath(string path) => this.Assign<string>(path, (Action<ISynonymGraphTokenFilter, string>) ((a, v) => a.SynonymsPath = v));

    public SynonymGraphTokenFilterDescriptor Format(SynonymFormat? format) => this.Assign<SynonymFormat?>(format, (Action<ISynonymGraphTokenFilter, SynonymFormat?>) ((a, v) => a.Format = v));

    public SynonymGraphTokenFilterDescriptor Synonyms(IEnumerable<string> synonymGraphs) => this.Assign<IEnumerable<string>>(synonymGraphs, (Action<ISynonymGraphTokenFilter, IEnumerable<string>>) ((a, v) => a.Synonyms = v));

    public SynonymGraphTokenFilterDescriptor Synonyms(params string[] synonymGraphs) => this.Assign<string[]>(synonymGraphs, (Action<ISynonymGraphTokenFilter, string[]>) ((a, v) => a.Synonyms = (IEnumerable<string>) v));

    public SynonymGraphTokenFilterDescriptor Updateable(bool? updateable = true) => this.Assign<bool?>(updateable, (Action<ISynonymGraphTokenFilter, bool?>) ((a, v) => a.Updateable = v));
  }
}
