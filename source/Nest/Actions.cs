// Decompiled with JetBrains decompiler
// Type: Nest.Actions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  [JsonFormatter(typeof (ActionsFormatter))]
  public class Actions : 
    IsADictionaryBase<string, IAction>,
    IActions,
    IIsADictionary<string, IAction>,
    IDictionary<string, IAction>,
    ICollection<KeyValuePair<string, IAction>>,
    IEnumerable<KeyValuePair<string, IAction>>,
    IEnumerable,
    IIsADictionary
  {
    public Actions()
    {
    }

    public Actions(IDictionary<string, IAction> actions)
      : base(Actions.ReduceCombinators(actions))
    {
    }

    private static IDictionary<string, IAction> ReduceCombinators(
      IDictionary<string, IAction> actions)
    {
      if (!actions.Values.OfType<ActionCombinator>().Any<ActionCombinator>())
        return actions;
      Dictionary<string, IAction> dictionary = new Dictionary<string, IAction>(actions.Count);
      foreach (KeyValuePair<string, IAction> action1 in (IEnumerable<KeyValuePair<string, IAction>>) actions)
      {
        if (action1.Value is ActionCombinator actionCombinator)
        {
          foreach (ActionBase action2 in actionCombinator.Actions)
          {
            if (action2.Name.IsNullOrEmpty())
              throw new ArgumentException(action2.GetType().Name + ".Name is not set!");
            dictionary.Add(action2.Name, (IAction) action2);
          }
        }
        else
          dictionary.Add(action1.Key, action1.Value);
      }
      return (IDictionary<string, IAction>) dictionary;
    }

    public static implicit operator Actions(ActionBase action)
    {
      if (action == null)
        return (Actions) null;
      if (action is ActionCombinator actionCombinator)
      {
        Dictionary<string, IAction> actions = new Dictionary<string, IAction>(actionCombinator.Actions.Count);
        foreach (ActionBase action1 in actionCombinator.Actions)
        {
          if (action1.Name.IsNullOrEmpty())
            throw new ArgumentException(action1.GetType().Name + ".Name is not set!");
          actions.Add(action1.Name, (IAction) action1);
        }
        return new Actions((IDictionary<string, IAction>) actions);
      }
      return !action.Name.IsNullOrEmpty() ? new Actions((IDictionary<string, IAction>) new Dictionary<string, IAction>()
      {
        {
          action.Name,
          (IAction) action
        }
      }) : throw new ArgumentException(action.GetType().Name + ".Name is not set!");
    }
  }
}
