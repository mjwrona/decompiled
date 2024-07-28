// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestLogStoreAFDService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestLogStoreAFDService : 
    TeamFoundationTestManagementService,
    ITestLogStoreAFDService,
    IVssFrameworkService
  {
    private static readonly string[] HostSuffixWhitelist = new string[4]
    {
      "vstmrblob.vsassets.io",
      "blob.core.windows.net",
      "vstmrblob.vsts.io",
      EdgeCacheUrlBuilder.BlobStorageEmulatorHostPort
    };
    private string _hostSuffix;

    public TestLogStoreAFDService(string hostSuffix) => this._hostSuffix = hostSuffix;

    public string GetAFDEndpoint(
      TestManagementRequestContext tcmRequestContext,
      string uri,
      int runid,
      int buildId,
      string filePath)
    {
      tcmRequestContext.RequestContext.TraceEnter("TraceLayer.RestLayer", nameof (GetAFDEndpoint));
      string afdEndpoint = uri;
      try
      {
        this.ValidateHostSuffix(this._hostSuffix);
        afdEndpoint = new EdgeCacheUrlBuilder(this._hostSuffix).Create(new Uri(uri)).AbsoluteUri;
      }
      catch (Exception ex)
      {
        tcmRequestContext.RequestContext.TraceError("RestLayer", "TestLogStoreAFDService.GetAFDEndpoint AFD Conversion error runId = {0}, buildId= {1}, filePath= {2}, Ex: {3}", (object) runid, (object) buildId, (object) filePath, (object) ex.Message);
      }
      finally
      {
        tcmRequestContext.RequestContext.TraceLeave("TraceLayer.RestLayer", nameof (GetAFDEndpoint));
      }
      return afdEndpoint;
    }

    private void ValidateHostSuffix(string hostSuffix)
    {
      for (int index = 0; index < TestLogStoreAFDService.HostSuffixWhitelist.Length; ++index)
      {
        if (hostSuffix.Equals(TestLogStoreAFDService.HostSuffixWhitelist[index], StringComparison.InvariantCultureIgnoreCase))
          return;
      }
      throw new ArgumentOutOfRangeException(nameof (hostSuffix), (object) hostSuffix, "Host suffix was not in the acceptable whitelist");
    }
  }
}
