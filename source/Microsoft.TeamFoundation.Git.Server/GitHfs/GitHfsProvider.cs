// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitHfs.GitHfsProvider
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.TeamFoundation.Git.Server.LinkedContent;
using Microsoft.TeamFoundation.Git.Server.Streams;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Git.Server.GitHfs
{
  public class GitHfsProvider : TfsLinkedContentResolverBase
  {
    private const string c_gitHfsPrefix = "~~Git-hfs";
    private const int c_defaultFileReadSize = 120;

    public GitHfsProvider(IVssRequestContext requestContext)
      : base(requestContext, "~~Git-hfs")
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
    }

    public override Stream Resolve(RepoKey repoKey, Stream stream)
    {
      if (!(stream is IRewindableStream rewindableStream1))
        rewindableStream1 = (IRewindableStream) new RewindableStream(stream);
      IRewindableStream rewindableStream2 = rewindableStream1;
      int length1 = "~~Git-hfs".Length;
      using (StreamReader streamReader = new StreamReader((Stream) rewindableStream2, GitEncodingUtil.SafeUtf8NoBom, true, length1, true))
      {
        char[] buffer1 = new char[length1];
        int length2 = streamReader.Read(buffer1, 0, length1);
        if (length2 > 0)
        {
          string str1 = new string(buffer1, 0, length2);
          if (str1 != null)
          {
            if (str1.StartsWith("~~Git-hfs"))
            {
              int count = this.RequestContext.GetService<IVssRegistryService>().GetValue<int>(this.RequestContext, (RegistryQuery) "/Service/Git/Settings/GitHfsFileReadSize", 120);
              char[] buffer2 = new char[count];
              int length3 = streamReader.Read(buffer2, 0, count);
              string str2 = new string(buffer2, 0, length3);
              try
              {
                return this.GetBlobStream(str1 + str2);
              }
              catch (GitArgumentException ex)
              {
                this.RequestContext.TraceException(10139373, TraceLevel.Error, GitServerUtils.TraceArea, nameof (GitHfsProvider), (Exception) ex);
              }
            }
          }
        }
      }
      rewindableStream2.Restart();
      return (Stream) rewindableStream2;
    }

    private Stream GetBlobStream(string hfsContent)
    {
      string pattern = "blob:(\\w+)\\s*size:(\\d+)";
      System.Text.RegularExpressions.Match match = Regex.Match(hfsContent, pattern);
      if (!match.Success || match.Groups.Count != 3)
      {
        this.RequestContext.Trace(10139373, TraceLevel.Error, GitServerUtils.TraceArea, nameof (GitHfsProvider), "Unable to parse GitHfs file");
        throw new GitArgumentException(Microsoft.TeamFoundation.Git.Server.Resources.Format("GitHfsFileParseError"));
      }
      string valueIncludingAlgorithm = match.Groups[1].Value;
      try
      {
        BlobIdentifier blobIdentifier = BlobIdentifier.Deserialize(valueIncludingAlgorithm);
        DomainBlobstoreHttpClient client1 = this.RequestContext.GetClient<DomainBlobstoreHttpClient>();
        DomainBlobHttpClientWrapper client = new DomainBlobHttpClientWrapper(WellKnownDomainIds.DefaultDomainId, (IDomainBlobStoreHttpClient) client1);
        return this.RequestContext.RunSynchronously<Stream>((Func<Task<Stream>>) (() => client.GetBlobAsync(blobIdentifier, CancellationToken.None))) ?? throw new GitArgumentException(Microsoft.TeamFoundation.Git.Server.Resources.Format("GitHfsBlobDoesNotExistError"));
      }
      catch (Exception ex)
      {
        if (ex.GetType() == typeof (GitArgumentException))
        {
          this.RequestContext.Trace(10139373, TraceLevel.Error, GitServerUtils.TraceArea, nameof (GitHfsProvider), "Blob file does not exist");
          this.RequestContext.TraceException(10139373, TraceLevel.Error, GitServerUtils.TraceArea, nameof (GitHfsProvider), ex);
          throw new GitArgumentException(Microsoft.TeamFoundation.Git.Server.Resources.Format("GitHfsBlobDoesNotExistError"));
        }
        this.RequestContext.Trace(10139373, TraceLevel.Error, GitServerUtils.TraceArea, nameof (GitHfsProvider), "Unable to download file for display");
        this.RequestContext.TraceException(10139373, TraceLevel.Error, GitServerUtils.TraceArea, nameof (GitHfsProvider), ex);
        throw new GitArgumentException(Microsoft.TeamFoundation.Git.Server.Resources.Format("GitHfsFileDownloadError"));
      }
    }
  }
}
