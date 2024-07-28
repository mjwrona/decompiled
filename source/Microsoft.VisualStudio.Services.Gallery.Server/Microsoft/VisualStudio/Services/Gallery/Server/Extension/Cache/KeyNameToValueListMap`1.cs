// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.KeyNameToValueListMap`1
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache
{
  internal class KeyNameToValueListMap<T>
  {
    private readonly IDictionary<string, List<T>> m_Dictionary;

    public KeyNameToValueListMap() => this.m_Dictionary = (IDictionary<string, List<T>>) new Dictionary<string, List<T>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public bool TryAddValueToKey(string keyName, T value)
    {
      if (string.IsNullOrEmpty(keyName) || (object) value == null)
        return false;
      List<T> objList;
      if (!this.m_Dictionary.TryGetValue(keyName, out objList))
      {
        objList = new List<T>();
        this.m_Dictionary.Add(keyName, objList);
      }
      objList.Add(value);
      return true;
    }

    public bool TryAddValueToKeys(List<string> keyNames, T value)
    {
      bool keys = true;
      if (keyNames != null)
      {
        for (int index = 0; index < keyNames.Count; ++index)
          keys = keys && this.TryAddValueToKey(keyNames[index], value);
      }
      return keys;
    }

    public bool TryGetValueListForKey(string keyName, out List<T> valueListForKey)
    {
      if (!string.IsNullOrEmpty(keyName) && this.m_Dictionary.TryGetValue(keyName, out valueListForKey))
        return true;
      valueListForKey = (List<T>) null;
      return false;
    }

    public bool TryRemoveValueFromKey(string keyName, T value)
    {
      List<T> objList;
      if (string.IsNullOrEmpty(keyName) || !this.m_Dictionary.TryGetValue(keyName, out objList))
        return false;
      int num = objList.Remove(value) ? 1 : 0;
      if (objList.Count != 0)
        return num != 0;
      this.m_Dictionary.Remove(keyName);
      return num != 0;
    }

    public bool TryRemoveValueFromKeys(List<string> keyNames, T value)
    {
      bool flag = true;
      if (keyNames != null)
      {
        for (int index = 0; index < keyNames.Count; ++index)
          flag = flag && this.TryRemoveValueFromKey(keyNames[index], value);
      }
      return flag;
    }
  }
}
