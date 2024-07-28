// Decompiled with JetBrains decompiler
// Type: Nest.StepKeyDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class StepKeyDescriptor : DescriptorBase<StepKeyDescriptor, IStepKey>, IStepKey
  {
    string IStepKey.Action { get; set; }

    string IStepKey.Name { get; set; }

    string IStepKey.Phase { get; set; }

    public StepKeyDescriptor Phase(string phase) => this.Assign<string>(phase, (System.Action<IStepKey, string>) ((a, v) => a.Phase = v));

    public StepKeyDescriptor Action(string action) => this.Assign<string>(action, (System.Action<IStepKey, string>) ((a, v) => a.Action = v));

    public StepKeyDescriptor Name(string name) => this.Assign<string>(name, (System.Action<IStepKey, string>) ((a, v) => a.Name = v));
  }
}
