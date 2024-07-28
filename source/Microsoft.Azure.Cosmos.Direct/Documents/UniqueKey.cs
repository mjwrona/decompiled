// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.UniqueKey
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;

namespace Microsoft.Azure.Documents
{
  internal sealed class UniqueKey : JsonSerializable
  {
    private Collection<string> paths;
    private JObject filter;

    [JsonProperty(PropertyName = "paths")]
    public Collection<string> Paths
    {
      get
      {
        if (this.paths == null)
        {
          this.paths = this.GetValue<Collection<string>>("paths");
          if (this.paths == null)
            this.paths = new Collection<string>();
        }
        return this.paths;
      }
      set
      {
        this.paths = value;
        this.SetValue("paths", (object) value);
      }
    }

    [JsonProperty(PropertyName = "filter", NullValueHandling = NullValueHandling.Ignore)]
    internal JObject Filter
    {
      get
      {
        this.filter = this.GetValue<JObject>("filter");
        return this.filter;
      }
      set
      {
        this.filter = value;
        this.SetValue("filter", (object) value);
      }
    }

    internal override void Validate()
    {
      base.Validate();
      this.GetValue<Collection<string>>("paths");
      this.GetValue<JObject>("filter");
    }

    public override bool Equals(object obj)
    {
      if (!(obj is UniqueKey uniqueKey) || this.Paths.Count != uniqueKey.Paths.Count)
        return false;
      foreach (string path in uniqueKey.paths)
      {
        if (!this.Paths.Contains(path))
          return false;
      }
      if (this.Filter == null && uniqueKey.Filter == null)
        return true;
      return this.Filter != null && uniqueKey.Filter != null && new JTokenEqualityComparer().Equals((JToken) this.Filter, (JToken) uniqueKey.Filter);
    }

    public override int GetHashCode()
    {
      int hashCode = 0;
      foreach (string path in this.Paths)
        hashCode ^= path.GetHashCode();
      if (this.Filter != null)
      {
        JTokenEqualityComparer equalityComparer = new JTokenEqualityComparer();
        hashCode ^= equalityComparer.GetHashCode((JToken) this.Filter.GetHashCode());
      }
      return hashCode;
    }

    internal override void OnSave()
    {
      if (this.paths != null)
        this.SetValue("paths", (object) this.paths);
      if (this.filter == null)
        return;
      this.SetValue("filter", (object) this.filter);
    }
  }
}
