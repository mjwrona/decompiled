// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.RawPackageInnerFileRequest
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts
{
  public class RawPackageInnerFileRequest : 
    RawPackageFileRequest,
    IRawPackageInnerFileRequest,
    IRawPackageFileRequest,
    IRawPackageRequest,
    IRawPackageNameRequest,
    IFeedRequest,
    IProtocolAgnosticFeedRequest
  {
    public RawPackageInnerFileRequest(
      IFeedRequest feedRequest,
      string packageName,
      string packageVersion,
      string filePath,
      string innerFilePath)
      : base(feedRequest, packageName, packageVersion, filePath)
    {
      this.InnerFilePath = innerFilePath;
    }

    public string InnerFilePath { get; }
  }
}
