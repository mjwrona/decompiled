// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExtensionDraftService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Setup;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class ExtensionDraftService : IExtensionDraftService, IVssFrameworkService
  {
    private const string ExtensionBasePathPlaceholder = "{CURRENT_EXTENSION_ASSET_BASE_PATH}";
    private ExtensionPayloadValidatorFactory _payloadValidatorFactory;
    private ExtensionDataBuilder _extensionDataBuilder;
    private ExtensionDraftServiceHelper _extensionDraftServiceHelper;
    private const string s_area = "Gallery";
    private const string s_layer = "ExtensionDraftService";
    private readonly IDictionary<string, string> _knownAssetTypeToContentTypeMap = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "Microsoft.VisualStudio.Services.Content.Details",
        "text/plain"
      },
      {
        "Microsoft.VisualStudio.Services.Content.License",
        "text/plain"
      },
      {
        "Microsoft.VisualStudio.Services.Content.Changelog",
        "text/plain"
      },
      {
        "Microsoft.VisualStudio.Services.Icons.Default",
        "image/png"
      }
    };
    private const string DraftCreateEvent = "DraftCreate";
    private const string DraftCreateErrorEvent = "DraftCreateError";
    private const string AddAssetToDraftEvent = "AddAssetToDraft";
    private const string GetAssetFromDraftEvent = "GetAssetFromDraft";
    private const string AddAssetToDraftErrorEvent = "AddAssetToDraftError";
    private const string ExtensionPublishFromDraftEvent = "ExtensionPublishFromDraft";
    private const string ExtensionPublishFromDraftErrorEvent = "ExtensionPublishFromDraftError";
    private const string CancelDraftEvent = "CancelDraft";
    private const string CancelDraftErrorEvent = "CancelDraftError";
    private const string UpdatePayloadInDraftEvent = "UpdatePayloadInDraft";
    private const string UpdatePayloadInDraftErrorEvent = "UpdatePayloadInDraftError";
    private const int MaxDraftsPerUser = 15;

    public ExtensionDraftService()
    {
      this._payloadValidatorFactory = new ExtensionPayloadValidatorFactory();
      this._extensionDataBuilder = new ExtensionDataBuilder();
      this._extensionDraftServiceHelper = new ExtensionDraftServiceHelper();
    }

    internal ExtensionDraftService(
      ExtensionPayloadValidatorFactory extensionPayloadValidatorFactory,
      ExtensionDataBuilder extensionDataBuilder,
      ExtensionDraftServiceHelper extensionDraftServiceUtil)
    {
      this._payloadValidatorFactory = extensionPayloadValidatorFactory;
      this._extensionDataBuilder = extensionDataBuilder;
      this._extensionDraftServiceHelper = extensionDraftServiceUtil;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public ExtensionDraft CreateExtensionDraftForNewExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string payloadFileName,
      string productType,
      Stream payloadStream)
    {
      return this.CreateNewDraft(requestContext, publisherName, (string) null, payloadFileName, productType, payloadStream);
    }

    public ExtensionDraft UpdatePayloadInDraft(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Guid draftId,
      string payloadFileName,
      Stream payloadStream)
    {
      ExtensionDraft extensionDraft = (ExtensionDraft) null;
      try
      {
        ExtensionDeploymentTechnology deploymentTechnology1 = this._payloadValidatorFactory.GetFileDeploymentTechnology(payloadFileName, payloadStream);
        if (payloadStream != null)
          GalleryServerUtil.ValidatePackageSize(requestContext, payloadStream.Length, 482344960L);
        GalleryUtil.CheckAssetType(payloadFileName);
        Guid userId = requestContext.GetUserId();
        Publisher publisher;
        PublishedExtension extension;
        this.PerformCommonValidations(requestContext, publisherName, extensionName, out publisher, out extension);
        extensionDraft = this.FetchDraft(requestContext, draftId, userId);
        this.ValidatePayloadReuploadRules(deploymentTechnology1, extensionName, extensionDraft);
        this.ValidateDraftData(extensionDraft, publisher, extension, userId, publisherName, extensionName, draftId, (UnpackagedExtensionData) null);
        IExtensionPayloadValidator deploymentTechnology2 = this._payloadValidatorFactory.GetValidatorForDeploymentTechnology(extensionDraft.Product, deploymentTechnology1);
        PayloadValidationResult payloadValidationResult = deploymentTechnology2.ValidatePayload(requestContext, payloadStream, payloadFileName, publisher, extension);
        extensionDraft.ValidationErrors = payloadValidationResult.ValidationErrors;
        if (payloadValidationResult.IsValid)
        {
          ExtensionPayload validationResult = deploymentTechnology2.GetPayloadFromValidationResult(payloadValidationResult);
          extensionDraft.LastUpdated = DateTime.UtcNow;
          extensionDraft.Payload = validationResult;
          extensionDraft.ValidationErrors = payloadValidationResult.ValidationErrors;
          extensionDraft.ValidationWarnings = payloadValidationResult.ValidationWarnings;
          if (extension != null && ExtensionDeploymentTechnology.Vsix.Equals((object) deploymentTechnology1))
          {
            string newExtensionVersion = payloadValidationResult.VsixMetadata?["VsixVersion"];
            this._extensionDraftServiceHelper.ValidateExtensionVersionImmutabilityOnUpload(requestContext, newExtensionVersion, extension, extensionDraft);
          }
          this.DeletePayloadAssetsFromDraft(requestContext, extensionDraft, payloadValidationResult);
          this.AddPayloadAssetsToDraft(requestContext, payloadStream, payloadValidationResult, extensionDraft);
          using (ExtensionDraftComponent component = requestContext.CreateComponent<ExtensionDraftComponent>())
            component.CreateOrUpdateExtensionDraft(extensionDraft);
          ExtensionDraftService.PublishDraftCTEvent(requestContext, nameof (UpdatePayloadInDraft), extensionDraft, publisherName, extensionName, Guid.Empty, (Exception) null);
          return extensionDraft;
        }
        ExtensionDraftService.PublishDraftCTEvent(requestContext, "UpdatePayloadInDraftError", extensionDraft, publisherName, extensionName, Guid.Empty, (Exception) null);
        return extensionDraft;
      }
      catch (Exception ex)
      {
        ExtensionDraftService.PublishDraftCTEvent(requestContext, "UpdatePayloadInDraftError", extensionDraft, publisherName, extensionName, Guid.Empty, ex);
        throw;
      }
    }

    public ExtensionDraft CreateExtensionDraftForEditExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      return this.CreateNewDraft(requestContext, publisherName, extensionName, (string) null, (string) null, (Stream) null);
    }

    private ExtensionDraft CreateNewDraft(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string payloadFileName,
      string productType,
      Stream payloadStream)
    {
      try
      {
        if (payloadStream != null)
          GalleryServerUtil.ValidatePackageSize(requestContext, payloadStream.Length, 482344960L);
        if (!string.IsNullOrEmpty(payloadFileName))
          GalleryUtil.CheckAssetType(payloadFileName);
        Publisher publisher;
        PublishedExtension extension;
        this.PerformCommonValidations(requestContext, publisherName, extensionName, out publisher, out extension);
        Guid userId = requestContext.GetUserId();
        if (this.GetAllDrafts(requestContext, userId).Count<ExtensionDraft>((Func<ExtensionDraft, bool>) (x => x.DraftState == DraftStateType.Unpublished)) >= 15)
          throw new ExtensionDraftTooManyEditsException(GalleryResources.ErrorTooManyEditAttempts());
        bool flag = extension != null;
        if (!flag)
          productType = this.ValidateProductType(productType);
        ExtensionDeploymentTechnology deploymentTechnology1 = flag ? this._payloadValidatorFactory.GetDeploymentTechnology(extension) : this._payloadValidatorFactory.GetFileDeploymentTechnology(payloadFileName, payloadStream);
        ExtensionDraft newDraft = new ExtensionDraft()
        {
          ExtensionName = extension?.ExtensionName,
          ExtensionId = extension != null ? extension.ExtensionId : Guid.Empty,
          PublisherName = publisherName,
          Id = Guid.NewGuid(),
          UserId = userId,
          CreatedDate = DateTime.UtcNow,
          LastUpdated = DateTime.UtcNow,
          EditReferenceDate = extension != null ? extension.LastUpdated : DateTime.UtcNow,
          Product = extension != null ? GalleryUtil.GetProductTypeForInstallationTargets((IEnumerable<InstallationTarget>) extension.InstallationTargets) : productType,
          DraftState = DraftStateType.Unpublished
        };
        PayloadValidationResult payloadValidationResult = (PayloadValidationResult) null;
        ExtensionPayload extensionPayload;
        if (flag)
        {
          extensionPayload = new ExtensionPayload()
          {
            Type = deploymentTechnology1,
            IsValid = true,
            InstallationTargets = extension.InstallationTargets,
            FileName = GalleryServerUtil.GetPayloadFileName(extension)
          };
        }
        else
        {
          IExtensionPayloadValidator deploymentTechnology2 = this._payloadValidatorFactory.GetValidatorForDeploymentTechnology(productType, deploymentTechnology1);
          payloadValidationResult = deploymentTechnology2.ValidatePayload(requestContext, payloadStream, payloadFileName, publisher, extension);
          newDraft.ValidationErrors = payloadValidationResult.ValidationErrors;
          newDraft.ValidationWarnings = payloadValidationResult.ValidationWarnings;
          if (payloadValidationResult.IsValid)
          {
            extensionPayload = deploymentTechnology2.GetPayloadFromValidationResult(payloadValidationResult);
          }
          else
          {
            ExtensionDraftService.PublishDraftCTEvent(requestContext, "DraftCreateError", newDraft, publisherName, extensionName, Guid.Empty, (Exception) null);
            return newDraft;
          }
        }
        newDraft.Payload = extensionPayload;
        using (ExtensionDraftComponent component = requestContext.CreateComponent<ExtensionDraftComponent>())
          component.CreateOrUpdateExtensionDraft(newDraft);
        if (flag)
          this.LinkExistingAssetsToDraft(requestContext, extension, newDraft);
        else
          this.AddPayloadAssetsToDraft(requestContext, payloadStream, payloadValidationResult, newDraft);
        ExtensionDraftService.PublishDraftCTEvent(requestContext, "DraftCreate", newDraft, publisherName, extensionName, Guid.Empty, (Exception) null);
        return newDraft;
      }
      catch (Exception ex)
      {
        ExtensionDraftService.PublishDraftCTEvent(requestContext, "DraftCreateError", (ExtensionDraft) null, publisherName, extensionName, Guid.Empty, ex);
        throw;
      }
    }

    public ExtensionDraft CancelDraft(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Guid draftId)
    {
      ExtensionDraft extensionDraft = (ExtensionDraft) null;
      try
      {
        Guid userId = requestContext.GetUserId();
        Publisher publisher;
        PublishedExtension extension;
        this.PerformCommonValidations(requestContext, publisherName, extensionName, out publisher, out extension);
        extensionDraft = this.FetchDraft(requestContext, draftId, userId);
        this.ValidateDraftData(extensionDraft, publisher, extension, userId, publisherName, extensionName, draftId, (UnpackagedExtensionData) null);
        extensionDraft.DraftState = DraftStateType.Cancelled;
        extensionDraft.LastUpdated = DateTime.UtcNow;
        using (ExtensionDraftComponent component = requestContext.CreateComponent<ExtensionDraftComponent>())
          component.CreateOrUpdateExtensionDraft(extensionDraft);
        ExtensionDraftService.PublishDraftCTEvent(requestContext, nameof (CancelDraft), extensionDraft, publisherName, extensionName, draftId, (Exception) null);
        return extensionDraft;
      }
      catch (Exception ex)
      {
        ExtensionDraftService.PublishDraftCTEvent(requestContext, "CancelDraftError", extensionDraft, publisherName, extensionName, draftId, ex);
        throw;
      }
    }

    public ExtensionDraft CreateExtensionFromDraft(
      IVssRequestContext requestContext,
      string publisherName,
      Guid draftId,
      UnpackagedExtensionData extensionData)
    {
      return this.PublishExtensionFromDraft(requestContext, publisherName, (string) null, draftId, extensionData);
    }

    public ExtensionDraft UpdateExtensionFromDraft(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Guid draftId,
      UnpackagedExtensionData extensionData)
    {
      return this.PublishExtensionFromDraft(requestContext, publisherName, extensionName, draftId, extensionData);
    }

    private ExtensionDraft PublishExtensionFromDraft(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Guid draftId,
      UnpackagedExtensionData extensionData)
    {
      ExtensionDraft extensionDraft = (ExtensionDraft) null;
      string layer = nameof (ExtensionDraftService);
      string methodName = nameof (PublishExtensionFromDraft);
      requestContext.TraceEnter(12062075, "gallery", layer, methodName);
      try
      {
        if (extensionData == null)
          throw new ArgumentException("Extension data not provided");
        Guid userId = requestContext.GetUserId();
        Publisher publisher;
        PublishedExtension extension;
        this.PerformCommonValidations(requestContext, publisherName, extensionName, out publisher, out extension);
        extensionDraft = this.FetchDraft(requestContext, draftId, userId);
        this.ValidateDraftData(extensionDraft, publisher, extension, userId, publisherName, extensionName, draftId, extensionData);
        bool flag1 = extension != null;
        bool flag2 = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DisableLinkTypeExtensions");
        bool flag3 = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DisableLinkTypeExtensionUpdate");
        if (extensionDraft.Payload.Type == ExtensionDeploymentTechnology.ReferralLink && (flag1 & flag3 || !flag1 & flag2))
          throw new ExtensionLinkTypeDisabledException(GalleryResources.ExtensionLinkTypeDisabledException());
        if (flag1 && extension.LastUpdated != extensionDraft.EditReferenceDate)
        {
          extensionDraft.DraftState = DraftStateType.Error;
          extensionDraft.LastUpdated = DateTime.UtcNow;
          using (ExtensionDraftComponent component = requestContext.CreateComponent<ExtensionDraftComponent>())
            component.CreateOrUpdateExtensionDraft(extensionDraft);
          extensionDraft.ValidationErrors = new List<KeyValuePair<string, string>>()
          {
            new KeyValuePair<string, string>("Extension", GalleryResources.ExtensionAlreadyUpdated())
          };
          ExtensionDraftService.PublishDraftCTEvent(requestContext, "ExtensionPublishFromDraftError", extensionDraft, publisherName, extensionName, draftId, (Exception) null);
          return extensionDraft;
        }
        if (string.Equals(extensionDraft.Product, "vs", StringComparison.OrdinalIgnoreCase))
          this.AddVsIdeInstallationTarget(extensionData);
        IExtensionPayloadValidator deploymentTechnology = this._payloadValidatorFactory.GetValidatorForDeploymentTechnology(extensionDraft.Product, extensionDraft.Payload.Type);
        ExtensionDraftAsset extensionDraftAsset = extensionDraft.Assets.Find((Predicate<ExtensionDraftAsset>) (x => x.AssetType == "Microsoft.VisualStudio.Ide.Payload"));
        ExtensionDraftAsset detailsAsset = extensionDraft.Assets.Find((Predicate<ExtensionDraftAsset>) (x => x.AssetType == "Microsoft.VisualStudio.Services.Content.Details"));
        PayloadValidationResult validationResult;
        if (extensionDraftAsset != null && !extensionDraftAsset.IsOldAsset && extensionDraft.Payload.Type == ExtensionDeploymentTechnology.Vsix)
        {
          using (Stream payloadStream = requestContext.GetService<ITeamFoundationFileService>().RetrieveFile(requestContext, (long) extensionDraftAsset.FileId, false, out byte[] _, out long _, out CompressionType _))
            validationResult = deploymentTechnology.ValidatePayloadDetails(requestContext, payloadStream, extensionDraft.Payload.FileName, extensionData, publisher, extension);
        }
        else
          validationResult = deploymentTechnology.ValidatePayloadDetails(requestContext, (Stream) null, extensionDraft.Payload.FileName, extensionData, publisher, extension);
        if (this.ShouldScanOverviewContentForBlockedHosts(requestContext, validationResult.IsValid, detailsAsset, extensionDraft.Product))
          validationResult = deploymentTechnology.ScanOverviewContentForBlockedHosts(requestContext, detailsAsset.FileId, validationResult, extensionData, publisher);
        extensionDraft.ValidationErrors = validationResult.ValidationErrors;
        if (validationResult.IsValid)
        {
          bool isValidationNeeded = true;
          requestContext.Trace(12062075, TraceLevel.Info, "gallery", layer, "ExtensionValidation successful for the extension : {0}", (object) extensionData.ExtensionName);
          IPublishedExtensionService service = requestContext.GetService<IPublishedExtensionService>();
          if (!flag1)
          {
            extensionData = this._extensionDataBuilder.PrepareExtensionDataForCreate(extensionData, validationResult, publisher);
            if (extensionDraft.Payload.Type == ExtensionDeploymentTechnology.Vsix && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableVsixConsolidationForNewVsExtensions"))
              extensionData.Metadata.Add(new KeyValuePair<string, string>("HasConsolidatedVsix", bool.TrueString));
            service.CreateExtensionFromUnpackagedData(requestContext, publisherName, (IEnumerable<ExtensionFile>) extensionDraft.Assets, extensionData);
          }
          else
          {
            extensionData = this._extensionDataBuilder.PrepareExtensionDataForEdit(extensionData, validationResult, extension, publisher);
            this._extensionDraftServiceHelper.ValidateExtensionVersionImmutability(requestContext, extension, extensionData);
            isValidationNeeded = this.IsVsExtensionValidationNeeded(requestContext, extensionDraft);
            bool validationNeeded = !requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableVsExtensionEditOpt") || isValidationNeeded;
            this.CreateNewAssetsforPublish(requestContext, extensionDraft);
            service.UpdateExtensionFromUnpackagedData(requestContext, publisherName, extensionName, (IEnumerable<ExtensionFile>) extensionDraft.Assets, extensionData, validationNeeded);
          }
          extensionDraft.DraftState = DraftStateType.Published;
          extensionDraft.LastUpdated = DateTime.UtcNow;
          using (ExtensionDraftComponent component = requestContext.CreateComponent<ExtensionDraftComponent>())
            component.CreateOrUpdateExtensionDraft(extensionDraft);
          ExtensionDraftService.PublishDraftCTEvent(requestContext, "ExtensionPublishFromDraft", extensionDraft, publisherName, extensionName, draftId, (Exception) null, isValidationNeeded);
        }
        else
        {
          requestContext.Trace(12062075, TraceLevel.Info, "gallery", layer, "ExtensionValidation failed for the extension : {0}", (object) extensionData.ExtensionName);
          ExtensionDraftService.PublishDraftCTEvent(requestContext, "ExtensionPublishFromDraftError", extensionDraft, publisherName, extensionName, draftId, (Exception) null);
        }
        return extensionDraft;
      }
      catch (Exception ex)
      {
        ExtensionDraftService.PublishDraftCTEvent(requestContext, "ExtensionPublishFromDraftError", extensionDraft, publisherName, extensionName, draftId, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(12062075, "gallery", layer, methodName);
      }
    }

    private bool IsVsExtensionValidationNeeded(
      IVssRequestContext requestContext,
      ExtensionDraft draft)
    {
      foreach (ExtensionDraftAsset asset in draft?.Assets)
      {
        if (!asset.IsOldAsset && asset.AssetType != "Microsoft.VisualStudio.Services.Content.Details")
          return true;
      }
      return false;
    }

    private void CreateNewAssetsforPublish(IVssRequestContext requestContext, ExtensionDraft draft)
    {
      ITeamFoundationFileService service = requestContext.GetService<ITeamFoundationFileService>();
      IDictionary<int, int> dictionary = (IDictionary<int, int>) new Dictionary<int, int>();
      foreach (ExtensionDraftAsset asset in draft.Assets)
      {
        if (asset.IsOldAsset)
        {
          int fileId1;
          if (!dictionary.TryGetValue(asset.FileId, out fileId1))
          {
            using (Stream content = service.RetrieveFile(requestContext, (long) asset.FileId, false, out byte[] _, out long _, out CompressionType _))
              fileId1 = service.UploadFile(requestContext, content, OwnerId.Gallery, Guid.Empty);
          }
          int fileId2 = asset.FileId;
          asset.FileId = fileId1;
          asset.IsOldAsset = false;
          dictionary[fileId2] = fileId1;
          using (ExtensionDraftComponent component = requestContext.CreateComponent<ExtensionDraftComponent>())
            component.AddAssetToDraft(draft.Id, asset.AssetType, asset.ContentType, asset.Language, fileId1, false, false, DateTime.UtcNow);
        }
      }
    }

    private void ValidatePayloadReuploadRules(
      ExtensionDeploymentTechnology deploymentTechnology,
      string extensionName,
      ExtensionDraft extensionDraft)
    {
      if (extensionName != null && deploymentTechnology == ExtensionDeploymentTechnology.ReferralLink && extensionDraft.Payload.Type != ExtensionDeploymentTechnology.ReferralLink)
        throw new ArgumentException("Extension payload update is not allowed from Vsix/Msi/Exe to Link for edit extension");
    }

    private void ValidateDraftData(
      ExtensionDraft extensionDraft,
      Publisher publisher,
      PublishedExtension extension,
      Guid userId,
      string publisherName,
      string extensionName,
      Guid draftId,
      UnpackagedExtensionData extensionData)
    {
      bool flag = true;
      string empty = string.Empty;
      if (extensionDraft.Id != draftId)
      {
        flag = false;
        empty += "DraftId should match with the Id in Draft ";
      }
      if (!string.Equals(extensionDraft.PublisherName, publisherName, StringComparison.OrdinalIgnoreCase))
      {
        flag = false;
        empty += "PublisherName should match with the draft ";
      }
      if (!string.Equals(publisher.PublisherName, publisherName, StringComparison.OrdinalIgnoreCase))
      {
        flag = false;
        empty += "PublisherName should match with the publisher ";
      }
      if (extensionDraft.UserId != userId)
      {
        flag = false;
        empty += "User id should match with the userId in draft ";
      }
      if (extensionName != null)
      {
        if (!string.Equals(extensionDraft.ExtensionName, extensionName, StringComparison.OrdinalIgnoreCase))
        {
          flag = false;
          empty += "ExtensionName should match with the draft ";
        }
        if (extension == null)
        {
          flag = false;
          empty += "Extension doesn't exist ";
        }
        else
        {
          if (extensionDraft.ExtensionId != extension.ExtensionId)
          {
            flag = false;
            empty += "ExtensionId should match with the draft ";
          }
          if (!string.Equals(extension.ExtensionName, extensionName, StringComparison.OrdinalIgnoreCase))
          {
            flag = false;
            empty += "Extension's name should be same ";
          }
          if (!string.Equals(extension.Publisher.PublisherName, publisherName, StringComparison.OrdinalIgnoreCase))
          {
            flag = false;
            empty += "Extension's publisher should be same ";
          }
        }
      }
      if (extensionDraft.DraftState != DraftStateType.Unpublished)
      {
        flag = false;
        empty += GalleryResources.ErrorDraftCannotBeUsed();
      }
      if (extensionData != null)
      {
        if (extensionData.DraftId != draftId)
        {
          flag = false;
          empty += "DraftId should match with extension data ";
        }
        if (!string.Equals(extensionData.PublisherName, publisherName, StringComparison.OrdinalIgnoreCase))
        {
          flag = false;
          empty += "PublisherName should match with extension data ";
        }
        if (extensionName != null && !string.Equals(extensionData.ExtensionName, extensionName, StringComparison.OrdinalIgnoreCase))
        {
          flag = false;
          empty += "ExtensionName should match with extension data ";
        }
      }
      if (!flag)
        throw new ArgumentException(empty);
    }

    public ExtensionDraftAsset AddAssetInDraftForNewExtension(
      IVssRequestContext requestContext,
      string publisherName,
      Guid draftId,
      string assetType,
      Stream assetStream)
    {
      return this.AddAssetToDraft(requestContext, publisherName, (string) null, draftId, assetType, assetStream);
    }

    public ExtensionDraftAsset GetAssetFromNewExtensionDraft(
      IVssRequestContext requestContext,
      string publisherName,
      Guid draftId,
      string assetType)
    {
      return this.GetAssetFromDraft(requestContext, publisherName, (string) null, draftId, assetType);
    }

    private ExtensionDraftAsset AddAssetToDraft(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Guid draftId,
      string assetType,
      Stream assetStream)
    {
      ExtensionDraft extensionDraft = (ExtensionDraft) null;
      try
      {
        Guid userId = requestContext.GetUserId();
        GalleryServerUtil.ValidatePackageSize(requestContext, assetStream.Length, 2097152L);
        GalleryUtil.CheckAssetType(assetType);
        Publisher publisher;
        PublishedExtension extension;
        this.PerformCommonValidations(requestContext, publisherName, extensionName, out publisher, out extension);
        extensionDraft = this.FetchDraft(requestContext, draftId, userId);
        this.ValidateDraftData(extensionDraft, publisher, extension, userId, publisherName, extensionName, draftId, (UnpackagedExtensionData) null);
        ExtensionDraftAsset extensionDraftAsset = (ExtensionDraftAsset) null;
        if (!extensionDraft.Assets.IsNullOrEmpty<ExtensionDraftAsset>())
          extensionDraftAsset = extensionDraft.Assets.Find((Predicate<ExtensionDraftAsset>) (x => x.AssetType.Equals(assetType, StringComparison.Ordinal)));
        string contentTypeForAsset = this.GetContentTypeForAsset(assetType);
        ITeamFoundationFileService service = requestContext.GetService<ITeamFoundationFileService>();
        int fileId = service.UploadFile(requestContext, assetStream, OwnerId.Gallery, Guid.Empty);
        using (ExtensionDraftComponent component = requestContext.CreateComponent<ExtensionDraftComponent>())
          component.AddAssetToDraft(draftId, assetType, contentTypeForAsset, (string) null, fileId, false, false, DateTime.UtcNow);
        if (extensionDraftAsset != null && !extensionDraftAsset.IsOldAsset)
          service.DeleteFile(requestContext, (long) extensionDraftAsset.FileId);
        ExtensionDraftService.PublishDraftCTEvent(requestContext, nameof (AddAssetToDraft), extensionDraft, publisherName, extensionName, draftId, (Exception) null);
        return this.GetDraftAsset(requestContext, extensionDraft, assetType, fileId, contentTypeForAsset, (string) null, false, false);
      }
      catch (Exception ex)
      {
        ExtensionDraftService.PublishDraftCTEvent(requestContext, "AddAssetToDraftError", extensionDraft, publisherName, extensionName, draftId, ex);
        throw;
      }
    }

    private ExtensionDraftAsset GetAssetFromDraft(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Guid draftId,
      string assetType)
    {
      Guid userId = requestContext.GetUserId();
      Publisher publisher;
      PublishedExtension extension;
      this.PerformCommonValidations(requestContext, publisherName, extensionName, out publisher, out extension);
      ExtensionDraft extensionDraft = this.FetchDraft(requestContext, draftId, userId);
      this.ValidateDraftData(extensionDraft, publisher, extension, userId, publisherName, extensionName, draftId, (UnpackagedExtensionData) null);
      ExtensionDraftAsset assetFromDraft = (ExtensionDraftAsset) null;
      if (!extensionDraft.Assets.IsNullOrEmpty<ExtensionDraftAsset>())
        assetFromDraft = extensionDraft.Assets.Find((Predicate<ExtensionDraftAsset>) (x => x.AssetType.Equals(assetType, StringComparison.Ordinal)));
      ExtensionDraftService.PublishDraftCTEvent(requestContext, nameof (GetAssetFromDraft), extensionDraft, publisherName, extensionName, draftId, (Exception) null);
      return assetFromDraft;
    }

    private void DeletePayloadAssetsFromDraft(
      IVssRequestContext requestContext,
      ExtensionDraft extensionDraft,
      PayloadValidationResult payloadValidationResult)
    {
      HashSet<string> stringSet = new HashSet<string>();
      int num = 0;
      foreach (ExtensionDraftAsset asset in extensionDraft.Assets)
      {
        if (string.Equals(asset.AssetType, "Microsoft.VisualStudio.Ide.Payload"))
        {
          stringSet.Add("Microsoft.VisualStudio.Ide.Payload");
          num = asset.FileId;
          break;
        }
      }
      foreach (ExtensionDraftAsset asset in extensionDraft.Assets)
      {
        if (!string.Equals(asset.AssetType, "Microsoft.VisualStudio.Ide.Payload") && asset.FileId == num)
        {
          stringSet.Add(asset.AssetType);
          break;
        }
      }
      int? nullable = payloadValidationResult.VsixThumbnailFileId;
      if (nullable.HasValue)
        stringSet.Add("Microsoft.VisualStudio.Services.Icons.Default");
      nullable = payloadValidationResult.VsixScreenShotFileId;
      if (nullable.HasValue)
        stringSet.Add("Microsoft.VisualStudio.Services.Image.Preview");
      if (payloadValidationResult.VsixLocalizedStrings != null)
      {
        foreach (ProjectLocalizedStrings vsixLocalizedString in payloadValidationResult.VsixLocalizedStrings)
        {
          nullable = vsixLocalizedString.LicenseFileId;
          if (nullable.HasValue)
            stringSet.Add("Microsoft.VisualStudio.Services.Content.License");
        }
      }
      HashSet<int> fileIds = new HashSet<int>();
      List<ExtensionDraftAsset> extensionDraftAssetList = new List<ExtensionDraftAsset>();
      if (extensionDraft.Assets != null)
      {
        using (ExtensionDraftComponent component = requestContext.CreateComponent<ExtensionDraftComponent>())
        {
          foreach (ExtensionDraftAsset asset in extensionDraft.Assets)
          {
            if (stringSet.Contains(asset.AssetType) || asset.IsPayloadAsset)
            {
              if (!asset.IsOldAsset)
                fileIds.Add(asset.FileId);
              component.DeleteAssetFromDraft(extensionDraft.Id, asset.AssetType, asset.Language);
              extensionDraftAssetList.Add(asset);
            }
          }
        }
      }
      if (fileIds.Count > 0)
      {
        try
        {
          requestContext.GetService<ITeamFoundationFileService>().DeleteFiles(requestContext, (IEnumerable<int>) fileIds);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(12061114, "Gallery", nameof (ExtensionDraftService), ex);
          throw;
        }
      }
      Extensions.RemoveRange<ExtensionDraftAsset>((ICollection<ExtensionDraftAsset>) extensionDraft.Assets, (IEnumerable<ExtensionDraftAsset>) extensionDraftAssetList);
    }

    public ExtensionDraftAsset AddAssetInDraftForEditExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Guid draftId,
      string assetType,
      Stream assetStream)
    {
      return this.AddAssetToDraft(requestContext, publisherName, extensionName, draftId, assetType, assetStream);
    }

    public ExtensionDraftAsset GetAssetFromEditExtensionDraft(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Guid draftId,
      string assetType)
    {
      return this.GetAssetFromDraft(requestContext, publisherName, extensionName, draftId, assetType);
    }

    private void LinkExistingAssetsToDraft(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      ExtensionDraft extensionDraft)
    {
      if (extension.Versions.IsNullOrEmpty<ExtensionVersion>())
        return;
      List<ExtensionDraftAsset> assets = new List<ExtensionDraftAsset>();
      IDictionary<string, string> assetTypeOldToNewNameMap;
      IDictionary<string, string> assetTypesNeedFix;
      this.ValidateAndFixAssetTypes(extension.Versions[0].Files, out assetTypeOldToNewNameMap, out assetTypesNeedFix);
      bool flag1 = !assetTypesNeedFix.IsNullOrEmpty<KeyValuePair<string, string>>();
      List<ExtensionFile> extensionFileList = new List<ExtensionFile>();
      bool flag2 = this.IsMultipleDetailsOrLicenseAssets(extension.Versions[0].Files);
      foreach (ExtensionFile file in extension.Versions[0].Files)
      {
        string assetType = assetTypeOldToNewNameMap[file.AssetType];
        GalleryUtil.CheckAssetType(assetType);
        bool flag3 = !(file.AssetType == "Microsoft.VisualStudio.Services.Content.Details") && !(file.AssetType == "Microsoft.VisualStudio.Services.Content.License") || !flag2 || file.Language == null;
        if (((!flag1 ? 0 : (file.AssetType == "Microsoft.VisualStudio.Services.Content.Details" ? 1 : 0)) & (flag3 ? 1 : 0)) != 0)
          extensionFileList.Add(file);
        else if (flag3)
        {
          ExtensionDraftAsset draftAsset = this.GetDraftAsset(requestContext, extensionDraft, assetType, file.FileId, file.ContentType, file.Language, true, false);
          assets.Add(draftAsset);
        }
      }
      ITeamFoundationFileService service = requestContext.GetService<ITeamFoundationFileService>();
      foreach (ExtensionFile extensionFile in extensionFileList)
      {
        string assetType = assetTypeOldToNewNameMap[extensionFile.AssetType];
        using (Stream stream = service.RetrieveFile(requestContext, (long) extensionFile.FileId, false, out byte[] _, out long _, out CompressionType _))
        {
          int fileId;
          using (Stream content = (Stream) new MemoryStream(Encoding.UTF8.GetBytes(this.FixAssetReferences(new StreamReader(stream).ReadToEnd(), assetTypesNeedFix))))
            fileId = service.UploadFile(requestContext, content, OwnerId.Gallery, Guid.Empty);
          ExtensionDraftAsset draftAsset = this.GetDraftAsset(requestContext, extensionDraft, assetType, fileId, extensionFile.ContentType, extensionFile.Language, false, false);
          assets.Add(draftAsset);
        }
      }
      extensionDraft.Assets = new List<ExtensionDraftAsset>();
      this.AddAssetsToExistingDraft(requestContext, assets, extensionDraft);
    }

    private void AddPayloadAssetsToDraft(
      IVssRequestContext requestContext,
      Stream payloadStream,
      PayloadValidationResult payloadValidationResult,
      ExtensionDraft extensionDraft)
    {
      List<ExtensionDraftAsset> assets = new List<ExtensionDraftAsset>();
      if (payloadStream != null && payloadStream.Length > 0L)
      {
        int fileId = requestContext.GetService<ITeamFoundationFileService>().UploadFile(requestContext, payloadStream, OwnerId.Gallery, Guid.Empty);
        ExtensionDraftAsset draftAsset1 = this.GetDraftAsset(requestContext, extensionDraft, "Microsoft.VisualStudio.Ide.Payload", fileId, "application/octet-stream", (string) null, false, true);
        assets.Add(draftAsset1);
        ExtensionDraftAsset draftAsset2 = this.GetDraftAsset(requestContext, extensionDraft, extensionDraft.Payload.FileName, fileId, "application/octet-stream", (string) null, false, true);
        assets.Add(draftAsset2);
      }
      int? nullable = payloadValidationResult.VsixThumbnailFileId;
      if (nullable.HasValue)
      {
        IVssRequestContext requestContext1 = requestContext;
        ExtensionDraft extensionDraft1 = extensionDraft;
        nullable = payloadValidationResult.VsixThumbnailFileId;
        int fileId = nullable.Value;
        ExtensionDraftAsset draftAsset = this.GetDraftAsset(requestContext1, extensionDraft1, "Microsoft.VisualStudio.Services.Icons.Default", fileId, "image/png", (string) null, false, true);
        assets.Add(draftAsset);
      }
      nullable = payloadValidationResult.VsixScreenShotFileId;
      if (nullable.HasValue)
      {
        IVssRequestContext requestContext2 = requestContext;
        ExtensionDraft extensionDraft2 = extensionDraft;
        nullable = payloadValidationResult.VsixScreenShotFileId;
        int fileId = nullable.Value;
        ExtensionDraftAsset draftAsset = this.GetDraftAsset(requestContext2, extensionDraft2, "Microsoft.VisualStudio.Services.Image.Preview", fileId, "image/png", (string) null, false, true);
        assets.Add(draftAsset);
      }
      if (payloadValidationResult.VsixLocalizedStrings != null)
      {
        ProjectLocalizedStrings localizedStrings = (ProjectLocalizedStrings) null;
        bool flag = payloadValidationResult.VsixLocalizedStrings.Count<ProjectLocalizedStrings>((Func<ProjectLocalizedStrings, bool>) (x => x.LicenseFileId.HasValue)) > 1;
        foreach (ProjectLocalizedStrings vsixLocalizedString in payloadValidationResult.VsixLocalizedStrings)
        {
          nullable = vsixLocalizedString.LicenseFileId;
          if (nullable.HasValue)
          {
            string languageCodeForLcid = LcidToLanguageMapper.GetLanguageCodeForLcid(vsixLocalizedString.Lcid);
            IVssRequestContext requestContext3 = requestContext;
            ExtensionDraft extensionDraft3 = extensionDraft;
            nullable = vsixLocalizedString.LicenseFileId;
            int fileId = nullable.Value;
            string language = flag ? languageCodeForLcid : (string) null;
            ExtensionDraftAsset draftAsset = this.GetDraftAsset(requestContext3, extensionDraft3, "Microsoft.VisualStudio.Services.Content.License", fileId, "text/plain", language, false, true);
            assets.Add(draftAsset);
            if (localizedStrings == null && payloadValidationResult.VsixPrimaryLanguage == vsixLocalizedString.Lcid)
              localizedStrings = vsixLocalizedString;
          }
        }
        if (localizedStrings != null & flag)
        {
          ExtensionDraftAsset draftAsset = this.GetDraftAsset(requestContext, extensionDraft, "Microsoft.VisualStudio.Services.Content.License", localizedStrings.LicenseFileId.Value, "text/plain", (string) null, false, true);
          assets.Add(draftAsset);
        }
      }
      if (extensionDraft.Assets == null)
        extensionDraft.Assets = new List<ExtensionDraftAsset>();
      this.AddAssetsToExistingDraft(requestContext, assets, extensionDraft);
    }

    private void AddAssetsToExistingDraft(
      IVssRequestContext requestContext,
      List<ExtensionDraftAsset> assets,
      ExtensionDraft extensionDraft)
    {
      using (ExtensionDraftComponent component = requestContext.CreateComponent<ExtensionDraftComponent>())
      {
        foreach (ExtensionDraftAsset asset in assets)
        {
          component.AddAssetToDraft(extensionDraft.Id, asset.AssetType, asset.ContentType, asset.Language, asset.FileId, asset.IsOldAsset, asset.IsPayloadAsset, extensionDraft.LastUpdated);
          extensionDraft.Assets.Add(asset);
        }
      }
    }

    private ExtensionDraft FetchDraft(IVssRequestContext requestContext, Guid draftId, Guid userId)
    {
      IList<ExtensionDraft> allDrafts = this.GetAllDrafts(requestContext, userId);
      ExtensionDraft extensionDraft1 = (ExtensionDraft) null;
      foreach (ExtensionDraft extensionDraft2 in (IEnumerable<ExtensionDraft>) allDrafts)
      {
        if (extensionDraft2.Id == draftId)
        {
          extensionDraft1 = extensionDraft2;
          break;
        }
      }
      if (extensionDraft1 == null)
        throw new ExtensionDraftNotFoundException("Draft Id not found");
      if (extensionDraft1.Assets != null)
      {
        foreach (ExtensionDraftAsset asset in extensionDraft1.Assets)
          asset.Source = this.GetDraftAssetSourceUrl(requestContext, extensionDraft1, asset.AssetType);
      }
      return extensionDraft1;
    }

    public IList<ExtensionDraft> GetAllDrafts(IVssRequestContext requestContext, Guid userId)
    {
      using (ExtensionDraftComponent component = requestContext.CreateComponent<ExtensionDraftComponent>())
        return component.GetExtensionDrafts(userId);
    }

    public void DeleteExtensionDraft(IVssRequestContext requestContext, Guid draftId)
    {
      using (ExtensionDraftComponent component = requestContext.CreateComponent<ExtensionDraftComponent>())
        component.DeleteExtensionDraft(draftId);
    }

    private Publisher FetchPublisher(IVssRequestContext requestContext, string publisherName)
    {
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
        return component.QueryPublisher(publisherName, PublisherQueryFlags.None);
    }

    private void PerformCommonValidations(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      out Publisher publisher,
      out PublishedExtension extension)
    {
      GalleryUtil.CheckPublisherName(publisherName);
      publisher = this.FetchPublisher(requestContext, publisherName);
      if (!extensionName.IsNullOrEmpty<char>())
      {
        GalleryUtil.CheckExtensionName(extensionName);
        IPublishedExtensionService service = requestContext.GetService<IPublishedExtensionService>();
        extension = service.QueryExtension(requestContext, publisherName, extensionName, (string) null, ExtensionQueryFlags.IncludeFiles | ExtensionQueryFlags.IncludeVersionProperties | ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeLatestVersionOnly | ExtensionQueryFlags.IncludeMetadata | ExtensionQueryFlags.IncludeLcids, (string) null);
        if (!extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Validated))
          extension.Versions = (List<ExtensionVersion>) null;
        else if (!extension.Versions[0].Flags.HasFlag((Enum) ExtensionVersionFlags.Validated))
          extension = service.QueryExtension(requestContext, publisherName, extensionName, (string) null, ExtensionQueryFlags.IncludeFiles | ExtensionQueryFlags.IncludeVersionProperties | ExtensionQueryFlags.ExcludeNonValidated | ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeLatestVersionOnly | ExtensionQueryFlags.IncludeMetadata | ExtensionQueryFlags.IncludeLcids, (string) null);
        GallerySecurity.CheckExtensionPermission(requestContext, extension, (string) null, PublisherPermissions.UpdateExtension, false);
      }
      else
      {
        GallerySecurity.CheckPublisherPermission(requestContext, publisher, PublisherPermissions.PublishExtension);
        extension = (PublishedExtension) null;
      }
    }

    private ExtensionDraftAsset GetDraftAsset(
      IVssRequestContext requestContext,
      ExtensionDraft extensionDraft,
      string assetType,
      int fileId,
      string contentType,
      string language,
      bool isOldAsset,
      bool isPayloadAsset)
    {
      ExtensionDraftAsset draftAsset = new ExtensionDraftAsset();
      draftAsset.AssetType = assetType;
      draftAsset.FileId = fileId;
      draftAsset.ContentType = contentType;
      draftAsset.IsOldAsset = isOldAsset;
      draftAsset.IsPayloadAsset = isPayloadAsset;
      draftAsset.Language = language;
      draftAsset.Source = this.GetDraftAssetSourceUrl(requestContext, extensionDraft, assetType);
      return draftAsset;
    }

    private string GetDraftAssetSourceUrl(
      IVssRequestContext requestContext,
      ExtensionDraft extensionDraft,
      string assetType)
    {
      int num = !extensionDraft.ExtensionName.IsNullOrEmpty<char>() ? 1 : 0;
      ILocationService service = requestContext.GetService<ILocationService>();
      Dictionary<string, object> routeValues = new Dictionary<string, object>();
      routeValues["publisherName"] = (object) extensionDraft.PublisherName;
      if (num != 0)
        routeValues["extensionName"] = (object) extensionDraft.ExtensionName;
      routeValues[nameof (assetType)] = (object) assetType;
      routeValues["draftId"] = (object) extensionDraft.Id;
      Guid identifier = num != 0 ? GalleryResourceIds.ExtensionAssetInDraftByExtensionLocationId : GalleryResourceIds.ExtensionAssetInDraftByPublisherLocationId;
      return new UriBuilder(service.GetResourceUri(requestContext, "gallery", identifier, (object) routeValues)).Uri.AbsoluteUri;
    }

    private void AddVsIdeInstallationTarget(UnpackagedExtensionData extensionData)
    {
      if (extensionData.InstallationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (x => string.Equals(x.Target, "Microsoft.VisualStudio.Ide"))))
        return;
      extensionData.InstallationTargets.Add(new InstallationTarget()
      {
        Target = "Microsoft.VisualStudio.Ide",
        TargetVersion = "[0.0, 0.0]"
      });
    }

    private string GetContentTypeForAsset(string assetType)
    {
      string contentTypeForAsset;
      if (this._knownAssetTypeToContentTypeMap.TryGetValue(assetType, out contentTypeForAsset))
        return contentTypeForAsset;
      if (assetType.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) || assetType.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
        return "image/jpeg";
      if (assetType.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
        return "image/png";
      if (assetType.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
        return "image/gif";
      if (assetType.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
        return "image/x-ms-bmp";
      throw new ArgumentException(GalleryResources.UnsupportedFileTypeError());
    }

    private string ValidateProductType(string productType)
    {
      if (string.Equals("vs", productType, StringComparison.OrdinalIgnoreCase))
        return "vs";
      if (string.Equals("vscode", productType, StringComparison.OrdinalIgnoreCase))
        return "vscode";
      if (string.Equals("vsts", productType, StringComparison.OrdinalIgnoreCase))
        return "vsts";
      throw new ArgumentException("Invalid Product type specified");
    }

    public static void PublishDraftCTEvent(
      IVssRequestContext requestContext,
      string eventType,
      ExtensionDraft draft,
      string publisherName,
      string extensionName,
      Guid draftId,
      Exception exception,
      bool isValidationNeeded = true)
    {
      ClientTraceData properties = new ClientTraceData();
      properties.Add("EventType", (object) eventType);
      properties.Add("PublisherNameInput", (object) publisherName);
      properties.Add("ExtensionNameInput", (object) extensionName);
      properties.Add("DraftIdInput", (object) draftId.ToString());
      properties.Add("UserId", (object) requestContext.GetUserId().ToString());
      if (draft != null)
      {
        properties.Add("PublisherName", (object) draft.PublisherName);
        properties.Add("ExtensionName", (object) draft.ExtensionName);
        properties.Add("DraftId", (object) draft.Id.ToString());
        properties.Add("DraftState", (object) draft.DraftState.ToString());
        bool flag = !draft.ExtensionName.IsNullOrEmpty<char>();
        properties.Add("IsEdit", flag ? (object) "true" : (object) "false");
        if (flag)
          properties.Add("IsValidationNeeded", (object) isValidationNeeded);
        if (!draft.ValidationWarnings.IsNullOrEmpty<KeyValuePair<string, string>>())
        {
          List<string> stringList = new List<string>();
          foreach (KeyValuePair<string, string> validationWarning in draft.ValidationWarnings)
            stringList.Add(validationWarning.Key + " : " + validationWarning.Value);
          properties.Add("ValidationWarnings", (object) stringList);
        }
        if (!draft.ValidationErrors.IsNullOrEmpty<KeyValuePair<string, string>>())
        {
          List<string> stringList = new List<string>();
          foreach (KeyValuePair<string, string> validationError in draft.ValidationErrors)
            stringList.Add(validationError.Key + " : " + validationError.Value);
          properties.Add("ValidationErrors", (object) stringList);
        }
      }
      if (exception != null)
      {
        properties.Add("Exception", (object) exception.Message);
        properties.Add("ExceptionStack", (object) exception.StackTrace);
      }
      requestContext.GetService<ClientTraceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "ExtensionDraft", properties);
    }

    private void ValidateAndFixAssetTypes(
      List<ExtensionFile> files,
      out IDictionary<string, string> assetTypeOldToNewNameMap,
      out IDictionary<string, string> assetTypesNeedFix)
    {
      assetTypeOldToNewNameMap = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      int num = 1;
      foreach (ExtensionFile file in files)
        assetTypeOldToNewNameMap[file.AssetType] = file.AssetType;
      assetTypesNeedFix = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      ISet<string> stringSet = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (ExtensionFile file in files)
      {
        string assetType = file.AssetType;
        if (!dictionary.ContainsKey(assetType))
        {
          if (!assetTypesNeedFix.ContainsKey(assetType))
          {
            string str;
            try
            {
              GalleryUtil.CheckAssetType(assetType);
              dictionary[assetType] = assetType;
              stringSet.Add(assetType);
              continue;
            }
            catch (ArgumentException ex)
            {
              str = GalleryUtil.FixAssetType(assetType, '-');
            }
            try
            {
              GalleryUtil.CheckAssetType(str);
              if (stringSet.Contains(str))
                throw new ArgumentException("newAssetType");
            }
            catch (ArgumentException ex)
            {
              string extension = Path.GetExtension(str);
              str = "VS_Gallery_MigratedAsset" + num++.ToString() + extension;
            }
            assetTypesNeedFix[assetType] = str;
            assetTypeOldToNewNameMap[assetType] = str;
            stringSet.Add(str);
          }
        }
      }
    }

    private string FixAssetReferences(
      string originalContent,
      IDictionary<string, string> assetTypesNeedFix)
    {
      string str = originalContent;
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) assetTypesNeedFix)
      {
        string oldValue = "{CURRENT_EXTENSION_ASSET_BASE_PATH}/" + keyValuePair.Key;
        string newValue = "{CURRENT_EXTENSION_ASSET_BASE_PATH}/" + keyValuePair.Value;
        str = str.Replace(oldValue, newValue);
      }
      return str;
    }

    private bool IsMultipleDetailsOrLicenseAssets(List<ExtensionFile> extensionFiles)
    {
      int num1 = 0;
      int num2 = 0;
      foreach (ExtensionFile extensionFile in extensionFiles)
      {
        if (extensionFile.AssetType == "Microsoft.VisualStudio.Services.Content.Details")
          ++num1;
        else if (extensionFile.AssetType == "Microsoft.VisualStudio.Services.Content.License")
          ++num2;
        if (num1 > 1 || num2 > 1)
          return true;
      }
      return false;
    }

    private bool ShouldScanOverviewContentForBlockedHosts(
      IVssRequestContext requestContext,
      bool isPayloadValid,
      ExtensionDraftAsset detailsAsset,
      string productType)
    {
      return requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.ScanOverviewContentForBlockedHosts") & isPayloadValid && detailsAsset != null && string.Equals(productType, "vs", StringComparison.OrdinalIgnoreCase);
    }
  }
}
