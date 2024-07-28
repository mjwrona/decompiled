// Decompiled with JetBrains decompiler
// Type: Nest.ActionBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public abstract class ActionBase : IAction
  {
    internal ActionBase()
    {
    }

    protected ActionBase(string name) => this.Name = name;

    public abstract ActionType ActionType { get; }

    public string Name { get; set; }

    public Time ThrottlePeriod { get; set; }

    public string Foreach { get; set; }

    public int? MaxIterations { get; set; }

    public TransformContainer Transform { get; set; }

    public ConditionContainer Condition { get; set; }

    public static bool operator false(ActionBase a) => false;

    public static bool operator true(ActionBase a) => false;

    public static ActionBase operator &(ActionBase left, ActionBase right) => (ActionBase) new ActionCombinator(left, right);
  }
}
