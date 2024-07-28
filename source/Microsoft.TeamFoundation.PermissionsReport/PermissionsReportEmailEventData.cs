// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.PermissionsReportEmailEventData
// Assembly: Microsoft.TeamFoundation.PermissionsReport, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5F2A073E-CE9C-4394-ABB1-512D7FB702F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.PermissionsReport.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.PermissionsReport
{
  [DataContract]
  public class PermissionsReportEmailEventData
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string RequesterDisplayName { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public Guid RequesterIdentityId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string ReportUrl { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string ReportRequestTime { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public Dictionary<string, IList<string>> ResourceMap { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string ReportError { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string LearnMoreUrl { get; set; }
  }
}
