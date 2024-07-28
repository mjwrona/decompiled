// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferSubscriptionCacheContainer
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class OfferSubscriptionCacheContainer
  {
    private readonly ConcurrentDictionary<int, List<OfferSubscriptionInternal>> storedResources;
    private OfferSubscriptionCacheContainer.StoreMeterType storeType;

    public OfferSubscriptionCacheContainer() => this.storedResources = new ConcurrentDictionary<int, List<OfferSubscriptionInternal>>();

    public bool TryGetOfferSubscription(int? meterId, out List<OfferSubscriptionInternal> resources)
    {
      if (meterId.HasValue)
      {
        List<OfferSubscriptionInternal> source;
        bool flag = this.storedResources.TryGetValue(meterId.Value, out source);
        if (this.storeType == OfferSubscriptionCacheContainer.StoreMeterType.Selected && !flag)
        {
          resources = (List<OfferSubscriptionInternal>) null;
          return false;
        }
        resources = source == null ? new List<OfferSubscriptionInternal>() : source.ToList<OfferSubscriptionInternal>();
        return true;
      }
      if (this.storeType == OfferSubscriptionCacheContainer.StoreMeterType.Selected)
      {
        resources = (List<OfferSubscriptionInternal>) null;
        return false;
      }
      resources = this.storedResources.Values.SelectMany<List<OfferSubscriptionInternal>, OfferSubscriptionInternal>((Func<List<OfferSubscriptionInternal>, IEnumerable<OfferSubscriptionInternal>>) (m => (IEnumerable<OfferSubscriptionInternal>) m)).ToList<OfferSubscriptionInternal>();
      return true;
    }

    public void Update(IEnumerable<OfferSubscriptionInternal> resources, int? meterId)
    {
      if (!meterId.HasValue)
      {
        this.storedResources.Clear();
        this.storeType = OfferSubscriptionCacheContainer.StoreMeterType.All;
        foreach (IGrouping<int, OfferSubscriptionInternal> source in resources.GroupBy<OfferSubscriptionInternal, int>((Func<OfferSubscriptionInternal, int>) (g => g.MeterId)))
          this.storedResources.TryAdd(source.Key, source.Select<OfferSubscriptionInternal, OfferSubscriptionInternal>((Func<OfferSubscriptionInternal, OfferSubscriptionInternal>) (g => g)).ToList<OfferSubscriptionInternal>());
      }
      else
      {
        this.storeType = OfferSubscriptionCacheContainer.StoreMeterType.Selected;
        List<OfferSubscriptionInternal> list = resources.Where<OfferSubscriptionInternal>((Func<OfferSubscriptionInternal, bool>) (r => r.MeterId == meterId.Value)).ToList<OfferSubscriptionInternal>();
        this.storedResources[meterId.Value] = list;
      }
    }

    public void SelectiveUpdate(OfferSubscriptionInternal resource)
    {
      int meterId = resource.MeterId;
      ResourceRenewalGroup renewalGroup = resource.RenewalGroup;
      List<OfferSubscriptionInternal> storedResource = this.storedResources.ContainsKey(meterId) ? this.storedResources[meterId] : (List<OfferSubscriptionInternal>) null;
      if (storedResource == null)
      {
        this.storeType = OfferSubscriptionCacheContainer.StoreMeterType.Selected;
        this.storedResources[meterId] = new List<OfferSubscriptionInternal>()
        {
          resource
        };
      }
      else
      {
        OfferSubscriptionInternal subscriptionInternal = new List<OfferSubscriptionInternal>((IEnumerable<OfferSubscriptionInternal>) storedResource).Where<OfferSubscriptionInternal>((Func<OfferSubscriptionInternal, bool>) (r => r.RenewalGroup == renewalGroup)).FirstOrDefault<OfferSubscriptionInternal>();
        if (subscriptionInternal != null)
          storedResource.Remove(subscriptionInternal);
        storedResource.Add(resource);
        this.storedResources[meterId] = storedResource;
      }
    }

    public void SelectiveInvalidate(int meterId)
    {
      this.storeType = OfferSubscriptionCacheContainer.StoreMeterType.Selected;
      this.storedResources.TryRemove(meterId, out List<OfferSubscriptionInternal> _);
    }

    private enum StoreMeterType
    {
      All,
      Selected,
    }
  }
}
