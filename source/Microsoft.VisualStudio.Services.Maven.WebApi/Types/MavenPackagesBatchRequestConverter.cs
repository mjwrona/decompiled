// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.WebApi.Types.MavenPackagesBatchRequestConverter
// Assembly: Microsoft.VisualStudio.Services.Maven.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 62CDE373-A3CE-478E-B824-A307191D9BE2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.WebApi.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Maven.WebApi.Types
{
  public class MavenPackagesBatchRequestConverter : 
    VssJsonCreationConverter<MavenPackagesBatchRequest>
  {
    private Dictionary<MavenBatchOperationType, Func<BatchOperationData>> supportedTypes = new Dictionary<MavenBatchOperationType, Func<BatchOperationData>>()
    {
      {
        MavenBatchOperationType.Promote,
        (Func<BatchOperationData>) (() => (BatchOperationData) new BatchPromoteData())
      },
      {
        MavenBatchOperationType.Delete,
        (Func<BatchOperationData>) (() => (BatchOperationData) null)
      },
      {
        MavenBatchOperationType.PermanentDelete,
        (Func<BatchOperationData>) (() => (BatchOperationData) null)
      },
      {
        MavenBatchOperationType.RestoreToFeed,
        (Func<BatchOperationData>) (() => (BatchOperationData) null)
      }
    };

    protected override MavenPackagesBatchRequest Create(Type objectType, JObject jsonObject)
    {
      string operationTypeString = this.GetBatchOperationTypeString(jsonObject);
      return !this.IsSupportedOperation(operationTypeString) ? (MavenPackagesBatchRequest) null : this.Create(operationTypeString);
    }

    protected string GetBatchOperationTypeString(JObject jsonObject) => this.GetValueOrDefault<string>(jsonObject, "operation");

    protected T GetValueOrDefault<T>(JObject jsonObject, string property)
    {
      JToken jtoken = jsonObject.GetValue(property, StringComparison.OrdinalIgnoreCase);
      return jtoken == null ? default (T) : jtoken.Value<T>();
    }

    public bool IsSupportedOperation(string operation) => Enum.TryParse<MavenBatchOperationType>(operation, true, out MavenBatchOperationType _);

    public MavenPackagesBatchRequest Create(string operationType)
    {
      MavenBatchOperationType key = (MavenBatchOperationType) Enum.Parse(typeof (MavenBatchOperationType), operationType, true);
      BatchOperationData batchOperationData = this.supportedTypes[key]();
      return new MavenPackagesBatchRequest()
      {
        Operation = key,
        Data = batchOperationData
      };
    }
  }
}
