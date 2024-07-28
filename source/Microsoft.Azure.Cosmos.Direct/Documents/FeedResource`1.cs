// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.FeedResource`1
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Azure.Documents
{
  internal sealed class FeedResource<T> : Resource, IEnumerable<T>, IEnumerable where T : JsonSerializable, new()
  {
    private static string collectionName;

    private static string CollectionName
    {
      get
      {
        if (FeedResource<T>.collectionName == null)
          FeedResource<T>.collectionName = !typeof (Document).IsAssignableFrom(typeof (T)) ? (!typeof (Attachment).IsAssignableFrom(typeof (T)) ? typeof (T).Name + "s" : "Attachments") : "Documents";
        return FeedResource<T>.collectionName;
      }
    }

    public int Count => this.InnerCollection.Count;

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.InnerCollection.GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => this.InnerCollection.GetEnumerator();

    internal Collection<T> InnerCollection
    {
      get
      {
        Collection<T> innerCollection = this.GetObjectCollection<T>(FeedResource<T>.CollectionName, typeof (T), this.AltLink);
        if (innerCollection == null)
        {
          innerCollection = new Collection<T>();
          this.SetObjectCollection<T>(FeedResource<T>.CollectionName, innerCollection);
        }
        return innerCollection;
      }
      set => this.SetObjectCollection<T>(FeedResource<T>.CollectionName, value);
    }

    internal override void OnSave() => this.SetValue("_count", (object) this.InnerCollection.Count);
  }
}
