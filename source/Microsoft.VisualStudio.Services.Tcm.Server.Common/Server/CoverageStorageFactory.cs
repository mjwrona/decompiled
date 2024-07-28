// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageStorageFactory
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public static class CoverageStorageFactory
  {
    public static ICoverageStorage GetCoverageInstance(TestManagementRequestContext requestContext)
    {
      if (requestContext.IsFeatureEnabled("TestManagement.Server.TestLogStoreOnTCMService"))
      {
        requestContext.Logger.Verbose(1015521, "The feature flag 'TestManagement.Server.TestLogStoreOnTCMService' is enabled.");
        return (ICoverageStorage) new TcmLogStoreCoverageStorage();
      }
      requestContext.Logger.Verbose(1015523, "Returning old attachment storage implementation.");
      return (ICoverageStorage) new TcmAttachmentCoverageStorage();
    }

    public static ICoverageStorage GetCoverageStorage(
      TestManagementRequestContext requestContext,
      string storageName)
    {
      if (string.IsNullOrEmpty(storageName))
      {
        requestContext.Logger.Error(1015524, "Storage Name should not be null.");
        return (ICoverageStorage) null;
      }
      if (string.Equals(typeof (TcmLogStoreCoverageStorage).Name, storageName, StringComparison.OrdinalIgnoreCase))
      {
        requestContext.Logger.Info(1015525, "Returning TcmLogStoreCoverageStorage.");
        return (ICoverageStorage) new TcmLogStoreCoverageStorage();
      }
      if (string.Equals(typeof (TcmAttachmentCoverageStorage).Name, storageName, StringComparison.OrdinalIgnoreCase))
      {
        requestContext.Logger.Info(1015526, "Returning TcmAttachmentCoverageStorage.");
        return (ICoverageStorage) new TcmAttachmentCoverageStorage();
      }
      requestContext.Logger.Error(1015526, "Returning null as storage name didnt match.");
      return (ICoverageStorage) null;
    }
  }
}
