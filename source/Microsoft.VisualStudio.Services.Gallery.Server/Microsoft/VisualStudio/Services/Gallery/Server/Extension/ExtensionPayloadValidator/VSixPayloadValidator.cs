// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator.VSixPayloadValidator
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using VSIXValidationLibrary;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator
{
  internal class VSixPayloadValidator : ExtensionPayloadValidatorBase
  {
    private IVSIXValidator _vsixValidator;
    private bool _ignoreVsixWarnings;
    private IconExtractor _iconExtractor;
    public static readonly Regex VsixValidatorSupportedVsEditions = new Regex("(?<VersionRange>[\\[\\(][0-9\\.\\,]*[\\]\\)]|[0-9\\.]*),?(?<EditionName>[\\w\\.]*),?(?<ProductArchitecture>[\\w]*)");
    private const string _serviceLayer = "VSixPayloadValidator";
    private readonly ExtensionDeploymentTechnology _supportedDeploymentTechnology;

    public VSixPayloadValidator(ExtensionDeploymentTechnology deploymentTechnology)
    {
      this._iconExtractor = new IconExtractor();
      this._supportedDeploymentTechnology = deploymentTechnology;
    }

    internal VSixPayloadValidator(
      ExtensionDeploymentTechnology deploymentTechnology,
      IVSIXValidator vsixValidator,
      IconExtractor iconExtractor)
    {
      this._vsixValidator = vsixValidator;
      this._iconExtractor = iconExtractor;
      this._supportedDeploymentTechnology = deploymentTechnology;
    }

    public override ExtensionPayload GetPayloadFromValidationResult(
      PayloadValidationResult payloadValidationResult)
    {
      ExtensionPayload validationResult = new ExtensionPayload();
      validationResult.FileName = payloadValidationResult.FileName;
      validationResult.Type = payloadValidationResult.DeploymentTechnology;
      validationResult.InstallationTargets = payloadValidationResult.InstallationTargets;
      validationResult.IsSignedByMicrosoft = payloadValidationResult.IsSignedByMicrosoft;
      validationResult.IsValid = payloadValidationResult.IsValid;
      validationResult.IsPreview = payloadValidationResult.IsPreview;
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) payloadValidationResult.VsixMetadata)
        keyValuePairList.Add(new KeyValuePair<string, string>(keyValuePair.Key, keyValuePair.Value ?? string.Empty));
      validationResult.Metadata = keyValuePairList;
      foreach (ProjectLocalizedStrings vsixLocalizedString in payloadValidationResult.VsixLocalizedStrings)
      {
        if (vsixLocalizedString.Lcid == payloadValidationResult.VsixPrimaryLanguage)
        {
          validationResult.DisplayName = vsixLocalizedString.Title;
          validationResult.Description = vsixLocalizedString.Summary;
        }
      }
      return validationResult;
    }

    public override PayloadValidationResult ValidatePayloadDetails(
      IVssRequestContext requestContext,
      Stream payloadStream,
      string fileName,
      UnpackagedExtensionData extensionData,
      Publisher publisher,
      PublishedExtension existingExtension)
    {
      PayloadValidationResult validationResult = this.ValidatePayloadInternal(requestContext, payloadStream, fileName, false, publisher, existingExtension);
      if (validationResult.IsValid)
      {
        extensionData.Lcids = new List<int>();
        if (validationResult.PayloadVerificationSkipped)
        {
          extensionData.Lcids = existingExtension.Lcids;
        }
        else
        {
          foreach (ProjectLocalizedStrings vsixLocalizedString in validationResult.VsixLocalizedStrings)
            extensionData.Lcids.Add(vsixLocalizedString.Lcid);
        }
        validationResult.ValidationErrors = new List<KeyValuePair<string, string>>();
        bool flag = this.PerformCommonValidations(requestContext, validationResult, extensionData, existingExtension) && this.ValidateCorrectCateogryType(validationResult, existingExtension);
        if (flag && requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.CheckForExtensionSpam") && !AntiSpamService.IsKnownGenuinePublisher(requestContext, publisher))
        {
          requestContext.Trace(12062075, TraceLevel.Info, "gallery", nameof (VSixPayloadValidator), "ExtensionSpamWords validation started for the extensions : {0}", (object) extensionData.ExtensionName);
          if (AntiSpamService.ExtensionHasSuspectedSpamContent(requestContext, extensionData, publisher))
          {
            validationResult.ValidationErrors.Add(new KeyValuePair<string, string>("extensionHasSuspiciousContent", GalleryResources.ExtensionMetadataHasSuspiciousContent()));
            flag = false;
          }
        }
        validationResult.IsValid = flag;
      }
      return validationResult;
    }

    private bool ValidateCorrectCateogryType(
      PayloadValidationResult validationResult,
      PublishedExtension existingExtension)
    {
      string str = !validationResult.PayloadVerificationSkipped ? validationResult.VsixMetadata["Type"] : existingExtension.Metadata.Find((Predicate<ExtensionMetadata>) (x => x.Key == "Type")).Value;
      if ((validationResult.ExtensionType != VsExtensionType.Tool || !str.Equals("ToolboxControl") && !str.EndsWith("Template", StringComparison.Ordinal)) && (validationResult.ExtensionType != VsExtensionType.Control || str.Equals("ToolboxControl")) && (validationResult.ExtensionType != VsExtensionType.Template || str.EndsWith("Template", StringComparison.Ordinal)))
        return true;
      if (str.Equals("ToolboxControl", StringComparison.OrdinalIgnoreCase))
        validationResult.ValidationErrors.Add(new KeyValuePair<string, string>("categories", GalleryResources.ValidationMessageForIncorrectVsixTypeShouldBeControl()));
      else if (str.EndsWith("Template", StringComparison.OrdinalIgnoreCase))
        validationResult.ValidationErrors.Add(new KeyValuePair<string, string>("categories", GalleryResources.ValidationMessageForIncorrectVsixTypeShouldBeTemplate()));
      else
        validationResult.ValidationErrors.Add(new KeyValuePair<string, string>("categories", GalleryResources.ValidationMessageForIncorrectVsixTypeShouldBeTool()));
      return false;
    }

    public override PayloadValidationResult ValidatePayload(
      IVssRequestContext requestContext,
      Stream payloadStream,
      string fileName,
      Publisher publisher,
      PublishedExtension existingExtension)
    {
      return this.ValidatePayloadInternal(requestContext, payloadStream, fileName, true, publisher, existingExtension);
    }

    private PayloadValidationResult ValidatePayloadInternal(
      IVssRequestContext requestContext,
      Stream payloadStream,
      string fileName,
      bool uploadAssets,
      Publisher publisher,
      PublishedExtension existingExtension)
    {
      PayloadValidationResult payloadValidationResult = new PayloadValidationResult();
      payloadValidationResult.FileName = fileName;
      payloadValidationResult.DeploymentTechnology = this._supportedDeploymentTechnology;
      if (payloadStream == null && existingExtension != null)
      {
        payloadValidationResult.PayloadVerificationSkipped = true;
        payloadValidationResult.IsValid = true;
        payloadValidationResult.IsExtensionSdk = VSixPayloadValidator.IsExtensionSdk((IDictionary<string, string>) null, existingExtension.Metadata);
        payloadValidationResult.IsPreview = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnablePreviewSupportForVS") && VSixPayloadValidator.IsExtensionInPreview((IDictionary<string, string>) null, existingExtension.Metadata);
        payloadValidationResult.IsExtensionPack = VSixPayloadValidator.IsExtensionPack((IDictionary<string, string>) null, existingExtension.Metadata);
        return payloadValidationResult;
      }
      IVSIXResult result = (this._vsixValidator != null ? this._vsixValidator : (IVSIXValidator) new VSIXValidator()).Validate(payloadStream);
      payloadValidationResult.VsixValidationErrors = result.PackageOutputs.Where<ValidationOutput>((Func<ValidationOutput, bool>) (x => x.Severity == null && x.OutputType != 42)).Select<ValidationOutput, VsixValidationError>((Func<ValidationOutput, VsixValidationError>) (x => new VsixValidationError(x.Message, x.Error, x.OutputType.ToString()))).ToList<VsixValidationError>();
      if (!this._ignoreVsixWarnings)
      {
        payloadValidationResult.VsixValidationWarnings = result.PackageOutputs.Where<ValidationOutput>((Func<ValidationOutput, bool>) (x => x.Severity == 1)).Select<ValidationOutput, VsixValidationWarning>((Func<ValidationOutput, VsixValidationWarning>) (x => new VsixValidationWarning(x.Message, x.OutputType.ToString(), x.ID.ToString()))).ToList<VsixValidationWarning>();
        if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableOldPiaWarning"))
          payloadValidationResult.VsixValidationWarnings.RemoveAll((Predicate<VsixValidationWarning>) (x => x.ErrorType.ToString().Equals("VsixIncompatible")));
      }
      if (requestContext.GetService<IFirstPartyPublisherAccessService>().IsMicrosoftEmployee(requestContext, publisher, existingExtension) && payloadValidationResult.VsixValidationErrors.Count<VsixValidationError>() == 1 && "InvalidTypeTargetingExpress".Equals(payloadValidationResult.VsixValidationErrors.First<VsixValidationError>().ErrorType))
      {
        payloadValidationResult.VsixValidationErrors = new List<VsixValidationError>();
        IVsixIdManagerService service = requestContext.GetService<IVsixIdManagerService>();
        if (result.PackageMetadata != null && !service.IsReservedVsixId(requestContext, result.PackageMetadata["VsixId"], ReservedVsixIdPurposeType.AllowedForVsExpress))
          payloadValidationResult.VsixValidationErrors.Add(new VsixValidationError("InvalidVsixIdTargetingExpress"));
      }
      if (existingExtension == null && result.PackageMetadata != null && !string.Equals(publisher.DisplayName, result.PackageMetadata["Author"], StringComparison.OrdinalIgnoreCase))
        payloadValidationResult.VsixValidationErrors.Add(new VsixValidationError(GalleryResources.AuthorNamePublisherDisplayNameMismatch((object) publisher.DisplayName, (object) result.PackageMetadata["Author"]), (Exception) null, (string) null));
      if (!payloadValidationResult.VsixValidationErrors.Any<VsixValidationError>())
      {
        this.SetMetadataAndLanguage(payloadValidationResult, result);
        payloadValidationResult.IsExtensionSdk = VSixPayloadValidator.IsExtensionSdk(payloadValidationResult.VsixMetadata, (List<ExtensionMetadata>) null);
        payloadValidationResult.IsExtensionPack = VSixPayloadValidator.IsExtensionPack(payloadValidationResult.VsixMetadata, (List<ExtensionMetadata>) null);
        payloadValidationResult.IsPreview = VSixPayloadValidator.IsExtensionInPreview(payloadValidationResult.VsixMetadata, (List<ExtensionMetadata>) null);
        if (uploadAssets)
        {
          if (result.Icon != null)
            this.BuildThumbnail(requestContext, payloadValidationResult, result);
          if (result.PreviewImage != null)
            this.BuildScreenshot(requestContext, payloadValidationResult, result);
          this.BuildLicenses(requestContext, payloadValidationResult);
        }
      }
      payloadValidationResult.ValidationErrors = new List<KeyValuePair<string, string>>();
      payloadValidationResult.ValidationWarnings = new List<KeyValuePair<string, string>>();
      payloadValidationResult.IsValid = this.ValidateVsixData(requestContext, payloadValidationResult, publisher, existingExtension);
      payloadStream.Seek(0L, SeekOrigin.Begin);
      return payloadValidationResult;
    }

    private bool ValidateVsixData(
      IVssRequestContext requestContext,
      PayloadValidationResult payloadValidationResult,
      Publisher publisher,
      PublishedExtension existingExtension)
    {
      if (payloadValidationResult.VsixValidationErrors.Any<VsixValidationError>())
      {
        payloadValidationResult.VsixValidationErrors.ForEach((Action<VsixValidationError>) (error => payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("UploadFile", this.GetVsixValidationErrorMessage(error)))));
        return false;
      }
      if (payloadValidationResult.VsixValidationWarnings.Any<VsixValidationWarning>())
        payloadValidationResult.VsixValidationWarnings.ForEach((Action<VsixValidationWarning>) (warning => payloadValidationResult.ValidationWarnings.Add(new KeyValuePair<string, string>(warning.MessageId, warning.Message))));
      if (payloadValidationResult.VsixId != null && payloadValidationResult.VsixId.Trim() != payloadValidationResult.VsixId)
      {
        payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("vsixId", GalleryResources.ErrorNonTruncatedVsixId()));
        return false;
      }
      if (!this.PerformMicrosoftEmployeeCheck(requestContext, payloadValidationResult, publisher) || !this.PerformMicrosoftExtensionCheck(requestContext, payloadValidationResult, publisher) || this.IsInvalidVsixId(requestContext, payloadValidationResult.VsixId, payloadValidationResult, existingExtension))
        return false;
      foreach (ProjectLocalizedStrings vsixLocalizedString in payloadValidationResult.VsixLocalizedStrings)
      {
        if (string.IsNullOrEmpty(vsixLocalizedString.Summary))
        {
          payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("shortDesrciption", GalleryResources.ErrorDescriptionRequired()));
          return false;
        }
        if (vsixLocalizedString.Summary.Length > 280)
        {
          payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("shortDesrciption", GalleryResources.ErrorDescriptionMaxLength()));
          return false;
        }
      }
      return true;
    }

    private void SetMetadataAndLanguage(
      PayloadValidationResult payloadValidationResult,
      IVSIXResult result)
    {
      payloadValidationResult.VsixPrimaryLanguage = result.LocalizedMetadata.First<KeyValuePair<CultureInfo, ILocalizedMetadata>>((Func<KeyValuePair<CultureInfo, ILocalizedMetadata>, bool>) (x => x.Value.IsPrimary)).Key.LCID;
      payloadValidationResult.VsixId = result.PackageMetadata?["VsixId"];
      VSixPayloadValidator.BuildLocalizedMetadata(payloadValidationResult, result);
      payloadValidationResult.VsixMetadata = (IDictionary<string, string>) this.BuildVsixMetadata(result);
      payloadValidationResult.InstallationTargets = this.BuildInstallationTargets(payloadValidationResult.VsixMetadata);
      payloadValidationResult.IsSignedByMicrosoft = result.SignedByMicrosoft;
    }

    private Dictionary<string, string> BuildVsixMetadata(IVSIXResult result)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) result.PackageMetadata)
      {
        if (keyValuePair.Key.Equals("SupportedVSEditions", StringComparison.OrdinalIgnoreCase))
          dictionary.Add("SupportedVSEditions", this.BuildSupportedVSEditionsValue(keyValuePair.Value));
        else if (keyValuePair.Key.Equals("Type", StringComparison.OrdinalIgnoreCase))
        {
          string str1 = "Microsoft.VisualStudio.";
          string str2 = keyValuePair.Value;
          if (str2.StartsWith(str1, StringComparison.OrdinalIgnoreCase))
            str2 = str2.Substring(str1.Length);
          dictionary.Add("Type", str2);
        }
        else
          dictionary.Add(keyValuePair.Key, keyValuePair.Value);
      }
      foreach (KeyValuePair<CultureInfo, ILocalizedMetadata> keyValuePair in (IEnumerable<KeyValuePair<CultureInfo, ILocalizedMetadata>>) result.LocalizedMetadata)
      {
        if (string.Equals(keyValuePair.Key.ToString(), "en-us", StringComparison.OrdinalIgnoreCase))
        {
          if (keyValuePair.Value.MoreInfoUrl != (Uri) null)
            dictionary.Add("MoreInfoUrl", keyValuePair.Value.MoreInfoUrl.ToString());
          if (keyValuePair.Value.ReleaseNotes != (Uri) null)
            dictionary.Add("ReleaseNotes", keyValuePair.Value.ReleaseNotes.ToString());
        }
      }
      return dictionary;
    }

    private string BuildSupportedVSEditionsValue(string value) => string.Join(";", ((IEnumerable<string>) value.Split(new char[1]
    {
      ';'
    }, StringSplitOptions.RemoveEmptyEntries)).Distinct<string>()) + ";";

    private static void BuildLocalizedMetadata(
      PayloadValidationResult payloadValidationResult,
      IVSIXResult result)
    {
      payloadValidationResult.VsixLocalizedStrings = new List<ProjectLocalizedStrings>();
      foreach (KeyValuePair<CultureInfo, ILocalizedMetadata> keyValuePair in (IEnumerable<KeyValuePair<CultureInfo, ILocalizedMetadata>>) result.LocalizedMetadata)
      {
        string str = (string) null;
        if (keyValuePair.Value.License != null)
        {
          using (Stream license = keyValuePair.Value.License)
            str = new StreamReader(license).ReadToEnd();
        }
        payloadValidationResult.VsixLocalizedStrings.Add(new ProjectLocalizedStrings()
        {
          Title = keyValuePair.Value.Name,
          Summary = VSixPayloadValidator.TruncateSummary(keyValuePair.Value.Description),
          License = str,
          Lcid = keyValuePair.Key.LCID
        });
      }
    }

    private static string TruncateSummary(string summary) => !string.IsNullOrEmpty(summary) && summary.Length > 280 ? summary.Substring(0, 277) + "..." : summary;

    private void BuildLicenses(
      IVssRequestContext requestContext,
      PayloadValidationResult payloadValidationResult)
    {
      ITeamFoundationFileService service = requestContext.GetService<ITeamFoundationFileService>();
      foreach (ProjectLocalizedStrings vsixLocalizedString in payloadValidationResult.VsixLocalizedStrings)
      {
        if (!vsixLocalizedString.License.IsNullOrEmpty<char>())
        {
          using (Stream content = (Stream) new MemoryStream(Encoding.UTF8.GetBytes(this.GetLicenseText(vsixLocalizedString.License))))
            vsixLocalizedString.LicenseFileId = new int?(service.UploadFile(requestContext, content, OwnerId.Gallery, Guid.Empty));
        }
      }
    }

    private string GetLicenseText(string license)
    {
      if (!license.StartsWith("{\\rtf", StringComparison.OrdinalIgnoreCase))
        return license;
      using (RichTextBox richTextBox = new RichTextBox())
      {
        richTextBox.Rtf = license;
        return richTextBox.Text;
      }
    }

    private void BuildScreenshot(
      IVssRequestContext requestContext,
      PayloadValidationResult payloadValidationResult,
      IVSIXResult result)
    {
      using (Stream content = this.Convert(result.PreviewImage, ImageFormat.Png))
      {
        ITeamFoundationFileService service = requestContext.GetService<ITeamFoundationFileService>();
        payloadValidationResult.VsixScreenShotFileId = new int?(service.UploadFile(requestContext, content, OwnerId.Gallery, Guid.Empty));
      }
    }

    public Stream ConvertImageToStream(Image image, ImageFormat format)
    {
      try
      {
        Image image1 = image;
        if (!image.RawFormat.Equals((object) format))
        {
          image1 = (Image) new Bitmap(image.Width, image.Height);
          using (Graphics graphics = Graphics.FromImage(image1))
            graphics.DrawImage(image, 0, 0);
        }
        string str = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".tmp");
        image1.Save(str, format);
        image1.Dispose();
        return (Stream) File.OpenRead(str);
      }
      catch (Exception ex)
      {
        using (Bitmap bitmap = new Bitmap(image.Width, image.Height))
        {
          MemoryStream stream = new MemoryStream();
          bitmap.Save((Stream) stream, format);
          stream.Position = 0L;
          return (Stream) stream;
        }
      }
    }

    public ImageFormat GetFormat(Stream stream) => Image.FromStream(stream).RawFormat;

    public Stream Convert(Stream image, ImageFormat format) => this.ConvertImageToStream(Image.FromStream(image), format);

    private void BuildThumbnail(
      IVssRequestContext requestContext,
      PayloadValidationResult payloadValidationResult,
      IVSIXResult result)
    {
      Stream content = !(this.GetFormat(result.Icon).Guid == ImageFormat.Icon.Guid) ? this.Convert(result.Icon, ImageFormat.Png) : this.ConvertImageToStream((Image) this._iconExtractor.GetLargestIconImage(result.Icon), ImageFormat.Png);
      ITeamFoundationFileService service = requestContext.GetService<ITeamFoundationFileService>();
      payloadValidationResult.VsixThumbnailFileId = new int?(service.UploadFile(requestContext, content, OwnerId.Gallery, Guid.Empty));
      content.Dispose();
    }

    private List<InstallationTarget> BuildInstallationTargets(
      IDictionary<string, string> packageMetadata)
    {
      KeyValuePair<string, string> keyValuePair = packageMetadata.FirstOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (x => x.Key.Equals("SupportedVSEditions", StringComparison.OrdinalIgnoreCase)));
      List<InstallationTarget> installationTargetList = new List<InstallationTarget>();
      if (string.IsNullOrEmpty(keyValuePair.Value))
        return installationTargetList;
      string str1 = keyValuePair.Value;
      char[] separator = new char[1]{ ';' };
      foreach (string input in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
      {
        Match match = VSixPayloadValidator.VsixValidatorSupportedVsEditions.Match(input);
        string versionRangeForDev15 = this.GetModifiedVersionRangeForDev15(match.Groups["VersionRange"].Value);
        string str2 = match.Groups["EditionName"].Value;
        string str3 = match.Groups["ProductArchitecture"].Value;
        installationTargetList.Add(new InstallationTarget()
        {
          Target = str2,
          TargetVersion = versionRangeForDev15,
          ProductArchitecture = str3
        });
      }
      return installationTargetList;
    }

    private string GetModifiedVersionRangeForDev15(string targetVersion)
    {
      string[] strArray = targetVersion.Substring(1, targetVersion.Length - 2).Trim().Split(',');
      if ((strArray.Length == 1 ? strArray[0] : strArray[1]).Trim().Split('.').Length == 2 && targetVersion.Contains("15.0]".ToUpperInvariant()))
        targetVersion = targetVersion.Replace("15.0]", "15]");
      return targetVersion;
    }

    private static bool IsExtensionSdk(
      IDictionary<string, string> vsixMetadata,
      List<ExtensionMetadata> existingExtensionMetadata)
    {
      if (vsixMetadata == null)
      {
        ExtensionMetadata extensionMetadata = existingExtensionMetadata?.Find((Predicate<ExtensionMetadata>) (x => x.Key == "Type"));
        if (extensionMetadata != null && extensionMetadata.Value == "Microsoft.ExtensionSDK")
          return true;
      }
      else if (vsixMetadata.ContainsKey("Type") && vsixMetadata["Type"] == "Microsoft.ExtensionSDK")
        return true;
      return false;
    }

    private static bool IsExtensionInPreview(
      IDictionary<string, string> vsixMetadata,
      List<ExtensionMetadata> existingExtensionMetadata)
    {
      bool result = false;
      if (vsixMetadata == null)
      {
        ExtensionMetadata extensionMetadata = existingExtensionMetadata?.Find((Predicate<ExtensionMetadata>) (x => x.Key == "Preview"));
        if (extensionMetadata != null && bool.TryParse(extensionMetadata.Value, out result))
          return result;
      }
      else if (vsixMetadata.ContainsKey("Preview") && bool.TryParse(vsixMetadata["Preview"], out result))
        return result;
      return false;
    }

    private static bool IsExtensionPack(
      IDictionary<string, string> vsixMetadata,
      List<ExtensionMetadata> existingExtensionMetadata)
    {
      return vsixMetadata == null ? existingExtensionMetadata?.Find((Predicate<ExtensionMetadata>) (x => x.Key == "PackedExtensionsVsixIDs")) != null : vsixMetadata.ContainsKey("PackedExtensionsVsixIDs");
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "We need switch case here to return different vsixValidationError messages")]
    private string GetVsixValidationErrorMessage(VsixValidationError vsixValidationError)
    {
      string str;
      if (!vsixValidationError.Message.IsNullOrEmpty<char>())
      {
        str = vsixValidationError.Message;
      }
      else
      {
        string errorType = vsixValidationError.ErrorType;
        if (errorType != null)
        {
          switch (errorType.Length)
          {
            case 11:
              switch (errorType[7])
              {
                case 'E':
                  if (errorType == "MissingEULA")
                  {
                    str = GalleryResources.ErrorMissingEULA();
                    goto label_54;
                  }
                  else
                    goto label_53;
                case 'I':
                  if (errorType == "MissingIcon")
                  {
                    str = GalleryResources.ErrorMissingIcon();
                    goto label_54;
                  }
                  else
                    goto label_53;
                default:
                  goto label_53;
              }
            case 16:
              switch (errorType[7])
              {
                case 'B':
                  if (errorType == "PathMayBeTooLong")
                  {
                    str = GalleryResources.ErrorPathMayBeTooLong();
                    goto label_54;
                  }
                  else
                    goto label_53;
                case 'I':
                  if (errorType == "InvalidImageFile")
                  {
                    str = GalleryResources.ErrorInvalidImageFile();
                    goto label_54;
                  }
                  else
                    goto label_53;
                case 'S':
                  if (errorType == "InvalidSignature")
                  {
                    str = GalleryResources.ErrorInvalidSignature();
                    goto label_54;
                  }
                  else
                    goto label_53;
                default:
                  goto label_53;
              }
            case 18:
              switch (errorType[7])
              {
                case 'C':
                  if (errorType == "InvalidCertificate")
                  {
                    str = GalleryResources.ErrorInvalidCertificate();
                    goto label_54;
                  }
                  else
                    goto label_53;
                case 'V':
                  if (errorType == "InvalidVSIXPackage")
                    break;
                  goto label_53;
                case 'x':
                  if (errorType == "VSIXIsExperimental")
                  {
                    str = GalleryResources.ErrorVSIXIsExperimental();
                    goto label_54;
                  }
                  else
                    goto label_53;
                default:
                  goto label_53;
              }
              break;
            case 19:
              switch (errorType[7])
              {
                case 'P':
                  if (errorType == "MissingPreviewImage")
                  {
                    str = GalleryResources.ErrorMissingPreviewImage();
                    goto label_54;
                  }
                  else
                    goto label_53;
                case 'T':
                  if (errorType == "MissingTemplateFile")
                  {
                    str = GalleryResources.ErrorMissingTemplateFile();
                    goto label_54;
                  }
                  else
                    goto label_53;
                case 'V':
                  switch (errorType)
                  {
                    case "InvalidVSIXManifest":
                      str = GalleryResources.ErrorInvalidVSIXManifest();
                      goto label_54;
                    case "MissingVSIXManifest":
                      str = GalleryResources.ErrorMissingVSIXManifest();
                      goto label_54;
                    default:
                      goto label_53;
                  }
                default:
                  goto label_53;
              }
            case 20:
              if (errorType == "PkgDefInTemplateVSIX")
                break;
              goto label_53;
            case 21:
              switch (errorType[0])
              {
                case 'I':
                  if (errorType == "InvalidVSTemplateFile")
                  {
                    str = GalleryResources.ErrorInvalidVSTemplateFile();
                    goto label_54;
                  }
                  else
                    goto label_53;
                case 'M':
                  if (errorType == "MissingVSTemplateFile")
                  {
                    str = GalleryResources.ErrorMissingVSTemplateFile();
                    goto label_54;
                  }
                  else
                    goto label_53;
                case 'P':
                  if (errorType == "PotentialLongPathName")
                  {
                    str = GalleryResources.ErrorPotentialLongPathName();
                    goto label_54;
                  }
                  else
                    goto label_53;
                default:
                  goto label_53;
              }
            case 22:
              switch (errorType[0])
              {
                case 'E':
                  if (errorType == "EULAExceedsMaximumSize")
                  {
                    str = GalleryResources.ErrorEULAExceedsMaximumSize((object) 1);
                    goto label_54;
                  }
                  else
                    goto label_53;
                case 'I':
                  if (errorType == "InvalidTemplateZipFile")
                  {
                    str = GalleryResources.ErrorInvalidTemplateZipFile();
                    goto label_54;
                  }
                  else
                    goto label_53;
                default:
                  goto label_53;
              }
            case 25:
              if (errorType == "TemplateEntryPointsToFile")
                break;
              goto label_53;
            case 27:
              if (errorType == "InvalidTypeTargetingExpress")
              {
                str = GalleryResources.ErrorInvalidTypeTargetingExpress();
                goto label_54;
              }
              else
                goto label_53;
            case 29:
              if (errorType == "InvalidVsixIdTargetingExpress")
              {
                str = GalleryResources.InvalidVsixIdTargetingExpress();
                goto label_54;
              }
              else
                goto label_53;
            case 30:
              if (errorType == "VsixFormatNotSupportedByVS2010")
              {
                str = GalleryResources.ErrorVsixFormatNotSupportedByVS2010();
                goto label_54;
              }
              else
                goto label_53;
            case 31:
              if (errorType == "InvalidVSIXLanguagePackManifest")
              {
                str = GalleryResources.ErrorInvalidVSIXLanguagePackManifest();
                goto label_54;
              }
              else
                goto label_53;
            default:
              goto label_53;
          }
          str = GalleryResources.ErrorInvalidVSIXPackage();
          goto label_54;
        }
label_53:
        str = vsixValidationError.Message;
      }
label_54:
      return str + " " + vsixValidationError.Error?.Message;
    }
  }
}
