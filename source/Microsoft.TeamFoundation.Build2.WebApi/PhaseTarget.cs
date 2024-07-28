// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.PhaseTarget
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  [KnownType(typeof (AgentPoolQueueTarget))]
  [KnownType(typeof (ServerTarget))]
  [JsonConverter(typeof (PhaseTargetJsonConverter))]
  public abstract class PhaseTarget : BaseSecuredObject
  {
    protected PhaseTarget()
    {
    }

    protected internal PhaseTarget(int type, ISecuredObject securedObject)
      : base(securedObject)
    {
      this.Type = type;
    }

    protected PhaseTarget(int type) => this.Type = type;

    [DataMember]
    public int Type { get; private set; }
  }
}
