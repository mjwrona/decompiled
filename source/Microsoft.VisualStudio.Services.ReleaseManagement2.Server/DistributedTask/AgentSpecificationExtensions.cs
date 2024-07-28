// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask.AgentSpecificationExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask
{
  internal static class AgentSpecificationExtensions
  {
    private const string AgentSpecificationIdentifierPropertyName = "VMImage";

    public static JObject ToJObject(this AgentSpecification agentSpecification) => new JObject()
    {
      {
        "VMImage",
        (JToken) agentSpecification.Identifier
      }
    };
  }
}
