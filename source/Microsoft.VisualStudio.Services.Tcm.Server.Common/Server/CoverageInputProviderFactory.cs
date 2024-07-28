// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageInputProviderFactory
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class CoverageInputProviderFactory : ICoverageInputProviderFactory
  {
    public virtual ICoverageInputProvider GetCoverageInputProvider(
      TestManagementRequestContext requestContext,
      string storageName,
      CoverageToolInput coverageInput)
    {
      if (string.IsNullOrWhiteSpace(storageName))
      {
        string str = "StorageName should not be null or empty.";
        requestContext.Logger.Error(1015527, str);
        throw new ArgumentException(str);
      }
      if (string.Equals(typeof (TcmLogStoreCoverageStorage).Name, storageName, StringComparison.OrdinalIgnoreCase))
      {
        requestContext.Logger.Info(1015528, "Returning LogStoreCoverageInputProvider.");
        return (ICoverageInputProvider) new LogStoreCoverageInputProvider(coverageInput);
      }
      if (string.Equals(typeof (TcmAttachmentCoverageStorage).Name, storageName, StringComparison.OrdinalIgnoreCase))
      {
        requestContext.Logger.Info(1015529, "Returning AttachmentStoreCoverageInputProvider.");
        return (ICoverageInputProvider) new AttachmentStoreCoverageInputProvider(coverageInput);
      }
      string str1 = "StorageName (" + storageName + ") did not match any supported coverage storage type.";
      requestContext.Logger.Error(1015530, str1);
      throw new NotSupportedException(str1);
    }
  }
}
