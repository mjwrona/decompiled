// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.CodeElementChangeResultV3
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public class CodeElementChangeResultV3
  {
    public const string ChangesCollectorId = "Microsoft.Changes";
    public const string Version = "3.0";

    public CodeElementChangeResultV3()
      : this((IDictionary<ElementChangeKind, HashSet<string>>) new Dictionary<ElementChangeKind, HashSet<string>>())
    {
    }

    public CodeElementChangeResultV3(
      IDictionary<ElementChangeKind, HashSet<string>> details)
    {
      this.Details = details;
    }

    public IDictionary<ElementChangeKind, HashSet<string>> Details { get; private set; }

    public void Add(ElementChangeKind changeKind, string changesId)
    {
      if (!this.Details.ContainsKey(changeKind))
        this.Details.Add(changeKind, new HashSet<string>());
      this.Details[changeKind].Add(changesId);
    }

    public void Filter(HashSet<string> changesets)
    {
      foreach (ElementChangeKind key in this.Details.Keys.ToArray<ElementChangeKind>())
      {
        HashSet<string> detail = this.Details[key];
        if (detail != null)
        {
          detail.ExceptWith((IEnumerable<string>) changesets);
          if (detail.Count == 0)
            this.Details.Remove(key);
        }
      }
    }

    [JsonIgnore]
    public bool IsEmpty => this.Details.Count == 0;

    public void Merge(CodeElementChangeResultV3 other)
    {
      foreach (KeyValuePair<ElementChangeKind, HashSet<string>> detail in (IEnumerable<KeyValuePair<ElementChangeKind, HashSet<string>>>) other.Details)
      {
        foreach (string changesId in detail.Value)
          this.Add(detail.Key, changesId);
      }
    }
  }
}
