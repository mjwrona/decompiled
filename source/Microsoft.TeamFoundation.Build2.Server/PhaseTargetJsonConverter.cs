// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.PhaseTargetJsonConverter
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class PhaseTargetJsonConverter : TypePropertyJsonConverter<PhaseTarget>
  {
    protected override PhaseTarget GetInstance(Type objectType)
    {
      if (objectType == typeof (AgentPoolQueueTarget))
        return (PhaseTarget) new AgentPoolQueueTarget();
      return objectType == typeof (ServerTarget) ? (PhaseTarget) new ServerTarget() : base.GetInstance(objectType);
    }

    protected override PhaseTarget GetInstance(int targetType, JObject value)
    {
      if (targetType == 1)
        return (PhaseTarget) new AgentPoolQueueTarget();
      return targetType == 2 ? (PhaseTarget) new ServerTarget() : (PhaseTarget) null;
    }
  }
}
