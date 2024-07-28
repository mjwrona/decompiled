// Decompiled with JetBrains decompiler
// Type: Nest.ActionCombinator
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  internal class ActionCombinator : ActionBase, IAction
  {
    public ActionCombinator(ActionBase left, ActionBase right)
      : base((string) null)
    {
      this.AddAction(left);
      this.AddAction(right);
    }

    public override ActionType ActionType => (ActionType) 10;

    internal List<ActionBase> Actions { get; } = new List<ActionBase>();

    private void AddAction(ActionBase agg)
    {
      if (agg == null)
        return;
      if ((agg is ActionCombinator actionCombinator ? new bool?(actionCombinator.Actions.HasAny<ActionBase>()) : new bool?()).GetValueOrDefault(false))
        this.Actions.AddRange((IEnumerable<ActionBase>) actionCombinator.Actions);
      else
        this.Actions.Add(agg);
    }
  }
}
