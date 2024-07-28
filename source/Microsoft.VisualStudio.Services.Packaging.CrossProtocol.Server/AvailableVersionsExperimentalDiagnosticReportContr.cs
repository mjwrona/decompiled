// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Controllers.AvailableVersionsExperimentalDiagnosticReportController
// Assembly: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9BF15B3F-0578-4452-9C4B-B5237E218DF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Controllers
{
  [ControllerApiVersion(6.1)]
  [VersionedApiControllerCustomName(Area = "AnyProtocolPackagingInternal", ResourceName = "AvailableVersionsExperimentalDiagnosticReport", ResourceVersion = 1)]
  [ClientIgnore]
  public class AvailableVersionsExperimentalDiagnosticReportController : 
    AnyProtocolPackagingApiController
  {
    [HttpGet]
    [ValidateModel]
    public async Task<HttpResponseMessage> GetAvailableVersionsExperimentalDiagnosticReportAsync(
      string protocol,
      string feedId,
      string packageName)
    {
      AvailableVersionsExperimentalDiagnosticReportController reportController = this;
      IFeedRequest feedRequest = reportController.GetFeedRequest(protocol, feedId);
      packageName = AlternativeUriEscaping.UnescapeString(packageName);
      IPackageNameRequest<IPackageName> packageNameRequest = feedRequest.WithPackageName<IPackageName>(ProtocolRegistrar.Instance.GetIdentityResolver(feedRequest.Protocol).ResolvePackageName(packageName));
      IRequiredProtocolBootstrappers bootstrappers = ProtocolRegistrar.Instance.GetBootstrappers(feedRequest.Protocol);
      bool canIngest = new FeedPermsFacade(reportController.TfsRequestContext).HasPermissions(feedRequest.Feed, FeedPermissionConstants.AddUpstreamPackage);
      MetadataDocument<IMetadataEntry> withoutRefreshAsync = await (await bootstrappers.GetReadMetadataDocumentServiceFactoryBootstrapper(reportController.TfsRequestContext).Bootstrap().Get(feedRequest)).GetGenericUnfilteredPackageVersionStatesDocumentWithoutRefreshAsync((IPackageNameRequest) packageNameRequest);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("WARNING: This API is temporary, experimental, undocumented, provided AS-IS, for diagnostic purposes only and may change or be removed at any time without notice. Do not take a dependency on it.");
      stringBuilder.AppendLine();
      if (!canIngest)
      {
        stringBuilder.AppendLine("**You do not have the Collaborator, Contributor or Owner role on this feed.** You will not be able to see or ingest versions that have not been ingested into the feed. If you are having trouble getting versions that you know are available in an upstream, this is likely why!");
        stringBuilder.AppendLine();
      }
      stringBuilder.AppendLine("Note: Upstream information was not refreshed as part of this request. Only cached information is shown.");
      stringBuilder.AppendLine();
      stringBuilder.AppendLine("# Versions available in the feed");
      if (withoutRefreshAsync != null && withoutRefreshAsync.Entries.Any<IMetadataEntry>())
      {
        StringBuilder stringBuilder1 = stringBuilder;
        DateTime? nullable = withoutRefreshAsync.Properties.UpstreamsLastRefreshedUtc;
        ref DateTime? local1 = ref nullable;
        DateTime valueOrDefault;
        string str1;
        if (!local1.HasValue)
        {
          str1 = (string) null;
        }
        else
        {
          valueOrDefault = local1.GetValueOrDefault();
          str1 = valueOrDefault.ToString("u");
        }
        if (str1 == null)
          str1 = "never";
        string str2 = "Upstream information last refreshed: " + str1 + " (may or may not have been a complete refresh)";
        stringBuilder1.AppendLine(str2);
        stringBuilder.AppendLine();
        foreach (IMetadataEntry entry in withoutRefreshAsync.Entries)
        {
          stringBuilder.AppendLine("* " + entry.PackageIdentity.Version.DisplayVersion);
          if (entry.IsPermanentlyDeleted())
          {
            stringBuilder.AppendLine("  - Version has been permanently deleted.");
            StringBuilder stringBuilder2 = stringBuilder;
            nullable = entry.DeletedDate;
            ref DateTime? local2 = ref nullable;
            string str3;
            if (!local2.HasValue)
            {
              str3 = (string) null;
            }
            else
            {
              valueOrDefault = local2.GetValueOrDefault();
              str3 = valueOrDefault.ToString("u");
            }
            if (str3 == null)
              str3 = "(unknown)";
            string str4 = "  - Soft-deleted at " + str3 + ".";
            stringBuilder2.AppendLine(str4);
            StringBuilder stringBuilder3 = stringBuilder;
            nullable = entry.ScheduledPermanentDeleteDate;
            ref DateTime? local3 = ref nullable;
            string str5;
            if (!local3.HasValue)
            {
              str5 = (string) null;
            }
            else
            {
              valueOrDefault = local3.GetValueOrDefault();
              str5 = valueOrDefault.ToString("u");
            }
            if (str5 == null)
              str5 = "(unknown)";
            string str6 = "  - Scheduled for permanent deletion at " + str5 + ".";
            stringBuilder3.AppendLine(str6);
            StringBuilder stringBuilder4 = stringBuilder;
            nullable = entry.PermanentDeletedDate;
            ref DateTime? local4 = ref nullable;
            string str7;
            if (!local4.HasValue)
            {
              str7 = (string) null;
            }
            else
            {
              valueOrDefault = local4.GetValueOrDefault();
              str7 = valueOrDefault.ToString("u");
            }
            if (str7 == null)
              str7 = "(unknown)";
            string str8 = "  - Permanently deleted at " + str7;
            stringBuilder4.AppendLine(str8);
          }
          else if (entry.IsDeleted())
          {
            stringBuilder.AppendLine("  - Version has been soft-deleted");
            StringBuilder stringBuilder5 = stringBuilder;
            nullable = entry.DeletedDate;
            ref DateTime? local5 = ref nullable;
            string str9;
            if (!local5.HasValue)
            {
              str9 = (string) null;
            }
            else
            {
              valueOrDefault = local5.GetValueOrDefault();
              str9 = valueOrDefault.ToString("u");
            }
            if (str9 == null)
              str9 = "(unknown)";
            string str10 = "  - Soft-deleted at " + str9 + ".";
            stringBuilder5.AppendLine(str10);
            StringBuilder stringBuilder6 = stringBuilder;
            nullable = entry.ScheduledPermanentDeleteDate;
            ref DateTime? local6 = ref nullable;
            string str11;
            if (!local6.HasValue)
            {
              str11 = (string) null;
            }
            else
            {
              valueOrDefault = local6.GetValueOrDefault();
              str11 = valueOrDefault.ToString("u");
            }
            if (str11 == null)
              str11 = "(unknown)";
            string str12 = "  - Scheduled for permanent deletion at " + str11 + ".";
            stringBuilder6.AppendLine(str12);
          }
          else
          {
            if (entry.IsLocal)
            {
              stringBuilder.AppendLine("  - Version is local and available to all Readers, Collaborators, Contributors and Owners of this feed");
              stringBuilder.AppendLine(string.Format("  - Version was ingested ({0}) at {1:u}", entry.IsFromUpstream ? (object) "from upstream" : (object) "direct push", (object) entry.CreatedDate));
              stringBuilder.AppendLine("  - Source chain:");
              int num = 1;
              foreach (UpstreamSourceInfo upstreamSourceInfo in entry.SourceChain)
              {
                stringBuilder.AppendLine(string.Format("    {0}. {1} - {2}", (object) num, (object) upstreamSourceInfo.Name, (object) upstreamSourceInfo.Location));
                ++num;
              }
            }
            else
              stringBuilder.AppendLine("  - Version is available from upstream but has not been ingested. It is NOT available to Readers" + (canIngest ? "" : " (INCLUDING YOU)") + ". It is available to Collaborators, Contributors, and Owners.");
            stringBuilder.AppendLine();
          }
        }
      }
      else
        stringBuilder.AppendLine("No versions of the package are available in the feed");
      stringBuilder.AppendLine();
      stringBuilder.AppendLine();
      stringBuilder.AppendLine();
      IUpstreamVersionListFile<IPackageVersion> versionListDocument = await (await bootstrappers.GetUpstreamVersionListServiceFactoryBootstrapper(reportController.TfsRequestContext).Bootstrap().Get(feedRequest)).GetGenericUpstreamVersionListDocument(packageNameRequest);
      stringBuilder.AppendLine("# All versions known to this feed to be in its upstreams (in no particular order)");
      stringBuilder.AppendLine();
      foreach (IUpstreamVersionListFileUpstream<IPackageVersion> upstream in (IEnumerable<IUpstreamVersionListFileUpstream<IPackageVersion>>) versionListDocument.Upstreams)
      {
        stringBuilder.AppendLine("## Upstream: " + upstream.UpstreamSourceInfo.Name);
        stringBuilder.AppendLine("Location: " + upstream.UpstreamSourceInfo.Location);
        stringBuilder.AppendLine();
        stringBuilder.AppendLine(string.Format("Version list last refreshed: {0:u}", (object) upstream.LastRefreshed));
        stringBuilder.AppendLine();
        foreach (IVersionWithSourceChain<IPackageVersion> version in (IEnumerable<IVersionWithSourceChain<IPackageVersion>>) upstream.Versions)
        {
          stringBuilder.AppendLine("* " + version.Version.DisplayVersion);
          stringBuilder.AppendLine("  - Source chain:");
          int num = 1;
          foreach (UpstreamSourceInfo upstreamSourceInfo in version.SourceChain.Prepend<UpstreamSourceInfo>(upstream.UpstreamSourceInfo))
          {
            stringBuilder.AppendLine(string.Format("    {0}. {1} - {2}", (object) num, (object) upstreamSourceInfo.Name, (object) upstreamSourceInfo.Location));
            ++num;
          }
          stringBuilder.AppendLine();
        }
        stringBuilder.AppendLine();
        stringBuilder.AppendLine();
        stringBuilder.AppendLine();
      }
      HttpResponseMessage diagnosticReportAsync = new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = (HttpContent) new StringContent(stringBuilder.ToString(), Encoding.UTF8, "text/plain")
      };
      feedRequest = (IFeedRequest) null;
      packageNameRequest = (IPackageNameRequest<IPackageName>) null;
      bootstrappers = (IRequiredProtocolBootstrappers) null;
      stringBuilder = (StringBuilder) null;
      return diagnosticReportAsync;
    }
  }
}
