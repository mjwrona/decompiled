// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Symbol.WebApi.DebugEntryCreateBatch
// Assembly: Microsoft.VisualStudio.Services.Symbol.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52CDDA61-EF2D-4AD8-A25B-09A8F04FE8C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Symbol.WebApi.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Symbol.WebApi
{
  [JsonObject(MemberSerialization.OptIn)]
  public class DebugEntryCreateBatch : JsonSerializeableObject
  {
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<string> ProofNodes;

    public DebugEntryCreateBatch(
      IEnumerable<DebugEntry> debugEntries,
      DebugEntryCreateBehavior createBehavior)
    {
      this.CreateBehavior = createBehavior;
      this.DebugEntries = debugEntries.ToList<DebugEntry>();
    }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public DebugEntryCreateBehavior CreateBehavior { get; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<DebugEntry> DebugEntries { get; }
  }
}
