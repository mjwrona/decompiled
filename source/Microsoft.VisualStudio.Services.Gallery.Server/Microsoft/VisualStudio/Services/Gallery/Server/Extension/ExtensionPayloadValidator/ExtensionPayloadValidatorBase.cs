// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator.ExtensionPayloadValidatorBase
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.VsGalleryMigration;
using Microsoft.VisualStudio.Services.Gallery.Server.Utility;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator
{
  internal abstract class ExtensionPayloadValidatorBase : IExtensionPayloadValidator
  {
    [StaticSafe("Grandfathered")]
    private static readonly IDictionary<string, VsExtensionType> CategoryToTypeMap = (IDictionary<string, VsExtensionType>) new Dictionary<string, VsExtensionType>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "tools",
        VsExtensionType.Tool
      },
      {
        "templates",
        VsExtensionType.Template
      },
      {
        "controls",
        VsExtensionType.Control
      }
    };

    protected bool PerformCommonValidations(
      IVssRequestContext requestContext,
      PayloadValidationResult payloadValidationResult,
      UnpackagedExtensionData extensionData,
      PublishedExtension existingExtension)
    {
      bool flag1 = true;
      bool flag2 = this.ValidateCategoriesNotEmpty(extensionData, payloadValidationResult) & flag1;
      bool flag3 = this.ValidateCategoriesCountLessThanFour(extensionData, payloadValidationResult) & flag2;
      bool flag4 = this.ValidateOnlyOneRootCategorySelected(requestContext, extensionData, payloadValidationResult) & flag3;
      bool flag5 = this.ValidateExtensionSdkCategoryIsSelected(requestContext, extensionData, payloadValidationResult) & flag4;
      bool flag6 = this.ValidateExtensionPackCategoryIsSelected(requestContext, extensionData, payloadValidationResult) & flag5;
      bool flag7 = this.ValidateTags(extensionData, payloadValidationResult) & flag6;
      bool flag8 = this.ValidateVsixId(requestContext, extensionData.VsixId, payloadValidationResult, existingExtension) & flag7;
      bool flag9 = this.ValidateDescription(extensionData, payloadValidationResult) & flag8;
      return this.ValidateSourceCodeUrl(extensionData, payloadValidationResult) & flag9;
    }

    protected bool PerformMicrosoftEmployeeCheck(
      IVssRequestContext requestContext,
      PayloadValidationResult payloadValidationResult,
      Publisher publisher)
    {
      IFirstPartyPublisherAccessService service = requestContext.GetService<IFirstPartyPublisherAccessService>();
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.MsIdentityCheck") || service.CheckAtMicrosftDotComAccessIfRequired(requestContext, publisher))
        return true;
      payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("UploadFile", GalleryResources.ErrorNotMicrosoftEmployee()));
      return false;
    }

    protected bool PerformMicrosoftExtensionCheck(
      IVssRequestContext requestContext,
      PayloadValidationResult payloadValidationResult,
      Publisher publisher)
    {
      if (!requestContext.GetService<IFirstPartyPublisherAccessService>().IsMicrosoftPublisher(requestContext, publisher) || payloadValidationResult.IsSignedByMicrosoft)
        return true;
      payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("UploadFile", GalleryResources.ErrorMicrosoftExtensionNotSigned()));
      return false;
    }

    protected bool PerformNonVsixValidations(
      PayloadValidationResult payloadValidationResult,
      UnpackagedExtensionData extensionData)
    {
      bool flag1 = this.ValidateTitleIfNotVsix(extensionData, payloadValidationResult);
      bool flag2 = this.ValidateLanguageIfNotVsix(extensionData, payloadValidationResult) & flag1;
      bool flag3 = this.ValidateInstallationTargetsNotEmptyIfNotVsix(extensionData, payloadValidationResult) & flag2;
      return this.ValidateEmptyProductArchitectureForNonVsix(extensionData, payloadValidationResult) & flag3;
    }

    private bool ValidateEmptyProductArchitectureForNonVsix(
      UnpackagedExtensionData extensionData,
      PayloadValidationResult payloadValidationResult)
    {
      if (extensionData.InstallationTargets.IsNullOrEmpty<InstallationTarget>() || !extensionData.InstallationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (it => !it.ProductArchitecture.IsNullOrEmpty<char>())))
        return true;
      payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("versions", GalleryResources.ErrorProductArchitectureForNonVsixVsExtension()));
      return false;
    }

    private bool ValidateOnlyOneRootCategorySelected(
      IVssRequestContext requextContext,
      UnpackagedExtensionData extensionData,
      PayloadValidationResult payloadValidationResult)
    {
      if (!extensionData.Categories.IsNullOrEmpty<string>())
      {
        IPublishedExtensionService service = requextContext.GetService<IPublishedExtensionService>();
        IVssRequestContext requestContext = requextContext;
        List<string> languages = new List<string>();
        languages.Add("en-us");
        string product = extensionData.Product;
        CategoriesResult categoriesResult = service.QueryAvailableCategories(requestContext, (IEnumerable<string>) languages, product: product);
        List<ExtensionCategory> extensionCategoryList = new List<ExtensionCategory>();
        foreach (string category1 in extensionData.Categories)
        {
          bool flag = false;
          foreach (ExtensionCategory category2 in categoriesResult.Categories)
          {
            if (string.Equals(category1, category2.CategoryName, StringComparison.OrdinalIgnoreCase))
            {
              extensionCategoryList.Add(category2);
              flag = true;
              break;
            }
          }
          if (!flag)
          {
            payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("categories", GalleryResources.ErrorRequestedCategoriesNotExist()));
            return false;
          }
        }
        ExtensionCategory category = (ExtensionCategory) null;
        foreach (ExtensionCategory extensionCategory in extensionCategoryList)
        {
          if (category == null)
            category = extensionCategory.Parent;
          else if (category != extensionCategory.Parent)
          {
            payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("categories", GalleryResources.ErrorOnlyOneRootCategoryAllowed()));
            return false;
          }
        }
        if (category == null)
        {
          payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("categories", GalleryResources.ErrorRootCategoryNotFound()));
          return false;
        }
        extensionCategoryList.Add(category);
        extensionData.Categories.Clear();
        foreach (ExtensionCategory extensionCategory in extensionCategoryList)
          extensionData.Categories.Add(extensionCategory.CategoryId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        payloadValidationResult.RootCategory = category;
        payloadValidationResult.ExtensionType = ExtensionPayloadValidatorBase.GetType(category);
      }
      return true;
    }

    private bool ValidateTitleIfNotVsix(
      UnpackagedExtensionData extensionData,
      PayloadValidationResult payloadValidationResult)
    {
      if (string.IsNullOrEmpty(extensionData.DisplayName) || extensionData.DisplayName.Trim().Length == 0)
      {
        payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("displayName", GalleryResources.ErrorDisplayNameRequired()));
        return false;
      }
      if (extensionData.DisplayName.Length <= 80)
        return true;
      payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("displayName", GalleryResources.ErrorDisplayNameTooLong()));
      return false;
    }

    private bool ValidateDescription(
      UnpackagedExtensionData extensionData,
      PayloadValidationResult payloadValidationResult)
    {
      if (string.IsNullOrEmpty(extensionData.Description) || extensionData.Description.Trim().Length == 0)
      {
        payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("shortDesrciption", GalleryResources.ErrorDescriptionRequired()));
        return false;
      }
      if (extensionData.Description.Length <= 280)
        return true;
      payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("shortDesrciption", GalleryResources.ErrorDescriptionMaxLength()));
      return false;
    }

    private bool ValidateCategoriesNotEmpty(
      UnpackagedExtensionData extensionData,
      PayloadValidationResult payloadValidationResult)
    {
      if (!extensionData.Categories.IsNullOrEmpty<string>())
        return true;
      payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("categories", GalleryResources.ErrorCategoriesRequired()));
      return false;
    }

    private bool ValidateInstallationTargetsNotEmptyIfNotVsix(
      UnpackagedExtensionData extensionData,
      PayloadValidationResult payloadValidationResult)
    {
      if (!extensionData.InstallationTargets.IsNullOrEmpty<InstallationTarget>())
        return true;
      payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("versions", GalleryResources.ErrorInstallationTargetsRequired()));
      return false;
    }

    private bool ValidateLanguageIfNotVsix(
      UnpackagedExtensionData extensionData,
      PayloadValidationResult payloadValidationResult)
    {
      if (payloadValidationResult.IsEditMode || !extensionData.Lcids.IsNullOrEmpty<int>())
        return true;
      payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("Language", GalleryResources.ErrorLanguageRequired()));
      return false;
    }

    private bool ValidateCategoriesCountLessThanFour(
      UnpackagedExtensionData extensionData,
      PayloadValidationResult payloadValidationResult)
    {
      if (extensionData.Categories == null || extensionData.Categories.Count <= 3)
        return true;
      payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("categories", GalleryResources.ErrorCategoriesRequired()));
      return false;
    }

    private bool ValidateExtensionSdkCategoryIsSelected(
      IVssRequestContext requextContext,
      UnpackagedExtensionData extensionData,
      PayloadValidationResult payloadValidationResult)
    {
      if (payloadValidationResult.IsExtensionSdk && extensionData.Categories != null)
      {
        ExtensionCategory category = this.GetCategory(requextContext, extensionData, "ExtensionSDK");
        if (category != null && extensionData.Categories.FirstOrDefault<string>((Func<string, bool>) (x => x == category.CategoryId.ToString((IFormatProvider) CultureInfo.InvariantCulture))) == null)
        {
          payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("categories", GalleryResources.ErrorExtensionSdkCategoryRequired()));
          return false;
        }
      }
      return true;
    }

    private bool ValidateExtensionPackCategoryIsSelected(
      IVssRequestContext requextContext,
      UnpackagedExtensionData extensionData,
      PayloadValidationResult payloadValidationResult)
    {
      if (extensionData.Categories != null)
      {
        ExtensionCategory category = this.GetCategory(requextContext, extensionData, "extensionpacks");
        if (category != null)
        {
          if (extensionData.Categories.FirstOrDefault<string>((Func<string, bool>) (x => string.Equals(x, category.CategoryId.ToString(), StringComparison.OrdinalIgnoreCase))) != null)
          {
            if (!payloadValidationResult.IsExtensionPack)
            {
              payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("categories", GalleryResources.ErrorExtensionPacksCategoryNotSupported()));
              return false;
            }
          }
          else if (payloadValidationResult.IsExtensionPack)
          {
            payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("categories", GalleryResources.ErrorExtensionPacksCategoryRequired()));
            return false;
          }
        }
      }
      return true;
    }

    private bool ValidateTags(
      UnpackagedExtensionData extensionData,
      PayloadValidationResult payloadValidationResult)
    {
      if (extensionData.Tags == null)
        return true;
      foreach (string tag in extensionData.Tags)
      {
        string str = tag.Trim();
        if (str.Length < 1)
        {
          payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("selectedTagsList", GalleryResources.ErrorTagEmpty()));
          return false;
        }
        if (str.Length > 50)
        {
          payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("selectedTagsList", GalleryResources.ErrorTagTooLong((object) 50)));
          return false;
        }
      }
      return true;
    }

    private bool ValidateSourceCodeUrl(
      UnpackagedExtensionData extensionData,
      PayloadValidationResult payloadValidationResult)
    {
      Uri result;
      if (extensionData.RepositoryUrl.IsNullOrEmpty<char>() || (!Uri.TryCreate(extensionData.RepositoryUrl, UriKind.Absolute, out result) ? 0 : (result.Scheme == Uri.UriSchemeHttp ? 1 : (result.Scheme == Uri.UriSchemeHttps ? 1 : 0))) != 0)
        return true;
      payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("sourceRepoURL", GalleryResources.ErrorInvalidSourceCodeUrl()));
      return false;
    }

    private static VsExtensionType GetType(ExtensionCategory category)
    {
      VsExtensionType type;
      if (ExtensionPayloadValidatorBase.CategoryToTypeMap.TryGetValue(category.CategoryName, out type))
        return type;
      throw new ArgumentException("Invalid root category name");
    }

    private ExtensionCategory GetCategory(
      IVssRequestContext requextContext,
      UnpackagedExtensionData extensionData,
      string categoryName)
    {
      IPublishedExtensionService service = requextContext.GetService<IPublishedExtensionService>();
      IVssRequestContext requestContext = requextContext;
      List<string> languages = new List<string>();
      languages.Add("en-us");
      string product = extensionData.Product;
      foreach (ExtensionCategory category in service.QueryAvailableCategories(requestContext, (IEnumerable<string>) languages, product: product).Categories)
      {
        if (string.Equals(categoryName, category.CategoryName, StringComparison.OrdinalIgnoreCase))
          return category;
      }
      return (ExtensionCategory) null;
    }

    private bool ValidateVsixId(
      IVssRequestContext requestContext,
      string vsixId,
      PayloadValidationResult payloadValidationResult,
      PublishedExtension existingExtension)
    {
      if (vsixId != null && vsixId.Trim() != vsixId)
      {
        payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>(nameof (vsixId), GalleryResources.ErrorNonTruncatedVsixId()));
        return false;
      }
      return string.IsNullOrEmpty(vsixId) || payloadValidationResult.DeploymentTechnology == ExtensionDeploymentTechnology.Vsix || !this.IsInvalidVsixId(requestContext, vsixId, payloadValidationResult, existingExtension);
    }

    protected bool IsInvalidVsixId(
      IVssRequestContext requestContext,
      string vsixId,
      PayloadValidationResult payloadValidationResult,
      PublishedExtension existingExtension)
    {
      if (existingExtension != null)
      {
        string enumerable = (string) null;
        foreach (ExtensionMetadata extensionMetadata in existingExtension.Metadata)
        {
          if (string.Equals(extensionMetadata.Key, "VsixId", StringComparison.OrdinalIgnoreCase))
          {
            enumerable = extensionMetadata.Value;
            break;
          }
        }
        if (!enumerable.IsNullOrEmpty<char>())
        {
          if (vsixId.Equals(enumerable, StringComparison.OrdinalIgnoreCase))
            return false;
          payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>(nameof (vsixId), GalleryResources.ErrorVsixIdCannotBeChanged()));
          return true;
        }
      }
      IVsixIdManagerService service = requestContext.GetService<IVsixIdManagerService>();
      if (service.IsExistingVsixId(requestContext, vsixId))
      {
        payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>(nameof (vsixId), GalleryResources.ErrorDuplicateVsixId()));
        return true;
      }
      if (!service.IsReservedVsixId(requestContext, vsixId, ReservedVsixIdPurposeType.Reserved))
        return false;
      payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>(nameof (vsixId), GalleryResources.ErrorDuplicateVsixId()));
      return true;
    }

    public PayloadValidationResult ScanOverviewContentForBlockedHosts(
      IVssRequestContext requestContext,
      int detailsFileId,
      PayloadValidationResult payloadValidation,
      UnpackagedExtensionData extensionData,
      Publisher publisher)
    {
      string fileContent = GalleryServerUtil.GetFileContent(requestContext, detailsFileId);
      try
      {
        if (!AntiSpamService.IsKnownGenuinePublisher(requestContext, publisher))
        {
          if (AntiSpamService.ContentHasBlockedHosts(requestContext, fileContent, extensionData, publisher))
          {
            payloadValidation.ValidationErrors.Add(new KeyValuePair<string, string>("ExtensionHasBlockedHosts", GalleryResources.ExtensionContainsBlockedHosts()));
            payloadValidation.IsValid = false;
            return payloadValidation;
          }
        }
      }
      catch (RegexMatchTimeoutException ex)
      {
        payloadValidation.ValidationErrors.Add(new KeyValuePair<string, string>("ExtensionOverviewScanTimedOut", GalleryResources.ExtensionOverviewScanTimedOut()));
        payloadValidation.IsValid = false;
      }
      return payloadValidation;
    }

    public abstract PayloadValidationResult ValidatePayloadDetails(
      IVssRequestContext requestContext,
      Stream payloadStream,
      string fileName,
      UnpackagedExtensionData extensionData,
      Publisher publisher,
      PublishedExtension existingExtension);

    public abstract PayloadValidationResult ValidatePayload(
      IVssRequestContext requestContext,
      Stream payloadStream,
      string fileName,
      Publisher publisher,
      PublishedExtension existingExtension);

    public abstract ExtensionPayload GetPayloadFromValidationResult(
      PayloadValidationResult payloadValidationResult);
  }
}
