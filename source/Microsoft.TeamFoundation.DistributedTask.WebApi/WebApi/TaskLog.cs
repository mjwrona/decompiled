// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskLog
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class TaskLog : TaskLogReference
  {
    internal TaskLog()
    {
    }

    public TaskLog(string path)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      this.Path = path;
    }

    [DataMember(EmitDefaultValue = false)]
    public Uri IndexLocation { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Path { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public long LineCount { get; set; }

    [DataMember]
    public DateTime CreatedOn { get; internal set; }

    [DataMember]
    public DateTime LastChangedOn { get; internal set; }
  }
}
