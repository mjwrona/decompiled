// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.ServiceProxies.BuildControllerService
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Channels;
using System.Xml;

namespace Microsoft.TeamFoundation.Build.Server.ServiceProxies
{
  internal static class BuildControllerService
  {
    public static Message DeleteBuildDrop(string dropLocation) => Message.CreateMessage(MessageVersion.Soap12WSAddressing10, "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/DeleteBuildDrop", (BodyWriter) new BuildControllerService.DeleteBuildDropBodyWriter(dropLocation));

    public static Message DeleteBuildSymbols(string storePath, string transactionId) => Message.CreateMessage(MessageVersion.Soap12WSAddressing10, "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/DeleteBuildSymbols", (BodyWriter) new BuildControllerService.DeleteBuildSymbolsBodyWriter(storePath, transactionId));

    public static Message NotifyAgentAvailable(
      string buildUri,
      int reservationId,
      string reservedAgentUri)
    {
      return Message.CreateMessage(MessageVersion.Soap12WSAddressing10, "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/AgentAcquired", (BodyWriter) new BuildControllerService.NotifyAgentAvailableBodyWriter(buildUri, reservationId, reservedAgentUri));
    }

    public static Message StartBuild(string buildUri, int queueId) => Message.CreateMessage(MessageVersion.Soap12WSAddressing10, "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/StartBuild", (BodyWriter) new BuildControllerService.StartStopBodyWriter(nameof (StartBuild), buildUri, queueId));

    public static Message StopBuild(BuildDetail build) => Message.CreateMessage(MessageVersion.Soap12WSAddressing10, "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/StopBuild", (BodyWriter) new BuildControllerService.StartStopBodyWriter(nameof (StopBuild), build.Uri, build.QueueIds.FirstOrDefault<int>()));

    public static Message RequestIntermediateLogs(
      string buildUri,
      string logLocation,
      string requestorName,
      Guid requestIdentifier)
    {
      return Message.CreateMessage(MessageVersion.Soap12WSAddressing10, "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/RequestIntermediateLogs", (BodyWriter) new BuildControllerService.RequestIntermediateLogsWriter(buildUri, logLocation, requestorName, requestIdentifier));
    }

    public static Message NotifyWorkflowCompleted(
      int reservationId,
      string ErrorCode,
      string ErrorMessage,
      Uri serviceUri)
    {
      return Message.CreateMessage(MessageVersion.Soap12WSAddressing10, "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/WorkflowCompleted", (BodyWriter) new BuildControllerService.NotifyWorkflowCompletedWriter(reservationId, ErrorCode, ErrorMessage, serviceUri));
    }

    private sealed class DeleteBuildDropBodyWriter : BodyWriter
    {
      private string m_dropLocation;

      public DeleteBuildDropBodyWriter(string dropLocation)
        : base(true)
      {
        this.m_dropLocation = dropLocation;
      }

      protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
      {
        writer.WriteStartElement("DeleteBuildDrop", "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting");
        writer.WriteElementString("dropLocation", this.m_dropLocation);
        writer.WriteEndElement();
      }
    }

    private sealed class DeleteBuildSymbolsBodyWriter : BodyWriter
    {
      private string m_storePath;
      private string m_transactionId;

      public DeleteBuildSymbolsBodyWriter(string storePath, string transactionId)
        : base(true)
      {
        this.m_storePath = storePath;
        this.m_transactionId = transactionId;
      }

      protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
      {
        writer.WriteStartElement("DeleteBuildSymbols", "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting");
        writer.WriteElementString("storePath", this.m_storePath);
        writer.WriteElementString("transactionId", this.m_transactionId);
        writer.WriteEndElement();
      }
    }

    private sealed class NotifyAgentAvailableBodyWriter : BodyWriter
    {
      private string m_buildUri;
      private int m_reservationId;
      private string m_reservedAgentUri;

      public NotifyAgentAvailableBodyWriter(string buildUri, int reservationId, string agentUri)
        : base(true)
      {
        this.m_buildUri = buildUri;
        this.m_reservationId = reservationId;
        this.m_reservedAgentUri = agentUri;
      }

      protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
      {
        writer.WriteStartElement("AgentAcquired", "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting");
        writer.WriteElementString("buildUri", this.m_buildUri);
        writer.WriteElementString("reservationId", this.m_reservationId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        writer.WriteElementString("reservedAgentUri", this.m_reservedAgentUri);
        writer.WriteEndElement();
      }
    }

    private sealed class RequestIntermediateLogsWriter : BodyWriter
    {
      private string m_buildUri;
      private string m_dropLocation;
      private string m_requestorName;
      private Guid m_requestIdentifier;

      public RequestIntermediateLogsWriter(
        string buildUri,
        string logLocation,
        string requestorName,
        Guid requestIdentifier)
        : base(true)
      {
        this.m_buildUri = buildUri;
        this.m_dropLocation = logLocation;
        this.m_requestorName = requestorName;
        this.m_requestIdentifier = requestIdentifier;
      }

      protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
      {
        writer.WriteStartElement("RequestIntermediateLogs", "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting");
        writer.WriteElementString("buildUri", this.m_buildUri);
        writer.WriteElementString("dropLocation", this.m_dropLocation);
        writer.WriteElementString("requestorName", this.m_requestorName);
        writer.WriteElementString("requestIdentifier", this.m_requestIdentifier.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture));
        writer.WriteEndElement();
      }
    }

    private sealed class StartStopBodyWriter : BodyWriter
    {
      private string m_action;
      private string m_buildUri;
      private int m_queueId;

      public StartStopBodyWriter(string action, string buildUri, int queueId)
        : base(true)
      {
        this.m_action = action;
        this.m_buildUri = buildUri;
        this.m_queueId = queueId;
      }

      protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
      {
        writer.WriteStartElement(this.m_action, "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting");
        writer.WriteElementString("buildUri", this.m_buildUri);
        if (this.m_queueId != 0)
          writer.WriteElementString("queueId", this.m_queueId.ToString());
        writer.WriteEndElement();
      }
    }

    private sealed class NotifyWorkflowCompletedWriter : BodyWriter
    {
      private int m_reservationId;
      private string m_errorCode;
      private string m_errorMessage;
      private Uri m_serviceUri;

      public NotifyWorkflowCompletedWriter(
        int reservationId,
        string errorCode,
        string errorMessage,
        Uri serviceUri)
        : base(true)
      {
        this.m_reservationId = reservationId;
        this.m_errorCode = errorCode;
        this.m_errorMessage = errorMessage;
        this.m_serviceUri = serviceUri;
      }

      protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
      {
        writer.WriteStartElement("WorkflowCompleted", "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting");
        writer.WriteElementString("reservationId", this.m_reservationId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        writer.WriteStartElement("error");
        writer.WriteElementString("ErrorCode", this.m_errorCode);
        writer.WriteElementString("ErrorMessage", this.m_errorMessage);
        writer.WriteElementString("ServiceUri", this.m_serviceUri.AbsoluteUri);
        writer.WriteEndElement();
        writer.WriteEndElement();
      }
    }
  }
}
