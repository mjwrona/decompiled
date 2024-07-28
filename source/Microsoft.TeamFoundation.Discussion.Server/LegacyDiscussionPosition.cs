// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Discussion.Server.LegacyDiscussionPosition
// Assembly: Microsoft.TeamFoundation.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4DCA91C2-88ED-4792-BE4A-3104961AE8D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Discussion.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Discussion.Server
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Internal)]
  [ClientType("DiscussionPosition")]
  [XmlRoot("DiscussionPosition")]
  public class LegacyDiscussionPosition
  {
    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public int StartLine { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public int EndLine { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public int StartColumn { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public int EndColumn { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public int StartCharPosition { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public int EndCharPosition { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string PositionContext { get; set; }
  }
}
