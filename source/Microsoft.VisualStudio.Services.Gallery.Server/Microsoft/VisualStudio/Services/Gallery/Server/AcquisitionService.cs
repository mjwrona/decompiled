// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.AcquisitionService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.Server.Remote;
using Microsoft.VisualStudio.Services.Gallery.Server.Remote.Billing;
using Microsoft.VisualStudio.Services.Gallery.Server.Remote.ExtensionManagement;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class AcquisitionService : IVssFrameworkService
  {
    private readonly IRemoteServiceClientFactory _clientFactory;
    private readonly string _serviceName = nameof (AcquisitionService);
    private static readonly char[] s_itemNameCharPartSeparator = new char[1]
    {
      '.'
    };
    [StaticSafe]
    private static Dictionary<AcquisitionService.ExtensionInstallationTargetType, List<string>> s_installationTargetMap = new Dictionary<AcquisitionService.ExtensionInstallationTargetType, List<string>>()
    {
      {
        AcquisitionService.ExtensionInstallationTargetType.VSS,
        new List<string>()
        {
          "Microsoft.VisualStudio.Services",
          "Microsoft.TeamFoundation.Server",
          "Microsoft.VisualStudio.Services.Cloud",
          "Microsoft.VisualStudio.Services.Resource.Cloud"
        }
      },
      {
        AcquisitionService.ExtensionInstallationTargetType.Offer,
        new List<string>() { "Microsoft.VisualStudio.Offer" }
      },
      {
        AcquisitionService.ExtensionInstallationTargetType.Integration,
        new List<string>()
        {
          "Microsoft.VisualStudio.Services.Integration",
          "Microsoft.TeamFoundation.Server.Integration",
          "Microsoft.VisualStudio.Services.Cloud.Integration"
        }
      }
    };

    public void ServiceStart(IVssRequestContext systemRequestContext) => GalleryKPIHelpers.EnsureKPIsAreRegistered(systemRequestContext);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public AcquisitionService() => this._clientFactory = (IRemoteServiceClientFactory) new RemoteServiceClientFactory();

    internal AcquisitionService(IRemoteServiceClientFactory clientFactory) => this._clientFactory = clientFactory;

    public ExtensionAcquisitionRequest RequestAcquisition(
      IVssRequestContext requestContext,
      ExtensionAcquisitionRequest acquisitionRequest)
    {
      string methodName = nameof (RequestAcquisition);
      requestContext.TraceEnter(12062000, "gallery", this._serviceName, methodName);
      ArgumentUtility.CheckForNull<ExtensionAcquisitionRequest>(acquisitionRequest, "AcquisitionRequest");
      if (acquisitionRequest != null)
        requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "RequestAcquisition: ItemId = {0}, operationType = {1}", (object) acquisitionRequest.ItemId, (object) Enum.GetName(typeof (Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationType), (object) acquisitionRequest.OperationType));
      else
        requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "RequestAcquisition: acquisitionRequest is null");
      if (acquisitionRequest.Targets != null && acquisitionRequest.Targets.Count > 0)
        requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "RequestAcquisition: accountId = {0}", (object) acquisitionRequest.Targets[0]);
      switch (acquisitionRequest.OperationType)
      {
        case Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationType.Get:
        case Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationType.None:
          throw new NotImplementedException();
        case Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationType.Install:
          requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "RequestAcquisition: Entered install option. Retrieving published extension");
          PublishedExtension extension1 = this.GetExtension(requestContext, acquisitionRequest);
          if (extension1 != null)
            requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "RequestAcquisition: install option. Retrieved published extension. Extension id : {0}", (object) extension1.GetFullyQualifiedName());
          try
          {
            this.InstallExtension(requestContext, extension1, acquisitionRequest);
          }
          catch (ExtensionAlreadyInstalledException ex)
          {
            requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "RequestAcquisition: Entered install workflow. Extension already installed for Extension: {0}, accountId: {1}", (object) extension1.GetFullyQualifiedName(), (object) acquisitionRequest.Targets[0]);
          }
          if (requestContext.ExecutionEnvironment.IsHostedDeployment && extension1.IsThirdPartyAndPaid())
          {
            requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "RequestAcquisition: install option. Retriving OfferSubscription.");
            IOfferSubscription offerSubscription = this.GetOfferSubscription(requestContext, extension1, this.GetTargetAccountId(acquisitionRequest));
            if (offerSubscription == null || offerSubscription.OfferMeter == (OfferMeter) null || offerSubscription.OfferMeter.FixedQuantityPlans == null || offerSubscription.OfferMeter.FixedQuantityPlans.Count<AzureOfferPlanDefinition>() == 0 || DateTime.Compare(DateTime.UtcNow, offerSubscription.OfferMeter.BillingStartDate ?? DateTime.UtcNow) <= 0)
            {
              requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "RequestAcquisition: install option. Trying to acquire trial or preview for extension : {0}", (object) extension1.GetFullyQualifiedName());
              this.AcquireTrialOrPreviewForExtension(requestContext, extension1, acquisitionRequest);
              requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "RequestAcquisition: install option. Acquired trial or preview for extension : {0}", (object) extension1.GetFullyQualifiedName());
              break;
            }
            break;
          }
          break;
        case Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationType.Buy:
          requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "RequestAcquisition: Entered Buy option. Retrieving published extension");
          PublishedExtension extension2 = this.GetExtension(requestContext, acquisitionRequest);
          if (extension2 != null)
            requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "RequestAcquisition: Buy option. Retrieved published extension. Extension id : {0}", (object) extension2.GetFullyQualifiedName());
          if (extension2.IsPaid())
          {
            if (!Guid.TryParse(acquisitionRequest.BillingId, out Guid _))
              throw new ArgumentException("BillingId");
            if (!this.ExtensionHasInstallationTargetType(extension2, AcquisitionService.ExtensionInstallationTargetType.VSS) && !this.ExtensionHasInstallationTargetType(extension2, AcquisitionService.ExtensionInstallationTargetType.Offer))
              throw new NotImplementedException();
          }
          try
          {
            bool flag = false;
            if (acquisitionRequest.Properties != null)
              acquisitionRequest.Properties.TryGetValue<bool>("skipInstall", out flag);
            if (!flag)
            {
              this.InstallExtension(requestContext, extension2, acquisitionRequest);
              break;
            }
            break;
          }
          catch (ExtensionAlreadyInstalledException ex)
          {
            requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "RequestAcquisition: Entered buy workflow. Extension already installed for extension: {0} account: {1}", (object) extension2.GetFullyQualifiedName(), (object) acquisitionRequest.Targets[0]);
            break;
          }
        case Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationType.Try:
          requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "RequestAcquisition: Entering Trial workflow for item: {0}", (object) acquisitionRequest.ItemId);
          PublishedExtension extension3 = this.GetExtension(requestContext, acquisitionRequest);
          if (!requestContext.ExecutionEnvironment.IsHostedDeployment || !extension3.IsPaid())
            throw new NotImplementedException();
          if (extension3 != null)
            requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "RequestAcquisition: Trial option. Retrieved published extension. Extension id : {0}", (object) extension3.GetFullyQualifiedName());
          try
          {
            this.InstallExtension(requestContext, extension3, acquisitionRequest);
          }
          catch (ExtensionAlreadyInstalledException ex)
          {
            requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "RequestAcquisition: Entered trial workflow. Extension already installed for item: {0} account: {1}", (object) extension3.GetFullyQualifiedName(), (object) acquisitionRequest.Targets[0]);
          }
          requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "RequestAcquisition: Trial workflow. Acquiring trial for item: {0}", (object) extension3.GetFullyQualifiedName());
          this.AcquireTrialOrPreviewForExtension(requestContext, extension3, acquisitionRequest);
          requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "RequestAcquisition: Entered trial workflow. Acquired trial for item: {0}", (object) extension3.GetFullyQualifiedName());
          break;
        case Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationType.Request:
          requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "RequestAcquisition: Request workflow for item: {0}", (object) acquisitionRequest.ItemId);
          this.RequestExtension(requestContext, acquisitionRequest);
          break;
      }
      requestContext.TraceLeave(12062000, "gallery", this._serviceName, methodName);
      return acquisitionRequest;
    }

    public Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionOption.AcquisitionOptions GetAcquisitionOptions(
      IVssRequestContext requestContext,
      string galleryItemName,
      string target,
      bool testCommerce = false,
      bool isFreeOrTrialInstall = false)
    {
      string methodName = nameof (GetAcquisitionOptions);
      requestContext.TraceEnter(12062000, "gallery", this._serviceName, methodName);
      AcquisitionService.CheckGalleryItemName(galleryItemName);
      Guid targetAccountId;
      AcquisitionService.CheckAcquisitionTarget(target, out targetAccountId);
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        this.CheckPermissionForCollectionAccess(requestContext, targetAccountId);
      requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "GetAcquisitionOptions: galleryItemName = {0} targetAccount = {1}", (object) galleryItemName, (object) targetAccountId);
      PublishedExtension extension = this.GetExtension(requestContext, galleryItemName, targetAccountId);
      if (extension == null)
      {
        requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "GetAcquisitionOptions: Published extension is null for {0}", (object) galleryItemName);
        throw new ExtensionDoesNotExistException(GalleryWebApiResources.ExtensionDoesNotExist((object) galleryItemName));
      }
      if (!this.IsVsoExtensionItem(extension))
      {
        requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "GetAcquisitionOptions: Called for non VSTS extension with ItemId: {0}", (object) galleryItemName);
        throw new NotSupportedException();
      }
      Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionOption.AcquisitionOptions acquisitionOptions = this.GetVsoExtensionItemAcquisitionOptions(requestContext, extension, targetAccountId, testCommerce, isFreeOrTrialInstall);
      acquisitionOptions.ItemId = galleryItemName;
      acquisitionOptions.Target = target;
      requestContext.TraceLeave(12062000, "gallery", this._serviceName, methodName);
      return acquisitionOptions;
    }

    private void CheckPermissionForCollectionAccess(
      IVssRequestContext requestContext,
      Guid collectionId)
    {
      using (IVssRequestContext requestContext1 = requestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(requestContext, collectionId, RequestContextType.UserContext))
      {
        IVssSecurityNamespace securityNamespace = requestContext1.Elevate(false).GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext1, FrameworkSecurity.FrameworkNamespaceId);
        try
        {
          securityNamespace.CheckPermission(requestContext1, FrameworkSecurity.FrameworkNamespaceToken, 1);
        }
        catch (AccessCheckException ex)
        {
          throw new HttpException(401, GalleryWebApiResources.NoAuthorizationToResource((object) requestContext.DomainUserName));
        }
      }
    }

    private void RequestExtension(
      IVssRequestContext requestContext,
      ExtensionAcquisitionRequest acquisitionRequest)
    {
      string methodName = nameof (RequestExtension);
      string publisherName;
      string extensionName;
      if (!AcquisitionService.TryParseGalleryItemName(acquisitionRequest.ItemId, out publisherName, out extensionName))
        throw new ArgumentException(GalleryResources.InvalidFullyQualifiedName());
      requestContext.TraceEnter(12062000, "gallery", this._serviceName, methodName);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) acquisitionRequest.Targets, "Targets");
      requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "RequestExtension: Extension Name = {0}, accountId = {1}", (object) extensionName, (object) acquisitionRequest.Targets[0]);
      Guid targetAccountId;
      AcquisitionService.CheckAcquisitionTarget(acquisitionRequest.Targets[0], out targetAccountId);
      string requestMessage = (string) null;
      if (acquisitionRequest.Properties != null)
        acquisitionRequest.Properties.TryGetValue<string>("requestMessage", out requestMessage);
      if (requestMessage != null && requestMessage.Length > 900)
        throw new CharacterLimitExceededException(GalleryWebApiResources.CharacterLimitExceeded((object) "requestMessage", (object) 900.ToString()));
      using (IExtensionManagementClient managementClient = this._clientFactory.GetNewExtensionManagementClient(requestContext, targetAccountId, ExtensionConstants.ServiceOwner))
        managementClient.RequestExtensionSync(requestContext, publisherName, extensionName, requestMessage, (object) requestContext);
    }

    private void PurchaseExtension(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension,
      ExtensionAcquisitionRequest acquisitionRequest)
    {
      requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "Entered purchaseExtension for extension: {0} billingId = {1} quantity = {2}", (object) publishedExtension.GetFullyQualifiedName(), (object) acquisitionRequest.BillingId, (object) acquisitionRequest.Quantity);
      acquisitionRequest.Quantity = Math.Max(0, acquisitionRequest.Quantity);
      ArgumentUtility.CheckStringForNullOrEmpty(acquisitionRequest.BillingId, "BillingId");
      Guid result;
      if (!Guid.TryParse(acquisitionRequest.BillingId, out result))
        throw new ArgumentException("BillingId");
      IBillingClient billingClient;
      if (this.ExtensionHasInstallationTargetType(publishedExtension, AcquisitionService.ExtensionInstallationTargetType.VSS) || this.ExtensionHasInstallationTargetType(publishedExtension, AcquisitionService.ExtensionInstallationTargetType.Integration))
      {
        requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "PurchaseExtension Getting billing client for account :{0}", (object) acquisitionRequest.Targets[0]);
        Guid targetAccountId;
        AcquisitionService.CheckAcquisitionTarget(acquisitionRequest.Targets[0], out targetAccountId);
        billingClient = this._clientFactory.GetNewNonDeploymentLevelBillingClient(requestContext, targetAccountId);
      }
      else
      {
        if (!this.ExtensionHasInstallationTargetType(publishedExtension, AcquisitionService.ExtensionInstallationTargetType.Offer))
          throw new NotImplementedException();
        requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "PurchaseExtension Getting billing client for offer");
        billingClient = this._clientFactory.GetNewBillingClient(requestContext);
      }
      requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "PurchaseExtension Calling billing client CreateOfferSubscriptionSync api");
      if (acquisitionRequest.Quantity > 0)
      {
        requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, string.Format("Purchase {0} count of {1}", (object) acquisitionRequest.Quantity, (object) publishedExtension.GetFullyQualifiedName()));
        billingClient.CreateOfferSubscriptionSync(requestContext, acquisitionRequest.ItemId, result, ResourceRenewalGroup.Monthly, acquisitionRequest.Quantity, new Guid?());
      }
      else
      {
        requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "Cancel purchase for extension " + publishedExtension.GetFullyQualifiedName());
        billingClient.CancelOfferSubscription(requestContext, acquisitionRequest.ItemId, result, ResourceRenewalGroup.Monthly, new Guid?());
      }
      requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "PurchaseExtension After CreateOfferSubscriptionSync api");
    }

    private void AcquireTrialOrPreviewForExtension(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension,
      ExtensionAcquisitionRequest acquisitionRequest)
    {
      string methodName = nameof (AcquireTrialOrPreviewForExtension);
      requestContext.TraceEnter(12062000, "gallery", this._serviceName, methodName);
      Guid targetAccountId;
      AcquisitionService.CheckAcquisitionTarget(acquisitionRequest.Targets[0], out targetAccountId);
      IBillingClient levelBillingClient = this._clientFactory.GetNewNonDeploymentLevelBillingClient(requestContext, targetAccountId);
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publishedExtension.Publisher.PublisherName, publishedExtension.ExtensionName);
      try
      {
        levelBillingClient.EnableTrialOrPreview(requestContext, fullyQualifiedName);
      }
      catch (VssServiceResponseException ex)
      {
        if (ex.InnerException == null || !(ex.InnerException is InvalidOperationException))
          throw;
        else
          requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "Extension already in trial or preview. ExtensionName = {0}, accountId = {1}", (object) publishedExtension.ExtensionName, (object) targetAccountId);
      }
      IOfferSubscription offerSubscription = levelBillingClient.GetOfferSubscription(requestContext, fullyQualifiedName);
      if (offerSubscription.TrialExpiryDate.HasValue)
        acquisitionRequest.Properties.Add("TrialExpiryDate", (JToken) offerSubscription.TrialExpiryDate);
      requestContext.TraceLeave(12062000, "gallery", this._serviceName, methodName);
    }

    private bool ExtensionHasInstallationTargetType(
      PublishedExtension publishedExtension,
      AcquisitionService.ExtensionInstallationTargetType installationTargetType)
    {
      if (publishedExtension.InstallationTargets == null)
        return true;
      List<string> installationTarget1 = AcquisitionService.s_installationTargetMap[installationTargetType];
      foreach (InstallationTarget installationTarget2 in publishedExtension.InstallationTargets)
      {
        InstallationTarget installationTarget = installationTarget2;
        if (installationTarget1.Any<string>((Func<string, bool>) (x => x.Equals(installationTarget.Target, StringComparison.OrdinalIgnoreCase))))
          return true;
      }
      return false;
    }

    private void InstallExtension(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension,
      ExtensionAcquisitionRequest acquisitionRequest)
    {
      string methodName = nameof (InstallExtension);
      requestContext.TraceEnter(12062000, "gallery", this._serviceName, methodName);
      requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "InstallExtension.Extension id:  {0}", (object) publishedExtension.GetFullyQualifiedName());
      if (!this.ExtensionHasInstallationTargetType(publishedExtension, AcquisitionService.ExtensionInstallationTargetType.VSS))
        return;
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) acquisitionRequest.Targets, "Targets");
      requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "InstallExtension: targetAccountId:  {0}", (object) acquisitionRequest.Targets[0]);
      Guid targetAccountId;
      AcquisitionService.CheckAcquisitionTarget(acquisitionRequest.Targets[0], out targetAccountId);
      requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "InstallExtension: Retrieving ems client");
      using (IExtensionManagementClient managementClient = this._clientFactory.GetNewExtensionManagementClient(requestContext, targetAccountId, ExtensionConstants.ServiceOwner))
      {
        requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "InstallExtension: Calling InstallExtensionByNameSync");
        managementClient.InstallExtensionByNameSync(requestContext, publishedExtension.Publisher.PublisherName, publishedExtension.ExtensionName, (object) requestContext);
      }
      requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "InstallExtension: After InstallExtensionByNameSync call");
      List<ExtensionStatisticUpdate> statistics = new List<ExtensionStatisticUpdate>();
      statistics.Add(new ExtensionStatisticUpdate()
      {
        Statistic = new ExtensionStatistic()
        {
          StatisticName = "install",
          Value = 1.0
        },
        Operation = ExtensionStatisticOperation.Increment,
        PublisherName = publishedExtension.Publisher.PublisherName,
        ExtensionName = publishedExtension.ExtensionName
      });
      requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "InstallExtension: Updating statistics");
      requestContext.GetService<IExtensionStatisticService>().UpdateStatistics(requestContext, (IEnumerable<ExtensionStatisticUpdate>) statistics);
      requestContext.TraceLeave(12062000, "gallery", this._serviceName, methodName);
    }

    private bool IsVsoExtensionItem(PublishedExtension publishedExtension) => publishedExtension.InstallationTargets == null || GalleryUtil.InstallationTargetsHasVSTS((IEnumerable<InstallationTarget>) publishedExtension.InstallationTargets);

    private Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionOption.AcquisitionOptions GetVsoExtensionItemAcquisitionOptions(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension,
      Guid targetAccountId,
      bool testCommerce,
      bool isFreeOrTrialInstall)
    {
      string methodName = nameof (GetVsoExtensionItemAcquisitionOptions);
      requestContext.TraceEnter(12062000, "gallery", this._serviceName, methodName);
      List<Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperation> acquisitionOperationList = new List<Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperation>();
      Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperation acquisitionOperation1 = (Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperation) null;
      using (IExtensionManagementClient managementClient = this._clientFactory.GetNewExtensionManagementClient(requestContext, targetAccountId, ExtensionConstants.ServiceOwner))
      {
        Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperation acquisitionOperation2 = new Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperation()
        {
          OperationType = Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationType.Install,
          OperationState = Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationState.Allow
        };
        Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperation acquisitionOperation3 = new Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperation()
        {
          OperationType = Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationType.Request,
          OperationState = Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationState.Allow
        };
        Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperation acquisitionOperation4 = new Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperation()
        {
          OperationType = Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationType.Try,
          OperationState = Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationState.Allow
        };
        if (((!requestContext.ExecutionEnvironment.IsHostedDeployment ? 0 : (publishedExtension.IsPaid() ? 1 : 0)) & (isFreeOrTrialInstall ? 1 : 0)) != 0)
        {
          requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "GetVsoExtensionItemAcquisitionOptions. Acquiring offerSubscription details for ItemId: {0}, accountId: {1}", (object) publishedExtension.GetFullyQualifiedName(), (object) targetAccountId);
          IOfferSubscription offerSubscription = this.GetOfferSubscription(requestContext, publishedExtension, targetAccountId);
          if (offerSubscription != null)
          {
            IEnumerable<AzureOfferPlanDefinition> fixedQuantityPlans = offerSubscription.OfferMeter.FixedQuantityPlans;
            AzureOfferPlanDefinition[] array = fixedQuantityPlans != null ? fixedQuantityPlans.ToArray<AzureOfferPlanDefinition>() : (AzureOfferPlanDefinition[]) null;
            if (publishedExtension.IsFirstPartyAndPaid() || array != null && (((IEnumerable<AzureOfferPlanDefinition>) array).Any<AzureOfferPlanDefinition>((Func<AzureOfferPlanDefinition, bool>) (plan => plan.IsPublic)) || array.Length != 0 & testCommerce))
            {
              acquisitionOperation1 = acquisitionOperation4;
              if (offerSubscription.IsPaidBillingEnabled)
              {
                requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "GetVsoExtensionItemAcquisitionOptions. Account in PaidBillingEnabled for ItemId: {0}, accountId: {1}", (object) publishedExtension.GetFullyQualifiedName(), (object) targetAccountId);
                acquisitionOperation4.OperationState = Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationState.Disallow;
                acquisitionOperation4.Reason = GalleryResources.ExtensionAlreadyPurchased((object) publishedExtension.DisplayName);
              }
              else if (offerSubscription.IsTrialOrPreview)
              {
                requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "GetVsoExtensionItemAcquisitionOptions. Account in TrialOrPreview for ItemId: {0}, accountId: {1}", (object) publishedExtension.GetFullyQualifiedName(), (object) targetAccountId);
                DateTime? trialExpiryDate;
                if (offerSubscription.TrialExpiryDate.HasValue)
                {
                  DateTime utcNow = DateTime.UtcNow;
                  trialExpiryDate = offerSubscription.TrialExpiryDate;
                  DateTime t2 = trialExpiryDate.Value;
                  if (DateTime.Compare(utcNow, t2) > 0)
                  {
                    IVssRequestContext requestContext1 = requestContext;
                    string serviceName = this._serviceName;
                    string fullyQualifiedName = publishedExtension.GetFullyQualifiedName();
                    // ISSUE: variable of a boxed type
                    __Boxed<Guid> local1 = (ValueType) targetAccountId;
                    trialExpiryDate = offerSubscription.TrialExpiryDate;
                    // ISSUE: variable of a boxed type
                    __Boxed<DateTime> local2 = (ValueType) trialExpiryDate.Value;
                    requestContext1.Trace(12062000, TraceLevel.Info, "gallery", serviceName, "GetVsoExtensionItemAcquisitionOptions. Offer trial expired for ItemId: {0}, accountId: {1}. Trial expiry date = {2}", (object) fullyQualifiedName, (object) local1, (object) local2);
                    acquisitionOperation4.OperationState = Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationState.Disallow;
                    Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperation acquisitionOperation5 = acquisitionOperation4;
                    trialExpiryDate = offerSubscription.TrialExpiryDate;
                    string str = GalleryResources.ExtensionTrialExpired((object) "{0}", (object) trialExpiryDate.Value.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern), (object) GalleryResources.BuyRedirectText());
                    acquisitionOperation5.Reason = str;
                    goto label_11;
                  }
                }
                acquisitionOperation4.OperationState = Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationState.Completed;
                Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperation acquisitionOperation6 = acquisitionOperation4;
                trialExpiryDate = offerSubscription.TrialExpiryDate;
                DateTime utcNow1 = trialExpiryDate.Value;
                DateTime date1 = utcNow1.Date;
                utcNow1 = DateTime.UtcNow;
                DateTime date2 = utcNow1.Date;
                string str1 = GalleryResources.ExtensionAlreadyUnderTrial((object) (date1 - date2).TotalDays);
                acquisitionOperation6.Reason = str1;
              }
            }
          }
        }
