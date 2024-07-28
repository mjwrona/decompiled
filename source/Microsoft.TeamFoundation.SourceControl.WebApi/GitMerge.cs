// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitMerge
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class GitMerge : GitMergeParameters
  {
    [DataMember(Name = "mergeOperationId")]
    public int OperationId { get; set; }

    [DataMember(Name = "status", EmitDefaultValue = false)]
    public GitAsyncOperationStatus Status { get; set; }

    [DataMember(Name = "detailedStatus", EmitDefaultValue = false)]
    public GitMergeOperationStatusDetail DetailedStatus { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false, IsRequired = false)]
    public ReferenceLinks Links { get; set; }
  }
}
