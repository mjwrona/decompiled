// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.Diagnostics.DictionaryTraceRecord
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.Collections;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs.Common.Diagnostics
{
  internal class DictionaryTraceRecord : TraceRecord
  {
    private IDictionary dictionary;

    internal DictionaryTraceRecord(IDictionary dictionary) => this.dictionary = dictionary;

    internal override string EventId => "http://schemas.microsoft.com/2006/08/ServiceModel/DictionaryTraceRecord";

    internal override void WriteTo(XmlWriter xml)
    {
      if (this.dictionary == null)
        return;
      foreach (object key in (IEnumerable) this.dictionary.Keys)
      {
        object obj = this.dictionary[key];
        xml.WriteElementString(key.ToString(), obj == null ? string.Empty : obj.ToString());
      }
    }
  }
}
