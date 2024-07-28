// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AcknowledgementHeaderV2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class AcknowledgementHeaderV2 : MessageHeader
  {
    private AcknowledgementHeaderV2(IList<AcknowledgementRange> ranges) => this.Ranges = new List<AcknowledgementRange>((IEnumerable<AcknowledgementRange>) ((object) ranges ?? (object) Array.Empty<AcknowledgementRange>()));

    public List<AcknowledgementRange> Ranges { get; private set; }

    public override string Name => "Acknowledgement";

    public override string Namespace => "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2";

    protected override void OnWriteHeaderContents(
      XmlDictionaryWriter writer,
      MessageVersion messageVersion)
    {
      foreach (AcknowledgementRange range in this.Ranges)
      {
        writer.WriteStartElement("AcknowledgementRange", this.Namespace);
        writer.WriteAttributeString("Lower", XmlConvert.ToString(range.Lower));
        writer.WriteAttributeString("Upper", XmlConvert.ToString(range.Upper));
        writer.WriteEndElement();
      }
    }

    public static AcknowledgementHeaderV2 ReadHeader(MessageHeaders headers)
    {
      int header = headers.FindHeader("Acknowledgement", "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2");
      if (header < 0)
        return (AcknowledgementHeaderV2) null;
      List<AcknowledgementRange> ranges = new List<AcknowledgementRange>();
      using (XmlDictionaryReader readerAtHeader = headers.GetReaderAtHeader(header))
      {
        readerAtHeader.Read();
        while (readerAtHeader.NodeType == XmlNodeType.Element)
        {
          if (readerAtHeader.IsStartElement("AcknowledgementRange", "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2"))
          {
            long num1 = long.MinValue;
            long num2 = long.MinValue;
            while (readerAtHeader.MoveToNextAttribute())
            {
              switch (readerAtHeader.LocalName)
              {
                case "Lower":
                  num1 = XmlConvert.ToInt64(readerAtHeader.Value);
                  continue;
                case "Upper":
                  num2 = XmlConvert.ToInt64(readerAtHeader.Value);
                  continue;
                default:
                  continue;
              }
            }
            if (num1 != long.MinValue && num2 != long.MinValue)
              ranges.Add(new AcknowledgementRange()
              {
                Lower = num1,
                Upper = num2
              });
            readerAtHeader.Read();
          }
          else
            readerAtHeader.Skip();
        }
      }
      return ranges.Count > 0 ? new AcknowledgementHeaderV2((IList<AcknowledgementRange>) ranges) : (AcknowledgementHeaderV2) null;
    }
  }
}
