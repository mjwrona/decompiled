// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Egress.CloudAnalyticsClientAdapter
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.Azure.Storage.Analytics;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Egress
{
  public class CloudAnalyticsClientAdapter : ICloudAnalyticsClientAdapter
  {
    public IEnumerable<LogRecordAdapter> GetLogRecords(ICloudBlob cloudBlob) => CloudAnalyticsClient.ParseLogBlob(cloudBlob).Select<LogRecord, LogRecordAdapter>((Func<LogRecord, LogRecordAdapter>) (logRecord => new LogRecordAdapter(logRecord)));
  }
}
