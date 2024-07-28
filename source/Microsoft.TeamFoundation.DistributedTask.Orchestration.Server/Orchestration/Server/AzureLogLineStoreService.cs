// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.AzureLogLineStoreService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal class AzureLogLineStoreService : IStoreService, IVssFrameworkService
  {
    private Dictionary<string, IAzureTableStorageProvider> storageProviderLookup = new Dictionary<string, IAzureTableStorageProvider>();
    private const string c_storageAccountBaseyKeyName = "DTLogLineAzureConnectionString";
    private const long c_maxLines = 999999999999;

    public void ServiceStart(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      int num = 0;
      while (true)
      {
        string lookupKey = "DTLogLineAzureConnectionString" + num.ToString();
        StrongBoxItemInfo itemInfo = service.GetItemInfo(vssRequestContext, "ConfigurationSecrets", lookupKey, false);
        if (itemInfo != null)
        {
          string connectionString = service.GetString(vssRequestContext, itemInfo);
          this.storageProviderLookup[itemInfo.LookupKey] = this.CreateStorageProvider(requestContext, connectionString);
          ++num;
        }
        else
          break;
      }
      if (this.storageProviderLookup.Count == 0)
        throw new VssServiceException("No connection strings configured with base name DTLogLineAzureConnectionString");
      service.RegisterNotification(vssRequestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxItemChanged), "ConfigurationSecrets", (IEnumerable<string>) new string[1]
      {
        "DTLogLineAzureConnectionString*"
      });
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      vssRequestContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(vssRequestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxItemChanged));
    }

    internal virtual IAzureTableStorageProvider CreateStorageProvider(
      IVssRequestContext requestContext,
      string connectionString)
    {
      return (IAzureTableStorageProvider) new AzureTableProvider(requestContext, connectionString);
    }

    private void OnStrongBoxItemChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      ITeamFoundationStrongBoxService service = requestContext.To(TeamFoundationHostType.Deployment).Elevate().GetService<ITeamFoundationStrongBoxService>();
      foreach (StrongBoxItemName itemName in itemNames)
      {
        StrongBoxItemInfo itemInfo = service.GetItemInfo(requestContext, itemName.DrawerId, itemName.LookupKey);
        string storageAccountConnectionString = service.GetString(requestContext, itemInfo);
        IAzureTableStorageProvider tableStorageProvider;
        if (this.storageProviderLookup.TryGetValue(itemName.LookupKey, out tableStorageProvider) && tableStorageProvider != null)
          tableStorageProvider.UpdateConnectionString(requestContext, storageAccountConnectionString);
      }
    }

    private string AllocateStorageProvider(IVssRequestContext requestContext, string shardKey)
    {
      string[] array = this.storageProviderLookup.Keys.ToArray<string>();
      return array[Math.Abs(shardKey.GetHashCode()) % array.Length];
    }

    private IAzureTableStorageProvider GetStorageProvider(
      IVssRequestContext requestContext,
      string storageProviderKey)
    {
      IAzureTableStorageProvider tableStorageProvider;
      return !this.storageProviderLookup.TryGetValue(storageProviderKey, out tableStorageProvider) ? (IAzureTableStorageProvider) null : tableStorageProvider;
    }

    public void CreateLogTable(
      IVssRequestContext requestContext,
      string tableName,
      out string storageAccountKey)
    {
      storageAccountKey = this.AllocateStorageProvider(requestContext, tableName);
      this.GetStorageProvider(requestContext, storageAccountKey)?.CreateIfNotExists(requestContext, tableName);
    }

    public void DeleteLogTable(
      IVssRequestContext requestContext,
      string tableName,
      string storageAccountKey)
    {
      this.GetStorageProvider(requestContext, storageAccountKey)?.DeleteIfExists(requestContext, tableName);
    }

    public void InsertLogLines(
      IVssRequestContext requestContext,
      string tableName,
      string storageAccountKey,
      Guid planId,
      Guid timelineId,
      Guid jobId,
      Guid taskId,
      IList<TimelineRecordLogLine> loglines)
    {
      IAzureTableStorageProvider storageProvider = this.GetStorageProvider(requestContext, storageAccountKey);
      if (storageProvider == null)
        return;
      AzureTableStorageHelper.ExecuteBatchInsertOrReplaceOperation(requestContext, storageProvider, tableName, (IEnumerable<ITableEntity>) loglines.Select<TimelineRecordLogLine, AzureLogLineStoreService.LogLineEntity>((Func<TimelineRecordLogLine, AzureLogLineStoreService.LogLineEntity>) (logLine => AzureLogLineStoreService.EntityFromLogLine(requestContext, planId, timelineId, jobId, taskId, logLine))));
    }

    public TimelineRecordLogLineResult QueryLogLines(
      IVssRequestContext requestContext,
      string tableName,
      string storageAccountKey,
      Guid planId,
      Guid timelineId,
      Guid jobId,
      Guid taskId,
      string continuationToken,
      long? endLine = null,
      int? takeCount = null)
    {
      TableQuery<AzureLogLineStoreService.LogLineEntity> tableQuery = new TableQuery<AzureLogLineStoreService.LogLineEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", "eq", AzureLogLineStoreService.GetPartitionKey(planId, timelineId, jobId, taskId)));
      TableContinuationToken continuationToken1 = (TableContinuationToken) null;
      if (endLine.HasValue)
        tableQuery = AzureLogLineStoreService.AndWhere<AzureLogLineStoreService.LogLineEntity>(tableQuery, TableQuery.GenerateFilterCondition("RowKey", "ge", AzureLogLineStoreService.GetRowKey(Convert.ToInt64((object) endLine))));
      if (takeCount.HasValue)
        tableQuery.TakeCount = takeCount;
      if (continuationToken != null)
        continuationToken1 = AzureLogLineStoreService.DeserializeToken(continuationToken);
      IAzureTableStorageProvider storageProvider = this.GetStorageProvider(requestContext, storageAccountKey);
      if (storageProvider == null)
        return (TimelineRecordLogLineResult) null;
      IList<AzureLogLineStoreService.LogLineEntity> source = storageProvider.QueryTable<AzureLogLineStoreService.LogLineEntity>(requestContext, tableName, tableQuery, ref continuationToken1);
      continuationToken = continuationToken1 == null ? (string) null : AzureLogLineStoreService.SerializeToken(continuationToken1);
      return new TimelineRecordLogLineResult((IList<TimelineRecordLogLine>) source.Select<AzureLogLineStoreService.LogLineEntity, TimelineRecordLogLine>((Func<AzureLogLineStoreService.LogLineEntity, TimelineRecordLogLine>) (entity => new TimelineRecordLogLine(entity.Content, entity.LineNumber))).Reverse<TimelineRecordLogLine>().ToList<TimelineRecordLogLine>(), continuationToken);
    }

    private static TableQuery<TElement> AndWhere<TElement>(
      TableQuery<TElement> tableQuery,
      string filter)
    {
      tableQuery.FilterString = TableQuery.CombineFilters(tableQuery.FilterString, "and", filter);
      return tableQuery;
    }

    private static AzureLogLineStoreService.LogLineEntity EntityFromLogLine(
      IVssRequestContext requestContext,
      Guid planId,
      Guid timelineId,
      Guid jobId,
      Guid taskId,
      TimelineRecordLogLine logLine)
    {
      return new AzureLogLineStoreService.LogLineEntity(AzureLogLineStoreService.GetPartitionKey(planId, timelineId, jobId, taskId), AzureLogLineStoreService.GetRowKey(logLine.LineNumber), logLine);
    }

    private static string GetPartitionKey(Guid planId, Guid timelineId, Guid jobId, Guid taskId) => string.Format("{0}-{1}-{2}-{3}", (object) planId.ToString(), (object) timelineId.ToString(), (object) jobId.ToString(), (object) taskId.ToString());

    private static string GetRowKey(long lineNumber) => (999999999999L - lineNumber).ToString();

    internal static string SerializeToken(TableContinuationToken token)
    {
      string str = (string) null;
      if (token != null)
      {
        string s = (string) null;
        using (StringWriter output = new StringWriter())
        {
          using (XmlWriter writer = XmlWriter.Create((TextWriter) output))
            token.WriteXml(writer);
          s = output.ToString();
        }
        str = Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
      }
      return str;
    }

    internal static TableContinuationToken DeserializeToken(string tokenString)
    {
      TableContinuationToken token = (TableContinuationToken) null;
      if (!string.IsNullOrWhiteSpace(tokenString))
      {
        using (StringReader input = new StringReader(Encoding.UTF8.GetString(Convert.FromBase64String(tokenString))))
        {
          token = new TableContinuationToken();
          using (XmlReader reader = XmlReader.Create((TextReader) input))
            token.ReadXml(reader);
        }
      }
      return token;
    }

    public class LogLineEntity : TableEntity
    {
      public LogLineEntity()
      {
      }

      public LogLineEntity(string partitionKey, string rowKey)
        : base(partitionKey, rowKey)
      {
      }

      public LogLineEntity(string partionKey, string rowKey, string content, long lineNumber)
        : this(partionKey, rowKey)
      {
        this.Content = content;
        this.LineNumber = lineNumber;
      }

      public LogLineEntity(string partionKey, string rowKey, TimelineRecordLogLine logLine)
        : this(partionKey, rowKey, logLine.Line, logLine.LineNumber)
      {
      }

      public string Content { get; set; }

      public long LineNumber { get; set; }
    }
  }
}
