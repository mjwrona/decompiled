// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.FileContainerNameProviderFactory
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public static class FileContainerNameProviderFactory
  {
    public static IFileContainerNameProvider GetProvider(string coverageTool)
    {
      DefaultFileContainerProvider provider1 = new DefaultFileContainerProvider();
      if (provider1.IsToolSupported(coverageTool))
        return (IFileContainerNameProvider) provider1;
      VsTestDotCoverageFileContainerProvider provider2 = new VsTestDotCoverageFileContainerProvider();
      if (provider2.IsToolSupported(coverageTool))
        return (IFileContainerNameProvider) provider2;
      if ("Mock".Equals(coverageTool))
        return (IFileContainerNameProvider) provider1;
      throw new NotSupportedException(coverageTool + " not supported");
    }
  }
}
