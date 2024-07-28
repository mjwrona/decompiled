// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureBlobProviderService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class AzureBlobProviderService : IAzureBlobProviderService, IVssFrameworkService
  {
    public const string DrawerNameSetting = "DrawerName";
    public const string LookupKeySetting = "LookupKey";

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckDeploymentRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public T CreateAndInitializeBlobProvider<T>(
      IVssRequestContext requestContext,
      int storageAccountId,
      bool throwOnError = true)
      where T : class, IBlobProvider
    {
      return this.CreateAndInitializeBlobProvider<T>(requestContext, "ConfigurationSecrets", TeamFoundationFileService.GetStorageAccountLookupKey(storageAccountId), throwOnError);
    }

    public T CreateAndInitializeBlobProvider<T>(
      IVssRequestContext requestContext,
      string drawerName,
      string lookupKey,
      bool throwOnError = true)
      where T : class, IBlobProvider
    {
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo itemInfo = service.GetItemInfo(requestContext, drawerName, lookupKey, throwOnError);
      if (itemInfo == null)
        return default (T);
      string str = service.GetString(requestContext, itemInfo);
      Dictionary<string, string> settings = new Dictionary<string, string>()
      {
        ["BlobStorageConnectionStringOverride"] = str,
        ["DrawerName"] = drawerName,
        ["LookupKey"] = lookupKey
      };
      return this.CreateAndInitializeBlobProvider<T>(requestContext, (IDictionary<string, string>) settings, throwOnError);
    }

    protected T CreateAndInitializeBlobProvider<T>(
      IVssRequestContext requestContext,
      IDictionary<string, string> settings,
      bool throwOnError = true)
      where T : class, IBlobProvider
    {
      T blobProvider;
      if (TeamFoundationFileService.TryCreateBlobProvider<T>(requestContext, out blobProvider))
      {
        blobProvider.ServiceStart(requestContext, settings);
        return blobProvider;
      }
      if (throwOnError)
        throw new BlobProviderNotFoundException("No remote blob provider was configured or the class was not found");
      return default (T);
    }
  }
}
