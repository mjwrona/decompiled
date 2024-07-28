// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Resource
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class Resource : IEquatable<Resource>
  {
    public Resource(string id, string name, string type)
    {
      this.id = id;
      this.name = name;
      this.type = type;
    }

    public string id { get; protected set; }

    public string name { get; protected set; }

    public string type { get; protected set; }

    public string location { get; set; }

    public Dictionary<string, string> tags { get; set; } = new Dictionary<string, string>();

    bool IEquatable<Resource>.Equals(Resource other) => this.id == other.id && this.name == other.name && this.type == other.type && this.location == other.location && this.tags.Except<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>) other.tags).Count<KeyValuePair<string, string>>() == 0 && other.tags.Except<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>) this.tags).Count<KeyValuePair<string, string>>() == 0;
  }
}
