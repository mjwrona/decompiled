// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheKey
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  [DataContract]
  [JsonConverter(typeof (ImsCacheKeyJsonConverter))]
  [JsonObject]
  internal abstract class ImsCacheKey : ICloneable
  {
    internal ImsCacheKey()
    {
    }

    internal ImsCacheKey(object id) => this.Id = id;

    [DataMember]
    public object Id { get; private set; }

    internal abstract string Serialize();

    public bool Equals(ImsCacheKey other)
    {
      if (this == other)
        return true;
      return other != null && this.IdEquals(other.Id);
    }

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      return obj != null && this.Equals(obj as ImsCacheKey);
    }

    public override int GetHashCode() => this.Id.GetHashCode();

    public abstract object Clone();

    protected abstract bool IdEquals(object other);
  }
}
