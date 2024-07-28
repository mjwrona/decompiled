// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.BasePackagesBatchRequestConverter
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9764DF62-33FE-41B6-9E79-DE201B497BE0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.WebApi
{
  public abstract class BasePackagesBatchRequestConverter : 
    VssJsonCreationConverter<IPackagesBatchRequest>
  {
    public abstract bool IsSupportedOperation(string operationType);

    public abstract IPackagesBatchRequest Create(string operationType);

    protected override IPackagesBatchRequest Create(Type objectType, JObject jsonObject)
    {
      string operationTypeString = this.GetBatchOperationTypeString(jsonObject);
      return !this.IsSupportedOperation(operationTypeString) ? (IPackagesBatchRequest) null : this.Create(operationTypeString);
    }

    protected string GetBatchOperationTypeString(JObject jsonObject) => this.GetValueOrDefault<string>(jsonObject, "operation");

    protected T GetValueOrDefault<T>(JObject jsonObject, string property)
    {
      JToken jtoken = jsonObject.GetValue(property, StringComparison.OrdinalIgnoreCase);
      return jtoken == null ? default (T) : jtoken.Value<T>();
    }
  }
}
