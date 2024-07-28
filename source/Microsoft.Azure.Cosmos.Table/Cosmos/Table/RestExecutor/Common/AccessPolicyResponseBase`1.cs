// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.Common.AccessPolicyResponseBase`1
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.Common
{
  internal abstract class AccessPolicyResponseBase<T> : ResponseParsingBase<KeyValuePair<string, T>> where T : new()
  {
    protected AccessPolicyResponseBase(Stream stream)
      : base(stream)
    {
    }

    public IEnumerable<KeyValuePair<string, T>> AccessIdentifiers => this.ObjectsToParse;

    protected abstract T ParseElement(XElement accessPolicyElement);

    protected override IEnumerable<KeyValuePair<string, T>> ParseXml()
    {
      AccessPolicyResponseBase<T> policyResponseBase = this;
      foreach (XElement element in XElement.Load(policyResponseBase.reader).Elements((XName) "SignedIdentifier"))
      {
        string key = (string) element.Element((XName) "Id");
        XElement accessPolicyElement = element.Element((XName) "AccessPolicy");
        T obj = accessPolicyElement == null ? new T() : policyResponseBase.ParseElement(accessPolicyElement);
        yield return new KeyValuePair<string, T>(key, obj);
      }
    }
  }
}
