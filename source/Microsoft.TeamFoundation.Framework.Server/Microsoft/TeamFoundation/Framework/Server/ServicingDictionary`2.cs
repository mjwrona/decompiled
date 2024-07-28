// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingDictionary`2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingDictionary<TKey, TValue> : 
    IDictionary<TKey, TValue>,
    ICollection<KeyValuePair<TKey, TValue>>,
    IEnumerable<KeyValuePair<TKey, TValue>>,
    IEnumerable
  {
    private ServicingContext m_servicingContext;
    private string m_elementType;
    private IDictionary<TKey, TValue> m_backingDictionary;
    private HashSet<TKey> m_secretTokenNames;

    public ServicingDictionary(
      ServicingContext servicingContext,
      string elementType,
      IEqualityComparer<TKey> comparer)
    {
      this.m_servicingContext = servicingContext;
      this.m_elementType = elementType;
      this.m_secretTokenNames = new HashSet<TKey>(comparer);
      this.m_backingDictionary = (IDictionary<TKey, TValue>) new ConcurrentDictionary<TKey, TValue>(comparer);
      this.m_servicingContext.LogInfo("Creating empty dictionary for {0}s.", new object[1]
      {
        (object) elementType
      });
    }

    public ServicingDictionary(
      ServicingContext servicingContext,
      string elementType,
      IDictionary<TKey, TValue> initialItems,
      IEqualityComparer<TKey> comparer)
    {
      this.m_servicingContext = servicingContext;
      this.m_elementType = elementType;
      this.m_secretTokenNames = new HashSet<TKey>(comparer);
      this.m_backingDictionary = (IDictionary<TKey, TValue>) new ConcurrentDictionary<TKey, TValue>((IEnumerable<KeyValuePair<TKey, TValue>>) initialItems, comparer);
      this.m_servicingContext.LogInfo("Creating dictionary with {0} initial {1}s:", new object[2]
      {
        (object) initialItems.Count,
        (object) elementType
      });
      foreach (KeyValuePair<TKey, TValue> initialItem in (IEnumerable<KeyValuePair<TKey, TValue>>) initialItems)
        this.m_servicingContext.LogInfo("\t{0} => {1}", new object[2]
        {
          (object) initialItem.Key,
          (object) this.GetValueDescription(initialItem.Key, initialItem.Value)
        });
    }

    public void SetSecretTokenNames(IEnumerable<TKey> secretTokenNames)
    {
      this.m_servicingContext.LogInfo("Setting secret token names: {0}", new object[1]
      {
        (object) string.Join<TKey>(",", secretTokenNames)
      });
      this.m_secretTokenNames.AddRange<TKey, HashSet<TKey>>(secretTokenNames);
    }

    public void Add(TKey key, TValue value) => this.Add(new KeyValuePair<TKey, TValue>(key, value));

    public void Add(KeyValuePair<TKey, TValue> item)
    {
      this.m_servicingContext.LogInfo("Adding {0}. Key: {1}. {2}.", new object[3]
      {
        (object) this.m_elementType,
        (object) item.Key,
        (object) this.GetValueDescription(item.Key, item.Value)
      });
      try
      {
        this.m_backingDictionary.Add(item);
      }
      catch (Exception ex)
      {
        throw new TeamFoundationServicingException(FrameworkResources.ErrorAccessingDictionaryKey((object) this.m_elementType, (object) item.Key), ex);
      }
    }

    public bool ContainsKey(TKey key)
    {
      try
      {
        return this.m_backingDictionary.ContainsKey(key);
      }
      catch (Exception ex)
      {
        throw new TeamFoundationServicingException(FrameworkResources.ErrorAccessingDictionaryKey((object) this.m_elementType, (object) key), ex);
      }
    }

    public ICollection<TKey> Keys => this.m_backingDictionary.Keys;

    public bool Remove(TKey key)
    {
      this.m_servicingContext.LogInfo("Removing {0}. Key: {1}.", new object[2]
      {
        (object) this.m_elementType,
        (object) key
      });
      try
      {
        return this.m_backingDictionary.Remove(key);
      }
      catch (Exception ex)
      {
        throw new TeamFoundationServicingException(FrameworkResources.ErrorAccessingDictionaryKey((object) this.m_elementType, (object) key), ex);
      }
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      try
      {
        return this.m_backingDictionary.TryGetValue(key, out value);
      }
      catch (Exception ex)
      {
        throw new TeamFoundationServicingException(FrameworkResources.ErrorAccessingDictionaryKey((object) this.m_elementType, (object) key), ex);
      }
    }

    public ICollection<TValue> Values => this.m_backingDictionary.Values;

    public TValue this[TKey key]
    {
      get
      {
        try
        {
          return this.m_backingDictionary[key];
        }
        catch (Exception ex)
        {
          throw new TeamFoundationServicingException(FrameworkResources.ErrorAccessingDictionaryKey((object) this.m_elementType, (object) key), ex);
        }
      }
      set
      {
        if (!ServicingTokenConstants.CurrentStepName.Equals((object) key) && !ServicingTokenConstants.CurrentStepGroupName.Equals((object) key))
          this.m_servicingContext.LogInfo("Setting {0}. Key: {1}. {2}.", new object[3]
          {
            (object) this.m_elementType,
            (object) key,
            (object) this.GetValueDescription(key, value)
          });
        try
        {
          this.m_backingDictionary[key] = value;
        }
        catch (Exception ex)
        {
          throw new TeamFoundationServicingException(FrameworkResources.ErrorAccessingDictionaryKey((object) this.m_elementType, (object) key), ex);
        }
      }
    }

    public void Clear()
    {
      this.m_servicingContext.LogInfo("Clearing dictionary, removing all {0}s.", new object[1]
      {
        (object) this.m_elementType
      });
      this.m_backingDictionary.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
      try
      {
        return this.m_backingDictionary.Contains(item);
      }
      catch (Exception ex)
      {
        throw new TeamFoundationServicingException(FrameworkResources.ErrorAccessingDictionaryKey((object) this.m_elementType, (object) item.Key), ex);
      }
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => this.m_backingDictionary.CopyTo(array, arrayIndex);

    public int Count => this.m_backingDictionary.Count;

    public bool IsReadOnly => this.m_backingDictionary.IsReadOnly;

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
      this.m_servicingContext.LogInfo("Removing {0}. Key: {1}. {2}.", new object[3]
      {
        (object) this.m_elementType,
        (object) item.Key,
        (object) this.GetValueDescription(item.Key, item.Value)
      });
      try
      {
        return ((ICollection<KeyValuePair<TKey, TValue>>) this.m_backingDictionary).Remove(item);
      }
      catch (Exception ex)
      {
        throw new TeamFoundationServicingException(FrameworkResources.ErrorAccessingDictionaryKey((object) this.m_elementType, (object) item.Key), ex);
      }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => this.m_backingDictionary.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.m_backingDictionary.GetEnumerator();

    internal string GetValueDescription(TKey key, TValue value)
    {
      if ((object) value == null)
        return "Value is null.";
      if (value is string b && string.IsNullOrEmpty(b))
        return "Value is an empty string.";
      if (key is string str1 && (str1.EndsWith("Password", StringComparison.OrdinalIgnoreCase) || str1.EndsWith("Password2", StringComparison.OrdinalIgnoreCase) || str1.EndsWith("PasswordZr", StringComparison.OrdinalIgnoreCase) || str1.EndsWith("Key", StringComparison.OrdinalIgnoreCase) || str1.EndsWith("KeyValue", StringComparison.OrdinalIgnoreCase) || str1.EndsWith("KeySecret", StringComparison.OrdinalIgnoreCase) || str1.EndsWith("SecretPrimary", StringComparison.OrdinalIgnoreCase) || str1.EndsWith("SecretSecondary", StringComparison.OrdinalIgnoreCase)))
        return "Value: *****";
      string a = SecretUtility.ScrubSecrets(value.ToString(), false);
      if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase) && this.m_secretTokenNames.Contains(key))
        return "Value: *****";
      if (b != null || !(value is IEnumerable source))
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Value: {0}", (object) a);
      string str2 = !source.GetEnumerator().MoveNext() ? "(empty collection)" : "{ " + string.Join<object>(", ", source.Cast<object>()) + " }";
      return string.Format("Value: {0} {1}", (object) value, (object) str2);
    }
  }
}
