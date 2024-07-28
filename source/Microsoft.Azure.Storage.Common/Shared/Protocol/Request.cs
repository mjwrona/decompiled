// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Shared.Protocol.Request
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.Azure.Storage.Shared.Protocol
{
  internal static class Request
  {
    internal static string ConvertDateTimeToSnapshotString(DateTimeOffset dateTime) => dateTime.UtcDateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffff'Z'", (IFormatProvider) CultureInfo.InvariantCulture);

    internal static void WriteSharedAccessIdentifiers<T>(
      IDictionary<string, T> sharedAccessPolicies,
      Stream outputStream,
      Action<T, XmlWriter> writePolicyXml)
    {
      CommonUtility.AssertNotNull(nameof (sharedAccessPolicies), (object) sharedAccessPolicies);
      CommonUtility.AssertNotNull(nameof (outputStream), (object) outputStream);
      if (sharedAccessPolicies.Count > 5)
        throw new ArgumentOutOfRangeException(nameof (sharedAccessPolicies), string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Too many '{0}' shared access policy identifiers provided. Server does not support setting more than '{1}' on a single container, queue, table, or share.", (object) sharedAccessPolicies.Count, (object) 5));
      using (XmlWriter xmlWriter = XmlWriter.Create(outputStream, new XmlWriterSettings()
      {
        Encoding = Encoding.UTF8
      }))
      {
        xmlWriter.WriteStartElement("SignedIdentifiers");
        foreach (string key in (IEnumerable<string>) sharedAccessPolicies.Keys)
        {
          xmlWriter.WriteStartElement("SignedIdentifier");
          xmlWriter.WriteElementString("Id", key);
          xmlWriter.WriteStartElement("AccessPolicy");
          T sharedAccessPolicy = sharedAccessPolicies[key];
          writePolicyXml(sharedAccessPolicy, xmlWriter);
          xmlWriter.WriteEndElement();
          xmlWriter.WriteEndElement();
        }
        xmlWriter.WriteEndDocument();
      }
    }
  }
}
