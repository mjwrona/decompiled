// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.PlatformCommercePackageSynchronizationService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CloudConnected;
using Microsoft.VisualStudio.Services.Commerce.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class PlatformCommercePackageSynchronizationService : 
    ICommercePackageSynchronizationService,
    IVssFrameworkService
  {
    private IBillingMessageHelper billingMessageHelper;
    private const string Layer = "PlatformCommercePackageSynchronizationService";
    private const string Area = "Commerce";

    public DateTime? GetGracePeriodEndDate(IVssRequestContext requestContext)
    {
      TimeSpan timeSpan1 = new TimeSpan(1, 0, 0, 0, 0);
      TimeSpan timeSpan2 = new TimeSpan(7, 0, 0, 0, 0);
      string query = "/Service/Commerce/CommercePackageLastSync";
      IEnumerable<RegistryItem> source = requestContext.GetService<IVssRegistryService>().Read(requestContext, (RegistryQuery) query);
      if (!source.Any<RegistryItem>())
        return new DateTime?();
      DateTime dateTime = DateTime.Parse(source.First<RegistryItem>().Value);
      return dateTime.AddSeconds(timeSpan1.TotalSeconds * 2.0) < DateTime.UtcNow ? new DateTime?(dateTime.AddDays(7.0)) : new DateTime?();
    }

    public ICommercePackage GetCommercePackage(IVssRequestContext requestContext)
    {
      VssConnection connection = requestContext.GetService<ICloudConnectedConnectionService>().GetConnection(requestContext);
      ITeamFoundationDatabaseProperties databaseProperties = requestContext.GetService<IDataspaceService>().GetDatabaseProperties(requestContext, "Default", Guid.Empty);
      return (ICommercePackage) connection.GetClient<CommercePackageHttpClient>().GetCommercePackage(databaseProperties.ServiceLevel).SyncResult<CommercePackage>();
    }

    public void SynchronizeCommerceData(IVssRequestContext requestContext)
    {
      string path1 = "/Service/Commerce/CommercePackageLastSync";
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      try
      {
        ICommercePackage commercePackage = this.GetCommercePackage(requestContext);
        IVssRequestContext vssRequestContext1 = requestContext.To(TeamFoundationHostType.ProjectCollection);
        IVssRequestContext vssRequestContext2 = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
        try
        {
          if (!commercePackage.Configuration.IsNullOrEmpty<KeyValuePair<string, string>>())
          {
            List<RegistryItem> items = new List<RegistryItem>();
            foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) commercePackage.Configuration)
            {
              string path2;
              if (CommercePackageSettings.UpdateRegistryMap.TryGetValue(keyValuePair.Key, out path2))
              {
                RegistryItem registryItem = new RegistryItem(path2, keyValuePair.Value);
                items.Add(registryItem);
              }
            }
            service.Write(requestContext, (IEnumerable<RegistryItem>) items);
          }
        }
        catch (Exception ex)
        {
        }
        using (CommerceSqlComponent component = vssRequestContext2.CreateComponent<CommerceSqlComponent>())
          component.RemoveOfferMeterDefinitions();
        using (CommerceSqlComponent component = vssRequestContext2.CreateComponent<CommerceSqlComponent>())
        {
          foreach (OfferMeter meterConfig in (IEnumerable<OfferMeter>) commercePackage.OfferMeters.OrderBy<OfferMeter, int>((Func<OfferMeter, int>) (x => x.MeterId)))
            component.CreateOfferMeterDefinition((IOfferMeter) meterConfig);
        }
        vssRequestContext2.GetService<IOfferMeterCachedAccessService>().Invalidate(vssRequestContext2);
        using (CommerceMeteringComponent component = vssRequestContext1.CreateComponent<CommerceMeteringComponent>())
        {
          IEnumerable<SubscriptionResourceUsage> accountQuantities = commercePackage.OfferSubscriptions.Select<OfferSubscription, SubscriptionResourceUsage>((Func<OfferSubscription, SubscriptionResourceUsage>) (x => this.OfferSubscriptionToResourceUsage((IOfferSubscription) x)));
          component.SetSubscriptionResourceUsage(accountQuantities);
        }
        vssRequestContext1.GetService<IOfferSubscriptionCachedAccessService>().Invalidate(vssRequestContext1);
        RegistryItem registryItem1 = new RegistryItem(path1, DateTime.UtcNow.ToString("o"));
        service.Write(requestContext, (IEnumerable<RegistryItem>) new RegistryItem[1]
        {
          registryItem1
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5108877, "Commerce", nameof (PlatformCommercePackageSynchronizationService), ex);
        DateTime? gracePeriodEndDate = this.GetGracePeriodEndDate(requestContext);
        DateTime utcNow = DateTime.UtcNow;
        if ((gracePeriodEndDate.HasValue ? (gracePeriodEndDate.GetValueOrDefault() < utcNow ? 1 : 0) : 0) == 0)
          return;
        using (CommerceMeteringComponent component = requestContext.CreateComponent<CommerceMeteringComponent>())
        {
          IEnumerable<SubscriptionResourceUsage> accountQuantities = (IEnumerable<SubscriptionResourceUsage>) new List<SubscriptionResourceUsage>();
          component.SetSubscriptionResourceUsage(accountQuantities);
        }
      }
    }

    private SubscriptionResourceUsage OfferSubscriptionToResourceUsage(
      IOfferSubscription offerSubscription)
    {
      return new SubscriptionResourceUsage()
      {
        CommittedQuantity = offerSubscription.CommittedQuantity,
        CurrentQuantity = offerSubscription.CommittedQuantity,
        IncludedQuantity = offerSubscription.IncludedQuantity,
        IsPaidBillingEnabled = offerSubscription.IsPaidBillingEnabled,
        IsTrialOrPreview = offerSubscription.IsTrialOrPreview,
        LastResetDate = offerSubscription.ResetDate,
        LastUpdated = DateTime.UtcNow,
        LastUpdatedBy = Guid.Empty,
        MaxQuantity = offerSubscription.MaximumQuantity,
        PaidBillingUpdated = DateTime.UtcNow,
        ResourceId = offerSubscription.OfferMeter.MeterId,
        ResourceSeq = (byte) offerSubscription.RenewalGroup,
        StartDate = offerSubscription.StartDate ?? DateTime.MinValue,
        AutoAssignOnAccess = offerSubscription.AutoAssignOnAccess
      };
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection, nameof (systemRequestContext));
      if (systemRequestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException("PlatformCommercePackageSynchronizationService is not available in the hosted environment.");
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    internal virtual IBillingMessageHelper BillingMessageHelper
    {
      get
      {
        if (this.billingMessageHelper == null)
          this.billingMessageHelper = (IBillingMessageHelper) new Microsoft.VisualStudio.Services.Commerce.BillingMessageHelper();
        return this.billingMessageHelper;
      }
    }
  }
}
