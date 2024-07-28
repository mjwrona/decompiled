// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Provenance.ProvenanceService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security.AntiXss;

namespace Microsoft.VisualStudio.Services.Feed.Server.Provenance
{
  public class ProvenanceService : IProvenanceService, IVssFrameworkService
  {
    private IFeedService feedService;
    private IdentityService identityService;
    private readonly IEnumerable<string> allowedSchemes = (IEnumerable<string>) new string[5]
    {
      "http",
      "https",
      "vsts",
      "build",
      "release"
    };
    private readonly IEnumerable<string> allowedKeys = (IEnumerable<string>) new string[24]
    {
      "System.DefinitionId",
      "Build.DefinitionName",
      "Build.DefinitionRevision",
      "Build.BuildId",
      "Build.BuildNumber",
      "Build.Repository.Id",
      "Build.Repository.Name",
      "Build.Repository.Provider",
      "Build.Repository.Uri",
      "Build.SourceBranch",
      "Build.SourceBranchName",
      "Build.SourceVersion",
      "System.CollectionId",
      "System.TeamProjectId",
      "System.TeamProject",
      "System.CollectionId",
      "System.TeamProjectId",
      "Release.DefinitionId",
      "Release.DefinitionName",
      "Release.ReleaseId",
      "Release.ReleaseName",
      "External.LinkText",
      "External.LinkUrl",
      "Common.IdentityDisplayName"
    };
    private const int MaxKeys = 16;
    private const int MaxValueLength = 2000;
    private const string ProjectIdKey = "System.TeamProjectId";

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => requestContext.TraceBlock(10019500, 10019501, 10019502, "Provenance", "Service", (Action) (() =>
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.Initialize(requestContext);
    }), "ServiceStart");

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext) => requestContext.TraceBlock(10019503, 10019504, 10019505, "Provenance", "Service", (Action) (() => ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext))), "ServiceEnd");

    internal void Initialize(IVssRequestContext requestContext)
    {
      this.feedService = requestContext.GetService<IFeedService>();
      this.identityService = requestContext.GetService<IdentityService>();
    }

    public PackageVersionProvenance GetPackageVersionProvenance(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      Guid packageVersionId)
    {
      return requestContext.TraceBlock<PackageVersionProvenance>(10019506, 10019507, 10019508, "Provenance", "Service", (Func<PackageVersionProvenance>) (() =>
      {
        PackageVersionProvenance versionProvenance;
        using (ProvenanceSqlResourceComponent component = requestContext.CreateComponent<ProvenanceSqlResourceComponent>())
          versionProvenance = component.GetPackageVersionProvenance(feed.GetIdentity(), packageId, packageVersionId);
        if (versionProvenance == null || versionProvenance.Provenance == null)
          throw new ProvenanceNotFoundException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_PackageVersionProvenanceNotFound((object) packageId, (object) packageVersionId, (object) feed.Name));
        versionProvenance.Provenance.Data = versionProvenance.Provenance.Data ?? (IDictionary<string, string>) new Dictionary<string, string>();
        this.SanitizeCrossProjectData(feed, versionProvenance.Provenance);
        this.SanitizeUrls(versionProvenance.Provenance);
        this.AddUserDisplayName(requestContext, versionProvenance.Provenance);
        return versionProvenance;
      }), nameof (GetPackageVersionProvenance));
    }

    public bool TryValidateProvenanceData(Microsoft.VisualStudio.Services.Feed.WebApi.Provenance provenance, out Exception exception)
    {
      try
      {
        this.ValidateProvenanceData(provenance);
      }
      catch (Exception ex)
      {
        exception = ex;
        return false;
      }
      exception = (Exception) null;
      return true;
    }

    private void AddUserDisplayName(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Provenance provenance)
    {
      if (provenance == null || provenance.PublisherUserIdentity == Guid.Empty)
        return;
      Guid publisherUserIdentity = provenance.PublisherUserIdentity;
      Microsoft.VisualStudio.Services.Identity.Identity identity = this.identityService.ReadIdentities(requestContext, (IList<Guid>) new List<Guid>()
      {
        publisherUserIdentity
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity == null)
        return;
      provenance.Data["Common.IdentityDisplayName"] = identity.DisplayName;
    }

    private void ValidateProvenanceData(Microsoft.VisualStudio.Services.Feed.WebApi.Provenance provenance)
    {
      if (provenance == null || provenance.Data == null)
        return;
      if (provenance.Data.Keys.Count > 16)
        throw new InvalidProvenanceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_ProvenanceTooManyKeys());
      if (provenance.Data.Keys.Any<string>((Func<string, bool>) (x => !this.allowedKeys.Any<string>((Func<string, bool>) (y => string.Equals(x, y, StringComparison.OrdinalIgnoreCase))))))
        throw new InvalidProvenanceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_ProvenanceInvalidKey());
      if (provenance.Data.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (x => x.Value != null && x.Value.Length > 2000)))
        throw new InvalidProvenanceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_ProvenanceValueTooLong());
    }

    private void SanitizeUrls(Microsoft.VisualStudio.Services.Feed.WebApi.Provenance provenance)
    {
      string[] provenanceUrlKeys = new string[2]
      {
        "Build.Repository.Uri",
        "External.LinkUrl"
      };
      provenance.Data.Keys.ToList<string>().ForEach((Action<string>) (key => provenance.Data[key] = ((IEnumerable<string>) provenanceUrlKeys).Contains<string>(key, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? this.SanitizeUrl(provenance.Data[key]) : this.EncodeOutput(provenance.Data[key])));
    }

    private string SanitizeUrl(string url)
    {
      Uri uriResult;
      return string.IsNullOrWhiteSpace(url) || Uri.TryCreate(url, UriKind.Absolute, out uriResult) && this.allowedSchemes.Any<string>((Func<string, bool>) (allowed => string.Equals(allowed, uriResult.Scheme, StringComparison.OrdinalIgnoreCase))) ? url : (string) null;
    }

    private string EncodeOutput(string input) => AntiXssEncoder.HtmlEncode(input, true);

    private void SanitizeCrossProjectData(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, Microsoft.VisualStudio.Services.Feed.WebApi.Provenance provenance)
    {
      ProjectReference project = feed.Project;
      int num;
      if ((object) project == null)
      {
        num = 1;
      }
      else
      {
        Guid id = project.Id;
        num = 0;
      }
      Guid result;
      if (num != 0 || !string.Equals(provenance.ProvenanceSource, "InternalBuild", StringComparison.OrdinalIgnoreCase) && !string.Equals(provenance.ProvenanceSource, "InternalRelease", StringComparison.OrdinalIgnoreCase) || provenance.Data.ContainsKey("System.TeamProjectId") && Guid.TryParse(provenance.Data["System.TeamProjectId"], out result) && !(feed.Project.Id != result))
        return;
      provenance.Data.Clear();
    }
  }
}
