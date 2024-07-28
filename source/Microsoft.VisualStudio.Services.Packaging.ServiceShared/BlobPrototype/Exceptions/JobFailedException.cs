// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Exceptions.JobFailedException
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Exceptions
{
  public class JobFailedException : Exception
  {
    public JobTelemetry Telemetry { get; set; }

    public JobFailedException(Exception ex)
      : this(ex, new JobTelemetry())
    {
    }

    public JobFailedException(Exception innerException, JobTelemetry telemetry)
      : base(string.Empty, innerException)
    {
      this.Telemetry = telemetry;
    }
  }
}
