// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WebAccessPluginFatEntry
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class WebAccessPluginFatEntry
  {
    public string Path;
    public int FileId;

    internal static IEnumerable<WebAccessPluginFatEntry> DeserializeEntries(Stream xmlStream)
    {
      ArgumentUtility.CheckForNull<Stream>(xmlStream, nameof (xmlStream));
      XmlSerializer xmlSerializer = new XmlSerializer(typeof (WebAccessPluginFatEntry[]));
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = (XmlResolver) null
      };
      XmlReader xmlReader = XmlReader.Create(xmlStream, settings);
      return (IEnumerable<WebAccessPluginFatEntry>) xmlSerializer.Deserialize(xmlReader);
    }

    internal static void SerializeEntries(
      IEnumerable<WebAccessPluginFatEntry> entries,
      Stream xmlStream)
    {
      WebAccessPluginFatEntry[] o = entries != null ? entries.ToArray<WebAccessPluginFatEntry>() : Array.Empty<WebAccessPluginFatEntry>();
      new XmlSerializer(typeof (WebAccessPluginFatEntry[])).Serialize(xmlStream, (object) o);
    }
  }
}
