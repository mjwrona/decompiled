// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.XEventBlobUtils
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Hosting;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal static class XEventBlobUtils
  {
    public static FileSystemProvider GetDevFabricBlobProvider(
      IVssRequestContext requestContext,
      string storagePath = null)
    {
      if (!requestContext.ExecutionEnvironment.IsDevFabricDeployment)
        throw new InvalidOperationException("The current execution environment is not DevFabric.");
      if (string.IsNullOrEmpty(storagePath))
      {
        storagePath = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, in XEventConstants.StoragePathQuery);
        if (string.IsNullOrEmpty(storagePath))
          throw new InvalidOperationException("A blob storage root must be provided or set.");
      }
      IDictionary<string, string> settings = (IDictionary<string, string>) new Dictionary<string, string>()
      {
        ["Root"] = storagePath
      };
      FileSystemProvider fabricBlobProvider = new FileSystemProvider();
      fabricBlobProvider.ServiceStart(requestContext, settings);
      return fabricBlobProvider;
    }

    public static AzurePageProvider GetAzureBlobProvider(
      IVssRequestContext requestContext,
      string diagnosticsConnectionString = null,
      TimeSpan? clientTimeout = null)
    {
      if (!requestContext.ExecutionEnvironment.IsCloudDeployment)
        throw new InvalidOperationException("The current execution environment is not Azure.");
      if (string.IsNullOrEmpty(diagnosticsConnectionString))
      {
        ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
        Guid drawerId = service.UnlockDrawer(requestContext, FrameworkServerConstants.ConfigurationSecretsDrawerName, true);
        diagnosticsConnectionString = service.GetString(requestContext, drawerId, "DiagnosticsConnectionString");
      }
      IDictionary<string, string> settings = (IDictionary<string, string>) new Dictionary<string, string>()
      {
        ["BlobStorageConnectionStringOverride"] = diagnosticsConnectionString
      };
      AzurePageProvider azureBlobProvider = new AzurePageProvider();
      azureBlobProvider.ServiceStart(requestContext, settings);
      azureBlobProvider.Client.DefaultRequestOptions.MaximumExecutionTime = clientTimeout;
      return azureBlobProvider;
    }
  }
}
