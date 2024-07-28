// Decompiled with JetBrains decompiler
// Type: Nest.AnalyzerDescriptorBase`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public abstract class AnalyzerDescriptorBase<TAnalyzer, TAnalyzerInterface> : 
    DescriptorBase<TAnalyzer, TAnalyzerInterface>,
    IAnalyzer
    where TAnalyzer : AnalyzerDescriptorBase<TAnalyzer, TAnalyzerInterface>, TAnalyzerInterface
    where TAnalyzerInterface : class, IAnalyzer
  {
    protected abstract string Type { get; }

    string IAnalyzer.Type => this.Type;

    string IAnalyzer.Version { get; set; }

    [Obsolete("Setting a version on analysis component has no effect and is deprecated.")]
    public TAnalyzer Version(string version) => this.Assign<string>(version, (Action<TAnalyzerInterface, string>) ((a, v) => a.Version = v));
  }
}
