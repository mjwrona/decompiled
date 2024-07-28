// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.GetContainerBlobsOrTableEntriesJob
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class GetContainerBlobsOrTableEntriesJob : VssAsyncJobExtension
  {
    private readonly TraceData traceData = new TraceData()
    {
      Area = "BlobStore",
      Layer = "Job"
    };
    private const int Tracepoint = 1100112;
    private const string resultContainerName = "proddumps";

    public override async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, this.traceData, 1100112, nameof (RunAsync)))
      {
        if (jobDefinition.Data == null)
          return new VssJobResult(TeamFoundationJobExecutionResult.Failed, "No JobData provided");
        GetContainerBlobsOrTableEntriesJobParameter jobParameters = TeamFoundationSerializationUtility.Deserialize<GetContainerBlobsOrTableEntriesJobParameter>(jobDefinition.Data);
        string message;
        if (!this.ValidateJobParameters(jobParameters, out message))
          return new VssJobResult(TeamFoundationJobExecutionResult.Failed, message);
        try
        {
          string resultBlobName = jobParameters.ServiceName + "_" + jobParameters.DeploymentName + "_" + DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss") + "_SortedEntriesIn_" + jobParameters.ResourceName + ".txt";
          this.UploadResultsToDiagnosticStorageAccount(await this.GetSortedEntriesFromStorageAsync(requestContext, jobParameters, tracer).ConfigureAwait(true), jobParameters, resultBlobName, tracer);
          return new VssJobResult(TeamFoundationJobExecutionResult.Succeeded, "Uploaded blob:" + resultBlobName + " containing the list of entry names within the container/table: " + jobParameters.ResourceName + " to the diagnostic blob storage container proddumps.");
        }
        catch (Exception ex)
        {
          return new VssJobResult(TeamFoundationJobExecutionResult.Failed, "Listing all entry names within the container/table '" + jobParameters.ResourceName + "' failed with " + JobHelper.GetNestedExceptionMessage(ex));
        }
      }
    }

    private async Task<SortedSet<string>> GetSortedEntriesFromStorageAsync(
      IVssRequestContext requestContext,
      GetContainerBlobsOrTableEntriesJobParameter jobParameter,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      ConcurrentBag<IEnumerable<string>> entryNamesSetsAcrossShards = new ConcurrentBag<IEnumerable<string>>();
      SortedSet<string> sortedEntryNamesAcrossShards = new SortedSet<string>();
      try
      {
        IEnumerable<StrongBoxConnectionString> inputs = StorageAccountConfigurationFacade.ReadAllStorageAccounts(requestContext);
        Func<StrongBoxConnectionString, Task> action = (Func<StrongBoxConnectionString, Task>) (async shardConnectionString =>
        {
          IEnumerable<string> strings;
          if (jobParameter.StorageType.Equals((object) StorageType.Blob))
            strings = await this.GetBlobNamesListForStorageAccountAsync(Microsoft.Azure.Storage.CloudStorageAccount.Parse(shardConnectionString.ConnectionString), jobParameter.ResourceName).ConfigureAwait(true);
          else if (jobParameter.StorageType.Equals((object) StorageType.Table))
          {
            strings = await this.GetTableRowEntriesForStorageAccountAsync(Microsoft.Azure.Cosmos.Table.CloudStorageAccount.Parse(shardConnectionString.ConnectionString), jobParameter.ResourceName).ConfigureAwait(true);
          }
          else
          {
            string message = string.Format("Storage type:{0} should be of type Blob/Table.", (object) jobParameter.StorageType);
            tracer.TraceError(message);
            throw new InvalidOperationException(message);
          }
          entryNamesSetsAcrossShards.Add(strings);
        });
        ExecutionDataflowBlockOptions dataflowBlockOptions = new ExecutionDataflowBlockOptions();
        dataflowBlockOptions.MaxDegreeOfParallelism = Environment.ProcessorCount;
        dataflowBlockOptions.CancellationToken = requestContext.CancellationToken;
        ActionBlock<StrongBoxConnectionString> actionBlock = NonSwallowingActionBlock.Create<StrongBoxConnectionString>(action, dataflowBlockOptions);
        await actionBlock.SendAllAndCompleteAsync<StrongBoxConnectionString, StrongBoxConnectionString>(inputs, (ITargetBlock<StrongBoxConnectionString>) actionBlock, requestContext.CancellationToken).ConfigureAwait(true);
        foreach (IEnumerable<string> other in entryNamesSetsAcrossShards)
          sortedEntryNamesAcrossShards.UnionWith(other);
        tracer.TraceInfo(string.Format("Found {0} entries in the container/table: {1}.", (object) entryNamesSetsAcrossShards.Count, (object) jobParameter.ResourceName));
      }
      catch (Exception ex)
      {
        tracer.TraceException(ex);
        throw;
      }
      SortedSet<string> fromStorageAsync = sortedEntryNamesAcrossShards;
      sortedEntryNamesAcrossShards = (SortedSet<string>) null;
      return fromStorageAsync;
    }

    private void UploadResultsToDiagnosticStorageAccount(
      SortedSet<string> entryNamesSet,
      GetContainerBlobsOrTableEntriesJobParameter jobParameter,
      string blobName,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      try
      {
        AzureBlobClient azureBlobClient = new AzureBlobClient(jobParameter.DiagnosticConnectionString, (ITFLogger) null);
        azureBlobClient.CreateContainerIfNotExist("proddumps", BlobContainerPublicAccessType.Off);
        using (Stream stream = (Stream) new MemoryStream(entryNamesSet.SelectMany<string, byte>((Func<string, IEnumerable<byte>>) (s => (IEnumerable<byte>) Encoding.ASCII.GetBytes(s + "\n"))).ToArray<byte>()))
          azureBlobClient.UploadBlob("proddumps", blobName, stream);
        tracer.TraceInfo("Uploading entry names set to diagnostics storage is complete. Results are in container 'proddumps' with name '" + blobName + "'");
      }
      catch (Exception ex)
      {
        tracer.TraceException(ex);
        throw;
      }
    }

    private bool ValidateJobParameters(
      GetContainerBlobsOrTableEntriesJobParameter jobParameter,
      out string message)
    {
      if (!jobParameter.StorageType.Equals((object) StorageType.Blob) && !jobParameter.StorageType.Equals((object) StorageType.Table))
        message = string.Format("Storage type: {0} should be of type Blob/Table!", (object) jobParameter.StorageType);
      else if (string.IsNullOrEmpty(jobParameter.ResourceName))
        message = "ResourceName param cannot be null/empty.";
      else if (string.IsNullOrEmpty(jobParameter.ServiceName))
        message = "ServiceName param cannot be null/empty.";
      else if (string.IsNullOrEmpty(jobParameter.DeploymentName))
        message = "DeploymentName param cannot be null/empty.";
      else if (string.IsNullOrEmpty(jobParameter.DiagnosticConnectionString))
      {
        message = "DiagnosticConnectionString param cannot be null/empty.";
      }
      else
      {
        message = (string) null;
        return true;
      }
      return false;
    }

    private async Task<IEnumerable<string>> GetBlobNamesListForStorageAccountAsync(
      Microsoft.Azure.Storage.CloudStorageAccount storageAccount,
      string containerName)
    {
      CloudBlobContainer cloudBlobContainer = storageAccount.CreateCloudBlobClient().GetContainerReference(containerName);
      List<string> blobNames = new List<string>();
      BlobContinuationToken currentToken = (BlobContinuationToken) null;
      do
      {
        Microsoft.Azure.Storage.Blob.BlobResultSegment blobResultSegment = await cloudBlobContainer.ListBlobsSegmentedAsync((string) null, true, BlobListingDetails.None, new int?(), currentToken, (BlobRequestOptions) null, (Microsoft.Azure.Storage.OperationContext) null).ConfigureAwait(true);
        List<string> stringList = blobNames;
        IEnumerable<IListBlobItem> results = blobResultSegment.Results;
        IEnumerable<string> collection = results != null ? results.Cast<CloudBlockBlob>().Select<CloudBlockBlob, string>((Func<CloudBlockBlob, string>) (blob => blob.Name)) : (IEnumerable<string>) null;
        stringList.AddRange(collection);
        currentToken = blobResultSegment.ContinuationToken;
      }
      while (currentToken != null);
      IEnumerable<string> storageAccountAsync = (IEnumerable<string>) blobNames;
      cloudBlobContainer = (CloudBlobContainer) null;
      blobNames = (List<string>) null;
      return storageAccountAsync;
    }

    private async Task<IEnumerable<string>> GetTableRowEntriesForStorageAccountAsync(
      Microsoft.Azure.Cosmos.Table.CloudStorageAccount storageAccount,
      string tableName)
    {
      CloudTable cloudTable = storageAccount.CreateCloudTableClient().GetTableReference(tableName);
      TableQuery<DynamicTableEntity> query = new TableQuery<DynamicTableEntity>();
      List<string> tableEntryNames = new List<string>();
      TableContinuationToken token = (TableContinuationToken) null;
      Dictionary<string, SortedSet<string>> partitionKeyToRowEntriesMap = new Dictionary<string, SortedSet<string>>();
      do
      {
        TableQuerySegment<DynamicTableEntity> tableQuerySegment = await cloudTable.ExecuteQuerySegmentedAsync<DynamicTableEntity>(query, token).ConfigureAwait(true);
        foreach (DynamicTableEntity result in tableQuerySegment.Results)
        {
          if (!partitionKeyToRowEntriesMap.ContainsKey(result.PartitionKey))
            partitionKeyToRowEntriesMap[result.PartitionKey] = new SortedSet<string>();
          partitionKeyToRowEntriesMap[result.PartitionKey].Add(result.RowKey);
        }
        token = tableQuerySegment.ContinuationToken;
      }
      while (token != null);
      foreach (KeyValuePair<string, SortedSet<string>> keyValuePair in partitionKeyToRowEntriesMap)
        tableEntryNames.Add(keyValuePair.Key + ":" + keyValuePair.Value.Aggregate<string>((Func<string, string, string>) ((x, y) => x + y)));
      IEnumerable<string> storageAccountAsync = (IEnumerable<string>) tableEntryNames;
      cloudTable = (CloudTable) null;
      query = (TableQuery<DynamicTableEntity>) null;
      tableEntryNames = (List<string>) null;
      partitionKeyToRowEntriesMap = (Dictionary<string, SortedSet<string>>) null;
      return storageAccountAsync;
    }
  }
}
