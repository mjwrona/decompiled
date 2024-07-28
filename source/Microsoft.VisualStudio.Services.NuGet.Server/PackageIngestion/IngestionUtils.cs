// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion.IngestionUtils
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion
{
  public class IngestionUtils
  {
    public const long MaxPushSizeDefault = 524288000;
    public const int MaxNuspecSizeDefault = 384000;
    private static readonly RegistryQuery MaxPushSizeRegistryPath = (RegistryQuery) "/Configuration/Packaging/NuGet/Ingestion/MaxSize";
    private static readonly RegistryQuery MaxNuspecSizeRegistryPath = (RegistryQuery) "/Configuration/Packaging/NuGet/Ingestion/MaxNuspecSize";

    public static long GetMaxPackageSize(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<long>(requestContext, in IngestionUtils.MaxPushSizeRegistryPath, true, 524288000L);

    public static int GetMaxNuspecSize(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, in IngestionUtils.MaxNuspecSizeRegistryPath, true, 384000);
  }
}
