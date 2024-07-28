// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.WebApi.ContainerResult
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 78BC9F0A-6262-4C96-81AF-14E523464B20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.DevSecOps.WebApi
{
  [DataContract]
  public class ContainerResult
  {
    public ContainerResult(string containerId, Microsoft.VisualStudio.Services.DevSecOps.WebApi.RulesetType? rulesetType, int? rulesetVersion)
    {
      this.ContainerId = containerId;
      this.RulesetType = rulesetType;
      this.RulesetVersion = rulesetVersion;
      this.ScanError = (ScanError) null;
      this.StreamResults = (IList<StreamResult>) new List<StreamResult>();
    }

    [DataMember]
    public string ContainerId { get; }

    [DataMember]
    public Microsoft.VisualStudio.Services.DevSecOps.WebApi.RulesetType? RulesetType { get; }

    [DataMember]
    public int? RulesetVersion { get; }

    [DataMember]
    public IList<StreamResult> StreamResults { get; }

    [DataMember]
    public ScanError ScanError { get; private set; }

    [DataMember]
    public bool WasSarifSubmitted { get; set; }

    public bool WasSuccessful => this.ScanError == null;

    public void SetScanError(ScanError scanError) => this.ScanError = scanError;

    public void SetWasSarifSubmitted() => this.WasSarifSubmitted = true;
  }
}
