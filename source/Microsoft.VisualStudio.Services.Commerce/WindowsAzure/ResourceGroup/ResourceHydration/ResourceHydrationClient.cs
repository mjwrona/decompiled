// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsAzure.ResourceGroup.ResourceHydration.ResourceHydrationClient
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.WindowsAzure.ResourceGroup.ResourceHydration
{
  [ExcludeFromCodeCoverage]
  public class ResourceHydrationClient
  {
    private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings()
    {
      MaxDepth = new int?(512),
      TypeNameHandling = TypeNameHandling.None,
      ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver(),
      Converters = (IList<JsonConverter>) new List<JsonConverter>()
      {
        (JsonConverter) new StringEnumConverter(),
        (JsonConverter) new IsoDateTimeConverter()
        {
          DateTimeStyles = DateTimeStyles.AssumeUniversal
        }
      }
    };
    private static readonly string QueueName = "resourcehydration";
    private readonly CloudQueue queue;
    private readonly TimeSpan visibilityTimeout;
    private bool isInitialized;

    public ResourceHydrationClient(
      string connectionString,
      TimeSpan? visibilityTimeout = null,
      IRequestOptions requestOptions = null)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(connectionString, nameof (connectionString));
      CloudQueueClient cloudQueueClient = CloudStorageAccount.Parse(connectionString).CreateCloudQueueClient();
      if (requestOptions != null)
      {
        cloudQueueClient.DefaultRequestOptions.ServerTimeout = requestOptions.ServerTimeout;
        cloudQueueClient.DefaultRequestOptions.MaximumExecutionTime = requestOptions.MaximumExecutionTime;
        cloudQueueClient.DefaultRequestOptions.RetryPolicy = requestOptions.RetryPolicy;
      }
      this.queue = cloudQueueClient.GetQueueReference(ResourceHydrationClient.QueueName);
      this.visibilityTimeout = visibilityTimeout ?? TimeSpan.FromMinutes(5.0);
    }

    public void PrepareResourceHydrationRequest(
      IVssRequestContext requestContext,
      ResourceHydrationRequest hydrationRequest)
    {
      this.SubmitResourceHydrationRequest(requestContext, hydrationRequest, this.visibilityTimeout);
    }

    public void CommitResourceHydrationRequest(
      IVssRequestContext requestContext,
      ResourceHydrationRequest hydrationRequest)
    {
      this.SubmitResourceHydrationRequest(requestContext, hydrationRequest, TimeSpan.Zero);
    }

    private void SubmitResourceHydrationRequest(
      IVssRequestContext requestContext,
      ResourceHydrationRequest hydrationRequest,
      TimeSpan delay)
    {
      ArgumentUtility.CheckForNull<ResourceHydrationRequest>(hydrationRequest, nameof (hydrationRequest));
      string content = JsonConvert.SerializeObject((object) hydrationRequest, ResourceHydrationClient.SerializerSettings);
      this.Initialize(requestContext);
      this.queue.AddMessage(new CloudQueueMessage(content), new TimeSpan?(CloudQueueMessage.MaxVisibilityTimeout), new TimeSpan?(delay));
    }

    private void Initialize(IVssRequestContext requestContext)
    {
      if (this.isInitialized)
        return;
      this.queue.CreateIfNotExists();
      this.isInitialized = true;
    }
  }
}
