// Decompiled with JetBrains decompiler
// Type: Nest.PipelineProcessor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class PipelineProcessor : ProcessorBase, IPipelineProcessor, IProcessor
  {
    internal const string ProcessorTypeName = "pipeline";

    [DataMember(Name = "name")]
    public string ProcessorName { get; set; }

    protected override string Name => "pipeline";
  }
}
