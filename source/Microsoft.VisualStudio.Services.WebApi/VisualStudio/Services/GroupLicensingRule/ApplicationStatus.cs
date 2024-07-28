// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.GroupLicensingRule.ApplicationStatus
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Operations;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.GroupLicensingRule
{
  [DataContract]
  public class ApplicationStatus
  {
    [DataMember]
    public OperationStatus Status { get; set; }

    [DataMember]
    public RuleOption Option { get; set; }

    [DataMember]
    public bool IsTruncated { get; set; }

    [DataMember]
    public ICollection<LicenseApplicationStatus> Licenses { get; set; }

    [DataMember]
    public ICollection<ExtensionApplicationStatus> Extensions { get; set; }

    public ApplicationStatus()
    {
    }

    public ApplicationStatus(OperationStatus status, RuleOption option = RuleOption.TestApplyGroupRule)
    {
      this.Status = status;
      this.Option = option;
    }
  }
}
