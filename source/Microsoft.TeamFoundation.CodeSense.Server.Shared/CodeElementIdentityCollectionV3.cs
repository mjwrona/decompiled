// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.CodeElementIdentityCollectionV3
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public class CodeElementIdentityCollectionV3
  {
    private IDictionary<CodeElementIdentityV3, int> _reverseLookup = (IDictionary<CodeElementIdentityV3, int>) new Dictionary<CodeElementIdentityV3, int>();
    private int _maxId;

    public CodeElementIdentityCollectionV3()
      : this((IDictionary<int, CodeElementIdentityV3>) new Dictionary<int, CodeElementIdentityV3>())
    {
    }

    public CodeElementIdentityCollectionV3(IDictionary<int, CodeElementIdentityV3> identities) => this.Identities = identities;

    [JsonProperty]
    public IDictionary<int, CodeElementIdentityV3> Identities { get; private set; }

    public CodeElementIdentityV3 this[int id] => this.Identities[id];

    public int Add(CodeElementIdentityV3 identity)
    {
      this.InitializeReverseLookupIfRequired();
      if (!this._reverseLookup.ContainsKey(identity))
      {
        int key = this.NextId();
        this.Identities.Add(key, identity);
        this._reverseLookup.Add(identity, key);
      }
      return this._reverseLookup[identity];
    }

    private int NextId() => ++this._maxId;

    public void Remove(HashSet<int> codeElementIds)
    {
      if (this.Identities == null || codeElementIds == null)
        return;
      foreach (int codeElementId in codeElementIds)
      {
        if (this.Identities.ContainsKey(codeElementId))
        {
          CodeElementIdentityV3 identity = this.Identities[codeElementId];
          this.Identities.Remove(codeElementId);
          if (this._reverseLookup.ContainsKey(identity))
            this._reverseLookup.Remove(identity);
        }
      }
    }

    public Dictionary<int, int> Merge(CodeElementIdentityCollectionV3 other)
    {
      Dictionary<int, int> dictionary = new Dictionary<int, int>();
      foreach (KeyValuePair<int, CodeElementIdentityV3> identity in (IEnumerable<KeyValuePair<int, CodeElementIdentityV3>>) other.Identities)
        dictionary.Add(identity.Key, this.Add(identity.Value));
      return dictionary;
    }

    private void InitializeReverseLookupIfRequired()
    {
      if (this._reverseLookup.Count != 0 || this.Identities.Count <= 0)
        return;
      foreach (KeyValuePair<int, CodeElementIdentityV3> identity in (IEnumerable<KeyValuePair<int, CodeElementIdentityV3>>) this.Identities)
      {
        if (!this._reverseLookup.ContainsKey(identity.Value))
          this._reverseLookup.Add(identity.Value, identity.Key);
        this._maxId = Math.Max(this._maxId, identity.Key);
      }
    }
  }
}
