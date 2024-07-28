// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceRequestFailedException
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Cloud.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class ServiceRequestFailedException : Exception
  {
    public Guid ServiceId { get; private set; }

    public ServicingOrchestrationRequestStatus Status { get; private set; }

    public ServiceRequestFailedException(Guid serviceId, ServicingOrchestrationRequestStatus status)
      : base(string.Format("Service {0} failed to process request, status={1}", (object) serviceId, (object) status))
    {
      this.ServiceId = serviceId;
      this.Status = status;
      this.MarkAsFatalServicingOrchestrationException();
    }
  }
}
