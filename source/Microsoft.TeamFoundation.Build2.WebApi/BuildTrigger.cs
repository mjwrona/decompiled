// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildTrigger
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  [KnownType(typeof (ContinuousIntegrationTrigger))]
  [KnownType(typeof (GatedCheckInTrigger))]
  [KnownType(typeof (ScheduleTrigger))]
  [KnownType(typeof (PullRequestTrigger))]
  [JsonConverter(typeof (BuildTriggerJsonConverter))]
  public abstract class BuildTrigger : BaseSecuredObject
  {
    protected BuildTrigger(DefinitionTriggerType triggerType)
      : this(triggerType, (ISecuredObject) null)
    {
    }

    protected internal BuildTrigger(DefinitionTriggerType triggerType, ISecuredObject securedObject)
      : base(securedObject)
    {
      this.TriggerType = triggerType;
    }

    [DataMember]
    public DefinitionTriggerType TriggerType { get; private set; }
  }
}
