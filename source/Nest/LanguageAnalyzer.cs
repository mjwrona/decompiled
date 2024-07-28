// Decompiled with JetBrains decompiler
// Type: Nest.LanguageAnalyzer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class LanguageAnalyzer : AnalyzerBase, ILanguageAnalyzer, IAnalyzer
  {
    private string _type = "language";

    [IgnoreDataMember]
    public Nest.Language? Language
    {
      get => this._type.ToEnum<Nest.Language>();
      set => this._type = ((Enum) (System.ValueType) value).GetStringValue().ToLowerInvariant();
    }

    public IEnumerable<string> StemExclusionList { get; set; }

    public StopWords StopWords { get; set; }

    public string StopwordsPath { get; set; }

    public override string Type
    {
      get => this._type;
      protected set
      {
        this._type = value;
        this.Language = value.ToEnum<Nest.Language>();
      }
    }
  }
}
