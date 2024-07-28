// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ErrorReportingTable
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.Azure.Cosmos.Table;
using System.Text;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class ErrorReportingTable : TableEntity
  {
    internal string ToStringValue()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("Partition Key:");
      stringBuilder.Append(this.PartitionKey);
      stringBuilder.Append(" Row Key:");
      stringBuilder.Append(this.RowKey);
      stringBuilder.Append(" Original Record Partition Key:");
      stringBuilder.Append(this.UsageRecordPartitionKey);
      stringBuilder.Append(" Original Usage Row Key:");
      stringBuilder.Append(this.UsageRecordRowKey);
      stringBuilder.Append(" Error Message:");
      stringBuilder.Append(this.Message);
      stringBuilder.Append(" Error Code:");
      stringBuilder.Append(this.Code);
      return stringBuilder.ToString();
    }

    public string UsageRecordPartitionKey { set; get; }

    public string UsageRecordRowKey { set; get; }

    public string Message { set; get; }

    public string Code { set; get; }
  }
}
