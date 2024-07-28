// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.ServiceProxies.BuildAgentService
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System.ServiceModel.Channels;
using System.Xml;

namespace Microsoft.TeamFoundation.Build.Server.ServiceProxies
{
  internal static class BuildAgentService
  {
    public static Message StopWorkflow(int reservationId) => Message.CreateMessage(MessageVersion.Soap12WSAddressing10, "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Agent/StopWorkflow", (BodyWriter) new BuildAgentService.StopWorkflowBodyWriter(reservationId));

    private sealed class StopWorkflowBodyWriter : BodyWriter
    {
      private int m_reservationId;

      public StopWorkflowBodyWriter(int reservationId)
        : base(true)
      {
        this.m_reservationId = reservationId;
      }

      protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
      {
        writer.WriteStartElement("StopWorkflow", "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting");
        writer.WriteElementString("reservationId", XmlConvert.ToString(this.m_reservationId));
        writer.WriteEndElement();
      }
    }
  }
}
