// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.ContributedContent
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions
{
  public class ContributedContent
  {
    private Dictionary<string, Dictionary<string, Dictionary<CultureInfo, ContributedContent.ContentFile>>> m_structuredAssets = new Dictionary<string, Dictionary<string, Dictionary<CultureInfo, ContributedContent.ContentFile>>>();
    private ContributionNode m_contributionNode;
    private LoaderPriority m_loaderPriority = LoaderPriority.TTI;
    private IEnumerable<string> m_moduleNamespaces;
    private IEnumerable<ContentSource> m_fixedSources;

    public ContributedContent(
      IVssRequestContext requestContext,
      IEnumerable<string> moduleNamespaces,
      LoaderPriority loaderPriority,
      IEnumerable<ContentSource> sources)
    {
      this.m_moduleNamespaces = moduleNamespaces;
      this.m_loaderPriority = loaderPriority;
      this.m_fixedSources = sources;
    }

    public ContributedContent(IVssRequestContext requestContext, ContributionNode contributionNode)
    {
      this.m_contributionNode = contributionNode;
      List<ContributedContent.ContentFile> contentFileList;
      if (this.m_contributionNode.Contribution.Properties.TryGetValue<List<ContributedContent.ContentFile>>("content", out contentFileList))
      {
        foreach (ContributedContent.ContentFile contentFile in contentFileList)
        {
          CultureInfo key = CultureInfo.InvariantCulture;
          if (!string.IsNullOrEmpty(contentFile.Lang))
          {
            try
            {
              key = CultureInfo.GetCultureInfo(contentFile.Lang);
            }
            catch (CultureNotFoundException ex)
            {
            }
          }
          Dictionary<string, Dictionary<CultureInfo, ContributedContent.ContentFile>> dictionary1;
          if (!this.m_structuredAssets.TryGetValue(contentFile.Type, out dictionary1))
          {
            dictionary1 = new Dictionary<string, Dictionary<CultureInfo, ContributedContent.ContentFile>>();
            this.m_structuredAssets[contentFile.Type] = dictionary1;
          }
          Dictionary<CultureInfo, ContributedContent.ContentFile> dictionary2;
          if (!dictionary1.TryGetValue(contentFile.Asset, out dictionary2))
          {
            dictionary2 = new Dictionary<CultureInfo, ContributedContent.ContentFile>();
            dictionary1.Add(contentFile.Asset, dictionary2);
          }
          dictionary2.Add(key, contentFile);
        }
      }
      LoaderPriority loaderPriority;
      if (this.m_contributionNode.Contribution.Properties.TryGetValue<LoaderPriority>("loaderPriority", out loaderPriority))
      {
        this.m_loaderPriority = loaderPriority;
      }
      else
      {
        ContentLoadFlags contentLoadFlags;
        if (this.m_contributionNode.Contribution.Properties.TryGetValue<ContentLoadFlags>("contentLoadFlags", out contentLoadFlags) && (object) contentLoadFlags is "sync")
          this.m_loaderPriority = LoaderPriority.Immediate;
      }
      this.m_contributionNode.Contribution.Properties.TryGetValue<IEnumerable<string>>("moduleNamespaces", out this.m_moduleNamespaces);
    }

    public string ContributionId => this.m_contributionNode?.Id;

    public IEnumerable<string> ModuleNamespaces => this.m_moduleNamespaces;

    public LoaderPriority LoaderPriority => this.m_loaderPriority;

    public IEnumerable<ContentSource> GetContentSources(
      IVssRequestContext requestContext,
      string fileType,
      string contentType,
      string preferredLocation = "Local",
      bool includeMultiUseContent = true)
    {
      if (this.m_fixedSources != null)
        return this.m_fixedSources.Where<ContentSource>((Func<ContentSource, bool>) (source => string.Equals(contentType, source.ContentType, StringComparison.OrdinalIgnoreCase)));
      HashSet<ContentSource> contentSources = new HashSet<ContentSource>();
      List<CultureInfo> acceptedCultures = RequestLanguage.GetAcceptedCultures(requestContext);
      Dictionary<string, Dictionary<CultureInfo, ContributedContent.ContentFile>> dictionary1;
      if (this.m_structuredAssets.TryGetValue(fileType, out dictionary1))
      {
        IContentService service = requestContext.GetService<IContentService>();
        foreach (KeyValuePair<string, Dictionary<CultureInfo, ContributedContent.ContentFile>> keyValuePair in dictionary1)
        {
          HashSet<CultureInfo> availableCultures = new HashSet<CultureInfo>((IEnumerable<CultureInfo>) keyValuePair.Value.Keys);
          CultureInfo bestCultureMatch = CultureResolution.GetBestCultureMatch((IList<CultureInfo>) acceptedCultures, (ISet<CultureInfo>) availableCultures);
          ContributedContent.ContentFile contentFile = keyValuePair.Value[bestCultureMatch ?? CultureInfo.InvariantCulture];
          Dictionary<string, string> dictionary2 = service.QueryContentLocations(requestContext, this.m_contributionNode.Id, contentFile.Asset);
          string str1 = (string) null;
          if (!string.IsNullOrEmpty(contentFile.HashType))
            str1 = contentFile.HashType + "-" + contentFile.HashCode;
          string str2;
          if (dictionary2.TryGetValue(preferredLocation, out str2) || dictionary2.TryGetValue("Local", out str2))
            contentSources.Add(new ContentSource()
            {
              Url = str2,
              ContentType = contentType,
              ClientId = this.m_contributionNode.Id + "/" + contentFile.Asset,
              ContributionId = this.m_contributionNode.Id,
              ContentLength = contentFile.LengthInBytes,
              Integrity = str1,
              ModuleNamespaces = this.ModuleNamespaces,
              Priority = (int) this.m_loaderPriority
            });
        }
      }
      if (includeMultiUseContent && (fileType.Contains("ES2017") || fileType.Contains("javascript")))
      {
        int startIndex = fileType.IndexOf(".");
        string str = startIndex < 0 ? "" : fileType.Substring(startIndex);
        contentSources.UnionWith(this.GetContentSources(requestContext, "javascript-and-ES2017" + str, contentType, preferredLocation, false));
        if (str.Contains("bundle"))
          contentSources.UnionWith(this.GetContentSources(requestContext, "javascript-and-ES2017" + str.Replace("bundle", "bundle-and-standalone"), contentType, preferredLocation, false));
        else
          contentSources.UnionWith(this.GetContentSources(requestContext, "javascript-and-ES2017.bundle-and-standalone", contentType, preferredLocation, false));
      }
      return (IEnumerable<ContentSource>) contentSources;
    }

    internal class ContentFile
    {
      public string Asset;
      public string Type;
      public string Lang;
      public long LengthInBytes;
      public string HashCode;
      public string HashType;
    }
  }
}
