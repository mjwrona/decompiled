// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.WebApi.Types.MavenPackagesBatchRequest
// Assembly: Microsoft.VisualStudio.Services.Maven.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 62CDE373-A3CE-478E-B824-A307191D9BE2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.WebApi.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.WebApi.Types
{
  [DataContract]
  [JsonConverter(typeof (MavenPackagesBatchRequestConverter))]
  public class MavenPackagesBatchRequest
  {
    [DataMember(IsRequired = true)]
    public MavenBatchOperationType Operation { get; set; }

    [DataMember(IsRequired = false)]
    [IgnoreDataMember]
    public BatchOperationData Data { get; set; }

    [DataMember(IsRequired = true)]
    public IEnumerable<MavenMinimalPackageDetails> Packages { get; set; }

    public IBatchOperationType GetOperationType() => OperationTypeParser.Parse<MavenBatchOperationType>((IEnumerable<IBatchOperationType>) new IBatchOperationType[0], this.Operation);
  }
}
