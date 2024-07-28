// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.FeedBatchDataConverter
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8DACB936-5231-4131-8ED8-082A1F46DC54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  public class FeedBatchDataConverter : VssJsonCreationConverter<FeedBatchData>
  {
    protected override FeedBatchData Create(Type objectType, JObject jsonObject)
    {
      FeedBatchOperation result;
      if (!Enum.TryParse<FeedBatchOperation>(this.GetValueOrDefault<string>(jsonObject, "operation"), true, out result))
        throw InvalidUserInputException.Create("operation");
      FeedBatchOperationData batchOperationData = result != FeedBatchOperation.SaveCachedPackages ? (FeedBatchOperationData) null : (FeedBatchOperationData) new SaveCachedPackagesData();
      return new FeedBatchData()
      {
        Operation = result,
        Data = batchOperationData
      };
    }

    protected T GetValueOrDefault<T>(JObject jsonObject, string property)
    {
      JToken jtoken = jsonObject.GetValue(property, StringComparison.OrdinalIgnoreCase);
      return jtoken == null ? default (T) : jtoken.Value<T>();
    }
  }
}
