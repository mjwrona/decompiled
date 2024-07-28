// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation.ValidationSteps.CertifiedPublisherValidationStep
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation.ValidationSteps
{
  internal class CertifiedPublisherValidationStep : PackageValidationStepBase
  {
    private const string s_layer = "CertifiedPublisherValidationStep";
    private const PackageValidationStepBase.ValidationStepType s_stepType = PackageValidationStepBase.ValidationStepType.CertifiedPublisherValidation;
    private bool m_bLicenseFileAvailable;
    private bool m_bPrivacyPolicyFileAvailable;

    public CertifiedPublisherValidationStep()
      : base(PackageValidationStepBase.ValidationStepType.CertifiedPublisherValidation)
    {
    }

    public override void BeginValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Stream packageStream)
    {
      this.m_result = ValidationStatus.Success;
      this.ResultMessage = string.Empty;
      string str1 = string.Empty;
      short num1 = 1;
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      bool flag = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableCertifiedPublisherVersionValidation");
      if (!flag || !extension.Publisher.Flags.HasFlag((Enum) PublisherFlags.Certified) || !extension.IsVSTSExtensionResourceOrIntegration() || !extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public))
      {
        requestContext.Trace(12062046, TraceLevel.Info, "Gallery", nameof (CertifiedPublisherValidationStep), "By-passing Certified Publisher extension version validation.\nFeature flag status:{0}, itemName:{1}, install target(s): {2}, extension flags: {3}, publisher flags: {4}", (object) flag, (object) extension.GetFullyQualifiedName(), (object) extension.InstallationTargets?.ToString(), (object) extension.Flags.ToString(), (object) extension.Publisher.Flags.ToString());
      }
      else
      {
        if (extension.Versions == null || extension.Versions.Count == 0)
          throw new ExtensionVersionNotFoundException(GalleryWebApiResources.ExtensionNoVersionFound((object) GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName)));
        VSIXPackage.Parse(packageStream, new Func<ManifestFile, Stream, bool>(this.CertifiedPublisherExtensionFilesCallback));
        KeyValuePair<string, string> keyValuePair;
        short num2;
        if (!this.m_bLicenseFileAvailable)
        {
          List<KeyValuePair<string, string>> properties = extension.Versions[0].Properties;
          string str2;
          if (properties == null)
          {
            str2 = (string) null;
          }
          else
          {
            keyValuePair = properties.Find((Predicate<KeyValuePair<string, string>>) (x => x.Key.Equals("Microsoft.VisualStudio.Services.Links.License", StringComparison.OrdinalIgnoreCase)));
            str2 = keyValuePair.Value;
          }
          if (str2 == null)
          {
            this.m_result = ValidationStatus.Failure;
            string[] strArray = new string[5]
            {
              str1,
              " ",
              null,
              null,
              null
            };
            num2 = num1++;
            strArray[2] = num2.ToString();
            strArray[3] = ".";
            strArray[4] = GalleryResources.CertifiedPublisherVersionValidationFailureSubMessage((object) GalleryResources.EulaText());
            str1 = string.Concat(strArray);
          }
        }
        if (!this.m_bPrivacyPolicyFileAvailable)
        {
          List<KeyValuePair<string, string>> properties = extension.Versions[0].Properties;
          string str3;
          if (properties == null)
          {
            str3 = (string) null;
          }
          else
          {
            keyValuePair = properties.Find((Predicate<KeyValuePair<string, string>>) (x => x.Key.Equals("Microsoft.VisualStudio.Services.Links.Privacypolicy", StringComparison.OrdinalIgnoreCase)));
            str3 = keyValuePair.Value;
          }
          if (str3 == null)
          {
            this.m_result = ValidationStatus.Failure;
            string[] strArray = new string[5]
            {
              str1,
              " ",
              null,
              null,
              null
            };
            num2 = num1++;
            strArray[2] = num2.ToString();
            strArray[3] = ".";
            strArray[4] = GalleryResources.CertifiedPublisherVersionValidationFailureSubMessage((object) GalleryResources.PrivacyPolicyText());
            str1 = string.Concat(strArray);
          }
        }
        List<KeyValuePair<string, string>> properties1 = extension.Versions[0].Properties;
        string str4;
        if (properties1 == null)
        {
          str4 = (string) null;
        }
        else
        {
          keyValuePair = properties1.Find((Predicate<KeyValuePair<string, string>>) (x => x.Key.Equals("Microsoft.VisualStudio.Services.Links.Support", StringComparison.OrdinalIgnoreCase)));
          str4 = keyValuePair.Value;
        }
        if (str4 == null)
        {
          this.m_result = ValidationStatus.Failure;
          string[] strArray = new string[5]
          {
            str1,
            " ",
            null,
            null,
            null
          };
          int num3 = (int) num1;
          short num4 = (short) (num3 + 1);
          num2 = (short) num3;
          strArray[2] = num2.ToString();
          strArray[3] = ".";
          strArray[4] = GalleryResources.CertifiedPublisherVersionValidationFailureSubMessage((object) GalleryResources.SupportInfoText());
          str1 = string.Concat(strArray);
        }
        if (string.IsNullOrEmpty(str1))
          return;
        this.ResultMessage = GalleryResources.CertifiedPublisherVersionValidationFailureMessage((object) str1);
      }
    }

    public ValidationStatus GetValidationStatus() => this.m_result;

    private bool CertifiedPublisherExtensionFilesCallback(ManifestFile manifestFile, Stream stream)
    {
      if (manifestFile != null && manifestFile.Addressable)
      {
        if (manifestFile.AssetType.Equals("Microsoft.VisualStudio.Services.Content.License", StringComparison.OrdinalIgnoreCase))
          this.m_bLicenseFileAvailable = true;
        else if (manifestFile.AssetType.Equals("Microsoft.VisualStudio.Services.Content.PrivacyPolicy", StringComparison.OrdinalIgnoreCase))
          this.m_bPrivacyPolicyFileAvailable = true;
      }
      return false;
    }
  }
}
