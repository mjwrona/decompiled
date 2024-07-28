// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.ServiceProxies.HostService
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using System.ServiceModel.Channels;
using System.Xml;

namespace Microsoft.TeamFoundation.Build.Server.ServiceProxies
{
  internal static class HostService
  {
    public static Message CreateAgentUpdated(string uri, ServiceAction action) => Message.CreateMessage(MessageVersion.Soap12WSAddressing10, "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/ServiceHost/AgentUpdated", (BodyWriter) new HostService.AgentUpdatedBodyWriter(uri, action));

    public static Message CreateControllerUpdated(string uri, ServiceAction action) => Message.CreateMessage(MessageVersion.Soap12WSAddressing10, "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/ServiceHost/ControllerUpdated", (BodyWriter) new HostService.ControllerUpdatedBodyWriter(uri, action));

    private sealed class AgentUpdatedBodyWriter : BodyWriter
    {
      private string m_uri;
      private ServiceAction m_action;

      public AgentUpdatedBodyWriter(string uri, ServiceAction action)
        : base(true)
      {
        this.m_uri = uri;
        this.m_action = action;
      }

      protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
      {
        writer.WriteStartElement("AgentUpdated", "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting");
        writer.WriteElementString("serviceUri", this.m_uri);
        writer.WriteElementString("action", this.m_action.ToString());
        writer.WriteEndElement();
      }
    }

    private sealed class ControllerUpdatedBodyWriter : BodyWriter
    {
      private string m_uri;
      private ServiceAction m_action;

      public ControllerUpdatedBodyWriter(string uri, ServiceAction action)
        : base(true)
      {
        this.m_uri = uri;
        this.m_action = action;
      }

      protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
      {
        writer.WriteStartElement("ControllerUpdated", "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting");
        writer.WriteElementString("serviceUri", this.m_uri);
        writer.WriteElementString("action", this.m_action.ToString());
        writer.WriteEndElement();
      }
    }
  }
}
