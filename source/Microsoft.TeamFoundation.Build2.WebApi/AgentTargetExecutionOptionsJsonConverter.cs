// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.AgentTargetExecutionOptionsJsonConverter
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  internal sealed class AgentTargetExecutionOptionsJsonConverter : 
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

    protected override AgentTargetExecutionOptions GetInstance(int targetType)
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
