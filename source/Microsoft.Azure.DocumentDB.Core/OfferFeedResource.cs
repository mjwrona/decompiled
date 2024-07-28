// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.OfferFeedResource
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Azure.Documents
{
  internal sealed class OfferFeedResource : Resource, IEnumerable<Offer>, IEnumerable
  {
    private static string CollectionName => typeof (Offer).Name + "s";

    public int Count => this.InnerCollection.Count;

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.InnerCollection.GetEnumerator();

    IEnumerator<Offer> IEnumerable<Offer>.GetEnumerator() => this.InnerCollection.GetEnumerator();

    internal Collection<Offer> InnerCollection
    {
      get
      {
        Collection<Offer> innerCollection = this.GetObjectCollection<Offer>(OfferFeedResource.CollectionName, typeof (Offer), this.AltLink, OfferTypeResolver.ResponseOfferTypeResolver);
        if (innerCollection == null)
        {
          innerCollection = new Collection<Offer>();
          this.SetObjectCollection<Offer>(OfferFeedResource.CollectionName, innerCollection);
        }
        return innerCollection;
      }
      set => this.SetObjectCollection<Offer>(OfferFeedResource.CollectionName, value);
    }

    internal override void OnSave() => this.SetValue("_count", (object) this.InnerCollection.Count);
  }
}
