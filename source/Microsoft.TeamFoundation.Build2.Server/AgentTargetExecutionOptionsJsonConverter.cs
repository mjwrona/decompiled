// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.AgentTargetExecutionOptionsJsonConverter
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class AgentTargetExecutionOptionsJsonConverter : 
    TypePropertyJsonConverter<AgentTargetExecutionOptions>
  {
    protected override AgentTargetExecutionOptions GetInstance(Type objectType)
    {
      if (objectType == typeof (AgentTargetExecutionType))
        return new AgentTargetExecutionOptions();
      if (objectType == typeof (VariableMultipliersAgentExecutionOptions))
        return (AgentTargetExecutionOptions) new VariableMultipliersAgentExecutionOptions();
      return objectType == typeof (MultipleAgentExecutionOptions) ? (AgentTargetExecutionOptions) new MultipleAgentExecutionOptions() : base.GetInstance(objectType);
    }

    protected override AgentTargetExecutionOptions GetInstance(int targetType, JObject value)
    {
      switch (targetType)
      {
        case 0:
          return new AgentTargetExecutionOptions();
        case 1:
          return (AgentTargetExecutionOptions) new VariableMultipliersAgentExecutionOptions();
        case 2:
          return (AgentTargetExecutionOptions) new MultipleAgentExecutionOptions();
        default:
          return (AgentTargetExecutionOptions) null;
      }
    }
  }
}
