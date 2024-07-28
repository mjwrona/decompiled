// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Shared.Protocol.AccessPolicyResponseBase`1
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Azure.Storage.Shared.Protocol
{
  internal abstract class AccessPolicyResponseBase<T> : IDisposable where T : new()
  {
    protected XmlReader reader;

    protected AccessPolicyResponseBase(Stream stream)
    {
      this.reader = XMLReaderExtensions.CreateAsAsync(stream);
      this.AccessIdentifiers = this.ParseAsync();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing && this.reader != null)
        this.reader.Close();
      this.reader = (XmlReader) null;
    }

    public Task<IEnumerable<KeyValuePair<string, T>>> AccessIdentifiers { get; private set; }

    protected abstract T ParseElement(XElement accessPolicyElement);

    protected Task<IEnumerable<KeyValuePair<string, T>>> ParseAsync() => Task.Run<IEnumerable<KeyValuePair<string, T>>>((Func<IEnumerable<KeyValuePair<string, T>>>) (() =>
    {
      List<KeyValuePair<string, T>> source = new List<KeyValuePair<string, T>>();
      foreach (XElement element in XElement.Load(this.reader).Elements((XName) "SignedIdentifier"))
      {
        string key = (string) element.Element((XName) "Id");
        XElement accessPolicyElement = element.Element((XName) "AccessPolicy");
        T obj = accessPolicyElement == null ? new T() : this.ParseElement(accessPolicyElement);
        source.Add(new KeyValuePair<string, T>(key, obj));
      }
      this.reader.Close();
      this.reader = (XmlReader) null;
      return source.AsEnumerable<KeyValuePair<string, T>>();
    }));
  }
}
