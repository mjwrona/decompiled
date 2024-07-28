// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.BillableCommitterDetail
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class BillableCommitterDetail : BillableCommitter
  {
    [DataMember]
    public Guid ProjectId { get; set; }

    [DataMember]
    public string ProjectName { get; set; }

    [DataMember]
    public string RepoName { get; set; }

    [DataMember]
    public string CommitId { get; set; }

    [DataMember]
    public string CommitterEmail { get; set; }

    [DataMember]
    public DateTime CommitTime { get; set; }

    [DataMember]
    public int PushId { get; set; }

    [DataMember]
    public DateTime PushedTime { get; set; }

    [DataMember]
    public Guid PusherId { get; set; }

    [DataMember]
    public string SamAccountName { get; set; }

    [DataMember]
    public string MailNickName { get; set; }

    [DataMember]
    public string DisplayName { get; set; }
  }
}
