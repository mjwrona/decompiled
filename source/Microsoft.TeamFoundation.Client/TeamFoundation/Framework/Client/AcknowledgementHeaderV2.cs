// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.AcknowledgementHeaderV2
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Channels;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal sealed class AcknowledgementHeaderV2 : TfsMessageHeader
  {
    public AcknowledgementHeaderV2(IList<AcknowledgementRange> ranges) => this.Ranges = (IList<AcknowledgementRange>) new List<AcknowledgementRange>((IEnumerable<AcknowledgementRange>) ((object) ranges ?? (object) Array.Empty<AcknowledgementRange>()));

    public override string Name => "Acknowledgement";

    public override string Namespace => "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2";

    public IList<AcknowledgementRange> Ranges { get; private set; }

    protected override void OnWriteHeaderContents(XmlDictionaryWriter writer)
    {
      foreach (AcknowledgementRange range in (IEnumerable<AcknowledgementRange>) this.Ranges)
      {
        writer.WriteStartElement("AcknowledgementRange", this.Namespace);
        writer.WriteAttributeString("Lower", XmlConvert.ToString(range.Lower));
        writer.WriteAttributeString("Upper", XmlConvert.ToString(range.Upper));
        writer.WriteEndElement();
      }
    }
  }
}
