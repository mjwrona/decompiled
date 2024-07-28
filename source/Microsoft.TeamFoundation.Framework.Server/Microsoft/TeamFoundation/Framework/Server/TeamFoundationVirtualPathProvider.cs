// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationVirtualPathProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class TeamFoundationVirtualPathProvider : VirtualPathProvider
  {
    private static readonly HashSet<string> s_tfsResourcesExtensions = new HashSet<string>((IEnumerable<string>) new string[5]
    {
      ".asmx",
      ".ashx",
      ".aspx",
      ".xsl",
      ".svc"
    }, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static readonly string[] s_denyList = new string[1]
    {
      "/ThrottleNotificationEmailTemplate.xsl"
    };
    private static IReadOnlyDictionary<string, string> s_tfsResources;
    private static readonly RegistryQuery s_tfsNegativeFileExtensions = new RegistryQuery("/Configuration/TeamFoundationVirtualPathProvider/NegativeFileExtensions");
    private readonly HashSet<string> m_negativeFileExtensions;
    private const string c_area = "TeamFoundationVirtualPathProvider";
    private const string c_layer = "WebServices";

    internal TeamFoundationVirtualPathProvider(IVssRequestContext requestContext) => this.m_negativeFileExtensions = new HashSet<string>((IEnumerable<string>) requestContext.GetService<ISqlRegistryService>().GetValue(requestContext, in TeamFoundationVirtualPathProvider.s_tfsNegativeFileExtensions, ".vbhtml").Split(new string[1]
    {
      ","
    }, StringSplitOptions.RemoveEmptyEntries), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    protected override void Initialize()
    {
      if (HttpContext.Current == null || TeamFoundationVirtualPathProvider.s_tfsResources != null)
        return;
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      string str1 = HttpContext.Current.Server.MapPath(FrameworkServerConstants.ServiceHostResourcesDirectory);
      if (Directory.Exists(str1))
      {
        foreach (string enumerateFile in Directory.EnumerateFiles(str1, "*.*", SearchOption.AllDirectories))
        {
          string str2 = enumerateFile.Replace(str1, "~/").Replace('\\', '/');
          string extension = VirtualPathUtility.GetExtension(str2);
          if (!string.IsNullOrEmpty(extension) && TeamFoundationVirtualPathProvider.s_tfsResourcesExtensions.Contains(extension) && !TeamFoundationVirtualPathProvider.IsInDenyList(str2))
            dictionary.Add(str2, enumerateFile);
        }
      }
      TeamFoundationVirtualPathProvider.s_tfsResources = (IReadOnlyDictionary<string, string>) dictionary;
    }

    private static bool IsInDenyList(string fileVirtualPath)
    {
      for (int index = 0; index < TeamFoundationVirtualPathProvider.s_denyList.Length; ++index)
      {
        if (fileVirtualPath.EndsWith(TeamFoundationVirtualPathProvider.s_denyList[index], StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }

    public override bool FileExists(string virtualPath)
    {
      if (virtualPath.StartsWith("~/_apis/", StringComparison.OrdinalIgnoreCase) || virtualPath.IndexOf("/_apis/", StringComparison.OrdinalIgnoreCase) != -1 || virtualPath.StartsWith("~/_packaging/", StringComparison.OrdinalIgnoreCase) || virtualPath.IndexOf("/_packaging/", StringComparison.OrdinalIgnoreCase) != -1)
        return false;
      if (this.IsVirtualPath(virtualPath, out string _))
        return true;
      return !this.m_negativeFileExtensions.Contains(VirtualPathUtility.GetExtension(virtualPath)) && this.Previous.FileExists(virtualPath);
    }

    public override string GetCacheKey(string virtualPath)
    {
      string physicalPath;
      return this.IsVirtualPath(virtualPath, out physicalPath) ? this.Previous.GetCacheKey(physicalPath) : this.Previous.GetCacheKey(virtualPath);
    }

    public override CacheDependency GetCacheDependency(
      string virtualPath,
      IEnumerable virtualPathDependencies,
      DateTime utcStart)
    {
      return this.IsVirtualPath(virtualPath, out string _) ? (CacheDependency) null : this.Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
    }

    public override VirtualFile GetFile(string virtualPath)
    {
      string physicalPath;
      return this.IsVirtualPath(virtualPath, out physicalPath) ? (VirtualFile) new TeamFoundationVirtualPathProvider.TeamFoundationVirtualFile(virtualPath, physicalPath) : this.Previous.GetFile(virtualPath);
    }

    internal bool IsVirtualPath(string virtualPath, out string physicalPath)
    {
      physicalPath = (string) null;
      IVssRequestContext requestContext;
      if (HttpContext.Current == null || TeamFoundationVirtualPathProvider.s_tfsResources == null || TeamFoundationVirtualPathProvider.s_tfsResources.Count == 0 || (requestContext = (IVssRequestContext) HttpContext.Current.Items[(object) HttpContextConstants.IVssRequestContext]) == null)
        return false;
      string extension = VirtualPathUtility.GetExtension(virtualPath);
      if (string.IsNullOrEmpty(extension) || !TeamFoundationVirtualPathProvider.s_tfsResourcesExtensions.Contains(extension))
        return false;
      requestContext.TraceEnter(60803, nameof (TeamFoundationVirtualPathProvider), "WebServices", nameof (IsVirtualPath));
      try
      {
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && virtualPath.EndsWith(LocationServiceConstants.ApplicationLocationServiceRelativePath, StringComparison.OrdinalIgnoreCase) && (requestContext.ExecutionEnvironment.IsOnPremisesDeployment || string.Compare(virtualPath, 1, "/DefaultCollection/", 0, "/DefaultCollection/".Length, StringComparison.OrdinalIgnoreCase) == 0))
          return false;
        string absolute = VirtualPathUtility.ToAbsolute(virtualPath);
        string key = "~" + requestContext.RemoveVirtualDirectory(absolute);
        return TeamFoundationVirtualPathProvider.s_tfsResources.TryGetValue(key, out physicalPath);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(60807, nameof (TeamFoundationVirtualPathProvider), "WebServices", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(60809, nameof (TeamFoundationVirtualPathProvider), "WebServices", nameof (IsVirtualPath));
      }
    }

    private class TeamFoundationVirtualFile : VirtualFile
    {
      private readonly string m_physicalPath;

      public TeamFoundationVirtualFile(string virtualPath, string physicalPath)
        : base(virtualPath)
      {
        this.m_physicalPath = physicalPath;
      }

      public override Stream Open() => (Stream) File.Open(this.m_physicalPath, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete);
    }
  }
}
