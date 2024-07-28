// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Utilities.PipelineWebHookNotificationUtilities
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications;
using Newtonsoft.Json;
using System.Xml;

namespace Microsoft.Azure.Pipelines.Deployment.Utilities
{
  internal static class PipelineWebHookNotificationUtilities
  {
    public static XmlNode SerializeToJsonXmlNode(this VssNotificationEvent notification) => TeamFoundationSerializationUtility.SerializeToXml((object) JsonConvert.SerializeObject((object) notification));

    public static VssNotificationEvent DeserializeFromJsonXmlNode(XmlNode notification) => JsonConvert.DeserializeObject<VssNotificationEvent>(TeamFoundationSerializationUtility.Deserialize<string>(notification));

    public static XmlNode SerializeToJsonXmlNode(this PipelineWebHookEventData eventData) => TeamFoundationSerializationUtility.SerializeToXml((object) JsonConvert.SerializeObject((object) eventData));

    public static PipelineWebHookEventData DeserializeFromWebHookEventDataXmlNode(XmlNode eventData) => JsonConvert.DeserializeObject<PipelineWebHookEventData>(TeamFoundationSerializationUtility.Deserialize<string>(eventData));
  }
}
