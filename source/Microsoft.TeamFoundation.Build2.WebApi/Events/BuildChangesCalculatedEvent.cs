// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.Events.BuildChangesCalculatedEvent
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi.Events
{
  [DataContract]
  [Obsolete("Use BuildEvent instead.")]
  public class BuildChangesCalculatedEvent : BuildUpdatedEvent
  {
    public BuildChangesCalculatedEvent(Microsoft.TeamFoundation.Build.WebApi.Build build, List<Change> changes)
      : base(build)
    {
      this.Changes = changes;
    }

    [DataMember(IsRequired = true)]
    public List<Change> Changes { get; private set; }
  }
}
