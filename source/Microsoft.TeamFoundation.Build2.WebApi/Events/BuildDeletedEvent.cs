// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.Events.BuildDeletedEvent
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi.Events
{
  [DataContract]
  public class BuildDeletedEvent : RealtimeBuildEvent
  {
    public BuildDeletedEvent(Microsoft.TeamFoundation.Build.WebApi.Build deletedBuild)
      : base(deletedBuild.Id)
    {
      this.Build = deletedBuild;
    }

    [DataMember(IsRequired = true)]
    public Microsoft.TeamFoundation.Build.WebApi.Build Build { get; private set; }
  }
}
