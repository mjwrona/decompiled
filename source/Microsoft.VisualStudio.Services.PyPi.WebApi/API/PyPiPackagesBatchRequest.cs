// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API.PyPiPackagesBatchRequest
// Assembly: Microsoft.VisualStudio.Services.PyPi.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17E1C323-94FE-4FF1-903A-ED51BA3159D2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.WebApi.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API
{
  [DataContract]
  [JsonConverter(typeof (PyPiPackagesBatchRequestConverter))]
  public class PyPiPackagesBatchRequest : IPackagesBatchRequest
  {
    [DataMember(IsRequired = true)]
    public PyPiBatchOperationType Operation { get; set; }

    [DataMember(IsRequired = false)]
    [IgnoreDataMember]
    public BatchOperationData Data { get; set; }

    [DataMember(IsRequired = true)]
    public IEnumerable<MinimalPackageDetails> Packages { get; set; }

    public IBatchOperationType GetOperationType() => OperationTypeParser.Parse<PyPiBatchOperationType>((IEnumerable<IBatchOperationType>) new IBatchOperationType[0], this.Operation);
  }
}
