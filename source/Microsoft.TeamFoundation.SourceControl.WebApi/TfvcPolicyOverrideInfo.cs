// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.TfvcPolicyOverrideInfo
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class TfvcPolicyOverrideInfo
  {
    public TfvcPolicyOverrideInfo()
    {
    }

    public TfvcPolicyOverrideInfo(string comment, IEnumerable<TfvcPolicyFailureInfo> policyFailures)
    {
      this.Comment = comment;
      this.PolicyFailures = policyFailures ?? Enumerable.Empty<TfvcPolicyFailureInfo>();
    }

    [DataMember(Name = "comment", EmitDefaultValue = false)]
    public string Comment { get; set; }

    [DataMember(Name = "policyFailures", EmitDefaultValue = false)]
    public IEnumerable<TfvcPolicyFailureInfo> PolicyFailures { get; set; }
  }
}
