// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskGroupRevision
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskGroupRevision
  {
    [DataMember]
    public Guid TaskGroupId { get; set; }

    [DataMember]
    public int Revision { get; set; }

    [DataMember]
    public int MajorVersion { get; set; }

    [DataMember]
    public IdentityRef ChangedBy { get; set; }

    [DataMember]
    public DateTime ChangedDate { get; set; }

    [DataMember]
    public AuditAction ChangeType { get; set; }

    [DataMember]
    public int FileId { get; set; }

    [DataMember]
    public string Comment { get; set; }
  }
}
