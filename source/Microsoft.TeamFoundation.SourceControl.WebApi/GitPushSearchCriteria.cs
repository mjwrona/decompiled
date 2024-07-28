// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitPushSearchCriteria
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class GitPushSearchCriteria
  {
    [DataMember(Name = "fromDate", EmitDefaultValue = false)]
    public DateTime? FromDate { get; set; }

    [DataMember(Name = "toDate", EmitDefaultValue = false)]
    public DateTime? ToDate { get; set; }

    [DataMember(Name = "pusherId", EmitDefaultValue = false)]
    public Guid? PusherId { get; set; }

    [DataMember(Name = "refName", EmitDefaultValue = false)]
    public string RefName { get; set; }

    [DataMember(Name = "includeRefUpdates", EmitDefaultValue = false)]
    public bool IncludeRefUpdates { get; set; }

    [DataMember(Name = "includeLinks", EmitDefaultValue = false)]
    public bool IncludeLinks { get; set; }
  }
}
