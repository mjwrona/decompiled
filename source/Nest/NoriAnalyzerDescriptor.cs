// Decompiled with JetBrains decompiler
// Type: Nest.NoriAnalyzerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class NoriAnalyzerDescriptor : 
    AnalyzerDescriptorBase<NoriAnalyzerDescriptor, INoriAnalyzer>,
    INoriAnalyzer,
    IAnalyzer
  {
    protected override string Type => "nori";

    NoriDecompoundMode? INoriAnalyzer.DecompoundMode { get; set; }

    IEnumerable<string> INoriAnalyzer.StopTags { get; set; }

    string INoriAnalyzer.UserDictionary { get; set; }

    public NoriAnalyzerDescriptor DecompoundMode(NoriDecompoundMode? mode) => this.Assign<NoriDecompoundMode?>(mode, (Action<INoriAnalyzer, NoriDecompoundMode?>) ((a, v) => a.DecompoundMode = v));

    public NoriAnalyzerDescriptor UserDictionary(string path) => this.Assign<string>(path, (Action<INoriAnalyzer, string>) ((a, v) => a.UserDictionary = v));

    public NoriAnalyzerDescriptor StopTags(IEnumerable<string> stopTags) => this.Assign<IEnumerable<string>>(stopTags, (Action<INoriAnalyzer, IEnumerable<string>>) ((a, v) => a.StopTags = v));

    public NoriAnalyzerDescriptor StopTags(params string[] stopTags) => this.Assign<string[]>(stopTags, (Action<INoriAnalyzer, string[]>) ((a, v) => a.StopTags = (IEnumerable<string>) v));
  }
}
