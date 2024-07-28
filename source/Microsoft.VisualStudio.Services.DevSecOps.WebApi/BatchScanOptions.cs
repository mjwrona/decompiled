// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.WebApi.BatchScanOptions
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 78BC9F0A-6262-4C96-81AF-14E523464B20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.DevSecOps.WebApi
{
  [DataContract]
  public class BatchScanOptions
  {
    public BatchScanOptions()
    {
    }

    public BatchScanOptions(
      bool returnViolations,
      bool forwardToAdvancedSecurity,
      bool extendedAnalysisEnabled)
    {
      this.ReturnViolations = returnViolations;
      this.ExtendedAnalysisEnabled = extendedAnalysisEnabled;
      this.ForwardToAdvancedSecurity = forwardToAdvancedSecurity;
    }

    [DataMember]
    public bool ReturnViolations { get; set; }

    [DataMember]
    public bool ExtendedAnalysisEnabled { get; set; }

    [DataMember]
    public bool ForwardToAdvancedSecurity { get; set; }
  }
}
