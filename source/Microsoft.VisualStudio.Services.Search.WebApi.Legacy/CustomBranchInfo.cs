// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.CustomBranchInfo
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.WebApi
{
  [DataContract]
  public class CustomBranchInfo
  {
    [DataMember(Name = "BranchName")]
    public string BranchName { get; set; }

    [DataMember(Name = "ChangeId")]
    public string ChangeId { get; set; }

    [DataMember(Name = "ChangeTime")]
    public DateTime ChangeTime { get; set; }

    [DataMember(Name = "AuditTrail")]
    public AuditTrail AuditTrail { get; set; }

    public CustomBranchInfo(string branchName, string changeId, DateTime changeTime)
    {
      this.BranchName = branchName;
      this.ChangeId = changeId;
      this.ChangeTime = changeTime;
    }

    public string ToString(int indentLevel)
    {
      StringBuilder sb = new StringBuilder();
      string indentSpacing = Extensions.GetIndentSpacing(indentLevel);
      sb.Append(indentSpacing, "BranchName: ").AppendLine(this.BranchName);
      sb.Append(indentSpacing, "ChangeId: ").AppendLine(this.ChangeId);
      sb.Append(indentSpacing, "ChangeTime: ").AppendLine(this.ChangeTime.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (this.AuditTrail != null)
        sb.Append(indentSpacing, "AuditTrail: ").AppendLine(this.AuditTrail.ToString());
      return sb.ToString();
    }

    public override string ToString() => this.ToString(0);
  }
}
