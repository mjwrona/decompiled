// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.Utils.XMLReaderExtensions
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.Utils
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

    public static async Task<bool> IsStartElementAsync(this XmlReader reader) => await reader.MoveToContentAsync().ConfigureAwait(false) == XmlNodeType.Element;

    public static async Task<bool> IsStartElementAsync(this XmlReader reader, string name) => await reader.MoveToContentAsync().ConfigureAwait(false) == XmlNodeType.Element && reader.Name == name;

    public static async Task<string> ReadElementContentAsStringAsync(
      this XmlReader reader,
      string localName,
      string namespaceURI)
    {
      reader.CheckElement(localName, namespaceURI);
      return await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
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
  }
}
