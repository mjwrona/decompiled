// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildResourceUsage
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public sealed class BuildResourceUsage
  {
    internal BuildResourceUsage()
    {
    }

    internal BuildResourceUsage(
      int xaml,
      int dtAgents,
      int paidAgentSlots,
      bool isThrottlingEnabled = false)
    {
      this.XamlControllers = xaml;
      this.DistributedTaskAgents = dtAgents;
      this.TotalUsage = this.XamlControllers + (isThrottlingEnabled ? 0 : this.DistributedTaskAgents);
      this.PaidPrivateAgentSlots = paidAgentSlots;
    }

    [DataMember]
    public int XamlControllers { get; internal set; }

    [DataMember]
    public int DistributedTaskAgents { get; internal set; }

    [DataMember]
    public int TotalUsage { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public int PaidPrivateAgentSlots { get; internal set; }
  }
}
