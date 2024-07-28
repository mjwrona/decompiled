// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Query.DiagnosticInfo
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

namespace Microsoft.Cloud.Metrics.Client.Query
{
  public sealed class DiagnosticInfo
  {
    public string TraceId { get; internal set; }

    public string HandlingServerId { get; internal set; }

    public string ErrorMessage { get; internal set; }

    public override string ToString() => "TraceId:" + this.TraceId + ", HandlingServerId:" + this.HandlingServerId + ", ErrorMessage:" + this.ErrorMessage + ".";
  }
}
