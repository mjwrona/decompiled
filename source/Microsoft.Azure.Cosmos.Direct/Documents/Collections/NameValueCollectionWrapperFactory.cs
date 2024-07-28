// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Collections.NameValueCollectionWrapperFactory
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Collections.Specialized;

namespace Microsoft.Azure.Documents.Collections
{
  internal class NameValueCollectionWrapperFactory : INameValueCollectionFactory
  {
    public INameValueCollection CreateNewNameValueCollection() => (INameValueCollection) new NameValueCollectionWrapper();

    public INameValueCollection CreateNewNameValueCollection(int capacity) => (INameValueCollection) new NameValueCollectionWrapper(capacity);

    public INameValueCollection CreateNewNameValueCollection(INameValueCollection collection) => (INameValueCollection) new NameValueCollectionWrapper(collection);

    public INameValueCollection CreateNewNameValueCollection(NameValueCollection collection) => (INameValueCollection) NameValueCollectionWrapper.Create(collection);

    public INameValueCollection CreateNewNameValueCollection(StringComparer comparer) => (INameValueCollection) new NameValueCollectionWrapper(comparer);
  }
}
