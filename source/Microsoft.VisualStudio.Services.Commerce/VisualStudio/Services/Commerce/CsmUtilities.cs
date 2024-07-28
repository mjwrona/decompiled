// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CsmUtilities
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class CsmUtilities
  {
    private const string Area = "Commerce";
    private const string Layer = "CsmUtilities";

    internal static AccountResource CreateResourceGetResponse(
      IVssRequestContext tfsRequestContext,
      AzureResourceAccount azureResourceAccount,
      string resourceProviderNamespace,
      string resourceType)
    {
      Uri accountUrl = (Uri) null;
      IVssRequestContext deploymentContext = tfsRequestContext.To(TeamFoundationHostType.Deployment);
      IUrlHostResolutionService resolutionService = deploymentContext.GetService<IUrlHostResolutionService>();
      IVssRequestContext requestContext = tfsRequestContext.Elevate();
      if (azureResourceAccount.AccountId != Guid.Empty)
        CollectionHelper.WithCollectionContext(requestContext, azureResourceAccount.CollectionId, (Action<IVssRequestContext>) (collectionContext =>
        {
          try
          {
            accountUrl = resolutionService.GetHostUri(deploymentContext, collectionContext.ServiceHost.CollectionServiceHost.InstanceId, ServiceInstanceTypes.TFS);
          }
          catch (Exception ex) when (ex is InvalidOperationException || ex is ServiceOwnerNotFoundException)
          {
            collectionContext.TraceException(5106611, "Commerce", nameof (CsmUtilities), ex);
          }
        }), method: nameof (CreateResourceGetResponse));
      Dictionary<string, string> dictionary = new Dictionary<string, string>()
      {
        {
          "AccountURL",
          accountUrl != (Uri) null ? accountUrl.AbsoluteUri : string.Empty
        }
      };
      string id = string.Format("/subscriptions/{0}/resourceGroups/{1}/providers/{2}/account/{3}", (object) azureResourceAccount.AzureSubscriptionId, (object) azureResourceAccount.AzureCloudServiceName, (object) resourceProviderNamespace, (object) azureResourceAccount.AzureResourceName);
      string azureResourceName = azureResourceAccount.AzureResourceName;
      string str = resourceProviderNamespace + "/" + resourceType;
      string name = azureResourceName;
      string type = str;
      AccountResource resourceGetResponse = new AccountResource(id, name, type);
      resourceGetResponse.location = azureResourceAccount.AzureGeoRegion;
      resourceGetResponse.tags = CsmUtilities.GetAccountTags(tfsRequestContext, azureResourceAccount);
      resourceGetResponse.properties = dictionary;
      return resourceGetResponse;
    }

    public static Dictionary<string, string> GetAccountTags(
      IVssRequestContext tfsRequestContext,
      AzureResourceAccount azureResourceAccount)
    {
      Dictionary<string, string> accountTags = new Dictionary<string, string>();
      if (tfsRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        CsmUtilities.GetAccountTags(tfsRequestContext, accountTags);
      else if (azureResourceAccount.AccountId != Guid.Empty)
        CollectionHelper.WithCollectionContext(tfsRequestContext, azureResourceAccount.AccountId, (Action<IVssRequestContext>) (collectionContext => CsmUtilities.GetAccountTags(collectionContext, accountTags)), method: nameof (GetAccountTags));
      return accountTags;
    }

    private static void GetAccountTags(
      IVssRequestContext requestContext,
      Dictionary<string, string> accountTags)
    {
      CommercePropertyStore commercePropertyStore = new CommercePropertyStore();
      if (!commercePropertyStore.HasPropertyKind(requestContext, CommerceConstants.AccountTagPropertyKind))
        return;
      foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) commercePropertyStore.GetProperties(requestContext, CommerceConstants.AccountTagPropertyKind))
        accountTags.TryAdd<string, string>(property.Key, property.Value.ToString());
    }
  }
}
