// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Utilities.MavenUrlUtility
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Maven.Server.Utilities
{
  public static class MavenUrlUtility
  {
    public static ReferenceLink GetUrlForIndex(
      IVssRequestContext context,
      FeedCore feed,
      MavenPackageName name,
      MavenPackageVersion version = null)
    {
      return MavenUrlUtility.RenderUriForMetadataResource(context, feed, name, version, (string) null);
    }

    public static ReferenceLink GetUrlForFile(
      IVssRequestContext context,
      FeedCore feed,
      MavenPackageName name,
      MavenPackageVersion version,
      string filePath)
    {
      return MavenUrlUtility.RenderUriForDefaultResource(context, feed, name, version, filePath);
    }

    public static ReferenceLink GetUrlForMetadataFile(
      IVssRequestContext context,
      FeedCore feed,
      MavenPackageName name,
      MavenPackageVersion version = null)
    {
      return MavenUrlUtility.RenderUriForDefaultResource(context, feed, name, version, "maven-metadata.xml");
    }

    private static ReferenceLink RenderUriForDefaultResource(
      IVssRequestContext context,
      FeedCore feed,
      MavenPackageName name,
      MavenPackageVersion version,
      string fileName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<MavenPackageName>(name, nameof (name));
      IList<string> values = (IList<string>) new List<string>();
      values.Add(name.GroupId.Replace('.', '/'));
      values.Add(name.ArtifactId);
      if (version != null)
        values.Add(version.DisplayVersion);
      if (!string.IsNullOrWhiteSpace(fileName))
        values.Add(fileName);
      return MavenUrlUtility.GetLocationService(context).GetResourceUri(context, "maven", ResourceIds.MavenDefaultResourceId, feed.Project.ToProjectIdOrEmptyGuid(), (object) new
      {
        feed = feed.Id,
        path = string.Join("/", (IEnumerable<string>) values)
      }).ToReferenceLink();
    }

    private static ReferenceLink RenderUriForMetadataResource(
      IVssRequestContext context,
      FeedCore feed,
      MavenPackageName name,
      MavenPackageVersion version,
      string fileName)
    {
      ArgumentUtility.CheckForNull<MavenPackageName>(name, nameof (name));
      return MavenUrlUtility.GetLocationService(context).GetResourceUri(context, "maven", ResourceIds.MavenMetadataResourceId, feed.Project.ToProjectIdOrEmptyGuid(), (object) new
      {
        feed = feed.Id,
        groupId = name.GroupId,
        artifactId = name.ArtifactId,
        version = version?.DisplayVersion,
        fileName = fileName
      }).ToReferenceLink();
    }

    private static ReferenceLink ToReferenceLink(this Uri link) => new ReferenceLink()
    {
      Href = link.ToString()
    };

    private static ILocationService GetLocationService(IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      return context.GetService<ILocationService>();
    }
  }
}
