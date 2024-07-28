// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.BulkCodeIndexResponse
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.WebApi
{
  [DataContract]
  public class BulkCodeIndexResponse
  {
    public BulkCodeIndexResponse() => this.Errors = (IEnumerable<ErrorData>) new List<ErrorData>();

    [DataMember(Name = "accepted")]
    public bool Accepted { get; set; }

    [DataMember(Name = "trackingId")]
    public string TrackingId { get; set; }

    [DataMember(Name = "errors")]
    public IEnumerable<ErrorData> Errors { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "indentLevel+1", Justification = "indentLevel is not likely to exceed int.Max range.")]
    public string ToString(int indentLevel)
    {
      StringBuilder sb = new StringBuilder();
      string indentSpacing = Extensions.GetIndentSpacing(indentLevel);
      sb.AppendLine(indentSpacing, "Accepted: ").Append(this.Accepted.ToString());
      sb.AppendLine(indentSpacing, "Tracking Id: ").Append(this.TrackingId.ToString());
      if (this.Errors != null)
      {
        sb.AppendLine(indentSpacing, "Errors: ");
        foreach (ErrorData error in this.Errors)
          sb.Append(error.ToString(indentLevel + 1));
      }
      return sb.ToString();
    }

    public override string ToString() => this.ToString(0);
  }
}
