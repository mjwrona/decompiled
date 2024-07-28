// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.DataImportRequestStatus
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  [DataContract]
  public class DataImportRequestStatus : ServicingOrchestrationRequestStatus
  {
    public DataImportRequestStatus()
    {
    }

    public DataImportRequestStatus(ServicingOrchestrationRequestStatus status)
    {
      this.RequestId = status.RequestId;
      this.ServicingJobId = status.ServicingJobId;
      this.Status = status.Status;
      this.StatusMessage = status.StatusMessage;
      this.CreatedDate = status.CreatedDate;
      this.StartDate = status.StartDate;
      this.CompletedDate = status.CompletedDate;
      this.TotalStepCount = status.TotalStepCount;
      this.CompletedStepCount = status.CompletedStepCount;
      this.Properties = status.Properties.Clone();
    }
  }
}
