// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITestLogClientService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [DefaultServiceImplementation(typeof (TestLogClientService))]
  [CLSCompliant(false)]
  public interface ITestLogClientService : IVssFrameworkService
  {
    Task<TestLogStatus> UploadTestLogAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      TestLogReference testLogReference,
      string logFileSourcePath,
      Dictionary<string, string> metaData,
      bool allowDuplicate);

    Task<TestLogStatus> UploadTestLogAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      TestLogReference testLogReference,
      Stream stream,
      Dictionary<string, string> metaData,
      bool allowDuplicate,
      bool overwrite = false);

    Task<TestLogStatus> DownloadTestLogAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      TestLogReference testLogReference,
      Stream stream);

    Task<PagedList<TestLog>> QueryTestLogAsync(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      TestLogQueryParameters logQueryParameters,
      TestLogReference logReference,
      int top = 1000);

    Task<LogStoreContainerCopyResponse> CopyTestLogStoreContainersAsync(
      TestManagementRequestContext tcmRequestContext,
      ITestLogStorageConnection sourceConnectionEndpoint,
      ITestLogStorageConnection targetConnectionEndpoint,
      List<string> containerNames,
      bool isServiceCopy,
      int maxDegreeParallelism = 8,
      int retryCount = 5);
  }
}
