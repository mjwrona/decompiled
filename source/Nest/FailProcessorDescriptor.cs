// Decompiled with JetBrains decompiler
// Type: Nest.FailProcessorDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class FailProcessorDescriptor : 
    ProcessorDescriptorBase<FailProcessorDescriptor, IFailProcessor>,
    IFailProcessor,
    IProcessor
  {
    protected override string Name => "fail";

    string IFailProcessor.Message { get; set; }

    public FailProcessorDescriptor Message(string message) => this.Assign<string>(message, (Action<IFailProcessor, string>) ((a, v) => a.Message = v));
  }
}
