// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.SqlEntity
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  internal class SqlEntity : SqlOperationResult, IComparable<SqlEntity>
  {
    internal string PartitionKey { get; set; }

    internal string RowKey { get; set; }

    internal ITableEntity AzureTableEntity { get; set; }

    public int CompareTo(SqlEntity other)
    {
      int num = string.Compare(this.PartitionKey, other.PartitionKey, StringComparison.Ordinal);
      if (num == 0)
        num = string.Compare(this.RowKey, other.RowKey, StringComparison.Ordinal);
      return num;
    }
  }
}
