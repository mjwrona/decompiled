// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.AcquisitionService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.AcquisitionRequest;
using Microsoft.VisualStudio.Services.Gallery.Types.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  public class AcquisitionService : IVssFrameworkService
  {
    private static readonly char[] s_itemNameCharPartSeparator = new char[1]
    {
      '.'
    };
    private static readonly Dictionary<AcquisitionService.ExtensionInstallationTargetType, List<string>> s_installationTargetMap = new Dictionary<AcquisitionService.ExtensionInstallationTargetType, List<string>>()
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
      }
    };
    private const string s_area = "AcquisitionService";
    private const string s_layer = "Service";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public ExtensionAcquisitionRequest RequestAcquisition(
      IVssRequestContext requestContext,
      ExtensionAcquisitionRequest acquisitionRequest)
    {
      ArgumentUtility.CheckForNull<ExtensionAcquisitionRequest>(acquisitionRequest, "AcquisitionRequest");
      requestContext.TraceEnter(10013520, nameof (AcquisitionService), "Service", nameof (RequestAcquisition));
      ExtensionIdentifier extensionIdentifier = new ExtensionIdentifier(acquisitionRequest.ItemId);
      IVssRequestContext vssRequestContext = requestContext;
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      IPublishedExtensionCache service1 = context.GetService<IPublishedExtensionCache>();
      string str1 = (string) null;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        str1 = vssRequestContext.GetService<IAccountTokenService>().GetToken(vssRequestContext);
      IVssRequestContext requestContext1 = context;
      string publisherName1 = extensionIdentifier.PublisherName;
      string extensionName1 = extensionIdentifier.ExtensionName;
      string accountToken = str1;
      PublishedExtension publishedExtension = service1.GetPublishedExtension(requestContext1, publisherName1, extensionName1, "latest", accountToken);
      bool flag = false;
      if (acquisitionRequest.Properties != null)
        acquisitionRequest.Properties.TryGetValue<bool>("skipInstall", out flag);
      switch (acquisitionRequest.OperationType)
      {
        case AcquisitionOperationType.Get:
        case AcquisitionOperationType.None:
          throw new NotImplementedException();
        case AcquisitionOperationType.Install:
          if (!flag)
          {
            this.InstallExtension(requestContext, extensionIdentifier.PublisherName, extensionIdentifier.ExtensionName);
            break;
          }
          break;
        case AcquisitionOperationType.Buy:
          requestContext.Trace(10013520, TraceLevel.Info, nameof (AcquisitionService), "Service", "RequestAcquisition: Entered Buy option. Retrieving published extension");
          if (!flag)
            this.InstallExtension(requestContext, extensionIdentifier.PublisherName, extensionIdentifier.ExtensionName);
          if (publishedExtension.IsPaid())
          {
            if (!Guid.TryParse(acquisitionRequest.BillingId, out Guid _))
              throw new ArgumentException("BillingId");
            if (!this.ExtensionHasInstallationTargetType(publishedExtension, AcquisitionService.ExtensionInstallationTargetType.VSS))
              throw new NotImplementedException();
            break;
          }
          break;
        case AcquisitionOperationType.Try:
          requestContext.Trace(10013520, TraceLevel.Info, nameof (AcquisitionService), "Service", "RequestAcquisition: Entering Trial workflow for item: {0}", (object) acquisitionRequest.ItemId);
          if (!requestContext.ExecutionEnvironment.IsHostedDeployment || !publishedExtension.IsFirstPartyAndPaid() && !publishedExtension.IsThirdPartyAndPaid())
            throw new NotImplementedException();
          if (this.IsVsoExtensionResourceItem(publishedExtension))
            throw new NotSupportedException();
          this.InstallExtension(requestContext, extensionIdentifier.PublisherName, extensionIdentifier.ExtensionName);
          break;
        case AcquisitionOperationType.Request:
          requestContext.Trace(10013520, TraceLevel.Info, nameof (AcquisitionService), "Service", "RequestAcquisition: Request workflow for item: {0}", (object) acquisitionRequest.ItemId);
          string str2 = (string) null;
          IExtensionRequestService service2 = requestContext.GetService<IExtensionRequestService>();
          if (acquisitionRequest.Properties != null)
            acquisitionRequest.Properties.TryGetValue<string>("requestMessage", out str2);
          if (str2 != null && str2.Length > 1000)
            throw new CharacterLimitExceededException(ExtensionManagementResources.CharacterLimitExceeded((object) "requestMessage", (object) 1000.ToString()));
          IVssRequestContext requestContext2 = requestContext;
          string publisherName2 = extensionIdentifier.PublisherName;
          string extensionName2 = extensionIdentifier.ExtensionName;
          string requestMessage = str2;
          service2.RequestExtension(requestContext2, publisherName2, extensionName2, requestMessage);
          break;
      }
      requestContext.TraceLeave(10013520, nameof (AcquisitionService), "Service", nameof (RequestAcquisition));
      return acquisitionRequest;
    }

    public AcquisitionOptions GetAcquisitionOptions(
      IVssRequestContext requestContext,
      string galleryItemName,
      bool testCommerce = false,
      bool isFreeOrTrialInstall = false,
      bool isBuyOperationValid = false,
      bool isAccountOwner = false,
      bool isAccountLinked = false,
      bool isConnectedServer = false)
    {
      requestContext.TraceEnter(10013520, nameof (AcquisitionService), "Service", nameof (GetAcquisitionOptions));
      ExtensionIdentifier extensionIdentifier = new ExtensionIdentifier(galleryItemName);
      IVssRequestContext vssRequestContext1 = requestContext;
      IVssRequestContext vssRequestContext2 = requestContext.To(TeamFoundationHostType.Deployment);
      IPublishedExtensionCache service = vssRequestContext2.GetService<IPublishedExtensionCache>();
      string accountToken = (string) null;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        accountToken = vssRequestContext1.GetService<IAccountTokenService>().GetToken(vssRequestContext1);
      PublishedExtension publishedExtension = this.ValidateExtension(service, vssRequestContext2, extensionIdentifier, accountToken);
      List<AcquisitionOperation> acquisitionOperationList = new List<AcquisitionOperation>();
      bool flag = this.IsExtensionAlreadyInstalled(requestContext, extensionIdentifier);
      bool userHasInstallPermission = requestContext.GetService<IExtensionPoliciesService>().GetPolicies(requestContext).Permissions.Install.HasFlag((Enum) ExtensionPolicyFlags.All);
      bool isPaidExtension = (publishedExtension.IsFirstPartyAndPaid() || publishedExtension.IsThirdPartyAndPaid()) && !AcquisitionService.IsFirstPartyPreviewExtension(publishedExtension);
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        isPaidExtension = isPaidExtension & !isFreeOrTrialInstall & isConnectedServer;
      bool isTrialPossible = false;
      bool isResourceTypeExtension = this.IsVsoExtensionResourceItem(publishedExtension);
      List<AcquisitionOperationDisallowReason> disallowReasonsIfAny = this.GetExtensionInstallDisallowReasonsIfAny(requestContext, vssRequestContext2, publishedExtension, userHasInstallPermission, isResourceTypeExtension);
      IOfferSubscription offerSubscription = (IOfferSubscription) null;
      AcquisitionOperation acquisitionOperation1 = this.GetRequestAcquisitionOperation(requestContext, extensionIdentifier, disallowReasonsIfAny, userHasInstallPermission, flag, isResourceTypeExtension);
      AcquisitionOperation acquisitionOperation2 = this.GetInstallAcquisitionOperation(disallowReasonsIfAny, userHasInstallPermission, flag, isResourceTypeExtension);
      AcquisitionOperation acquisitionOperation3 = this.GetTrialAcquisitionOperation(requestContext, publishedExtension, disallowReasonsIfAny, ref offerSubscription, testCommerce, userHasInstallPermission, isTrialPossible, isResourceTypeExtension, flag);
      AcquisitionOperation acquisitionOperation4 = this.GetBuyAcquisitionOperation(requestContext, publishedExtension, disallowReasonsIfAny, ref offerSubscription, testCommerce, userHasInstallPermission, isPaidExtension, flag, isResourceTypeExtension, isAccountOwner, isBuyOperationValid, isAccountLinked);
      AcquisitionOperation acquisitionOperation5 = this.GetPurchaseRequestAcquisitionOperation(requestContext, disallowReasonsIfAny, isPaidExtension, acquisitionOperation4.OperationState == AcquisitionOperationState.Disallow, isResourceTypeExtension);
      AcquisitionOperation defaultOperation = this.GetDefaultOperation(acquisitionOperation2, acquisitionOperation3, acquisitionOperation1, acquisitionOperation4, acquisitionOperation5, isTrialPossible, isPaidExtension, isBuyOperationValid);
      acquisitionOperationList.Add(acquisitionOperation2);
      acquisitionOperationList.Add(acquisitionOperation1);
      acquisitionOperationList.Add(acquisitionOperation3);
      if (isBuyOperationValid)
      {
        acquisitionOperationList.Add(acquisitionOperation4);
        acquisitionOperationList.Add(acquisitionOperation5);
      }
      JObject jobject = new JObject();
      if (offerSubscription != null)
        jobject.Add("offerSubscription", JToken.FromObject((object) offerSubscription, new JsonSerializer()
        {
          ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
        }));
      requestContext.TraceLeave(10013520, nameof (AcquisitionService), "Service", nameof (GetAcquisitionOptions));
      return new AcquisitionOptions()
      {
        Operations = acquisitionOperationList,
        DefaultOperation = defaultOperation,
        ItemId = galleryItemName,
        Properties = jobject
      };
    }

    private AcquisitionOperation GetDefaultOperation(
      AcquisitionOperation installOperation,
      AcquisitionOperation trialOperation,
      AcquisitionOperation requestOperation,
      AcquisitionOperation buyOperation,
      AcquisitionOperation purchaseRequestOperation,
      bool isTrialPossible = false,
      bool isPaidExtension = false,
      bool isBuyOperationValid = false)
    {
      AcquisitionOperation defaultOperation = installOperation;
      if (requestOperation.OperationState != AcquisitionOperationState.Disallow)
        defaultOperation = requestOperation;
      else if (isTrialPossible)
      {
        defaultOperation = trialOperation;
        if (this.ShouldBeInstallOperation(trialOperation, installOperation))
          defaultOperation = installOperation;
      }
      if ((defaultOperation.OperationType != AcquisitionOperationType.Try || defaultOperation.OperationType == AcquisitionOperationType.Try && defaultOperation.OperationState != AcquisitionOperationState.Allow) && isBuyOperationValid & isPaidExtension)
      {
        if (buyOperation.OperationState != AcquisitionOperationState.Disallow)
          defaultOperation = buyOperation;
        else if (purchaseRequestOperation.OperationState != AcquisitionOperationState.Disallow)
          defaultOperation = purchaseRequestOperation;
      }
      return defaultOperation;
    }

    private bool ShouldBeInstallOperation(
      AcquisitionOperation trialOperation,
      AcquisitionOperation installOperation)
    {
      if (trialOperation.OperationState == AcquisitionOperationState.Completed && installOperation.OperationState == AcquisitionOperationState.Allow)
        return true;
      if (trialOperation.OperationState != AcquisitionOperationState.Disallow)
        return false;
      if (trialOperation.Reasons[0].Type == "OfferSubscriptionNotAvailable" || trialOperation.Reasons[0].Type == "NoPublicOfferPlans" || trialOperation.Reasons[0].Type == "BillingNotStartedDisallowReason")
        return true;
      return trialOperation.Reasons[0].Type == "TrialDisallowedExtensionAlreadyPurchased" && installOperation.OperationState == AcquisitionOperationState.Allow;
    }

    private List<AcquisitionOperationDisallowReason> GetExtensionInstallDisallowReasonsIfAny(
      IVssRequestContext requestContext,
      IVssRequestContext deploymentContext,
      PublishedExtension publishedExtension,
      bool userHasInstallPermission = false,
      bool isResourceTypeExtension = false)
    {
      List<AcquisitionOperationDisallowReason> disallowReasonsIfAny = new List<AcquisitionOperationDisallowReason>();
      if (!isResourceTypeExtension)
      {
        ExtensionManifest extensionManifest = this.GetExtensionManifest(deploymentContext, publishedExtension);
        ExtensionDemandsResolutionResult demandsResolutionResult = deploymentContext.GetService<IExtensionDemandsResolutionService>().ResolveDemands(requestContext, publishedExtension.Publisher.PublisherName, publishedExtension.ExtensionName, extensionManifest, DemandsResolutionType.Installing);
        if (demandsResolutionResult.Status == DemandsResolutionStatus.Error)
          disallowReasonsIfAny = this.GetOnDemandsResolutionErrorDisallowReasons(requestContext, publishedExtension, demandsResolutionResult);
        else if (userHasInstallPermission && requestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          Guid? serviceInstanceType = extensionManifest.ServiceInstanceType;
          if (serviceInstanceType.HasValue)
          {
            serviceInstanceType = extensionManifest.ServiceInstanceType;
            if (serviceInstanceType.Value != Guid.Empty)
            {
              IVssRequestContext requestContext1 = requestContext;
              serviceInstanceType = extensionManifest.ServiceInstanceType;
              Guid instanceType = serviceInstanceType.Value;
              if (!this.IsInstanceTypeAvailableForAccount(requestContext1, instanceType))
                disallowReasonsIfAny.Add((AcquisitionOperationDisallowReason) new ExtensionNotAvailableInRegionDisallowReason(publishedExtension.DisplayName));
            }
          }
        }
      }
      return disallowReasonsIfAny;
    }

    private AcquisitionOperation GetRequestAcquisitionOperation(
      IVssRequestContext requestContext,
      ExtensionIdentifier extensionIdentifier,
      List<AcquisitionOperationDisallowReason> extensionInstallDisallowReasons,
      bool userHasInstallPermission = false,
      bool isInstalled = false,
      bool isResourceTypeExtension = false)
    {
      AcquisitionOperation acquisitionOperation = this.GetAcquisitionOperation(AcquisitionOperationType.Request);
      List<AcquisitionOperationDisallowReason> reasons = new List<AcquisitionOperationDisallowReason>();
      if (extensionInstallDisallowReasons != null && extensionInstallDisallowReasons.Count > 0)
        this.SetAcquisitionOperation(ref acquisitionOperation, AcquisitionOperationState.Disallow, extensionInstallDisallowReasons);
      else if (isResourceTypeExtension)
      {
        OperationNotAllowedForResourceTypeExtension resourceTypeExtension = new OperationNotAllowedForResourceTypeExtension(ExtensionResources.InstallOperation());
        reasons.Add((AcquisitionOperationDisallowReason) resourceTypeExtension);
        this.SetAcquisitionOperation(ref acquisitionOperation, AcquisitionOperationState.Disallow, reasons);
      }
      else if (isInstalled)
      {
        AlreadyInstalledDisallowReason installedDisallowReason = new AlreadyInstalledDisallowReason();
        reasons.Add((AcquisitionOperationDisallowReason) installedDisallowReason);
        this.SetAcquisitionOperation(ref acquisitionOperation, AcquisitionOperationState.Disallow, reasons);
      }
      else if (userHasInstallPermission)
      {
        reasons.Add((AcquisitionOperationDisallowReason) new UserCanNotRequestDisallowReason());
        this.SetAcquisitionOperation(ref acquisitionOperation, AcquisitionOperationState.Disallow, reasons);
      }
      else
      {
        RequestedExtension requestedExtension = requestContext.GetService<IExtensionRequestService>().GetRequests(requestContext).Find((Predicate<RequestedExtension>) (req => string.Equals(req.ExtensionName, extensionIdentifier.ExtensionName, StringComparison.OrdinalIgnoreCase) && string.Equals(req.PublisherName, extensionIdentifier.PublisherName, StringComparison.OrdinalIgnoreCase)));
        if (requestedExtension != null && requestedExtension.ExtensionRequests != null && requestedExtension.ExtensionRequests.Count > 0 && requestedExtension.ExtensionRequests[0].RequestState != ExtensionRequestState.Accepted)
        {
          reasons.Add((AcquisitionOperationDisallowReason) new AlreadyRequestedDisallowReason());
          this.SetAcquisitionOperation(ref acquisitionOperation, AcquisitionOperationState.Completed, reasons);
        }
      }
      return acquisitionOperation;
    }

    private AcquisitionOperation GetPurchaseRequestAcquisitionOperation(
      IVssRequestContext requestContext,
      List<AcquisitionOperationDisallowReason> extensionInstallDisallowReasons,
      bool isPaidExtension = false,
      bool isBuyOperationDisallowed = false,
      bool isResourceTypeExtension = false)
    {
      AcquisitionOperation acquisitionOperation = this.GetAcquisitionOperation(AcquisitionOperationType.PurchaseRequest);
      this.SetAcquisitionOperation(ref acquisitionOperation, AcquisitionOperationState.Disallow, new List<AcquisitionOperationDisallowReason>()
      {
        (AcquisitionOperationDisallowReason) new UserCanNotRequestDisallowReason()
      });
      return acquisitionOperation;
    }

    private AcquisitionOperation GetInstallAcquisitionOperation(
      List<AcquisitionOperationDisallowReason> extensionInstallDisallowReasons,
      bool userHasInstallPermission = false,
      bool isInstalled = false,
      bool isResourceTypeExtension = false)
    {
      AcquisitionOperation acquisitionOperation = this.GetAcquisitionOperation(AcquisitionOperationType.Install);
      List<AcquisitionOperationDisallowReason> reasons = new List<AcquisitionOperationDisallowReason>();
      if (extensionInstallDisallowReasons != null && extensionInstallDisallowReasons.Count > 0)
        this.SetAcquisitionOperation(ref acquisitionOperation, AcquisitionOperationState.Disallow, extensionInstallDisallowReasons);
      else if (isResourceTypeExtension)
      {
        OperationNotAllowedForResourceTypeExtension resourceTypeExtension = new OperationNotAllowedForResourceTypeExtension(ExtensionResources.InstallOperation());
        reasons.Add((AcquisitionOperationDisallowReason) resourceTypeExtension);
        this.SetAcquisitionOperation(ref acquisitionOperation, AcquisitionOperationState.Disallow, reasons);
      }
      else if (isInstalled)
      {
        reasons.Add((AcquisitionOperationDisallowReason) new AlreadyInstalledDisallowReason());
        this.SetAcquisitionOperation(ref acquisitionOperation, AcquisitionOperationState.Completed, reasons);
      }
      else if (!userHasInstallPermission)
      {
        reasons.Add((AcquisitionOperationDisallowReason) new UserDoesNotHavePermissionToInstallDisallowReason());
        this.SetAcquisitionOperation(ref acquisitionOperation, AcquisitionOperationState.Disallow, reasons);
      }
      return acquisitionOperation;
    }

    private AcquisitionOperation GetBuyAcquisitionOperation(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension,
      List<AcquisitionOperationDisallowReason> extensionInstallDisallowReasons,
      ref IOfferSubscription offerSubscription,
      bool testCommerce = false,
      bool userHasInstallPermission = false,
      bool isPaidExtension = false,
      bool isInstalled = false,
      bool isResourceTypeExtension = false,
      bool isAccountOwner = false,
      bool isBuyOperationValid = false,
      bool isAccountLinked = false)
    {
      AcquisitionOperation acquisitionOperation = this.GetAcquisitionOperation(AcquisitionOperationType.Buy);
      this.SetAcquisitionOperation(ref acquisitionOperation, AcquisitionOperationState.Disallow, new List<AcquisitionOperationDisallowReason>()
      {
        (AcquisitionOperationDisallowReason) new GetBuyNotPossibleDisallowReason()
      });
      return acquisitionOperation;
    }

    private AcquisitionOperation GetTrialAcquisitionOperation(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension,
      List<AcquisitionOperationDisallowReason> extensionInstallDisallowReasons,
      ref IOfferSubscription offerSubscription,
      bool testCommerce = false,
      bool userHasInstallPermission = false,
      bool isTrialPossible = false,
      bool isResourceTypeExtension = false,
      bool isExtensionInstalled = false)
    {
      AcquisitionOperation acquisitionOperation = this.GetAcquisitionOperation(AcquisitionOperationType.Try);
      this.SetAcquisitionOperation(ref acquisitionOperation, AcquisitionOperationState.Disallow, new List<AcquisitionOperationDisallowReason>()
      {
        (AcquisitionOperationDisallowReason) new GetTrialNotPossibleDisallowReason()
      });
      return acquisitionOperation;
    }

    private List<AcquisitionOperationDisallowReason> GetOnDemandsResolutionErrorDisallowReasons(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension,
      ExtensionDemandsResolutionResult demandsResolutionResult)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("Error", "Demands Check Failure");
      if (publishedExtension != null && publishedExtension.IsPaid())
        properties.Add(CustomerIntelligenceActions.ExtensionProperties.IsPaidExtension, true);
      else
        properties.Add(CustomerIntelligenceActions.ExtensionProperties.IsPaidExtension, false);
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.Contributions, ExtensionManagementCustomerIntelligenceFeature.ExtensionManagement, properties);
      List<AcquisitionOperationDisallowReason> errorDisallowReasons = new List<AcquisitionOperationDisallowReason>();
      foreach (DemandIssue demandIssue in demandsResolutionResult.DemandIssues)
        errorDisallowReasons.Add((AcquisitionOperationDisallowReason) new UnresolvedDemandDisallowReason(demandIssue.Message));
      return errorDisallowReasons;
    }

    private void SetAcquisitionOperation(
      ref AcquisitionOperation acquisitionOperation,
      AcquisitionOperationState operationState,
      List<AcquisitionOperationDisallowReason> reasons)
    {
      acquisitionOperation.OperationState = operationState;
      acquisitionOperation.Reasons = reasons;
    }

    private static bool IsExtensionInTrial(IOfferSubscription offerSubscription) => offerSubscription.IsTrialOrPreview && offerSubscription.TrialExpiryDate.HasValue;

    private static bool IsExtensionExpired(IOfferSubscription offerSubscription) => offerSubscription.TrialExpiryDate.HasValue && DateTime.Compare(DateTime.UtcNow, offerSubscription.TrialExpiryDate.Value) > 0;

    private static bool IsBillingStarted(IOfferSubscription offerSubscription)
    {
      if (offerSubscription.OfferMeter != (OfferMeter) null)
      {
        DateTime? billingStartDate = offerSubscription.OfferMeter.BillingStartDate;
        if (billingStartDate.HasValue)
        {
          DateTime utcNow = DateTime.UtcNow;
          billingStartDate = offerSubscription.OfferMeter.BillingStartDate;
          DateTime t2 = billingStartDate.Value;
          return DateTime.Compare(utcNow, t2) > 0;
        }
      }
      return false;
    }

    private static bool IsFirstPartyPreviewExtension(PublishedExtension publishedExtension) => publishedExtension.IsFirstParty() && publishedExtension.Flags.HasFlag((Enum) PublishedExtensionFlags.Preview);

    private bool IsExtensionAlreadyInstalled(
      IVssRequestContext requestContext,
      ExtensionIdentifier extensionIdentifier)
    {
      try
      {
        return !requestContext.GetService<IInstalledExtensionService>().GetInstalledExtension(requestContext, extensionIdentifier.PublisherName, extensionIdentifier.ExtensionName).InstallState.Flags.HasFlag((Enum) ExtensionStateFlags.UnInstalled);
      }
      catch (InstalledExtensionNotFoundException ex)
      {
        return false;
      }
    }

    private PublishedExtension ValidateExtension(
      IPublishedExtensionCache publishedExtensionCache,
      IVssRequestContext deploymentContext,
      ExtensionIdentifier extensionIdentifier,
      string accountToken)
    {
      PublishedExtension publishedExtension = publishedExtensionCache.GetPublishedExtension(deploymentContext, extensionIdentifier.PublisherName, extensionIdentifier.ExtensionName, "latest", accountToken);
      if (publishedExtension == null)
        throw new ExtensionDoesNotExistException(ExtensionResources.ExtensionDoesNotExist());
      return this.IsVsoExtensionItem(publishedExtension) || this.IsVsoExtensionResourceItem(publishedExtension) ? publishedExtension : throw new NotSupportedException();
    }

    private AcquisitionOperation GetAcquisitionOperation(
      AcquisitionOperationType acquisitionOperationType)
    {
      return new AcquisitionOperation()
      {
        OperationType = acquisitionOperationType,
        OperationState = AcquisitionOperationState.Allow,
        Reasons = new List<AcquisitionOperationDisallowReason>()
      };
    }

    private IOfferSubscription GetOfferSubscription(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension)
    {
      requestContext.TraceEnter(10013520, nameof (AcquisitionService), "Service", nameof (GetOfferSubscription));
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publishedExtension.Publisher.PublisherName, publishedExtension.ExtensionName);
      IOfferSubscriptionService service = requestContext.GetService<IOfferSubscriptionService>();
      IOfferSubscription offerSubscription = (IOfferSubscription) null;
      try
      {
        if (!publishedExtension.IsByolEnforcedExtension())
          offerSubscription = service.GetOfferSubscription(requestContext, fullyQualifiedName, true);
      }
      catch (VssServiceException ex)
      {
        if (ex.InnerException != null)
        {
          if (ex.InnerException is ArgumentException)
            goto label_7;
        }
        requestContext.Trace(10013520, TraceLevel.Info, nameof (AcquisitionService), "Service", "Exception while getting GetOfferSubscription. ExtensionName = {0}, Exception = {1}", (object) fullyQualifiedName, (object) ex.Message);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10013520, nameof (AcquisitionService), "Service", nameof (GetOfferSubscription));
      }
