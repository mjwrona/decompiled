// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.Page`1
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Microsoft.Rest.Azure;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.KeyVault.Models
{
  [JsonObject]
  public class Page<T> : IPage<T>, IEnumerable<T>, IEnumerable
  {
    [JsonProperty("nextLink")]
    public string NextPageLink { get; private set; }

    [JsonProperty("value")]
    private IList<T> Items { get; set; }

    public IEnumerator<T> GetEnumerator() => this.Items != null ? this.Items.GetEnumerator() : Enumerable.Empty<T>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
