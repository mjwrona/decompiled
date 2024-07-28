// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public sealed class CodeCoverageSummary : TestManagementBaseSecuredObject
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IList<CodeCoverageData> CoverageData { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public ShallowReference Build { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public ShallowReference DeltaBuild { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public CoverageSummaryStatus Status { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public CoverageDetailedSummaryStatus CoverageDetailedSummaryStatus { get; set; }

    internal override void InitializeSecureObject(ISecuredObject securedObject)
    {
      base.InitializeSecureObject(securedObject);
      if (this.CoverageData != null)
      {
        foreach (TestManagementBaseSecuredObject baseSecuredObject in (IEnumerable<CodeCoverageData>) this.CoverageData)
          baseSecuredObject.InitializeSecureObject(securedObject);
      }
      if (this.Build != null)
        this.Build.InitializeSecureObject(securedObject);
      if (this.DeltaBuild == null)
        return;
      this.DeltaBuild.InitializeSecureObject(securedObject);
    }
  }
}
