// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheObject`2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal abstract class ImsCacheObject<K, V> : ImsCacheObject
  {
    internal ImsCacheObject()
    {
    }

    internal ImsCacheObject(ImsCacheKey<K> key, V value)
      : base((ImsCacheKey) key, (object) value)
    {
    }

    internal ImsCacheObject(ImsCacheKey<K> key, V value, DateTimeOffset time)
      : base((ImsCacheKey) key, (object) value, time)
    {
    }

    [DataMember]
    internal V Value
    {
      get => (V) base.Value;
      private set => this.Value = (object) value;
    }

    protected override bool ValueEquals(object otherValue)
    {
      if ((object) this.Value == otherValue)
        return true;
      return otherValue is V v && this.Value.Equals((object) v);
    }
  }
}
