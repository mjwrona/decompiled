// Decompiled with JetBrains decompiler
// Type: Nest.ProcessorBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public abstract class ProcessorBase : IProcessor
  {
    public string If { get; set; }

    public string Tag { get; set; }

    public bool? IgnoreFailure { get; set; }

    public IEnumerable<IProcessor> OnFailure { get; set; }

    protected abstract string Name { get; }

    public string Description { get; set; }

    string IProcessor.Name => this.Name;
  }
}
