// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Telemetry.VmMetadataApiHandler
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Util;
using Microsoft.Azure.Documents;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Telemetry
{
  internal static class VmMetadataApiHandler
  {
    internal const string HashedMachineNamePrefix = "hashedMachineName:";
    internal const string HashedVmIdPrefix = "hashedVmId:";
    internal const string UuidPrefix = "uuid:";
    internal static readonly Uri vmMetadataEndpointUrl = new Uri("http://169.254.169.254/metadata/instance?api-version=2020-06-01");
    private static readonly string nonAzureCloud = "NonAzureVM";
    private static readonly object lockObject = new object();
    private static bool isInitialized = false;
    private static AzureVMMetadata azMetadata = (AzureVMMetadata) null;
    private static readonly Lazy<string> uniqueId = new Lazy<string>((Func<string>) (() =>
    {
      try
      {
        return "hashedMachineName:" + HashingExtension.ComputeHash(Environment.MachineName);
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceWarning("Error while generating hashed machine name " + ex.Message);
      }
      return string.Format("{0}{1}", (object) "uuid:", (object) Guid.NewGuid());
    }));

    internal static void TryInitialize(CosmosHttpClient httpClient)
    {
      if (VmMetadataApiHandler.isInitialized)
        return;
      lock (VmMetadataApiHandler.lockObject)
      {
        if (VmMetadataApiHandler.isInitialized)
          return;
        DefaultTrace.TraceInformation("Initializing VM Metadata API ");
        VmMetadataApiHandler.isInitialized = true;
        Task.Run((Func<Task>) (() => VmMetadataApiHandler.MetadataApiCallAsync(httpClient)), new CancellationToken());
      }
    }

    private static async Task MetadataApiCallAsync(CosmosHttpClient httpClient)
    {
      try
      {
        DefaultTrace.TraceInformation("Loading VM Metadata");
        VmMetadataApiHandler.azMetadata = await VmMetadataApiHandler.ProcessResponseAsync(await httpClient.SendHttpAsync(new Func<ValueTask<HttpRequestMessage>>(CreateRequestMessage), ResourceType.Telemetry, HttpTimeoutPolicyNoRetry.Instance, (IClientSideRequestStatistics) null, new CancellationToken()));
        DefaultTrace.TraceInformation("Succesfully get Instance Metadata Response : " + VmMetadataApiHandler.azMetadata.Compute.VMId);
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceInformation("Azure Environment metadata information not available. " + ex.Message);
      }

      static ValueTask<HttpRequestMessage> CreateRequestMessage()
      {
        HttpRequestMessage result = new HttpRequestMessage();
        result.RequestUri = VmMetadataApiHandler.vmMetadataEndpointUrl;
        result.Method = HttpMethod.Get;
        result.Headers.Add("Metadata", "true");
        return new ValueTask<HttpRequestMessage>(result);
      }
    }

    internal static async Task<AzureVMMetadata> ProcessResponseAsync(
      HttpResponseMessage httpResponseMessage)
    {
      return httpResponseMessage.Content == null ? (AzureVMMetadata) null : JObject.Parse(await httpResponseMessage.Content.ReadAsStringAsync()).ToObject<AzureVMMetadata>();
    }

    internal static string GetMachineId() => !string.IsNullOrWhiteSpace(VmMetadataApiHandler.azMetadata?.Compute?.VMId) ? VmMetadataApiHandler.azMetadata.Compute.VMId : VmMetadataApiHandler.uniqueId.Value;

    internal static Compute GetMachineInfo() => VmMetadataApiHandler.azMetadata?.Compute;

    internal static string GetMachineRegion() => VmMetadataApiHandler.azMetadata?.Compute?.Location;

    internal static string GetCloudInformation() => VmMetadataApiHandler.azMetadata?.Compute?.AzEnvironment ?? VmMetadataApiHandler.nonAzureCloud;
  }
}
