// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WebAccessPluginEntry
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
  public class WebAccessPluginEntry
  {
    public int FatFileId { get; set; }

    public string Name { get; set; }

    public string Version { get; set; }

    public string Vendor { get; set; }

    public bool Enabled { get; set; }

    public string MoreInfo { get; set; }

    public DateTime InstallDate { get; set; }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json.Add("name", (object) this.Name);
      json.Add("version", (object) this.Version);
      json.Add("vendor", (object) this.Vendor);
      json.Add("enabled", (object) this.Enabled);
      json.Add("moreInfo", (object) this.MoreInfo);
      json.Add("installDate", (object) this.InstallDate);
      return json;
    }

    internal static IEnumerable<WebAccessPluginEntry> DeserializeEntries(Stream xmlStream)
    {
      ArgumentUtility.CheckForNull<Stream>(xmlStream, nameof (xmlStream));
      XmlSerializer xmlSerializer = new XmlSerializer(typeof (WebAccessPluginEntry[]));
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = (XmlResolver) null
      };
      XmlReader xmlReader = XmlReader.Create(xmlStream, settings);
      return (IEnumerable<WebAccessPluginEntry>) xmlSerializer.Deserialize(xmlReader);
    }

    internal static void SerializeEntries(
      IEnumerable<WebAccessPluginEntry> entries,
      Stream xmlStream)
    {
      WebAccessPluginEntry[] o = entries != null ? entries.ToArray<WebAccessPluginEntry>() : Array.Empty<WebAccessPluginEntry>();
      new XmlSerializer(typeof (WebAccessPluginEntry[])).Serialize(xmlStream, (object) o);
    }
  }
}
