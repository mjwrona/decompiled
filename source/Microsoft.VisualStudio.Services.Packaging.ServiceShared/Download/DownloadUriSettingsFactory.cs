// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download.DownloadUriSettingsFactory
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System.IO;
using System.Text;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download
{
  public class DownloadUriSettingsFactory : IFactory<IPackageFileRequest, DownloadUriSettings>
  {
    private readonly IFeatureFlagService featureFlagService;
    private readonly IRegistryService registryService;

    public DownloadUriSettingsFactory(
      IFeatureFlagService featureFlagService,
      IRegistryService registryService)
    {
      this.featureFlagService = featureFlagService;
      this.registryService = registryService;
    }

    public DownloadUriSettings Get(IPackageFileRequest input)
    {
      int num = this.registryService.GetValue<bool>((RegistryQuery) string.Format("/Configuration/Packaging/{0}/Downloads/ContentDispositionUsesFileName/Enabled", (object) input.Protocol.CorrectlyCasedName), true, false) ? 1 : 0;
      DownloadFilenameMode downloadFilenameMode = DownloadFilenameMode.Filename;
      string fileName = Path.GetFileName(input.FilePath);
      bool flag = fileName != null && Encoding.UTF8.GetByteCount(fileName) == fileName.Length;
      if (num == 0 || !flag || fileName != null && fileName.Length > 256)
        downloadFilenameMode = DownloadFilenameMode.None;
      return new DownloadUriSettings(downloadFilenameMode);
    }
  }
}
