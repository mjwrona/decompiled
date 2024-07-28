// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheKey`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal abstract class ImsCacheKey<T> : ImsCacheKey
  {
    internal ImsCacheKey()
    {
    }

    internal ImsCacheKey(T id)
      : base((object) id)
    {
    }

    [IgnoreDataMember]
    internal T Id => (T) base.Id;

    protected override bool IdEquals(object other) => object.Equals((object) this.Id, other);
  }
}
