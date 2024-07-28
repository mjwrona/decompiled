// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.GitHubAppPushHandlerJobData
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class GitHubAppPushHandlerJobData
  {
    public Guid ProjectId { get; set; }

    public string PipelineProviderId { get; set; }

    public List<int> DefinitionIds { get; set; }

    public ExternalGitPush Push { get; set; }

    public XmlNode Serialize() => TeamFoundationSerializationUtility.SerializeToXml((object) JsonConvert.SerializeObject((object) this));

    public static GitHubAppPushHandlerJobData Deserialize(XmlNode xmlNode) => JsonConvert.DeserializeObject<GitHubAppPushHandlerJobData>(TeamFoundationSerializationUtility.Deserialize<string>(xmlNode));
  }
}
