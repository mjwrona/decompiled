// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.StubWindowedDictionaryCacheContainer`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Redis
{
  internal class StubWindowedDictionaryCacheContainer<K> : IWindowedDictionaryCacheContainer<K>
  {
    IEnumerable<long> IWindowedDictionaryCacheContainer<K>.Get(
      IVssRequestContext requestContext,
      IEnumerable<K> items)
    {
      return (IEnumerable<long>) new List<long>();
    }

    IEnumerable<long> IWindowedDictionaryCacheContainer<K>.IncrementBy(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<K, WindowItem>> items)
    {
      return (IEnumerable<long>) new List<long>();
    }

    IEnumerable<TimeSpan?> IWindowedDictionaryCacheContainer<K>.TimeToLive(
      IVssRequestContext requestContext,
      IEnumerable<K> keys)
    {
      return (IEnumerable<TimeSpan?>) new List<TimeSpan?>();
    }
  }
}
