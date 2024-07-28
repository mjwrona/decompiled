// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.LimitingHttpClientFactory
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using BuildXL.Cache.ContentStore.UtilitiesCore.Internal;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public class LimitingHttpClientFactory : IFactory<Uri, HttpClient>
  {
    private readonly Func<HttpClient> creatorFunc;
    private readonly ConcurrentDictionary<string, LimitingHttpClientFactory.State> statesByAuthority = new ConcurrentDictionary<string, LimitingHttpClientFactory.State>();
    private const long MaxUses = 20000;
    private static readonly TimeSpan MinTimeToConsiderStale = TimeSpan.FromHours(1.0);

    public LimitingHttpClientFactory(Func<HttpClient> creatorFunc) => this.creatorFunc = creatorFunc;

    public HttpClient Get(Uri input)
    {
      string leftPart = input.GetLeftPart(UriPartial.Authority);
      LimitingHttpClientFactory.State orAdd = this.statesByAuthority.GetOrAdd<Func<HttpClient>>(leftPart, (Func<string, Func<HttpClient>, LimitingHttpClientFactory.State>) ((_, creator) => new LimitingHttpClientFactory.State(creator())), this.creatorFunc);
      long num = orAdd.IncrementUseCount();
      if (num == 20000L || num > 40000L)
      {
        this.statesByAuthority.TryRemove(leftPart, out LimitingHttpClientFactory.State _);
        this.PurgeStaleStates();
      }
      return orAdd.HttpClient;
    }

    private void PurgeStaleStates()
    {
      DateTime utcNow = DateTime.UtcNow;
      foreach (KeyValuePair<string, LimitingHttpClientFactory.State> source in this.statesByAuthority)
      {
        string key1;
        LimitingHttpClientFactory.State state1;
        source.Deconstruct<string, LimitingHttpClientFactory.State>(out key1, out state1);
        string key2 = key1;
        LimitingHttpClientFactory.State state2 = state1;
        if (utcNow - state2.LastUseTime > LimitingHttpClientFactory.MinTimeToConsiderStale)
          this.statesByAuthority.TryRemove(key2, out state1);
      }
    }

    private class State
    {
      private long useCount;
      private long lastUseTimeInternal;

      public DateTime LastUseTime => DateTime.FromBinary(Volatile.Read(ref this.lastUseTimeInternal));

      public HttpClient HttpClient { get; }

      public State(HttpClient httpClient)
      {
        this.useCount = 0L;
        this.lastUseTimeInternal = DateTime.UtcNow.ToBinary();
        this.HttpClient = httpClient;
      }

      public long IncrementUseCount()
      {
        Volatile.Write(ref this.lastUseTimeInternal, DateTime.UtcNow.ToBinary());
        return Interlocked.Increment(ref this.useCount);
      }
    }
  }
}
