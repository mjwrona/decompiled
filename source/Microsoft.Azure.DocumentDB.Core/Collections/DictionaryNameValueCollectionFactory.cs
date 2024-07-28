// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Collections.DictionaryNameValueCollectionFactory
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Specialized;

namespace Microsoft.Azure.Documents.Collections
{
  internal class DictionaryNameValueCollectionFactory : INameValueCollectionFactory
  {
    public INameValueCollection CreateNewNameValueCollection() => (INameValueCollection) new DictionaryNameValueCollection();

    public INameValueCollection CreateNewNameValueCollection(int capacity) => (INameValueCollection) new DictionaryNameValueCollection(capacity);

    public INameValueCollection CreateNewNameValueCollection(INameValueCollection collection) => (INameValueCollection) new DictionaryNameValueCollection(collection);

    public INameValueCollection CreateNewNameValueCollection(NameValueCollection collection) => (INameValueCollection) new DictionaryNameValueCollection(collection);

    public INameValueCollection CreateNewNameValueCollection(StringComparer comparer) => (INameValueCollection) new DictionaryNameValueCollection(comparer);
  }
}
