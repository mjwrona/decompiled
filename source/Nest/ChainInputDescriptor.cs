// Decompiled with JetBrains decompiler
// Type: Nest.ChainInputDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class ChainInputDescriptor : 
    DescriptorBase<ChainInputDescriptor, IChainInput>,
    IChainInput,
    IInput
  {
    public ChainInputDescriptor()
    {
    }

    public ChainInputDescriptor(IDictionary<string, InputContainer> inputs) => this.Self.Inputs = inputs;

    IDictionary<string, InputContainer> IChainInput.Inputs { get; set; }

    public ChainInputDescriptor Input(string name, Func<InputDescriptor, InputContainer> selector)
    {
      if (this.Self.Inputs != null)
      {
        if (this.Self.Inputs.ContainsKey(name))
          throw new InvalidOperationException("An input named '" + name + "' has already been specified. Choose a different name");
      }
      else
        this.Self.Inputs = (IDictionary<string, InputContainer>) new Dictionary<string, InputContainer>();
      this.Self.Inputs.Add(name, selector.InvokeOrDefault<InputDescriptor, InputContainer>(new InputDescriptor()));
      return this;
    }
  }
}
