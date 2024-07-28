// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.AgentSpecificationExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Newtonsoft.Json.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class AgentSpecificationExtensions
  {
    private const string c_agentSpecificationIdentifierPropertyName = "VMImage";

    public static JObject ToJObject(this AgentSpecification agentSpecification) => new JObject()
    {
      {
        "VMImage",
        (JToken) agentSpecification.Identifier
      }
    };
  }
}
