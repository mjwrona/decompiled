// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.FeedResource`1
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

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
          FeedResource<T>.collectionName = !CustomTypeExtensions.IsAssignableFrom(typeof (Document), typeof (T)) ? (!CustomTypeExtensions.IsAssignableFrom(typeof (Attachment), typeof (T)) ? typeof (T).Name + "s" : "Attachments") : "Documents";
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
