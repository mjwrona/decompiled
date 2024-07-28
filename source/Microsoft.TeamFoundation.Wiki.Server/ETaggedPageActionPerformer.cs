// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.ETaggedPageActionPerformer
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public class ETaggedPageActionPerformer
  {
    protected ITfsGitRepository repository;
    protected string pagePath;
    protected Func<string> eTagCalculator;
    private Func<TfsGitRefUpdateResultSet> onAddAllowed;
    private Func<TfsGitRefUpdateResultSet> onEditAllowed;

    public ETaggedPageActionPerformer(
      ITfsGitRepository repository,
      string pagePath,
      Func<string> eTagCalculator,
      Func<TfsGitRefUpdateResultSet> onAddAllowed,
      Func<TfsGitRefUpdateResultSet> onEditAllowed)
    {
      this.repository = repository;
      this.pagePath = pagePath;
      this.eTagCalculator = eTagCalculator;
      this.onAddAllowed = onAddAllowed;
      this.onEditAllowed = onEditAllowed;
    }

    public TfsGitRefUpdateResultSet PerformAction(
      IVssRequestContext requestContext,
      HttpRequestMessage request)
    {
      IEnumerable<string> values;
      request.Headers.TryGetValues("If-Match", out values);
      string str = this.eTagCalculator();
      string sha1IdString = values != null ? values.First<string>()?.ToString() : (string) null;
      if (sha1IdString != null && sha1IdString.StartsWith("\"") && sha1IdString.EndsWith("\""))
        sha1IdString = sha1IdString.Substring(1, sha1IdString.Length - 2);
      if (sha1IdString == null)
      {
        if (str == null)
          return this.onAddAllowed();
        throw new WikiPageAlreadyExistsException(string.Format(Resources.ErrorMessagePageAlreadyExists, (object) PathHelper.GetPageReadablePath(this.pagePath, "/")));
      }
      if (!Sha1Id.TryParse(sha1IdString, out Sha1Id _))
        throw new InvalidArgumentValueException("IfMatch", Resources.ErrorMessagePreConditionHeaderInvalid);
      if (str == null)
        throw new WikiPageNotFoundException(string.Format(Resources.ErrorMessage_PageNotFound, (object) PathHelper.GetPageReadablePath(this.pagePath, "/")));
      if (!str.Equals(sha1IdString, StringComparison.OrdinalIgnoreCase))
        throw new WikiPageHasConflictsException(string.Format(Resources.ErrorMessagePageHasConflicts, (object) PathHelper.GetPageReadablePath(this.pagePath, "/")));
      return this.onEditAllowed();
    }
  }
}
