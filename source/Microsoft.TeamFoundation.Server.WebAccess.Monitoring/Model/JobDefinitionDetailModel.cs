// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Monitoring.Model.JobDefinitionDetailModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Monitoring, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2931506-B8BC-4923-B99C-2CD8E1087ABB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Monitoring.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Monitoring.Model
{
  [DataContract]
  public class JobDefinitionDetailModel
  {
    public JobDefinitionDetailModel(
      IVssRequestContext hostRequestContext,
      TeamFoundationJobDefinition job)
    {
      this.JobId = job.JobId;
      this.JobName = string.Empty;
      this.HostId = hostRequestContext.ServiceHost.InstanceId;
      this.HostName = hostRequestContext.ServiceHost.Name;
      this.ExtensionName = job.ExtensionName;
      if (job.Data != null)
        this.Data = string.IsNullOrEmpty(job.Data.InnerXml) ? job.Data.InnerText : job.Data.InnerXml;
      this.EnabledState = (int) (byte) job.EnabledState;
      this.PriorityClass = job.PriorityClass.ToString();
      this.EnabledStateString = job.EnabledState.ToString();
      this.PriorityClassString = job.PriorityClass.ToString();
    }

    [DataMember(Name = "jobId")]
    public Guid JobId { get; set; }

    [DataMember(Name = "hostId")]
    public Guid HostId { get; set; }

    [DataMember(Name = "jobName")]
    public string JobName { get; set; }

    [DataMember(Name = "hostName")]
    public string HostName { get; set; }

    [DataMember(Name = "extensionName")]
    public string ExtensionName { get; set; }

    [DataMember(Name = "data")]
    public string Data { get; set; }

    [DataMember(Name = "enabledState")]
    public int EnabledState { get; set; }

    [DataMember(Name = "priorityClass")]
    public string PriorityClass { get; set; }

    [DataMember(Name = "priorityClassString")]
    public string PriorityClassString { get; set; }

    [DataMember(Name = "enabledStateString")]
    public string EnabledStateString { get; set; }
  }
}
