// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.ExternalGitPushYamlHandlerJobDataUtilities
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System.Xml;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public static class ExternalGitPushYamlHandlerJobDataUtilities
  {
    public static XmlNode SerializeToJsonXmlNode(
      this ExternalGitPushYamlHandlerJobData notification)
    {
      return TeamFoundationSerializationUtility.SerializeToXml((object) JsonConvert.SerializeObject((object) notification));
    }

    public static ExternalGitPushYamlHandlerJobData DeserializeFromJsonXmlNode(XmlNode notification) => JsonConvert.DeserializeObject<ExternalGitPushYamlHandlerJobData>(TeamFoundationSerializationUtility.Deserialize<string>(notification));
  }
}
