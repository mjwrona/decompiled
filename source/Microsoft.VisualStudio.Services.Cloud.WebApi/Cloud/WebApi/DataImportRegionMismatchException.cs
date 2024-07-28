// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.DataImportRegionMismatchException
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  [Serializable]
  public class DataImportRegionMismatchException : DataImportException
  {
    public string SourceRegion { get; set; }

    public string TargetRegion { get; set; }

    public string AccountRegion { get; set; }

    public DataImportRegionMismatchException(
      string sourceRegion,
      string targetRegion,
      string accountRegion)
      : base(DataImportResources.DataImportSourceAndTargetRegionMismatch((object) sourceRegion, (object) targetRegion, (object) accountRegion))
    {
      this.SourceRegion = sourceRegion;
      this.TargetRegion = targetRegion;
      this.AccountRegion = accountRegion;
    }

    public DataImportRegionMismatchException(string sourceRegion, string targetRegion)
      : base(DataImportResources.DataImportSQLVMRegionMismatch((object) sourceRegion, (object) targetRegion))
    {
      this.SourceRegion = sourceRegion;
      this.TargetRegion = targetRegion;
    }

    public DataImportRegionMismatchException(string message)
      : base(message)
    {
    }

    public DataImportRegionMismatchException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected DataImportRegionMismatchException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
