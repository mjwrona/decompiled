// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.VssStorageEventLogger
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class VssStorageEventLogger : IVssEventLogger
  {
    private string m_roleName;
    private string m_deploymentId;
    private string m_serviceName;
    private string m_storageConnectionString;
    private string m_source;
    private CloudTable m_table;

    protected CloudTable Table
    {
      get
      {
        if (this.m_table == null)
        {
          CloudTableClient cloudTableClient = CloudStorageAccount.Parse(this.m_storageConnectionString).CreateCloudTableClient();
          cloudTableClient.DefaultRequestOptions.RetryPolicy = (IRetryPolicy) new LinearRetry(TimeSpan.FromSeconds(30.0), 10);
          this.m_table = cloudTableClient.GetTableReference(this.m_serviceName);
          this.m_table.CreateIfNotExists();
        }
        return this.m_table;
      }
    }

    public VssStorageEventLogger(
      string serviceName,
      string deploymentId,
      string storageConnectionString,
      string roleName = null,
      string source = null)
    {
      this.m_serviceName = !string.IsNullOrEmpty(serviceName) ? this.GetValidTableName(serviceName) : throw new ArgumentNullException(nameof (serviceName));
      this.m_deploymentId = deploymentId;
      this.m_storageConnectionString = storageConnectionString;
      this.m_roleName = roleName;
      this.m_source = source;
    }

    private string GetValidTableName(string serviceName) => serviceName.Replace("-", string.Empty);

    public void WriteEntry(string message, EventLogEntryType type, int eventId)
    {
      if (string.IsNullOrEmpty(message))
        throw new ArgumentNullException(nameof (message));
      if (string.IsNullOrEmpty(this.m_deploymentId))
        throw new ApplicationException("deploymentId needs to be initialized in order to write events");
      VssStorageEventLogEntry entity = new VssStorageEventLogEntry();
      entity.PartitionKey = this.m_deploymentId;
      entity.RowKey = Guid.NewGuid().ToString();
      entity.Message = message;
      entity.EntryType = type;
      entity.EventId = eventId;
      entity.TimeGenerated = DateTime.UtcNow;
      entity.TimeWritten = DateTime.UtcNow;
      entity.Source = this.m_source;
      entity.RoleName = this.m_roleName;
      entity.MachineName = Environment.MachineName;
      this.Table.Execute(TableOperation.Insert((ITableEntity) entity));
    }

    public List<IVssEventLogEntry> Get(DateTime? startTime = null)
    {
      List<string> values = new List<string>();
      if (!string.IsNullOrEmpty(this.m_deploymentId))
        values.Add(TableQuery.GenerateFilterCondition("PartitionKey", "eq", this.m_deploymentId));
      if (startTime.HasValue)
        values.Add(TableQuery.GenerateFilterConditionForDate("TimeGenerated", "ge", new DateTimeOffset(startTime.Value)));
      TableQuery<VssStorageEventLogEntry> query = new TableQuery<VssStorageEventLogEntry>();
      if (values.Count > 0)
        query = query.Where(string.Join(" and ", (IEnumerable<string>) values));
      return ((IEnumerable<IVssEventLogEntry>) this.Table.ExecuteQuery<VssStorageEventLogEntry>(query)).ToList<IVssEventLogEntry>();
    }
  }
}
