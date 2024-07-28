// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildTrigger
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  [JsonConverter(typeof (BuildTriggerJsonConverter))]
  public abstract class BuildTrigger
  {
    protected BuildTrigger(DefinitionTriggerType triggerType) => this.TriggerType = triggerType;

    protected virtual BuildTrigger CloneInternal(BuildTrigger trigger)
    {
      trigger.TriggerType = this.TriggerType;
      return trigger;
    }

    public abstract BuildTrigger Clone();

    [DataMember]
    public DefinitionTriggerType TriggerType { get; set; }
  }
}
