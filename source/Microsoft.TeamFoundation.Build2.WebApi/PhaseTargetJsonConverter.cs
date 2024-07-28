// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.PhaseTargetJsonConverter
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  internal sealed class PhaseTargetJsonConverter : TypePropertyJsonConverter<PhaseTarget>
  {
    protected override PhaseTarget GetInstance(Type objectType)
    {
      if (objectType == typeof (AgentPoolQueueTarget))
        return (PhaseTarget) new AgentPoolQueueTarget();
      return objectType == typeof (ServerTarget) ? (PhaseTarget) new ServerTarget() : base.GetInstance(objectType);
    }

    protected override PhaseTarget GetInstance(int targetType)
    {
      if (targetType == 1)
        return (PhaseTarget) new AgentPoolQueueTarget();
      return targetType == 2 ? (PhaseTarget) new ServerTarget() : (PhaseTarget) null;
    }
  }
}
