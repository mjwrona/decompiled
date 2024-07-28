// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataBatchOperationHeaders
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  public sealed class ODataBatchOperationHeaders : 
    IEnumerable<KeyValuePair<string, string>>,
    IEnumerable
  {
    private readonly Dictionary<string, string> caseSensitiveDictionary;

    public ODataBatchOperationHeaders() => this.caseSensitiveDictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal);

    public string this[string key]
    {
      get
      {
        string str;
        if (this.TryGetValue(key, out str))
          return str;
        throw new KeyNotFoundException(Strings.ODataBatchOperationHeaderDictionary_KeyNotFound((object) key));
      }
      set => this.caseSensitiveDictionary[key] = value;
    }

    public void Add(string key, string value) => this.caseSensitiveDictionary.Add(key, value);

    public bool ContainsKeyOrdinal(string key) => this.caseSensitiveDictionary.ContainsKey(key);

    public bool Remove(string key)
    {
      if (this.caseSensitiveDictionary.Remove(key))
        return true;
      key = this.FindKeyIgnoreCase(key);
      return key != null && this.caseSensitiveDictionary.Remove(key);
    }

    public bool TryGetValue(string key, out string value)
    {
      if (this.caseSensitiveDictionary.TryGetValue(key, out value))
        return true;
      key = this.FindKeyIgnoreCase(key);
      if (key != null)
        return this.caseSensitiveDictionary.TryGetValue(key, out value);
      value = (string) null;
      return false;
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => (IEnumerator<KeyValuePair<string, string>>) this.caseSensitiveDictionary.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.caseSensitiveDictionary.GetEnumerator();

    private string FindKeyIgnoreCase(string key)
    {
      string keyIgnoreCase = (string) null;
      foreach (string key1 in this.caseSensitiveDictionary.Keys)
      {
        if (string.Compare(key1, key, StringComparison.OrdinalIgnoreCase) == 0)
          keyIgnoreCase = keyIgnoreCase == null ? key1 : throw new ODataException(Strings.ODataBatchOperationHeaderDictionary_DuplicateCaseInsensitiveKeys((object) key));
      }
      return keyIgnoreCase;
    }
  }
}
