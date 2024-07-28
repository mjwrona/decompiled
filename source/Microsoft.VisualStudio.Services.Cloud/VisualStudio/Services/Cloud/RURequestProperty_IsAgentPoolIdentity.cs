// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RURequestProperty_IsAgentPoolIdentity
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class RURequestProperty_IsAgentPoolIdentity : RURequestProperty_Bool
  {
    private const string c_agentPoolIdentity = "AgentPoolIdentity";
    private const string c_agentPoolName = "AgentPool";

    public override bool ShouldOutputEntityToTelemetry { get; protected set; }

    public override object GetRequestValue(IVssRequestContext requestContext)
    {
      bool requestValue;
      if (!requestContext.RootContext.TryGetItem<bool>("AgentPoolIdentity", out requestValue))
      {
        string role;
        requestValue = IdentityHelper.TryParseFrameworkServiceIdentityDescriptor(requestContext.UserContext, out Guid _, out role, out string _) && string.Equals(role, "AgentPool", StringComparison.OrdinalIgnoreCase);
        requestContext.RootContext.Items["AgentPoolIdentity"] = (object) requestValue;
      }
      return (object) requestValue;
    }
  }
}
