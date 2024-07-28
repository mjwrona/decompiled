// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.HydrationHelper
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class HydrationHelper
  {
    private const string layerName = "ProjectHydrationPublishHelper";
    private const string areaName = "Commerce";

    internal virtual void StartHydration(
      IVssRequestContext requestContext,
      AzureResourceAccount azureResourceAccount)
    {
      try
      {
        requestContext.TraceEnter(5106540, "Commerce", "ProjectHydrationPublishHelper", nameof (StartHydration));
        HydrationHelper.CheckAzureResourceAccount(azureResourceAccount);
        requestContext.GetService<IResourceHydrationService>().CreateResourceHydrationRequest(requestContext, azureResourceAccount.AzureSubscriptionId.ToString(), azureResourceAccount.AzureCloudServiceName, "account", azureResourceAccount.AzureResourceName, azureResourceAccount.AzureGeoRegion);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106541, "Commerce", "ProjectHydrationPublishHelper", ex);
      }
      finally
      {
        requestContext.TraceLeave(5106542, "Commerce", "ProjectHydrationPublishHelper", nameof (StartHydration));
      }
    }

    internal virtual void StartDeHydration(
      IVssRequestContext requestContext,
      AzureResourceAccount azureResourceAccount)
    {
      try
      {
        requestContext.TraceEnter(5106543, "Commerce", "ProjectHydrationPublishHelper", nameof (StartDeHydration));
        HydrationHelper.CheckAzureResourceAccount(azureResourceAccount);
        requestContext.GetService<IResourceHydrationService>().DeleteResourceHydrationRequest(requestContext, azureResourceAccount.AzureSubscriptionId.ToString(), azureResourceAccount.AzureCloudServiceName, "account", azureResourceAccount.AzureResourceName, azureResourceAccount.AzureGeoRegion);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106544, "Commerce", "ProjectHydrationPublishHelper", ex);
      }
      finally
      {
        requestContext.TraceLeave(5106545, "Commerce", "ProjectHydrationPublishHelper", nameof (StartDeHydration));
      }
    }

    [ExcludeFromCodeCoverage]
    internal virtual HostInstanceMapping GetHostInstanceMapping(
      IVssRequestContext requestContext,
      Guid accountId)
    {
      ArgumentUtility.CheckForEmptyGuid(accountId, nameof (accountId));
      requestContext.Trace(5106202, TraceLevel.Info, "Commerce", "ProjectHydrationPublishHelper", string.Format("ServiceHost Request for Service Instance - AccountId {0}.", (object) accountId));
      HostInstanceMapping hostInstanceMapping = requestContext.GetService<IInstanceManagementService>().GetHostInstanceMapping(requestContext, accountId, ServiceInstanceTypes.TFS);
      int num = hostInstanceMapping != null ? 1 : 0;
      requestContext.Trace(5106203, TraceLevel.Info, "Commerce", "ProjectHydrationPublishHelper", string.Format("Host Instance Mapping found {0} for AccountId {1}.", (object) num, (object) accountId));
      return hostInstanceMapping;
    }

    private static void CheckAzureResourceAccount(AzureResourceAccount azureResourceAccount)
    {
      ArgumentUtility.CheckForNull<AzureResourceAccount>(azureResourceAccount, nameof (azureResourceAccount));
      ArgumentUtility.CheckForEmptyGuid(azureResourceAccount.AccountId, "AccountId");
      ArgumentUtility.CheckForEmptyGuid(azureResourceAccount.AzureSubscriptionId, "AzureSubscriptionId");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(azureResourceAccount.AzureCloudServiceName, "AzureCloudServiceName");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(azureResourceAccount.AzureResourceName, "AzureResourceName");
    }
  }
}
