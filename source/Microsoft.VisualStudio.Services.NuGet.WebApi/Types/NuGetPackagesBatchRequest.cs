// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.WebApi.Types.NuGetPackagesBatchRequest
// Assembly: Microsoft.VisualStudio.Services.NuGet.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D44F181-506D-4445-A06B-7AA7FD5D22D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.WebApi.dll

using Microsoft.VisualStudio.Services.NuGet.WebApi.Converters;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.NuGet.WebApi.Types
{
  [DataContract]
  [JsonConverter(typeof (NuGetPackagesBatchRequestConverter))]
  public class NuGetPackagesBatchRequest : IPackagesBatchRequest
  {
    public static readonly BatchOperationType List = new BatchOperationType(nameof (List));

    [DataMember(IsRequired = true)]
    public NuGetBatchOperationType Operation { get; set; }

    [DataMember(IsRequired = false)]
    [IgnoreDataMember]
    public BatchOperationData Data { get; set; }

    [DataMember(IsRequired = true)]
    public IEnumerable<MinimalPackageDetails> Packages { get; set; }

    public IBatchOperationType GetOperationType() => OperationTypeParser.Parse<NuGetBatchOperationType>((IEnumerable<IBatchOperationType>) new BatchOperationType[1]
    {
      NuGetPackagesBatchRequest.List
    }, this.Operation);
  }
}