label_11:
        if (acquisitionOperation1 == null)
          acquisitionOperation1 = acquisitionOperation2;
        requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "GetVsoExtensionItemAcquisitionOptions. DefaultOperationType for ItemId: {0}, accountId: {1} is {2}", (object) publishedExtension.GetFullyQualifiedName(), (object) targetAccountId, (object) Enum.GetName(typeof (Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationType), (object) acquisitionOperation1.OperationType));
        bool flag;
        try
        {
          flag = !managementClient.GetInstalledExtensionSync(requestContext, publishedExtension.Publisher.PublisherName, publishedExtension.ExtensionName).InstallState.Flags.HasFlag((Enum) ExtensionStateFlags.UnInstalled);
        }
        catch (InstalledExtensionNotFoundException ex)
        {
          flag = false;
        }
        requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "GetVsoExtensionItemAcquisitionOptions. Install state for ItemId: {0}, accountId: {1} is {2}", (object) publishedExtension.GetFullyQualifiedName(), (object) targetAccountId, (object) flag);
        if (flag)
        {
          acquisitionOperation2.OperationState = Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationState.Completed;
          acquisitionOperation3.OperationState = Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationState.Disallow;
          if (acquisitionOperation4.OperationState == Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationState.Allow)
            acquisitionOperation4.OperationState = Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationState.Completed;
        }
        else
        {
          requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "GetVsoExtensionItemAcquisitionOptions: Entered extension not installed flow");
          if (managementClient.GetPoliciesSync(requestContext, "me", (object) requestContext).Permissions.Install.HasFlag((Enum) ExtensionPolicyFlags.All))
          {
            requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "GetVsoExtensionItemAcquisitionOptions: User has install permissions");
            acquisitionOperation3.OperationState = Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationState.Disallow;
            ExtensionManifest extensionManifest = this.GetExtensionManifest(requestContext, publishedExtension);
            if (requestContext.ExecutionEnvironment.IsHostedDeployment && extensionManifest.ServiceInstanceType.HasValue && extensionManifest.ServiceInstanceType.Value != Guid.Empty && !this.IsInstanceTypeAvailableForAccount(requestContext, extensionManifest.ServiceInstanceType.Value, targetAccountId))
            {
              requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "GetVsoExtensionItemAcquisitionOptions: Instance type not available for account: {0} serviceInstanceType: {1}", (object) targetAccountId, (object) extensionManifest.ServiceInstanceType.Value);
              acquisitionOperation2.OperationState = Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationState.Disallow;
              acquisitionOperation2.Reason = GalleryResources.ExtensionNotAvailableInAccountRegion((object) publishedExtension.DisplayName);
              acquisitionOperation4.OperationState = Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationState.Disallow;
              acquisitionOperation4.Reason = GalleryResources.ExtensionNotAvailableInAccountRegion((object) publishedExtension.DisplayName);
            }
          }
          else
          {
            requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "GetVsoExtensionItemAcquisitionOptions: User doesn't have install permissions. Entered request workflow");
            acquisitionOperation1 = acquisitionOperation3;
            acquisitionOperation2.OperationState = Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationState.Disallow;
            RequestedExtension requestedExtension = managementClient.GetRequestsSync(requestContext, (object) requestContext).Find((Predicate<RequestedExtension>) (req => string.Equals(req.ExtensionName, publishedExtension.ExtensionName) && string.Equals(req.PublisherName, publishedExtension.Publisher.PublisherName)));
            if (requestedExtension != null && requestedExtension.ExtensionRequests != null && requestedExtension.ExtensionRequests.Count > 0 && requestedExtension.ExtensionRequests[0].RequestState != ExtensionRequestState.Accepted)
            {
              requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "GetVsoExtensionItemAcquisitionOptions: Extension already requested for itemId: {0}", (object) publishedExtension.GetFullyQualifiedName());
              acquisitionOperation3.OperationState = Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionRequest.AcquisitionOperationState.Completed;
            }
          }
        }
        acquisitionOperationList.Add(acquisitionOperation2);
        acquisitionOperationList.Add(acquisitionOperation3);
        acquisitionOperationList.Add(acquisitionOperation4);
      }
      requestContext.TraceLeave(12062000, "gallery", this._serviceName, methodName);
      return new Microsoft.VisualStudio.Services.Gallery.WebApi.AcquisitionOption.AcquisitionOptions()
      {
        Operations = acquisitionOperationList,
        DefaultOperation = acquisitionOperation1
      };
    }

    private bool IsInstanceTypeAvailableForAccount(
      IVssRequestContext requestContext,
      Guid instanceType,
      Guid targetAccountId)
    {
      string methodName = nameof (IsInstanceTypeAvailableForAccount);
      requestContext.TraceEnter(12062000, "gallery", this._serviceName, methodName);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      HostInstanceMapping hostInstanceMapping = vssRequestContext.GetService<IInstanceManagementService>().GetHostInstanceMapping(vssRequestContext, targetAccountId, instanceType);
      int num = hostInstanceMapping == null || hostInstanceMapping.ServiceInstance == null ? 0 : (hostInstanceMapping.ServiceInstance.PublicUri != (Uri) null ? 1 : 0);
      requestContext.TraceLeave(12062000, "gallery", this._serviceName, methodName);
      return num != 0;
    }

    private IOfferSubscription GetOfferSubscription(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension,
      Guid targetAccountId)
    {
      string methodName = nameof (GetOfferSubscription);
      requestContext.TraceEnter(12062000, "gallery", this._serviceName, methodName);
      IBillingClient levelBillingClient = this._clientFactory.GetNewNonDeploymentLevelBillingClient(requestContext, targetAccountId);
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publishedExtension.Publisher.PublisherName, publishedExtension.ExtensionName);
      IOfferSubscription offerSubscription = (IOfferSubscription) null;
      try
      {
        offerSubscription = levelBillingClient.GetOfferSubscription(requestContext, fullyQualifiedName);
      }
      catch (VssServiceException ex)
      {
        if (ex.InnerException != null)
        {
          if (ex.InnerException is ArgumentException)
            goto label_5;
        }
        throw;
      }
