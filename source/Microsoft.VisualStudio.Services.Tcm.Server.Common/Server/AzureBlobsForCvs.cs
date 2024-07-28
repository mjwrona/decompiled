// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.AzureBlobsForCvs
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class AzureBlobsForCvs : IAzureBlobForCvs
  {
    private List<ILogStoreCloudBlockBlob> _logStoreCloudBlockBlobs;
    private string _containerWaterMark;
    private string _blobWaterMark;

    public AzureBlobsForCvs()
    {
      this._logStoreCloudBlockBlobs = new List<ILogStoreCloudBlockBlob>();
      this._containerWaterMark = string.Empty;
      this._blobWaterMark = string.Empty;
    }

    public void AddRangeLogStoreCloudBlockBlob(
      List<ILogStoreCloudBlockBlob> logStoreCloudBlockBlobs)
    {
      this._logStoreCloudBlockBlobs.AddRange((IEnumerable<ILogStoreCloudBlockBlob>) logStoreCloudBlockBlobs);
    }

    public List<ILogStoreCloudBlockBlob> GetLogStoreCloudBlockBlobs() => this._logStoreCloudBlockBlobs;

    public string GetBlobWaterMark() => this._blobWaterMark;

    public string GetContainerWaterMark() => this._containerWaterMark;

    public void SetContainerWaterMark(string containerWaterMark) => this._containerWaterMark = containerWaterMark;

    public void SetBlobWaterMark(string blobWaterMark) => this._blobWaterMark = blobWaterMark;
  }
}
