// Decompiled with JetBrains decompiler
// Type: Nest.ScriptConditionBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public abstract class ScriptConditionBase : ConditionBase, IScriptCondition, ICondition
  {
    public string Lang { get; set; }

    public IDictionary<string, object> Params { get; set; }

    internal override void WrapInContainer(IConditionContainer container) => container.Script = (IScriptCondition) this;
  }
}
