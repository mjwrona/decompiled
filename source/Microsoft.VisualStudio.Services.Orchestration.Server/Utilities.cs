// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.Utilities
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Orchestration.History;
using System;
using System.Diagnostics;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  internal static class Utilities
  {
    public static TaskMessage GetForcedTerminateMessage(
      string sessionId,
      string reason,
      DateTime timestamp)
    {
      return new TaskMessage()
      {
        OrchestrationInstance = new OrchestrationInstance()
        {
          InstanceId = sessionId
        },
        Event = (HistoryEvent) new ExecutionTerminatedEvent(-1, timestamp, reason)
      };
    }

    public static OrchestrationMessage GetMessageFromObject(
      OrchestrationHubDescription description,
      OrchestrationSerializer serializer,
      TaskMessage messageObject)
    {
      return Utilities.GetMessageFromObject(description, serializer, (string) null, messageObject);
    }

    public static OrchestrationMessage GetMessageFromObject(
      OrchestrationHubDescription description,
      OrchestrationSerializer serializer,
      string sessionId,
      TaskMessage messageObject)
    {
      ArgumentUtility.CheckForNull<TaskMessage>(messageObject, nameof (messageObject));
      CompressionType compressionType;
      return new OrchestrationMessage(Utilities.SerializeObject(description, serializer, (object) messageObject, out compressionType, out int _))
      {
        ScheduledDeliveryTime = messageObject.FireAt,
        SessionId = sessionId,
        CompressionType = compressionType,
        DispatcherType = messageObject.DispatcherType
      };
    }

    public static T GetObjectFromMessage<T>(
      OrchestrationSerializer serializer,
      OrchestrationMessage message)
    {
      ArgumentUtility.CheckForNull<OrchestrationMessage>(message, nameof (message));
      return Utilities.DeserializeObject<T>(serializer, message.CompressionType, message.Content);
    }

    public static T DeserializeObject<T>(
      OrchestrationSerializer serializer,
      CompressionType compressionType,
      byte[] objectBytes)
    {
      return serializer.DeserializeFromBytes<T>(objectBytes, compressionType == CompressionType.GZip);
    }

    public static byte[] SerializeObject(
      OrchestrationHubDescription description,
      OrchestrationSerializer serializer,
      object serializableObject,
      out CompressionType compressionType,
      out int uncompressedSizeInBytes)
    {
      byte[] bytes = serializer.SerializeToBytes(serializableObject);
      if (description.CompressionSettings.Style != CompressionStyle.Always)
      {
        CompressionSettings compressionSettings = description.CompressionSettings;
        if (compressionSettings.Style == CompressionStyle.Threshold)
        {
          int length = bytes.Length;
          compressionSettings = description.CompressionSettings;
          int thresholdInBytes = compressionSettings.ThresholdInBytes;
          if (length > thresholdInBytes)
            goto label_3;
        }
        compressionType = CompressionType.None;
        uncompressedSizeInBytes = bytes.Length;
        return bytes;
      }
label_3:
      compressionType = CompressionType.GZip;
      uncompressedSizeInBytes = bytes.Length;
      return serializer.Compress(bytes);
    }

    public static string GetHubName(TeamFoundationJobDefinition jobDefinition) => Utilities.ParseJobData(jobDefinition).HubName;

    public static string GetDispatcherType(TeamFoundationJobDefinition jobDefinition) => Utilities.ParseJobData(jobDefinition).DispatcherType;

    public static XmlNode GetJobData(string hubName, string dispatcherType = null)
    {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.XmlResolver = (XmlResolver) null;
      XmlElement element1 = xmlDocument.CreateElement("TaskHub");
      XmlElement element2 = xmlDocument.CreateElement("Name");
      element1.AppendChild((XmlNode) element2).AppendChild((XmlNode) xmlDocument.CreateTextNode(hubName));
      if (!string.IsNullOrEmpty(dispatcherType))
      {
        XmlElement element3 = xmlDocument.CreateElement("DispatcherType");
        element1.AppendChild((XmlNode) element3).AppendChild((XmlNode) xmlDocument.CreateTextNode(dispatcherType));
      }
      return (XmlNode) element1;
    }

    public static TraceLevel ToTraceLevel(TraceEventType eventType)
    {
      switch (eventType)
      {
        case TraceEventType.Critical:
        case TraceEventType.Error:
          return TraceLevel.Error;
        case TraceEventType.Warning:
          return TraceLevel.Warning;
        case TraceEventType.Information:
          return TraceLevel.Info;
        case TraceEventType.Verbose:
          return TraceLevel.Verbose;
        default:
          return TraceLevel.Info;
      }
    }

    private static Utilities.JobDefinitionData ParseJobData(
      TeamFoundationJobDefinition jobDefinition)
    {
      Utilities.JobDefinitionData jobData = new Utilities.JobDefinitionData()
      {
        DispatcherType = string.Empty
      };
      if (jobDefinition.Data != null && jobDefinition.Data.LocalName == "TaskHub")
      {
        foreach (XmlNode selectNode in jobDefinition.Data.SelectNodes("/*"))
        {
          if (selectNode.LocalName == "Name")
            jobData.HubName = selectNode.InnerText;
          else if (selectNode.LocalName == "DispatcherType")
            jobData.DispatcherType = selectNode.InnerText;
        }
      }
      return jobData;
    }

    private struct JobDefinitionData
    {
      public string HubName;
      public string DispatcherType;
    }
  }
}
