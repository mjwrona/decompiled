// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.ServicingOrchestrationRequestStatus
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using Microsoft.VisualStudio.Services.Operations;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  [DataContract]
  public class ServicingOrchestrationRequestStatus
  {
    public ServicingOrchestrationRequestStatus() => this.Properties = new PropertyCollection();

    [DataMember]
    public Guid RequestId { get; set; }

    [DataMember]
    public Guid ServicingJobId { get; set; }

    [DataMember]
    public ServicingOrchestrationStatus Status { get; set; }

    [DataMember]
    public string StatusMessage { get; set; }

    [DataMember]
    public DateTime CreatedDate { get; set; }

    [DataMember]
    public DateTime StartDate { get; set; }

    [DataMember]
    public DateTime CompletedDate { get; set; }

    [DataMember]
    public short TotalStepCount { get; set; }

    [DataMember]
    public short CompletedStepCount { get; set; }

    [DataMember]
    public PropertyCollection Properties { get; set; }

    public override string ToString() => string.Format("[status={0}, completed={1}/{2}, message={3}, properties={4}]", (object) this.Status, (object) this.CompletedStepCount, (object) this.TotalStepCount, (object) this.StatusMessage, (object) this.Properties);

    public bool Succeeded() => this.Status == ServicingOrchestrationStatus.Completed;

    public bool Failed() => this.Status == ServicingOrchestrationStatus.Failed;

    public bool Completed() => this.Succeeded() || this.Failed();

    public bool Active() => !this.Completed();

    public OperationStatus OperationStatus
    {
      get
      {
        switch (this.Status)
        {
          case ServicingOrchestrationStatus.Created:
            return OperationStatus.NotSet;
          case ServicingOrchestrationStatus.Queued:
            return OperationStatus.Queued;
          case ServicingOrchestrationStatus.Running:
            return OperationStatus.InProgress;
          case ServicingOrchestrationStatus.Completed:
            return OperationStatus.Succeeded;
          case ServicingOrchestrationStatus.Failed:
            return OperationStatus.Failed;
          default:
            throw new NotSupportedException(string.Format("Unrecognized servicing status '{0}'", (object) this.Status));
        }
      }
    }
  }
}
