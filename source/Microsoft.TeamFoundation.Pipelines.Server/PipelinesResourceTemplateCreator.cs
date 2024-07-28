// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.PipelinesResourceTemplateCreator
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  internal class PipelinesResourceTemplateCreator : IResourceTemplateCreator
  {
    private const string c_templateResourceNamePrefix = "Microsoft.TeamFoundation.Pipelines.Server.Templates.Pipelines.Definitions.";
    private const string c_templateAssetPrefix = "Microsoft.TeamFoundation.Pipelines.Server.Templates.Pipelines.Assets.";
    private const string c_templateAssetTypeFile = "file";
    private readonly TemplateResourceNames m_resourceNames = new TemplateResourceNames("Microsoft.TeamFoundation.Pipelines.Server.Templates.Pipelines.Definitions.", "pipelines");

    public IEnumerable<string> GetTemplateIds() => this.m_resourceNames.GetTemplateIds();

    public Template CreateTemplateModel(
      IVssRequestContext requestContext,
      string templateId,
      CultureInfo cultureInfo)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(templateId, nameof (templateId));
      string str = this.GetTemplateIds().Any<string>((Func<string, bool>) (id => string.Equals(id, templateId, StringComparison.OrdinalIgnoreCase))) ? this.m_resourceNames.GetTemplateResourceName(templateId) : throw new ArgumentException("The template '" + templateId + "' does not exist", nameof (templateId));
      ResourceManager resourceManager = ResourceHelper.GetResourceManager(this.m_resourceNames.GetMetaDataResourceName(str));
      return new Template()
      {
        Id = resourceManager.GetString("Id", cultureInfo),
        Name = resourceManager.GetString("Name", cultureInfo),
        Description = resourceManager.GetString("Description", cultureInfo),
        Content = ResourceHelper.GetResourceStreamAsString(str),
        IconUrl = PipelinesResourceTemplateCreator.GetIconUrl(requestContext, resourceManager, "IconAssetPath"),
        Parameters = PipelinesResourceTemplateCreator.GetListFromResourceJsonString<TemplateParameterDefinition>(resourceManager, "Parameters", cultureInfo),
        DataSourceBindings = PipelinesResourceTemplateCreator.GetListFromResourceJsonString<TemplateDataSourceBinding>(resourceManager, "DataSourceBindings", cultureInfo),
        Assets = PipelinesResourceTemplateCreator.GetAssetsFromResourceJsonString(resourceManager, "Assets", cultureInfo)
      };
    }

    private static IReadOnlyList<T> GetListFromResourceJsonString<T>(
      ResourceManager resourceManager,
      string key,
      CultureInfo cultureInfo)
    {
      string json = resourceManager.GetString(key, cultureInfo);
      if (string.IsNullOrEmpty(json))
        return (IReadOnlyList<T>) new List<T>().AsReadOnly();
      try
      {
        return JsonUtilities.Deserialize<IReadOnlyList<T>>(json);
      }
      catch (JsonSerializationException ex)
      {
        return (IReadOnlyList<T>) new List<T>().AsReadOnly();
      }
    }

    private static IReadOnlyList<TemplateAsset> GetAssetsFromResourceJsonString(
      ResourceManager resourceManager,
      string key,
      CultureInfo cultureInfo)
    {
      IReadOnlyList<TemplateAsset> resourceJsonString = PipelinesResourceTemplateCreator.GetListFromResourceJsonString<TemplateAsset>(resourceManager, key, cultureInfo);
      foreach (TemplateAsset templateAsset in resourceJsonString.Where<TemplateAsset>((Func<TemplateAsset, bool>) (asset => asset.Type.Equals("file", StringComparison.OrdinalIgnoreCase))))
        templateAsset.Content = ResourceHelper.GetResourceStreamAsString("Microsoft.TeamFoundation.Pipelines.Server.Templates.Pipelines.Assets." + templateAsset.Path.Replace('/', '.'));
      return resourceJsonString;
    }

    private static string GetIconUrl(
      IVssRequestContext requestContext,
      ResourceManager resourceManager,
      string key)
    {
      string assetType = resourceManager.GetString(key, (CultureInfo) null);
      return string.IsNullOrEmpty(assetType) ? (string) null : requestContext.GetService<IContributionService>().QueryAssetLocation(requestContext, "ms.vss-build-web", assetType);
    }

    private static class ResourceKeys
    {
      public const string Description = "Description";
      public const string Id = "Id";
      public const string Name = "Name";
      public const string IconAssetPath = "IconAssetPath";
      public const string Parameters = "Parameters";
      public const string DataSourceBindings = "DataSourceBindings";
      public const string Assets = "Assets";
    }
  }
}
