// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.TracerCICorrelationForDeploymentDetails
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4AA9C920-1627-4C01-9F3D-849A7BC9C916
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry
{
  public class TracerCICorrelationForDeploymentDetails : ITracerCICorrelationDetails
  {
    public string CorrelationId { get; set; }

    public string CurrentContext { get; private set; }

    public string Operation { get; set; }

    public int Trigger { get; set; }

    public DateTime TriggerTimeUtc { get; set; }

    public TracerCICorrelationForDeploymentDetails(string correlationId, string currentContext)
    {
      this.CurrentContext = currentContext;
      this.CorrelationId = correlationId;
      this.Operation = string.Empty;
      this.Trigger = 0;
      this.TriggerTimeUtc = DateTime.UtcNow;
    }

    public virtual IDictionary<string, object> GetAllCorrelationDetails() => (IDictionary<string, object>) new Dictionary<string, object>()
    {
      ["CorrelationId"] = (object) this.CorrelationId,
      ["Operation"] = (object) this.Operation,
      ["Trigger"] = (object) this.Trigger,
      ["CurrentContext"] = (object) this.CurrentContext,
      ["TriggerTime"] = (object) this.TriggerTimeUtc
    };
  }
}
