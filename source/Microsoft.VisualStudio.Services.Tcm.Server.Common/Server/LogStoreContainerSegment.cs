// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LogStoreContainerSegment
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class LogStoreContainerSegment : ILogStoreContainerSegment
  {
    private IVssRequestContext _requestContext;
    private ContainerResultSegment _containerResultSegment;
    private LogStoreBlobContinuationToken _logStoreBlobContinuationToken;

    public LogStoreContainerSegment(
      ContainerResultSegment containerResultSegment,
      IVssRequestContext requestContext)
    {
      this._requestContext = requestContext;
      this._containerResultSegment = containerResultSegment;
      this._logStoreBlobContinuationToken = new LogStoreBlobContinuationToken(this._containerResultSegment?.ContinuationToken);
    }

    public LogStoreBlobContinuationToken GetLogStoreBlobContinuationToken() => this._logStoreBlobContinuationToken;

    public List<TestLogContainer> GetTestContainerList()
    {
      List<TestLogContainer> testContainerList = new List<TestLogContainer>();
      if (this._containerResultSegment == null)
        return testContainerList;
      foreach (CloudBlobContainer result in this._containerResultSegment.Results)
        testContainerList.Add(this.GetTestContainer(result));
      return testContainerList;
    }

    private TestLogContainer GetTestContainer(CloudBlobContainer container) => container != null && !string.IsNullOrEmpty(container.Name) ? LogStoreHelper.GetTestContainer(this._requestContext, container.Name, container.Properties.LastModified.Value.DateTime) : (TestLogContainer) null;
  }
}
