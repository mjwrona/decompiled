// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.NpmPackagesBatchRequest
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using Microsoft.VisualStudio.Services.Npm.WebApi.Converters;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Npm.WebApi
{
  [DataContract]
  [JsonConverter(typeof (NpmPackagesBatchRequestConverter))]
  public class NpmPackagesBatchRequest : IPackagesBatchRequest
  {
    public static readonly BatchOperationType Deprecate = new BatchOperationType(nameof (Deprecate));
    public static readonly BatchOperationType UpgradeCachedPackages = new BatchOperationType(nameof (UpgradeCachedPackages));

    [DataMember(IsRequired = true)]
    public NpmBatchOperationType Operation { get; set; }

    [DataMember(IsRequired = false)]
    [IgnoreDataMember]
    public BatchOperationData Data { get; set; }

    [DataMember(IsRequired = true)]
    public IEnumerable<MinimalPackageDetails> Packages { get; set; }

    public IBatchOperationType GetOperationType()
    {
      if (this.Operation == NpmBatchOperationType.Unpublish)
        return BatchOperationType.Delete;
      return OperationTypeParser.Parse<NpmBatchOperationType>((IEnumerable<IBatchOperationType>) new BatchOperationType[2]
      {
        NpmPackagesBatchRequest.Deprecate,
        NpmPackagesBatchRequest.UpgradeCachedPackages
      }, this.Operation);
    }
  }
}