label_5:
      requestContext.TraceLeave(12062000, "gallery", this._serviceName, methodName);
      return offerSubscription;
    }

    private ExtensionManifest GetExtensionManifest(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension)
    {
      string methodName = nameof (GetExtensionManifest);
      requestContext.TraceEnter(12062000, "gallery", this._serviceName, methodName);
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ExtensionAsset extensionAsset = vssRequestContext.GetService<IPublisherAssetService>().QueryAsset(vssRequestContext, publishedExtension.Publisher.PublisherName, publishedExtension.ExtensionName, publishedExtension.Versions[0].Version, Guid.Empty, (IEnumerable<AssetInfo>) new AssetInfo[1]
      {
        new AssetInfo("Microsoft.VisualStudio.Services.Manifest", (string) null)
      }, (string) null, (string) null, true);
      using (Stream manifestStream = vssRequestContext.GetService<ITeamFoundationFileService>().RetrieveFile(vssRequestContext, (long) extensionAsset.AssetFile.FileId, false, out byte[] _, out long _, out CompressionType _))
      {
        ExtensionManifest extensionManifest = ExtensionUtil.LoadManifest(publishedExtension.Publisher.PublisherName, publishedExtension.ExtensionName, publishedExtension.Versions[0].Version, manifestStream, (IDictionary<string, object>) ExtensionUtil.GetExtensionProperties(publishedExtension.Flags));
        requestContext.TraceLeave(12062000, "gallery", this._serviceName, methodName);
        return extensionManifest;
      }
    }

    private Guid GetTargetAccountId(ExtensionAcquisitionRequest acquisitionRequest)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) acquisitionRequest.Targets, "Targets");
      Guid targetAccountId;
      AcquisitionService.CheckAcquisitionTarget(acquisitionRequest.Targets[0], out targetAccountId);
      return targetAccountId;
    }

    private static void CheckGalleryItemName(string galleryItemName, string paramName = "galleryItemName")
    {
      ArgumentUtility.CheckStringForNullOrEmpty(galleryItemName, paramName);
      if (!AcquisitionService.TryParseGalleryItemName(galleryItemName, out string _, out string _))
        throw new ArgumentException(GalleryResources.InvalidFullyQualifiedName(), paramName);
    }

    private static void CheckAcquisitionTarget(
      string target,
      out Guid targetAccountId,
      string paramName = "target")
    {
      targetAccountId = Guid.Empty;
      ArgumentUtility.CheckStringForNullOrEmpty(target, paramName);
      if (!Guid.TryParse(target, out targetAccountId))
        throw new ArgumentException(GalleryResources.InvalidAcqusitionTargetFormat(), paramName);
      ArgumentUtility.CheckForEmptyGuid(targetAccountId, paramName);
    }

    private static bool TryParseGalleryItemName(
      string galleryItemName,
      out string publisherName,
      out string extensionName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(galleryItemName, nameof (galleryItemName));
      publisherName = (string) null;
      extensionName = (string) null;
      string[] strArray = galleryItemName.Split(AcquisitionService.s_itemNameCharPartSeparator, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length != 2)
        return false;
      publisherName = strArray[0];
      extensionName = strArray[1];
      return true;
    }

    private PublishedExtension GetExtension(
      IVssRequestContext requestContext,
      ExtensionAcquisitionRequest acquisitionRequest)
    {
      string methodName = nameof (GetExtension);
      requestContext.TraceEnter(12062000, "gallery", this._serviceName, methodName);
      Guid result = Guid.Empty;
      if (acquisitionRequest.Targets != null && acquisitionRequest.Targets.Count > 0)
        Guid.TryParse(acquisitionRequest.Targets[0], out result);
      PublishedExtension extension = this.GetExtension(requestContext, acquisitionRequest.ItemId, result);
      requestContext.TraceLeave(12062000, "gallery", this._serviceName, methodName);
      return extension;
    }

    private PublishedExtension GetExtension(
      IVssRequestContext requestContext,
      string itemId,
      Guid accountId)
    {
      string methodName = nameof (GetExtension);
      requestContext.TraceEnter(12062000, "gallery", this._serviceName, methodName);
      requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "ItemId = {0}, accountId = {1}", (object) itemId, (object) accountId);
      string publisherName;
      string extensionName;
      if (!AcquisitionService.TryParseGalleryItemName(itemId, out publisherName, out extensionName))
        throw new ArgumentException(GalleryResources.InvalidFullyQualifiedName());
      ExtensionQueryFlags flags = ExtensionQueryFlags.IncludeVersions | ExtensionQueryFlags.IncludeCategoryAndTags | ExtensionQueryFlags.IncludeInstallationTargets;
      string accountToken = (string) null;
      if (!accountId.Equals(Guid.Empty) && requestContext.ExecutionEnvironment.IsHostedDeployment)
        accountToken = this.GetAccountJwtToken(requestContext, accountId);
      PublishedExtension extension = requestContext.GetService<IPublishedExtensionService>().QueryExtension(requestContext, publisherName, extensionName, (string) null, flags, accountToken);
      requestContext.TraceLeave(12062000, "gallery", this._serviceName, methodName);
      return extension;
    }

    private string GetAccountJwtToken(IVssRequestContext requestContext, Guid accountId)
    {
      string methodName = nameof (GetAccountJwtToken);
      requestContext.TraceEnter(12062000, "gallery", this._serviceName, methodName);
      JwtClaims jwtClaims = new JwtClaims()
      {
        Expiration = new DateTime?(DateTime.UtcNow.AddDays(1.0)),
        ExtraClaims = new Dictionary<string, string>()
        {
          {
            "aid",
            accountId.ToString()
          }
        }
      };
      string jwtToken = requestContext.GetService<IGalleryJwtTokenService>().GenerateJwtToken(requestContext, "AccountSigningKey", jwtClaims);
      requestContext.Trace(12062000, TraceLevel.Info, "gallery", this._serviceName, "JwtToken = {0}", (object) jwtToken);
      requestContext.TraceLeave(12062000, "gallery", this._serviceName, methodName);
      return jwtToken;
    }

    private enum ExtensionInstallationTargetType
    {
      VSS,
      Offer,
      Integration,
    }
  }
}
