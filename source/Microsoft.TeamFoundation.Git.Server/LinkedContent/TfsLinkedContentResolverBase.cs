// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.LinkedContent.TfsLinkedContentResolverBase
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Streams;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Server.LinkedContent
{
  public abstract class TfsLinkedContentResolverBase : ITfsLinkedContentResolver
  {
    protected readonly IVssRequestContext RequestContext;
    protected readonly string LinkPrefix;
    protected Encoding ContentEncoding = GitEncodingUtil.SafeUtf8NoBom;

    public TfsLinkedContentResolverBase(IVssRequestContext requestContext, string linkPrefix)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(linkPrefix, nameof (linkPrefix));
      this.RequestContext = requestContext;
      this.LinkPrefix = linkPrefix;
    }

    public virtual bool CanResolve(IRewindableStream contentStream)
    {
      this.RequestContext.TraceEnter(10139370, GitServerUtils.TraceArea, nameof (TfsLinkedContentResolverBase), nameof (CanResolve));
      ArgumentUtility.CheckForNull<IRewindableStream>(contentStream, nameof (contentStream));
      if (string.IsNullOrWhiteSpace(this.LinkPrefix))
      {
        this.RequestContext.Trace(10139371, TraceLevel.Info, GitServerUtils.TraceArea, nameof (TfsLinkedContentResolverBase), "LinkPrefix is empty, resolution isn't possible");
        return false;
      }
      bool flag = false;
      int length1 = this.LinkPrefix.Length;
      try
      {
        using (StreamReader streamReader = new StreamReader((Stream) contentStream, this.ContentEncoding, true, length1, true))
        {
          char[] buffer = new char[length1];
          int length2 = streamReader.Read(buffer, 0, length1);
          if (length2 > 0)
          {
            string str = new string(buffer, 0, length2);
            flag = !string.IsNullOrWhiteSpace(str) && str.StartsWith(this.LinkPrefix);
          }
          else
            this.RequestContext.Trace(10139371, TraceLevel.Info, GitServerUtils.TraceArea, nameof (TfsLinkedContentResolverBase), "No bytes read");
        }
      }
      catch (Exception ex)
      {
        this.RequestContext.Trace(10139371, TraceLevel.Info, GitServerUtils.TraceArea, nameof (TfsLinkedContentResolverBase), "Failed to read linked content header");
        this.RequestContext.TraceException(10139371, TraceLevel.Info, GitServerUtils.TraceArea, nameof (TfsLinkedContentResolverBase), ex);
      }
      finally
      {
        contentStream.Restart();
      }
      this.RequestContext.Trace(10139372, TraceLevel.Info, GitServerUtils.TraceArea, nameof (TfsLinkedContentResolverBase), string.Format("{0} - Content resolution for {1}: {2}", (object) this.GetType(), (object) this.LinkPrefix, (object) flag));
      this.RequestContext.TraceLeave(10139370, GitServerUtils.TraceArea, nameof (TfsLinkedContentResolverBase), nameof (CanResolve));
      return flag;
    }

    public abstract Stream Resolve(RepoKey repoKey, Stream contentStream);
  }
}
