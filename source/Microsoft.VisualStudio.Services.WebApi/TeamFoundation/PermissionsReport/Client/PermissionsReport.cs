// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.Client.PermissionsReport
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.PermissionsReport.Client
{
  [DataContract]
  public class PermissionsReport
  {
    public PermissionsReport()
    {
    }

    public PermissionsReport(
      Guid reportId,
      string reportName,
      DateTime requestedTime,
      SubjectDescriptor requestor,
      PermissionsReportStatus reportStatus,
      DateTime reportStatusLastUpdatedTime,
      string error)
    {
      this.Id = reportId;
      this.ReportName = reportName;
      this.RequestedTime = requestedTime;
      this.Requestor = requestor;
      this.ReportStatus = reportStatus;
      this.ReportStatusLastUpdatedTime = reportStatusLastUpdatedTime;
      this.Error = error;
    }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public Guid Id { get; private set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public string ReportName { get; private set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public SubjectDescriptor Requestor { get; private set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public DateTime RequestedTime { get; private set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public PermissionsReportStatus ReportStatus { get; private set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public DateTime ReportStatusLastUpdatedTime { get; private set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public string Error { get; private set; }
  }
}
