// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.IImsDataCache
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal interface IImsDataCache
  {
    void AddObjectTypes(IVssRequestContext context, IEnumerable<Type> objectTypes);

    IDictionary<K, V> GetObjects<K, V>(IVssRequestContext context, ICollection<K> keys)
      where K : ImsCacheKey
      where V : ImsCacheObject;

    void AddObjects<T>(IVssRequestContext context, IEnumerable<T> objects) where T : ImsCacheObject;

    void RemoveObjects<K, V>(IVssRequestContext context, ICollection<K> keys)
      where K : ImsCacheKey
      where V : ImsCacheObject;
  }
}
