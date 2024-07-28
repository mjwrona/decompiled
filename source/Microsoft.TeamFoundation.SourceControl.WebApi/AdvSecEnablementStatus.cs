// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.AdvSecEnablementStatus
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class AdvSecEnablementStatus
  {
    [DataMember]
    public Guid ProjectId { get; set; }

    [DataMember]
    public Guid RepositoryId { get; set; }

    [DataMember]
    public bool? Enabled { get; set; }

    [DataMember]
    public DateTime? EnabledChangedOnDate { get; set; }

    [DataMember]
    public DateTime? ChangedOnDate { get; set; }

    [DataMember]
    public Guid ChangedById { get; set; }

    [DataMember(IsRequired = false)]
    public bool DependabotEnabled { get; set; }
  }
}
