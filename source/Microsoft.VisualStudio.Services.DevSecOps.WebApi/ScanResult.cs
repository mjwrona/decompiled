// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.WebApi.ScanResult
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 78BC9F0A-6262-4C96-81AF-14E523464B20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.DevSecOps.WebApi
{
  [DataContract]
  public class ScanResult
  {
    public ScanResult() => this.UserGroups = (ICollection<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public bool HasViolations
    {
      get
      {
        IList<Violation> violations = this.Violations;
        return violations != null && violations.Any<Violation>((Func<Violation, bool>) (v => v.IsValid));
      }
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ErrorMessage { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IList<Violation> Violations { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ICollection<string> UserGroups { get; set; }
  }
}
