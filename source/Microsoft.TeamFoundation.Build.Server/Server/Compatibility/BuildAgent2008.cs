// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildAgent2008
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [XmlType("BuildAgent")]
  [CallOnDeserialization("AfterDeserialize")]
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class BuildAgent2008 : BuildGroupItem2010
  {
    private string m_machineName;
    private int m_port;
    private bool m_requireSecureChannel;
    private int m_maxProcesses;
    private string m_buildDirectory;
    private Agent2008Status m_status;
    private string m_statusMessage;
    private string m_description;
    private int m_queueCount;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string MachineName
    {
      get => this.m_machineName;
      set => this.m_machineName = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public int Port
    {
      get => this.m_port;
      set => this.m_port = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public bool RequireSecureChannel
    {
      get => this.m_requireSecureChannel;
      set => this.m_requireSecureChannel = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public int MaxProcesses
    {
      get => this.m_maxProcesses;
      set => this.m_maxProcesses = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string BuildDirectory
    {
      get => this.m_buildDirectory;
      set => this.m_buildDirectory = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public Agent2008Status Status
    {
      get => this.m_status;
      set => this.m_status = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string StatusMessage
    {
      get => this.m_statusMessage;
      set
      {
        if (!string.IsNullOrEmpty(value) && value.Length > 512)
          this.m_statusMessage = value.Substring(0, 512);
        else
          this.m_statusMessage = value;
      }
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public int QueueCount
    {
      get => this.m_queueCount;
      set => this.m_queueCount = value;
    }

    [XmlElement]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string Description
    {
      get => this.m_description;
      set => this.m_description = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildAgent2008 Uri={0} Name={1} MachineName={2} Port={3} Status={4}]", (object) this.Uri, (object) this.Name, (object) this.MachineName, (object) this.Port, (object) this.Status);
  }
}
