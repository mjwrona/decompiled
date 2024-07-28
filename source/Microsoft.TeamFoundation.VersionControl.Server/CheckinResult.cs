// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CheckinResult
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class CheckinResult
  {
    private Microsoft.TeamFoundation.VersionControl.Common.CheckInState m_checkInState;

    public CheckinResult() => this.UndoneServerItems = new List<string>();

    [XmlAttribute("cset")]
    public int ChangesetId { get; set; }

    [XmlAttribute("date")]
    public DateTime CreationDate { get; set; }

    public List<string> UndoneServerItems { get; set; }

    public StreamingCollection<GetOperation> LocalVersionUpdates { get; set; }

    [XmlAttribute("state")]
    public int CheckInState
    {
      get => (int) this.m_checkInState;
      set => this.m_checkInState = (Microsoft.TeamFoundation.VersionControl.Common.CheckInState) value;
    }

    [XmlAttribute("ticket")]
    public int CheckInTicket { get; set; }
  }
}
