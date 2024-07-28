// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitAsyncRefOperation
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public abstract class GitAsyncRefOperation
  {
    [DataMember]
    public GitAsyncRefOperationParameters Parameters { get; set; }

    [DataMember]
    public GitAsyncOperationStatus Status { get; set; }

    [DataMember]
    public GitAsyncRefOperationDetail DetailedStatus { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "_links", IsRequired = false)]
    public ReferenceLinks Links { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Url { get; set; }
  }
}
