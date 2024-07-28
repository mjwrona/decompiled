// Decompiled with JetBrains decompiler
// Type: Nest.StopAnalyzer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class StopAnalyzer : AnalyzerBase, IStopAnalyzer, IAnalyzer
  {
    public StopAnalyzer()
      : base("stop")
    {
    }

    public StopWords StopWords { get; set; }

    public string StopwordsPath { get; set; }
  }
}