label_7:
      return offerSubscription;
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

    private InstalledExtension InstallExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(publisherName, nameof (publisherName));
      ArgumentUtility.CheckStringForNullOrEmpty(extensionName, nameof (extensionName));
      try
      {
        IInstalledExtensionService service = requestContext.GetService<IInstalledExtensionService>();
        InstalledExtension installedExtension = service.InstallExtension(requestContext, publisherName, extensionName);
        service.UpdateExtensionInstallCount(requestContext, publisherName, extensionName);
        return installedExtension;
      }
      catch (ExtensionAlreadyInstalledException ex)
      {
        requestContext.Trace(10013520, TraceLevel.Info, nameof (AcquisitionService), "Service", "InstallExtension: Entered install workflow. Extension already installed for Extension: {0}", (object) (publisherName + extensionName));
      }
      return (InstalledExtension) null;
    }

    private bool IsVsoExtensionItem(PublishedExtension publishedExtension) => publishedExtension.InstallationTargets == null || GalleryUtil.InstallationTargetsHasVSTS((IEnumerable<InstallationTarget>) publishedExtension.InstallationTargets);

    private bool IsVsoExtensionResourceItem(PublishedExtension publishedExtension) => publishedExtension.InstallationTargets == null || GalleryUtil.InstallationTargetsHasVSTSResource((IEnumerable<InstallationTarget>) publishedExtension.InstallationTargets);

    private bool ExtensionIsPaid(PublishedExtension publishedExtension) => publishedExtension.Tags != null && publishedExtension.Tags.Any<string>((Func<string, bool>) (s => s.Equals("$IsPaid", StringComparison.OrdinalIgnoreCase))) || publishedExtension.Flags.HasFlag((Enum) PublishedExtensionFlags.Paid);

    private bool IsInstanceTypeAvailableForAccount(
      IVssRequestContext requestContext,
      Guid instanceType)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      HostInstanceMapping hostInstanceMapping = vssRequestContext.GetService<IInstanceManagementService>().GetHostInstanceMapping(vssRequestContext, requestContext.ServiceHost.InstanceId, instanceType, true);
      return hostInstanceMapping != null && hostInstanceMapping.ServiceInstance != null && hostInstanceMapping.ServiceInstance.PublicUri != (Uri) null;
    }

    private ExtensionManifest GetExtensionManifest(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension)
    {
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      IContributionManifestService service = context.GetService<IContributionManifestService>();
      string str = (string) null;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        IVssRequestContext vssRequestContext = requestContext;
        str = vssRequestContext.GetService<IAccountTokenService>().GetToken(vssRequestContext);
      }
      IVssRequestContext requestContext1 = context;
      string publisherName = publishedExtension.Publisher.PublisherName;
      string extensionName = publishedExtension.ExtensionName;
      string latestVersion = AcquisitionService.GetLatestVersion(publishedExtension.Versions);
      string accountToken = str;
      ExtensionManifest extensionManifest;
      ref ExtensionManifest local = ref extensionManifest;
      service.TryGetManifest(requestContext1, publisherName, extensionName, latestVersion, accountToken, out local);
      return extensionManifest;
    }

    private bool IsThirdPartyPaidExtension(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension)
    {
      return this.IsThirdPartyExtension(publishedExtension) && this.ExtensionIsPaid(publishedExtension);
    }

    private bool IsThirdPartyExtension(PublishedExtension publishedExtension) => !string.Equals(publishedExtension.Publisher.PublisherName, "ms", StringComparison.OrdinalIgnoreCase);

    private void AcquireTrialOrPreviewForExtension(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension,
      ExtensionAcquisitionRequest acquisitionRequest)
    {
      IOfferSubscriptionService service = requestContext.GetService<IOfferSubscriptionService>();
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publishedExtension.Publisher.PublisherName, publishedExtension.ExtensionName);
      try
      {
        service.EnableTrialOrPreview(requestContext, fullyQualifiedName, ResourceRenewalGroup.Monthly);
      }
      catch (VssServiceResponseException ex)
      {
        if (ex.InnerException == null || !(ex.InnerException is InvalidOperationException))
          throw;
        else
          requestContext.Trace(10013520, TraceLevel.Info, nameof (AcquisitionService), "Service", "Extension already in trial or preview. ExtensionName = {0}", (object) publishedExtension.ExtensionName);
      }
      IOfferSubscription offerSubscription = this.GetOfferSubscription(requestContext, publishedExtension);
      if (offerSubscription == null || !offerSubscription.TrialExpiryDate.HasValue)
        return;
      acquisitionRequest.Properties.Add("TrialExpiryDate", (JToken) offerSubscription.TrialExpiryDate);
    }

    private static string GetLatestVersion(List<ExtensionVersion> versions)
    {
      string latestVersion = (string) null;
      if (versions != null && versions.Count > 0)
        latestVersion = versions[0].Version;
      return latestVersion;
    }

    private enum ExtensionInstallationTargetType
    {
      VSS,
      Offer,
      Integration,
    }
  }
}
