// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.PhaseTarget
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  [KnownType(typeof (AgentPoolQueueTarget))]
  [KnownType(typeof (ServerTarget))]
  [JsonConverter(typeof (PhaseTargetJsonConverter))]
  public abstract class PhaseTarget
  {
    protected PhaseTarget(int type) => this.Type = type;

    [DataMember]
    public int Type { get; private set; }
  }
}
