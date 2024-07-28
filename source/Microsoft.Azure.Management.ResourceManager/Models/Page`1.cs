// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.Page`1
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Rest.Azure;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  [JsonObject]
  public class Page<T> : IPage<T>, IEnumerable<T>, IEnumerable
  {
    [JsonProperty("nextLink")]
    public string NextPageLink { get; private set; }

    [JsonProperty("value")]
    private IList<T> Items { get; set; }

    public IEnumerator<T> GetEnumerator() => this.Items == null ? Enumerable.Empty<T>().GetEnumerator() : this.Items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
