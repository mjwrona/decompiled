// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.IFeatureClient
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Rest;
using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.Management.ResourceManager
{
  public interface IFeatureClient : IDisposable
  {
    Uri BaseUri { get; set; }

    JsonSerializerSettings SerializationSettings { get; }

    JsonSerializerSettings DeserializationSettings { get; }

    ServiceClientCredentials Credentials { get; }

    string SubscriptionId { get; set; }

    string ApiVersion { get; }

    string AcceptLanguage { get; set; }

    int? LongRunningOperationRetryTimeout { get; set; }

    bool? GenerateClientRequestId { get; set; }

    IFeaturesOperations Features { get; }
  }
}
