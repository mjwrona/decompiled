// Decompiled with JetBrains decompiler
// Type: Nest.ActionsDescriptorBase`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public abstract class ActionsDescriptorBase<TDescriptor, TInterface> : 
    DescriptorBase<TDescriptor, TInterface>,
    IAction
    where TDescriptor : DescriptorBase<TDescriptor, TInterface>, TInterface
    where TInterface : class, IAction
  {
    private string _name;

    protected ActionsDescriptorBase(string name)
    {
      switch (name)
      {
        case null:
          throw new ArgumentNullException(nameof (name));
        case "":
          throw new ArgumentException("cannot be empty");
        default:
          this._name = name;
          break;
      }
    }

    protected abstract ActionType ActionType { get; }

    ActionType IAction.ActionType => this.ActionType;

    string IAction.Name
    {
      get => this._name;
      set => this._name = value;
    }

    Time IAction.ThrottlePeriod { get; set; }

    TransformContainer IAction.Transform { get; set; }

    ConditionContainer IAction.Condition { get; set; }

    string IAction.Foreach { get; set; }

    int? IAction.MaxIterations { get; set; }

    public TDescriptor Transform(
      Func<TransformDescriptor, TransformContainer> selector)
    {
      return this.Assign<TransformContainer>(selector.InvokeOrDefault<TransformDescriptor, TransformContainer>(new TransformDescriptor()), (Action<TInterface, TransformContainer>) ((a, v) => a.Transform = v));
    }

    public TDescriptor Condition(
      Func<ConditionDescriptor, ConditionContainer> selector)
    {
      return this.Assign<ConditionContainer>(selector.InvokeOrDefault<ConditionDescriptor, ConditionContainer>(new ConditionDescriptor()), (Action<TInterface, ConditionContainer>) ((a, v) => a.Condition = v));
    }

    public TDescriptor ThrottlePeriod(Time throttlePeriod) => this.Assign<Time>(throttlePeriod, (Action<TInterface, Time>) ((a, v) => a.ThrottlePeriod = v));

    public TDescriptor Foreach(string @foreach) => this.Assign<string>(@foreach, (Action<TInterface, string>) ((a, v) => a.Foreach = v));

    public TDescriptor MaxIterations(int? maxIterations) => this.Assign<int?>(maxIterations, (Action<TInterface, int?>) ((a, v) => a.MaxIterations = v));
  }
}
