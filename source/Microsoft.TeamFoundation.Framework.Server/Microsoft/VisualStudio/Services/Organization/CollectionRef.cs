// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.CollectionRef
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Organization
{
  public class CollectionRef : ICloneable
  {
    public CollectionRef(Guid id) => this.Id = id;

    public Guid Id { get; }

    [JsonProperty]
    public string Name { get; internal set; }

    public CollectionRef Clone() => new CollectionRef(this.Id)
    {
      Name = this.Name
    };

    object ICloneable.Clone() => (object) this.Clone();

    private bool Equals(CollectionRef obj)
    {
      if (this == obj)
        return true;
      return object.Equals((object) this.Id, (object) obj.Id) && VssStringComparer.Hostname.Equals(this.Name, obj.Name);
    }

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      return obj != null && this.GetType() == obj.GetType() && this.Equals((CollectionRef) obj);
    }

    public override int GetHashCode() => this.Id.GetHashCode() + 23 * StringComparer.OrdinalIgnoreCase.GetHashCode(this.Name ?? string.Empty);

    public override string ToString() => string.Format("[Id={0}, Name={1}]", (object) this.Id, (object) this.Name);
  }
}
