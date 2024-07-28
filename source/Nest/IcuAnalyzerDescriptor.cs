// Decompiled with JetBrains decompiler
// Type: Nest.IcuAnalyzerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class IcuAnalyzerDescriptor : 
    AnalyzerDescriptorBase<IcuAnalyzerDescriptor, IIcuAnalyzer>,
    IIcuAnalyzer,
    IAnalyzer
  {
    protected override string Type => "icu_analyzer";

    IcuNormalizationType? IIcuAnalyzer.Method { get; set; }

    IcuNormalizationMode? IIcuAnalyzer.Mode { get; set; }

    public IcuAnalyzerDescriptor Method(IcuNormalizationType? method) => this.Assign<IcuNormalizationType?>(method, (Action<IIcuAnalyzer, IcuNormalizationType?>) ((a, v) => a.Method = v));

    public IcuAnalyzerDescriptor Mode(IcuNormalizationMode? mode) => this.Assign<IcuNormalizationMode?>(mode, (Action<IIcuAnalyzer, IcuNormalizationMode?>) ((a, v) => a.Mode = v));
  }
}
