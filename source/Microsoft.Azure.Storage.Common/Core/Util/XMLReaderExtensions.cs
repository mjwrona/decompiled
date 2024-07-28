// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.XMLReaderExtensions
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal static class XMLReaderExtensions
  {
    public static XmlReader CreateAsAsync(Stream stream) => XmlReader.Create(stream, new XmlReaderSettings()
    {
      IgnoreWhitespace = true,
      Async = true
    });

    public static async Task ReadStartElementAsync(
      this XmlReader reader,
      string localname,
      string ns)
    {
      if (await reader.MoveToContentAsync().ConfigureAwait(false) != XmlNodeType.Element)
        throw new InvalidOperationException(reader.NodeType.ToString() + " is an invalid XmlNodeType");
      if (!(reader.LocalName == localname) || !(reader.NamespaceURI == ns))
        throw new InvalidOperationException("localName or namespace doesn’t match");
      int num = await reader.ReadAsync().ConfigureAwait(false) ? 1 : 0;
    }

    public static async Task ReadStartElementAsync(this XmlReader reader)
    {
      if (await reader.MoveToContentAsync().ConfigureAwait(false) != XmlNodeType.Element)
        throw new InvalidOperationException(reader.NodeType.ToString() + " is an invalid XmlNodeType");
      int num = await reader.ReadAsync().ConfigureAwait(false) ? 1 : 0;
    }

    public static async Task ReadEndElementAsync(this XmlReader reader)
    {
      if (await reader.MoveToContentAsync().ConfigureAwait(false) != XmlNodeType.EndElement)
        throw new InvalidOperationException();
      int num = await reader.ReadAsync().ConfigureAwait(false) ? 1 : 0;
    }

    public static async Task<string> ReadElementContentAsStringAsync(
      this XmlReader reader,
      string localName,
      string namespaceURI)
    {
      reader.CheckElement(localName, namespaceURI);
      return await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
    }

    public static async Task<bool> ReadElementContentAsBooleanAsync(this XmlReader reader) => XmlConvert.ToBoolean(await reader.ReadElementContentAsStringAsync().ConfigureAwait(false));

    public static async Task<bool> ReadElementContentAsBooleanAsync(
      this XmlReader reader,
      string localName,
      string namespaceURI)
    {
      reader.CheckElement(localName, namespaceURI);
      return XmlConvert.ToBoolean(await reader.ReadElementContentAsStringAsync().ConfigureAwait(false));
    }

    public static async Task<int> ReadElementContentAsInt32Async(this XmlReader reader) => XmlConvert.ToInt32(await reader.ReadElementContentAsStringAsync().ConfigureAwait(false));

    public static async Task<int> ReadElementContentAsInt32Async(
      this XmlReader reader,
      string localName,
      string namespaceURI)
    {
      reader.CheckElement(localName, namespaceURI);
      return XmlConvert.ToInt32(await reader.ReadElementContentAsStringAsync().ConfigureAwait(false));
    }

    public static async Task<long> ReadElementContentAsInt64Async(this XmlReader reader) => XmlConvert.ToInt64(await reader.ReadElementContentAsStringAsync().ConfigureAwait(false));

    public static async Task<long> ReadElementContentAsInt64Async(
      this XmlReader reader,
      string localName,
      string namespaceURI)
    {
      reader.CheckElement(localName, namespaceURI);
      return XmlConvert.ToInt64(await reader.ReadElementContentAsStringAsync().ConfigureAwait(false));
    }

    public static async Task<DateTimeOffset> ReadElementContentAsDateTimeOffsetAsync(
      this XmlReader reader)
    {
      return (DateTimeOffset) (await reader.ReadElementContentAsStringAsync().ConfigureAwait(false)).ToUTCTime();
    }

    public static async Task<DateTimeOffset> ReadElementContentAsDateTimeOffsetAsync(
      this XmlReader reader,
      string localName,
      string namespaceURI)
    {
      reader.CheckElement(localName, namespaceURI);
      return XmlConvert.ToDateTimeOffset(await reader.ReadElementContentAsStringAsync().ConfigureAwait(false));
    }

    private static void CheckElement(this XmlReader reader, string localName, string namespaceURI)
    {
      if (localName == null || localName.Length == 0)
        throw new InvalidOperationException("localName is null or empty");
      if (namespaceURI == null)
        throw new ArgumentNullException(nameof (namespaceURI));
      if (reader.NodeType != XmlNodeType.Element)
        throw new InvalidOperationException(reader.NodeType.ToString() + " is an invalid XmlNodeType");
      if (reader.LocalName != localName || reader.NamespaceURI != namespaceURI)
        throw new InvalidOperationException("localName or namespace doesn’t match");
    }

    public static async Task<bool> IsStartElementAsync(this XmlReader reader) => await reader.MoveToContentAsync().ConfigureAwait(false) == XmlNodeType.Element;

    public static async Task<bool> IsStartElementAsync(this XmlReader reader, string name) => await reader.MoveToContentAsync().ConfigureAwait(false) == XmlNodeType.Element && reader.Name == name;

    public static async Task<bool> ReadToFollowingAsync(
      this XmlReader reader,
      string localName,
      string namespaceURI)
    {
      if (localName == null || localName.Length == 0)
        throw new ArgumentException("localName is empty or null");
      if (namespaceURI == null)
        throw new ArgumentNullException(nameof (namespaceURI));
      localName = reader.NameTable.Add(localName);
      namespaceURI = reader.NameTable.Add(namespaceURI);
      do
      {
        if (!await reader.ReadAsync().ConfigureAwait(false))
          goto label_9;
      }
      while (reader.NodeType != XmlNodeType.Element || (object) localName != (object) reader.LocalName || (object) namespaceURI != (object) reader.NamespaceURI);
      return true;
label_9:
      return false;
    }

    public static async Task<bool> ReadToFollowingAsync(this XmlReader reader, string localName)
    {
      if (localName == null || localName.Length == 0)
        throw new ArgumentException("localName is empty or null");
      localName = reader.NameTable.Add(localName);
      do
      {
        if (!await reader.ReadAsync().ConfigureAwait(false))
          goto label_7;
      }
      while (reader.NodeType != XmlNodeType.Element || (object) localName != (object) reader.LocalName);
      return true;
label_7:
      return false;
    }
  }
}
