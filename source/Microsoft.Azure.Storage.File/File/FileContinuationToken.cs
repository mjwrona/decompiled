// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.FileContinuationToken
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Azure.Storage.File
{
  [XmlRoot("ContinuationToken", IsNullable = false)]
  [Serializable]
  public sealed class FileContinuationToken : IContinuationToken
  {
    private string version = "2.0";
    private string type = "File";

    private string Version
    {
      get => this.version;
      set
      {
        this.version = value;
        if (this.version != "2.0")
          throw new XmlException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unexpected Element '{0}'", (object) this.version));
      }
    }

    private string Type
    {
      get => this.type;
      set
      {
        this.type = value;
        if (this.type != "File")
          throw new XmlException("Unexpected Continuation Type");
      }
    }

    public string NextMarker { get; set; }

    public StorageLocation? TargetLocation { get; set; }

    public XmlSchema GetSchema() => (XmlSchema) null;

    public async Task ReadXmlAsync(XmlReader reader)
    {
      CommonUtility.AssertNotNull(nameof (reader), (object) reader);
      int num1 = (int) await reader.MoveToContentAsync().ConfigureAwait(false);
      await reader.ReadStartElementAsync().ConfigureAwait(false);
      int num2 = (int) await reader.MoveToContentAsync().ConfigureAwait(false);
      if (reader.Name == "ContinuationToken")
        await reader.ReadStartElementAsync().ConfigureAwait(false);
      string str;
      while (true)
      {
        StorageLocation result;
        do
        {
          if (await reader.IsStartElementAsync().ConfigureAwait(false))
          {
            switch (reader.Name)
            {
              case "Version":
                goto label_6;
              case "NextMarker":
                goto label_7;
              case "TargetLocation":
                str = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                if (!Enum.TryParse<StorageLocation>(str, out result))
                  continue;
                goto label_9;
              case "Type":
                goto label_12;
              default:
                goto label_13;
            }
          }
          else
            goto label_16;
        }
        while (string.IsNullOrEmpty(str));
        break;
label_6:
        this.Version = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
        continue;
label_7:
        this.NextMarker = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
        continue;
label_9:
        this.TargetLocation = new StorageLocation?(result);
        continue;
label_12:
        this.Type = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
      }
      throw new XmlException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unexpected Location '{0}'", (object) str));
label_13:
      throw new XmlException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unexpected Element '{0}'", (object) reader.Name));
label_16:;
    }

    public void WriteXml(XmlWriter writer)
    {
      CommonUtility.AssertNotNull(nameof (writer), (object) writer);
      writer.WriteStartElement("ContinuationToken");
      writer.WriteElementString("Version", this.Version);
      writer.WriteElementString("Type", this.Type);
      if (this.NextMarker != null)
        writer.WriteElementString("NextMarker", this.NextMarker);
      writer.WriteElementString("TargetLocation", this.TargetLocation.ToString());
      writer.WriteEndElement();
    }
  }
}
