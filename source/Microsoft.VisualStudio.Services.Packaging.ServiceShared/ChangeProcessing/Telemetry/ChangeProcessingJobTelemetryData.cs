// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing.Telemetry.ChangeProcessingJobTelemetryData
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing.Telemetry
{
  public class ChangeProcessingJobTelemetryData : JobTelemetry
  {
    public ChangeProcessingJobTelemetryData()
    {
      this.ErrorLogLines = new List<string>();
      this.MaxTryCount = 1;
    }

    public int MaxTryCount { get; set; }

    public int ExecutedTryCount { get; set; }

    public List<string> ErrorLogLines { get; set; }

    public void LogErrorMessage(string errorMessage, [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int callerLineNumber = 0) => this.ErrorLogLines.Add(string.Format("[Try #{0}/{1}] [{2}:L{3}] {4}", (object) (this.ExecutedTryCount + 1), (object) this.MaxTryCount, (object) callerMemberName, (object) callerLineNumber, (object) errorMessage));
  }
}
