// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.AgentSpecificationUtility
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  public static class AgentSpecificationUtility
  {
    private static IEnumerable<string> GetVMImageProperties(JObject agentSpecification) => agentSpecification == null ? (IEnumerable<string>) new string[0] : agentSpecification.Properties().Where<JProperty>((Func<JProperty, bool>) (prop => string.Equals(prop?.Name, TaskAgentRequestConstants.VmImage, StringComparison.OrdinalIgnoreCase))).Select<JProperty, string>((Func<JProperty, string>) (prop => prop?.Value?.ToString()));

    public static IEnumerable<string> GetVMImageProperties(AgentQueueTarget agentQueueTarget) => AgentSpecificationUtility.GetVMImageProperties(agentQueueTarget.AgentSpecification);

    public static IEnumerable<string> GetVMImageProperties(TimelineRecord timelineRecord) => AgentSpecificationUtility.GetVMImageProperties(timelineRecord.AgentSpecification);
  }
}
