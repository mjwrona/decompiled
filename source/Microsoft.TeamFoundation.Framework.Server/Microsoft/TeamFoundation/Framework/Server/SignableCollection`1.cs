// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SignableCollection`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SignableCollection<T>
  {
    private Dictionary<T, string> m_signatures;
    private SignableCollection<T>.GetFileIdDelegate m_getFileId;

    public SignableCollection(SignableCollection<T>.GetFileIdDelegate getFileId)
    {
      ArgumentUtility.CheckForNull<SignableCollection<T>.GetFileIdDelegate>(getFileId, nameof (getFileId));
      this.m_signatures = new Dictionary<T, string>();
      this.m_getFileId = getFileId;
    }

    public IEnumerable<T> Items => (IEnumerable<T>) this.m_signatures.Keys;

    public void Add(T item)
    {
      ArgumentUtility.CheckGenericForNull((object) item, nameof (item));
      this.m_signatures[item] = (string) null;
    }

    public void AddRange(IEnumerable<T> items)
    {
      ArgumentUtility.CheckForNull<IEnumerable<T>>(items, nameof (items));
      foreach (T obj in items)
        this.Add(obj);
    }

    public string GetSignature(T item)
    {
      ArgumentUtility.CheckGenericForNull((object) item, nameof (item));
      return this.m_signatures[item];
    }

    public void Sign(IVssRequestContext requestContext, DateTime expiration)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (UrlSigner urlSigner = new UrlSigner(requestContext, expiration))
        urlSigner.SignObject((ISignable) new SignableCollection<T>.InternalSignableCollection(this));
    }

    public delegate int GetFileIdDelegate(T obj);

    private class InternalSignableCollection : ISignable
    {
      private SignableCollection<T> m_collection;
      private List<T> m_items;

      public InternalSignableCollection(SignableCollection<T> collection)
      {
        this.m_collection = collection;
        this.m_items = new List<T>((IEnumerable<T>) collection.m_signatures.Keys);
      }

      public int GetDownloadUrlCount() => this.m_items.Count;

      public int GetFileId(int index) => this.m_collection.m_getFileId(this.m_items[index]);

      public byte[] GetHashValue(int index) => (byte[]) null;

      public void SetDownloadUrl(int index, string downloadUrl) => this.m_collection.m_signatures[this.m_items[index]] = downloadUrl;
    }
  }
}
