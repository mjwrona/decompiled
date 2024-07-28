// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Collections.INameValueCollection
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.Azure.Documents.Collections
{
  internal interface INameValueCollection : IEnumerable
  {
    void Add(string key, string value);

    void Set(string key, string value);

    string Get(string key);

    string this[string key] { get; set; }

    void Remove(string key);

    void Clear();

    int Count();

    INameValueCollection Clone();

    void Add(INameValueCollection collection);

    string[] GetValues(string key);

    string[] AllKeys();

    IEnumerable<string> Keys();

    NameValueCollection ToNameValueCollection();
  }
}
