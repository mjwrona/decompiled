// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupValidation.DedupValidationResult
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupValidation
{
  [DataContract]
  public class DedupValidationResult : TraversalResultBase
  {
    [DataMember(Name = "offendingDedup", EmitDefaultValue = false)]
    public IDedupInfo OffendingDedup { get; private set; }

    [DataMember(Name = "status", EmitDefaultValue = true)]
    public ResultStatus Status { get; private set; }

    [DataMember(Name = "diagnostics", EmitDefaultValue = false)]
    public Diagnostics Diagnostics { get; set; }

    public override string StatusMessage
    {
      get
      {
        string statusMessage = this.GetStatusMessage();
        if (!string.IsNullOrEmpty(this.ExtraInfo))
          statusMessage = "[" + this.ExtraInfo + "] " + statusMessage;
        return statusMessage;
      }
    }

    private string GetStatusMessage()
    {
      if (this.Exception != null)
        return this.Exception.Message;
      switch (this.OffendingDedup.Status)
      {
        case HealthStatus.Absent:
          return "Couldn't find the blob: " + this.OffendingDedup.Identifier?.ToString();
        case HealthStatus.MissingMetadata:
          return "Couldn't find metadata on the blob: " + this.OffendingDedup.Identifier?.ToString();
        default:
          return this.Status.ToString();
      }
    }

    public DedupValidationResult(DedupTraversalConfig config, string extraInfo)
    {
      this.Status = ResultStatus.Success;
      this.ExtraInfo = extraInfo;
      if (!config.EnableDiagnostics)
        return;
      this.Diagnostics = new Diagnostics();
    }

    public void SetFailure(Exception ex)
    {
      this.Exception = ex;
      this.Status = ResultStatus.ValidationFaulted;
    }

    public void SetFailure(IDedupInfo info)
    {
      switch (info.Status)
      {
        case HealthStatus.Absent:
          this.Status = ResultStatus.NotFound;
          break;
        case HealthStatus.MissingMetadata:
          this.Status = ResultStatus.NotIntact;
          break;
        case HealthStatus.Undetermined:
          this.Exception = new Exception("Couldn't determine the health status of blob: " + info.Identifier?.ToString());
          this.Status = ResultStatus.ValidationFaulted;
          break;
      }
      this.OffendingDedup = info;
    }

    public void SetKeepUntilViolation(IDedupInfo parent, IDedupInfo child)
    {
      this.Exception = new Exception("The keep-until values on a parent and one of its children violate the constraints. " + string.Format("Parent: {0} (KeepUntil = {1}); ", (object) parent.Identifier.ValueString, (object) parent.KeepUntil) + string.Format("Child: {0} (KeepUntil = {1})", (object) child.Identifier.ValueString, (object) child.KeepUntil));
      this.Status = ResultStatus.KeepUntilViolated;
      this.OffendingDedup = child;
    }

    public void IncrementDedupInfoCacheHit() => this.Diagnostics?.IncrementMetrics(ValidationMetrics.DedupInfoCacheHit, 0L);

    public void IncrementValidationCacheHit() => this.Diagnostics?.IncrementMetrics(ValidationMetrics.ValidationCacheHit, 0L);

    public void SetTotalQueueLength(long length) => this.Diagnostics?.IncrementMetrics(ValidationMetrics.EnqueueTotal, length);
  }
}
