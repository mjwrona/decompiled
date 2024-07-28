// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FileSystemExtensionsService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal class FileSystemExtensionsService : 
    LocalExtensionService,
    IFileSystemExtensionsService,
    ILocalExtensionsService,
    IVssFrameworkService
  {
    private static JsonSerializer s_serializer = new VssJsonMediaTypeFormatter().CreateJsonSerializer();
    private static readonly string s_area = nameof (FileSystemExtensionsService);
    private static readonly string s_layer = nameof (FileSystemExtensionsService);
    private string m_extensionDirectory;
    private FileSystemWatcher m_watcher;
    private int m_refreshInProgress;

    public override void ServiceStart(IVssRequestContext requestContext)
    {
      base.ServiceStart(requestContext);
      if (!string.IsNullOrEmpty(this.StaticContentVersion()))
      {
        this.m_extensionDirectory = Path.Combine(requestContext.ServiceHost.PhysicalDirectory, string.Format("_static\\tfs\\{0}\\_ext\\", (object) this.StaticContentVersion()));
        this.m_extensionDirectory = FileSpec.GetFullPath(this.m_extensionDirectory);
        this.ConfigureFileWatcher();
      }
      if (!this.LocalFileExtensionsExist(requestContext))
        this.LocalExtensions = new Dictionary<string, LocalExtension>();
      else
        this.GetExtensions(requestContext);
    }

    public override void ServiceEnd(IVssRequestContext requestContext)
    {
      base.ServiceEnd(requestContext);
      if (this.m_watcher == null)
        return;
      this.m_watcher.Dispose();
      this.m_watcher = (FileSystemWatcher) null;
    }

    public override Dictionary<string, LocalExtension> GetExtensions(
      IVssRequestContext requestContext)
    {
      if (!this.LocalFileExtensionsExist(requestContext))
      {
        requestContext.Trace(10013312, TraceLevel.Info, FileSystemExtensionsService.s_area, FileSystemExtensionsService.s_layer, "FileSystemExtensionsService: Unable to return extensions from file system.");
        return this.LocalExtensions;
      }
      requestContext.Trace(10013313, TraceLevel.Info, FileSystemExtensionsService.s_area, FileSystemExtensionsService.s_layer, "FileSystemExtensionsService: Returning extensions from local file system.");
      if (this.LocalExtensions != null)
      {
        if (this.IsDataCurrent())
          return this.LocalExtensions;
        if (!requestContext.IsFeatureEnabled("VisualStudio.Services.ExtensionManagement.RefreshLocalExtensionIfNeededOnAllrequests"))
        {
          if (Interlocked.CompareExchange(ref this.m_refreshInProgress, 1, 0) == 0)
            this.QueueExtensionRefresh(requestContext, true);
          requestContext.Trace(10013310, TraceLevel.Info, FileSystemExtensionsService.s_area, FileSystemExtensionsService.s_layer, "Returning expired value since refresh is currently in progress");
          return this.LocalExtensions;
        }
        requestContext.Trace(10013311, TraceLevel.Info, FileSystemExtensionsService.s_area, FileSystemExtensionsService.s_layer, "All requests attempting to perform local extension refresh.");
      }
      return this.RefreshExtensions(requestContext);
    }

    internal override Dictionary<string, LocalExtension> RefreshExtensions(
      IVssRequestContext requestContext)
    {
      try
      {
        requestContext.Trace(10013315, TraceLevel.Info, FileSystemExtensionsService.s_area, FileSystemExtensionsService.s_layer, "Refreshing local extension cache");
        int currentDataVersion = this.CurrentDataVersion;
        Dictionary<string, LocalExtensionProvider> localProviders = new Dictionary<string, LocalExtensionProvider>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        Dictionary<string, LocalExtension> localExtensions = new Dictionary<string, LocalExtension>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        string query = string.Format("{0}**/{1}", (object) "/Configuration/LocalContributionProviders/", (object) "DisplayName");
        this.LoadProviders(requestContext, query, localProviders);
        int num = this.LoadExtensions(requestContext, localProviders, localExtensions) ? 1 : 0;
        if (currentDataVersion == this.CurrentDataVersion)
        {
          this.LoadedDataVersion = this.CurrentDataVersion;
          this.LocalExtensions = localExtensions;
        }
        if (num != 0)
          this.ResetRefreshAttempts();
        else
          this.QueueExtensionRefresh(requestContext, false);
        return localExtensions;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10013730, FileSystemExtensionsService.s_area, FileSystemExtensionsService.s_layer, ex);
        throw;
      }
      finally
      {
        this.m_refreshInProgress = 0;
      }
    }

    internal virtual bool LoadExtensions(
      IVssRequestContext requestContext,
      Dictionary<string, LocalExtensionProvider> localProviders,
      Dictionary<string, LocalExtension> localExtensions)
    {
      bool flag = true;
      string[] directories = Directory.GetDirectories(this.m_extensionDirectory);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      ContribtuionLookupService service = vssRequestContext.GetService<ContribtuionLookupService>();
      bool includeDebugAssets = !requestContext.IsFeatureEnabled("VisualStudio.Services.ExtensionManagement.DoNotLoadDebugAssets");
      foreach (string path in directories)
      {
        LocalExtension localExtension = (LocalExtension) null;
        ExtensionIdentifier extensionIdentifier = new ExtensionIdentifier(new DirectoryInfo(path).Name);
        foreach (string file in Directory.GetFiles(path, "*.vsomanifest", SearchOption.AllDirectories))
        {
          string key = "_";
          string fileName = Path.GetFileName(Path.GetDirectoryName(file));
          try
          {
            CultureInfo.GetCultureInfo(fileName);
            key = fileName;
          }
          catch (CultureNotFoundException ex)
          {
          }
          using (Stream stream = (Stream) new FileStream(file, FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
          {
            string base64String = Convert.ToBase64String(MD5Util.CalculateMD5(stream, true));
            IDictionary<string, object> extensionProperties = (IDictionary<string, object>) ExtensionUtil.GetExtensionProperties(PublishedExtensionFlags.BuiltIn | PublishedExtensionFlags.Trusted | PublishedExtensionFlags.Public | PublishedExtensionFlags.MultiVersion);
            extensionProperties.Add("::Hashcode", (object) base64String);
            ExtensionManifest extensionManifest = ExtensionUtil.LoadManifest(extensionIdentifier.PublisherName, extensionIdentifier.ExtensionName, "1", stream, extensionProperties);
            if (extensionManifest == null)
              throw new ManifestNotFoundException("The 'manifest' value cannot be null", (Exception) new NullReferenceException());
            if (extensionManifest.Contributions != null)
            {
              if (requestContext.IsFeatureEnabled("VisualStudio.Services.ExtensionManagement.LookForContributionMatch"))
              {
                List<Contribution> contributionList = new List<Contribution>();
                foreach (Contribution contribution in extensionManifest.Contributions)
                  contributionList.Add(service.GetContribution(vssRequestContext, contribution));
                extensionManifest.Contributions = (IEnumerable<Contribution>) contributionList;
              }
              string str = (string) null;
              LocalExtensionProvider extensionProvider;
              if (localProviders.TryGetValue(extensionIdentifier.PublisherName, out extensionProvider))
                str = extensionProvider.DisplayName;
              if (!localExtensions.TryGetValue(extensionIdentifier.ExtensionName, out localExtension))
              {
                localExtension = new LocalExtension()
                {
                  SupportedHosts = TeamFoundationHostType.All
                };
                localExtensions[extensionIdentifier.ExtensionName] = localExtension;
              }
              IContributionProvider contributionProvider1;
              if (localExtension.ContributionsByLanguage.TryGetValue(key, out contributionProvider1))
              {
                if (contributionProvider1 is FileSystemContributionProvider contributionProvider2)
                  contributionProvider2.UpdateContributions(extensionManifest.Contributions, includeDebugAssets);
              }
              else
              {
                LocalContributionDetails contributionDetails = new LocalContributionDetails()
                {
                  PublisherName = extensionIdentifier.PublisherName,
                  PublisherDisplayName = str,
                  ExtensionName = extensionIdentifier.ExtensionName,
                  Contributions = extensionManifest.Contributions,
                  Constraints = extensionManifest.Constraints,
                  Version = "1"
                };
                localExtension.ContributionsByLanguage[key] = (IContributionProvider) new FileSystemContributionProvider(contributionDetails, includeDebugAssets);
              }
            }
          }
        }
        if (localExtension != null)
        {
          Dictionary<string, Contribution> dictionary = new Dictionary<string, Contribution>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          foreach (string file in Directory.GetFiles(path, string.Format("{0}.*.json", (object) extensionIdentifier.ToString()), SearchOption.AllDirectories))
          {
            using (Stream stream = (Stream) new FileStream(file, FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
            {
              using (TextReader reader1 = (TextReader) new StreamReader(stream, Encoding.UTF8))
              {
                using (JsonReader reader2 = (JsonReader) new JsonTextReader(reader1))
                {
                  FileSystemExtensionsService.ContentFile contentFile = FileSystemExtensionsService.s_serializer.Deserialize<FileSystemExtensionsService.ContentFile>(reader2);
                  if (contentFile == null)
                    throw new ValueCannotBeNullException("contentFile");
                  if (contentFile.Contributions != null)
                  {
                    foreach (Contribution contribution in contentFile.Contributions)
                    {
                      contribution.Id = ExtensionUtil.GetFullyQualifiedReference(extensionIdentifier.ToString(), contribution.Id, false);
                      if (contribution.Type != null)
                        contribution.Type = ExtensionUtil.GetFullyQualifiedReference(extensionIdentifier.ToString(), contribution.Type, true);
                      ExtensionUtil.FullyQualifyContributionIds(contribution.Targets, extensionIdentifier.ToString());
                      ExtensionUtil.FullyQualifyContributionIds(contribution.Includes, extensionIdentifier.ToString());
                      dictionary[contribution.Id] = contribution;
                    }
                  }
                }
              }
            }
          }
          if (dictionary.Count > 0)
          {
            List<Contribution> contributionList = new List<Contribution>();
            try
            {
              foreach (Contribution contribution1 in localExtension.ContributionsByLanguage["_"].Contributions)
              {
                Contribution contribution2;
                if (dictionary.TryGetValue(contribution1.Id, out contribution2))
                  contributionList.Add(contribution2);
                else
                  contributionList.Add(contribution1);
              }
              (localExtension.ContributionsByLanguage["_"] as FileSystemContributionProvider).ReplaceContributions((IEnumerable<Contribution>) contributionList, includeDebugAssets);
            }
            catch (Exception ex)
            {
              string str = "extension.ContributionsByLanguage";
              if (localExtension.ContributionsByLanguage != null)
              {
                str += "[\"_\"]";
                if (localExtension.ContributionsByLanguage.ContainsKey("_"))
                {
                  str += ".Contributions";
                  if (localExtension.ContributionsByLanguage["_"].Contributions != null)
                    str = (string) null;
                }
              }
              requestContext.TraceException(10013540, TraceLevel.Error, FileSystemExtensionsService.s_area, FileSystemExtensionsService.s_layer, ex, str != null ? str + " cannot be null" : string.Empty);
              throw;
            }
          }
        }
      }
      return flag;
    }

    private void ConfigureFileWatcher()
    {
      if (!Directory.Exists(this.m_extensionDirectory))
        return;
      this.m_watcher = new FileSystemWatcher(this.m_extensionDirectory);
      this.m_watcher.IncludeSubdirectories = true;
      this.m_watcher.Changed += new FileSystemEventHandler(this.OnWatcherEvent);
      this.m_watcher.Created += new FileSystemEventHandler(this.OnWatcherEvent);
      this.m_watcher.Deleted += new FileSystemEventHandler(this.OnWatcherEvent);
      this.m_watcher.Renamed += new RenamedEventHandler(this.OnWatcherEvent);
      this.m_watcher.EnableRaisingEvents = true;
    }

    private void OnWatcherEvent(object sender, FileSystemEventArgs e) => this.InvalidateCache();

    private string StaticContentVersion() => ConfigurationManager.AppSettings["staticContentVersion"];

    private bool LocalFileExtensionsExist(IVssRequestContext requestContext) => !requestContext.IsHostProcessType(HostProcessType.JobAgent) && Directory.Exists(this.m_extensionDirectory);

    [DataContract]
    public class ContentFile
    {
      [DataMember(Name = "contributions")]
      public Contribution[] Contributions;
    }
  }
}
