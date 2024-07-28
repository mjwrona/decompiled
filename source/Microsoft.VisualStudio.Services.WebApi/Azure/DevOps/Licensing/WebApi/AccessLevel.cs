// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.Licensing.WebApi.AccessLevel
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Licensing;
using System.Runtime.Serialization;

namespace Microsoft.Azure.DevOps.Licensing.WebApi
{
  [DataContract]
  public class AccessLevel
  {
    [DataMember]
    public LicensingSource LicensingSource { get; set; }

    [DataMember]
    public AccountLicenseType AccountLicenseType { get; set; }

    [DataMember]
    public MsdnLicenseType MsdnLicenseType { get; set; }

    [DataMember]
    public string LicenseDisplayName { get; set; }

    [DataMember]
    public AccountUserStatus Status { get; set; }

    [DataMember]
    public string StatusMessage { get; set; }

    [DataMember]
    public AssignmentSource AssignmentSource { get; set; }
  }
}
