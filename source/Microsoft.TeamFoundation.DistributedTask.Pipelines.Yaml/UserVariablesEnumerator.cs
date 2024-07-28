// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.UserVariablesEnumerator
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml
{
  internal class UserVariablesEnumerator : 
    IEnumerator<KeyValuePair<string, string>>,
    IDisposable,
    IEnumerator
  {
    private readonly Stack<Dictionary<string, string>> m_dictionaries = new Stack<Dictionary<string, string>>();
    private readonly UserVariablesInternal m_userVariables;
    private IEnumerator<KeyValuePair<string, string>> m_enumerator;

    public UserVariablesEnumerator(UserVariablesInternal scope)
    {
      this.m_userVariables = scope;
      this.PushDictionaries();
    }

    public KeyValuePair<string, string> Current => this.m_enumerator.Current;

    object IEnumerator.Current => (object) this.m_enumerator.Current;

    public void Dispose()
    {
      this.m_enumerator?.Dispose();
      this.m_enumerator = (IEnumerator<KeyValuePair<string, string>>) null;
    }

    public bool MoveNext()
    {
      bool flag;
      do
      {
        if (this.m_enumerator == null)
        {
          if (this.m_dictionaries.Count == 0)
            return false;
          this.m_enumerator = (IEnumerator<KeyValuePair<string, string>>) this.m_dictionaries.Pop().GetEnumerator();
        }
        if (!this.m_enumerator.MoveNext())
        {
          this.m_enumerator.Dispose();
          this.m_enumerator = (IEnumerator<KeyValuePair<string, string>>) null;
        }
        else
        {
          flag = false;
          string key = this.m_enumerator.Current.Key;
          foreach (Dictionary<string, string> dictionary in this.m_dictionaries)
          {
            if (dictionary.ContainsKey(key))
            {
              flag = true;
              break;
            }
          }
        }
      }
      while (flag);
      return true;
    }

    public void Reset()
    {
      this.Dispose();
      this.m_dictionaries.Clear();
      this.PushDictionaries();
    }

    private void PushDictionaries()
    {
      for (UserVariablesInternal variablesInternal = this.m_userVariables; variablesInternal != null; variablesInternal = variablesInternal.Parent)
        this.m_dictionaries.Push(variablesInternal.Dictionary);
    }
  }
}
