// Decompiled with JetBrains decompiler
// Type: Nest.SynonymTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class SynonymTokenFilterDescriptor : 
    TokenFilterDescriptorBase<SynonymTokenFilterDescriptor, ISynonymTokenFilter>,
    ISynonymTokenFilter,
    ITokenFilter
  {
    protected override string Type => "synonym";

    bool? ISynonymTokenFilter.Expand { get; set; }

    SynonymFormat? ISynonymTokenFilter.Format { get; set; }

    bool? ISynonymTokenFilter.Lenient { get; set; }

    IEnumerable<string> ISynonymTokenFilter.Synonyms { get; set; }

    string ISynonymTokenFilter.SynonymsPath { get; set; }

    string ISynonymTokenFilter.Tokenizer { get; set; }

    bool? ISynonymTokenFilter.Updateable { get; set; }

    public SynonymTokenFilterDescriptor Expand(bool? expand = true) => this.Assign<bool?>(expand, (Action<ISynonymTokenFilter, bool?>) ((a, v) => a.Expand = v));

    public SynonymTokenFilterDescriptor Lenient(bool? lenient = true) => this.Assign<bool?>(lenient, (Action<ISynonymTokenFilter, bool?>) ((a, v) => a.Lenient = v));

    public SynonymTokenFilterDescriptor Tokenizer(string tokenizer) => this.Assign<string>(tokenizer, (Action<ISynonymTokenFilter, string>) ((a, v) => a.Tokenizer = v));

    public SynonymTokenFilterDescriptor SynonymsPath(string path) => this.Assign<string>(path, (Action<ISynonymTokenFilter, string>) ((a, v) => a.SynonymsPath = v));

    public SynonymTokenFilterDescriptor Format(SynonymFormat? format) => this.Assign<SynonymFormat?>(format, (Action<ISynonymTokenFilter, SynonymFormat?>) ((a, v) => a.Format = v));

    public SynonymTokenFilterDescriptor Synonyms(IEnumerable<string> synonyms) => this.Assign<IEnumerable<string>>(synonyms, (Action<ISynonymTokenFilter, IEnumerable<string>>) ((a, v) => a.Synonyms = v));

    public SynonymTokenFilterDescriptor Synonyms(params string[] synonyms) => this.Assign<string[]>(synonyms, (Action<ISynonymTokenFilter, string[]>) ((a, v) => a.Synonyms = (IEnumerable<string>) v));

    public SynonymTokenFilterDescriptor Updateable(bool? updateable = true) => this.Assign<bool?>(updateable, (Action<ISynonymTokenFilter, bool?>) ((a, v) => a.Updateable = v));
  }
}
