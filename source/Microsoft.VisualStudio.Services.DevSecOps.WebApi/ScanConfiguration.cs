// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.WebApi.ScanConfiguration
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 78BC9F0A-6262-4C96-81AF-14E523464B20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.DevSecOps.WebApi
{
  [DataContract]
  public class ScanConfiguration
  {
    [DataMember]
    public int RuleSetVersion { get; }

    [DataMember]
    public ISet<string> UnsupportedExtensions { get; }

    public ScanConfiguration(int ruleSetVersion, ISet<string> unsupportedExtensions)
    {
      this.RuleSetVersion = ruleSetVersion;
      this.UnsupportedExtensions = unsupportedExtensions;
    }
  }
}
