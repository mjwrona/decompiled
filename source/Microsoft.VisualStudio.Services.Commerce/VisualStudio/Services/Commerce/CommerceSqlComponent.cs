// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceSqlComponent
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.WindowsAzure.CloudServiceManagement.ResourceProviderCommunication;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceSqlComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[24]
    {
      (IComponentCreator) new ComponentCreator<CommerceSqlComponent2>(1),
      (IComponentCreator) new ComponentCreator<CommerceSqlComponent3>(2),
      (IComponentCreator) new ComponentCreator<CommerceSqlComponent4>(3),
      (IComponentCreator) new ComponentCreator<CommerceSqlComponent5>(4),
      (IComponentCreator) new ComponentCreator<CommerceSqlComponent6>(5),
      (IComponentCreator) new ComponentCreator<CommerceSqlComponent7>(6),
      (IComponentCreator) new ComponentCreator<CommerceSqlComponent7>(7),
      (IComponentCreator) new ComponentCreator<CommerceSqlComponent8>(8),
      (IComponentCreator) new ComponentCreator<CommerceSqlComponent9>(9),
      (IComponentCreator) new ComponentCreator<CommerceSqlComponent10>(10),
      (IComponentCreator) new ComponentCreator<CommerceSqlComponent11>(11),
      (IComponentCreator) new ComponentCreator<CommerceSqlComponent12>(12),
      (IComponentCreator) new ComponentCreator<CommerceSqlComponent13>(13),
      (IComponentCreator) new ComponentCreator<CommerceSqlComponent14>(14),
      (IComponentCreator) new ComponentCreator<CommerceSqlComponent15>(15),
      (IComponentCreator) new ComponentCreator<CommerceSqlComponent16>(16),
      (IComponentCreator) new ComponentCreator<CommerceSqlComponent17>(17),
      (IComponentCreator) new ComponentCreator<CommerceSqlComponent18>(18),
      (IComponentCreator) new ComponentCreator<CommerceSqlComponent19>(19),
      (IComponentCreator) new ComponentCreator<CommerceSqlComponent20>(20),
      (IComponentCreator) new ComponentCreator<CommerceSqlComponent21>(21),
      (IComponentCreator) new ComponentCreator<CommerceSqlComponent22>(22),
      (IComponentCreator) new ComponentCreator<CommerceSqlComponent23>(23),
      (IComponentCreator) new ComponentCreator<CommerceSqlComponent24>(24)
    }, "VsCommerce_Config");
    private static Dictionary<int, SqlExceptionFactory> sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        2100002,
        new SqlExceptionFactory(typeof (CommerceInvalidQuantitySqlException))
      },
      {
        2100006,
        new SqlExceptionFactory(typeof (CommerceInvalidQuantitySqlException))
      },
      {
        2100005,
        new SqlExceptionFactory(typeof (CommerceInvalidQuantitySqlException))
      },
      {
        2100003,
        new SqlExceptionFactory(typeof (CommerceInvalidQuantitySqlException))
      },
      {
        2100004,
        new SqlExceptionFactory(typeof (CommerceInvalidQuantitySqlException))
      },
      {
        2100007,
        new SqlExceptionFactory(typeof (CommerceInvalidQuantitySqlException))
      },
      {
        2100008,
        new SqlExceptionFactory(typeof (CommerceInvalidResourceSqlException))
      },
      {
        2100001,
        new SqlExceptionFactory(typeof (CommerceInvalidSubscriptionSqlException))
      },
      {
        2100009,
        new SqlExceptionFactory(typeof (CommerceInvalidResourceSqlException))
      },
      {
        2100010,
        new SqlExceptionFactory(typeof (CommerceInvalidQuantitySqlException))
      },
      {
        2100012,
        new SqlExceptionFactory(typeof (CommerceResourceUpdateFailedSqlException))
      }
    };

    public CommerceSqlComponent() => this.ContainerErrorCode = 50000;

    protected override void SetupComponentCircuitBreaker(
      IVssRequestContext requestContext,
      string databaseName,
      ApplicationIntent applicationIntent)
    {
      string key = "CommerceSqlComponent-" + databaseName;
      this.ComponentLevelCommandSetter.AndCommandKey((Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) key);
      this.ComponentLevelCircuitBreakerProperties = (ICommandProperties) new CommandPropertiesRegistry(requestContext, (Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) key, this.ComponentLevelCommandSetter.CommandPropertiesDefaults);
    }

    [ExcludeFromCodeCoverage]
    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) CommerceSqlComponent.sqlExceptionFactories;

    [ExcludeFromCodeCoverage]
    internal virtual IVssRequestContext ComponentRequestContext => this.RequestContext;

    protected override string TraceArea => "Commerce";

    protected virtual string Layer => nameof (CommerceSqlComponent);

    public virtual AzureSubscriptionInternal GetAzureSubscription(
      Guid azureSubscriptionId,
      AccountProviderNamespace providerNamespaceId)
    {
      throw new NotSupportedException();
    }

    public virtual IEnumerable<AzureSubscriptionInternal> GetAzureSubscriptionsForUpdates(
      Guid azureSubscription,
      int batchSize)
    {
      throw new NotSupportedException();
    }

    public virtual void AddAzureSubscription(
      Guid azureSubscriptionId,
      AccountProviderNamespace providerNamespaceId,
      int azureSubscriptionAnniversaryDay,
      SubscriptionStatus subscriptionStatusId,
      SubscriptionSource subscriptionSource)
    {
      throw new NotSupportedException();
    }

    public virtual void AddAzureSubscription(
      Guid azureSubscriptionId,
      AccountProviderNamespace providerNamespaceId,
      int azureSubscriptionAnniversaryDay,
      SubscriptionStatus subscriptionStatusId,
      SubscriptionSource subscriptionSource,
      string azureOfferCode)
    {
      this.AddAzureSubscription(azureSubscriptionId, providerNamespaceId, azureSubscriptionAnniversaryDay, subscriptionStatusId, subscriptionSource);
    }

    public virtual int GetAzureResourceAccountsCountBySubscriptionId(
      Guid azureSubscriptionId,
      AccountProviderNamespace providerNamespaceId)
    {
      throw new NotSupportedException();
    }

    public virtual IEnumerable<AzureResourceAccount> GetAzureResourceAccountsBySubscriptionId(
      Guid azureSubscriptionId,
      AccountProviderNamespace providerNamespaceId,
      bool useCollectionId)
    {
      throw new NotSupportedException();
    }

    public virtual IEnumerable<AzureResourceAccount> GetAzureResourceAccountsBySubscriptionId(
      Guid azureSubscriptionId,
      bool useCollectionId)
    {
      throw new NotSupportedException();
    }

    public virtual AzureResourceAccount GetAzureResourceAccount(
      Guid azureSubscriptionId,
      AccountProviderNamespace providerNamespaceId,
      string azureResourceName,
      bool useCollectionId)
    {
      throw new NotSupportedException();
    }

    public virtual AzureResourceAccount GetAzureResourceAccountByAccountId(
      Guid accountId,
      bool useCollectionId)
    {
      throw new NotSupportedException();
    }

    public virtual void AddAzureResourceAccount(
      Guid accountId,
      Guid collectionId,
      Guid azureSubscriptionId,
      AccountProviderNamespace providerNamespaceId,
      string azureCloudServiceName,
      string alternateCloudServiceName,
      string azureResourceName,
      string eTag,
      string azureGeoRegion,
      OperationResult operationResult)
    {
      throw new NotSupportedException();
    }

    public virtual void RemoveAzureResourceAccount(
      Guid azureSubscriptionId,
      AccountProviderNamespace providerNamespaceId,
      string azureResourceName,
      string azureCloudServiceName)
    {
      throw new NotSupportedException();
    }

    public virtual int CleanupAzureResourceAccounts() => throw new NotSupportedException();

    public virtual IEnumerable<AzureResourceAccount> RemoveCloudService(
      Guid azureSubscriptionId,
      AccountProviderNamespace providerNamespaceId,
      string azureCloudServiceName,
      bool useCollectionId)
    {
      throw new NotSupportedException();
    }

    public virtual List<AzureSubscriptionAccount> GetSubscriptionsForAccounts(
      IEnumerable<Guid> accountIds,
      AccountProviderNamespace providerNamespaceId,
      bool useCollectionId)
    {
      throw new NotSupportedException();
    }

    public virtual IList<AzureResourceAccount> GetAzureSubscriptionAccounts(
      AccountProviderNamespace providerNamespaceId,
      Guid? startingAccountId,
      int maxRecordCountToReturn,
      bool useCollectionId)
    {
      throw new NotSupportedException();
    }

    public virtual IList<AzureResourceAccount> GetAzureResourceAccountsInResourceGroup(
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      string resourceGroupName,
      bool useCollectionId)
    {
      return (IList<AzureResourceAccount>) new List<AzureResourceAccount>();
    }

    public virtual IList<OfferMeter> GetOfferMeterConfiguration(int? meterId) => (IList<OfferMeter>) new List<OfferMeter>();

    public virtual OfferMeter GetOfferMeterConfigurationByName(string meterName) => throw new NotSupportedException();

    public virtual void CreateOfferMeterDefinition(IOfferMeter meterConfig) => throw new NotSupportedException();

    public virtual void CreateOfferMeterDefinitionPlan(IOfferMeter meterConfig, int meterId) => throw new NotSupportedException();

    public virtual void UpdateOfferMeterDefinitionName(IOfferMeter meterConfig) => throw new NotSupportedException();

    public virtual IList<OfferMeterPrice> GetOfferMeterPrice(string meterName) => throw new NotSupportedException();

    public virtual void AddOfferMeterPrice(IEnumerable<OfferMeterPrice> priceList, string meterName) => throw new NotSupportedException();

    public virtual void AddOfferMeterPrice(
      IEnumerable<OfferMeterPrice> priceList,
      string meterName,
      bool isFistParty)
    {
      throw new NotSupportedException();
    }

    public virtual void RemoveOfferMeterDefinitions() => throw new NotSupportedException();

    public virtual void UpdateAzureResourceAccount(
      Guid accountId,
      Guid collectionId,
      Guid azureSubscriptionId,
      AccountProviderNamespace providerNamespaceId,
      string azureCloudServiceName,
      string azureResourceName)
    {
      throw new NotImplementedException();
    }

    public virtual AzureSubscriptionInternal GetAzureSubscription(Guid azureSubscriptionId) => throw new NotImplementedException();

    public virtual void AddAzureSubscription(
      Guid azureSubscriptionId,
      SubscriptionStatus subscriptionStatusId,
      SubscriptionSource subscriptionSource,
      string azureOfferCode)
    {
      throw new NotImplementedException();
    }

    public virtual void AddAzureSubscription(
      Guid azureSubscriptionId,
      SubscriptionStatus subscriptionStatusId,
      SubscriptionSource subscriptionSource)
    {
      throw new NotImplementedException();
    }

    public virtual void MigrateAzureResourceAccounts(
      IEnumerable<AzureResourceAccount> azureResourceAccounts,
      bool isTarget)
    {
      throw new NotImplementedException();
    }

    public virtual OfferSubscriptionQuantity GetOfferSubscriptionQuantity(
      Guid azureSubscriptionId,
      int meterId)
    {
      throw new NotImplementedException();
    }
  }
}
