// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.CompositeVariablesEnumerator
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml
{
  internal sealed class CompositeVariablesEnumerator : 
    IEnumerator<KeyValuePair<string, string>>,
    IDisposable,
    IEnumerator
  {
    private bool m_processingSystemVariables = true;
    private IEnumerator<KeyValuePair<string, string>> m_systemVariablesEnumerator;
    private IEnumerator<KeyValuePair<string, string>> m_userVariablesEnumerator;

    public CompositeVariablesEnumerator(
      IEnumerator<KeyValuePair<string, string>> systemVariablesEnumerator,
      IEnumerator<KeyValuePair<string, string>> userVariablesEnumerator)
    {
      this.m_systemVariablesEnumerator = systemVariablesEnumerator;
      this.m_userVariablesEnumerator = userVariablesEnumerator;
    }

    public KeyValuePair<string, string> Current
    {
      get
      {
        KeyValuePair<string, string> keyValuePair = this.m_processingSystemVariables ? this.m_systemVariablesEnumerator.Current : this.m_userVariablesEnumerator.Current;
        return new KeyValuePair<string, string>(keyValuePair.Key, keyValuePair.Value);
      }
    }

    object IEnumerator.Current => (object) this.Current;

    public void Dispose()
    {
      this.m_systemVariablesEnumerator?.Dispose();
      this.m_systemVariablesEnumerator = (IEnumerator<KeyValuePair<string, string>>) null;
      this.m_userVariablesEnumerator?.Dispose();
      this.m_userVariablesEnumerator = (IEnumerator<KeyValuePair<string, string>>) null;
    }

    public bool MoveNext()
    {
      if (this.m_processingSystemVariables)
      {
        if (this.m_systemVariablesEnumerator.MoveNext())
          return true;
        this.m_processingSystemVariables = false;
      }
      return this.m_userVariablesEnumerator.MoveNext();
    }

    public void Reset()
    {
      this.m_processingSystemVariables = true;
      this.m_systemVariablesEnumerator.Reset();
      this.m_userVariablesEnumerator.Reset();
    }
  }
}
