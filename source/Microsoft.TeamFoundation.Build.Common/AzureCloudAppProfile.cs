// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.AzureCloudAppProfile
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Build.Common
{
  public class AzureCloudAppProfile
  {
    public static AzureCloudAppProfile Parse(string fileText)
    {
      AzureCloudAppProfile azureCloudAppProfile = new AzureCloudAppProfile();
      foreach (XElement element in XDocument.Parse(fileText).Root.Element((XName) "{http://schemas.microsoft.com/developer/msbuild/2003}PropertyGroup").Elements())
      {
        string str = element.Name.ToString();
        if (str != null)
        {
          switch (str.Length)
          {
            case 62:
              if (str == "{http://schemas.microsoft.com/developer/msbuild/2003}AzureSlot")
              {
                azureCloudAppProfile.AzureSlot = (AzureDeploymentSlot) Enum.Parse(typeof (AzureDeploymentSlot), (string) element);
                continue;
              }
              break;
            case 70:
              if (str == "{http://schemas.microsoft.com/developer/msbuild/2003}AzureAllowUpgrade")
              {
                azureCloudAppProfile.AzureAllowUpgrade = bool.Parse((string) element);
                continue;
              }
              break;
            case 73:
              if (str == "{http://schemas.microsoft.com/developer/msbuild/2003}AzureDeploymentLabel")
              {
                azureCloudAppProfile.AzureDeploymentLabel = (string) element;
                continue;
              }
              break;
            case 75:
              if (str == "{http://schemas.microsoft.com/developer/msbuild/2003}AzureHostedServiceName")
              {
                azureCloudAppProfile.AzureHostedServiceName = (string) element;
                continue;
              }
              break;
            case 76:
              switch (str[58])
              {
                case 'H':
                  if (str == "{http://schemas.microsoft.com/developer/msbuild/2003}AzureHostedServiceLabel")
                  {
                    azureCloudAppProfile.AzureHostedServiceLabel = (string) element;
                    continue;
                  }
                  break;
                case 'S':
                  if (str == "{http://schemas.microsoft.com/developer/msbuild/2003}AzureStorageAccountName")
                  {
                    azureCloudAppProfile.AzureStorageAccountName = (string) element;
                    continue;
                  }
                  break;
              }
              break;
            case 77:
              if (str == "{http://schemas.microsoft.com/developer/msbuild/2003}AzureStorageAccountLabel")
              {
                azureCloudAppProfile.AzureStorageAccountLabel = (string) element;
                continue;
              }
              break;
            case 78:
              if (str == "{http://schemas.microsoft.com/developer/msbuild/2003}AzureServiceConfiguration")
              {
                azureCloudAppProfile.AzureServiceConfiguration = (string) element;
                continue;
              }
              break;
            case 79:
              if (str == "{http://schemas.microsoft.com/developer/msbuild/2003}AzureSolutionConfiguration")
              {
                azureCloudAppProfile.AzureSolutionConfiguration = (string) element;
                continue;
              }
              break;
            case 90:
              if (str == "{http://schemas.microsoft.com/developer/msbuild/2003}AzureAppendTimestampToDeploymentLabel")
              {
                azureCloudAppProfile.AzureAppendTimestampToDeploymentLabel = bool.Parse((string) element);
                continue;
              }
              break;
            case 99:
              if (str == "{http://schemas.microsoft.com/developer/msbuild/2003}AzureFallbackToDeleteAndRecreateIfUpgradeFails")
              {
                azureCloudAppProfile.AzureFallbackToDeleteAndRecreateIfUpgradeFails = bool.Parse((string) element);
                continue;
              }
              break;
          }
        }
        azureCloudAppProfile.Properties.Add(element.Name.LocalName, (string) element);
      }
      return azureCloudAppProfile;
    }

    public static AzureCloudAppProfile CreateDefaultProfile(
      string hostedServiceName,
      string storageAccountName,
      string deploymentLabel)
    {
      return new AzureCloudAppProfile()
      {
        AzureHostedServiceName = hostedServiceName,
        AzureStorageAccountName = storageAccountName,
        AzureDeploymentLabel = deploymentLabel,
        AzureSlot = AzureDeploymentSlot.Staging,
        AzureAppendTimestampToDeploymentLabel = true,
        AzureAllowUpgrade = true
      };
    }

    private AzureCloudAppProfile()
    {
      this.Properties = (IDictionary<string, string>) new Dictionary<string, string>();
      this.AzureFallbackToDeleteAndRecreateIfUpgradeFails = true;
    }

    public string AzureHostedServiceName { get; set; }

    public string AzureHostedServiceLabel { get; private set; }

    public AzureDeploymentSlot AzureSlot { get; set; }

    public string AzureStorageAccountName { get; set; }

    public string AzureStorageAccountLabel { get; private set; }

    public string AzureDeploymentLabel { get; set; }

    public string AzureSolutionConfiguration { get; private set; }

    public string AzureServiceConfiguration { get; set; }

    public bool AzureAppendTimestampToDeploymentLabel { get; private set; }

    public bool AzureAllowUpgrade { get; set; }

    public bool AzureFallbackToDeleteAndRecreateIfUpgradeFails { get; set; }

    public IDictionary<string, string> Properties { get; private set; }

    public string ToXml()
    {
      XDocument xdocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), Array.Empty<object>());
      XElement content1 = new XElement((XName) "{http://schemas.microsoft.com/developer/msbuild/2003}Project");
      content1.SetAttributeValue((XName) "{http://schemas.microsoft.com/developer/msbuild/2003}ToolsVersion", (object) "4.0");
      content1.SetAttributeValue((XName) "xmlns", (object) "http://schemas.microsoft.com/developer/msbuild/2003");
      XElement content2 = new XElement((XName) "{http://schemas.microsoft.com/developer/msbuild/2003}PropertyGroup");
      if (!string.IsNullOrEmpty(this.AzureHostedServiceName))
        content2.SetElementValue((XName) "{http://schemas.microsoft.com/developer/msbuild/2003}AzureHostedServiceName", (object) this.AzureHostedServiceName);
      if (!string.IsNullOrEmpty(this.AzureHostedServiceLabel))
        content2.SetElementValue((XName) "{http://schemas.microsoft.com/developer/msbuild/2003}AzureHostedServiceLabel", (object) this.AzureHostedServiceLabel);
      content2.SetElementValue((XName) "{http://schemas.microsoft.com/developer/msbuild/2003}AzureSlot", (object) this.AzureSlot.ToString());
      if (!string.IsNullOrEmpty(this.AzureStorageAccountName))
        content2.SetElementValue((XName) "{http://schemas.microsoft.com/developer/msbuild/2003}AzureStorageAccountName", (object) this.AzureStorageAccountName);
      if (!string.IsNullOrEmpty(this.AzureStorageAccountLabel))
        content2.SetElementValue((XName) "{http://schemas.microsoft.com/developer/msbuild/2003}AzureStorageAccountLabel", (object) this.AzureStorageAccountLabel);
      if (!string.IsNullOrEmpty(this.AzureDeploymentLabel))
        content2.SetElementValue((XName) "{http://schemas.microsoft.com/developer/msbuild/2003}AzureDeploymentLabel", (object) this.AzureDeploymentLabel);
      if (!string.IsNullOrEmpty(this.AzureSolutionConfiguration))
        content2.SetElementValue((XName) "{http://schemas.microsoft.com/developer/msbuild/2003}AzureSolutionConfiguration", (object) this.AzureSolutionConfiguration);
      if (!string.IsNullOrEmpty(this.AzureServiceConfiguration))
        content2.SetElementValue((XName) "{http://schemas.microsoft.com/developer/msbuild/2003}AzureServiceConfiguration", (object) this.AzureServiceConfiguration);
      if (!string.IsNullOrEmpty(this.AzureHostedServiceLabel))
        content2.SetElementValue((XName) "{http://schemas.microsoft.com/developer/msbuild/2003}AzureHostedServiceLabel", (object) this.AzureHostedServiceLabel);
      content2.SetElementValue((XName) "{http://schemas.microsoft.com/developer/msbuild/2003}AzureFallbackToDeleteAndRecreateIfUpgradeFails", (object) this.AzureFallbackToDeleteAndRecreateIfUpgradeFails);
      content2.SetElementValue((XName) "{http://schemas.microsoft.com/developer/msbuild/2003}AzureAppendTimestampToDeploymentLabel", (object) this.AzureAppendTimestampToDeploymentLabel.ToString());
      content2.SetElementValue((XName) "{http://schemas.microsoft.com/developer/msbuild/2003}AzureAllowUpgrade", (object) this.AzureAllowUpgrade.ToString());
      foreach (KeyValuePair<string, string> property in (IEnumerable<KeyValuePair<string, string>>) this.Properties)
        content2.SetElementValue((XName) property.Key, (object) property.Value);
      content1.Add((object) content2);
      xdocument.Add((object) content1);
      return xdocument.ToString();
    }
  }
}
