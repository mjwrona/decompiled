// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitServerUtils
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Bitmap;
using Microsoft.TeamFoundation.Git.Server.Graph;
using Microsoft.TeamFoundation.Git.Server.Routing;
using Microsoft.TeamFoundation.Git.Server.Storage;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.Server.Compression;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.IdentityImage;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class GitServerUtils
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string GitPullRequestLayer = "TfsGitPullRequest";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string GitBuildPolicyLayer = "TfsGitBuildPolicy";
    private static bool s_canUseFileCachingService;
    private static bool s_calculatedCanUseFileCachingService;
    private static readonly string s_cachingServiceTraceLayer = "RepositoryServiceCache";
    private const string s_cloneUrlTraceLayer = "CloneUrls";

    public static void CheckForEmptySha1Id(Sha1Id id, string varName)
    {
      if (id.IsEmpty)
        throw new ArgumentOutOfRangeException(varName, Resources.Get("EmptySha1IdNotAllowed"));
    }

    public static void CheckIsLittleEndian()
    {
      if (!BitConverter.IsLittleEndian)
        throw new PlatformNotSupportedException("!IsLittleEndian");
    }

    public static void CheckOdbRequestContext(IVssRequestContext rc) => rc.CheckProjectCollectionRequestContext();

    public static long ReadElapsedMsAndRestart(this Stopwatch stopwatch)
    {
      long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
      stopwatch.Restart();
      return elapsedMilliseconds;
    }

    public static IEnumerable<T> SkipUntilAfter<T>(
      this IEnumerable<T> results,
      Predicate<T> condition)
    {
      bool foundFirst = false;
      foreach (T result in results)
      {
        if (foundFirst)
          yield return result;
        else
          foundFirst = condition(result);
      }
    }

    internal static void GetTypeAndLengthFromObjectStream(
      Stream inStream,
      out GitPackObjectType objectType,
      out long length)
    {
      byte[] bytes = new byte[28];
      int num1 = -1;
      int count = -1;
      for (int index = 0; index < 28; ++index)
      {
        int num2 = inStream.ReadByte();
        if (num2 != -1)
        {
          bytes[index] = (byte) num2;
          if (bytes[index] == (byte) 32)
            count = index;
          else if (bytes[index] == (byte) 0)
          {
            num1 = index;
            break;
          }
        }
        else
          break;
      }
      if (count == -1 || num1 == -1 || count >= num1 || count == 0)
        throw new InvalidOperationException();
      Encoding safeUtf8NoBom = GitEncodingUtil.SafeUtf8NoBom;
      string gitType = safeUtf8NoBom.GetString(bytes, 0, count);
      objectType = GitObjectTypeExtensions.GetPackType(gitType);
      if (!long.TryParse(safeUtf8NoBom.GetString(bytes, count + 1, num1 - count - 1), out length))
        throw new InvalidOperationException();
    }

    internal static Odb GetOdb(ITfsGitRepository repository) => repository is TfsGitRepository tfsGitRepository ? tfsGitRepository.Odb : throw new InvalidOperationException("Invalid repository instance, expected TfsGitRepository.");

    internal static IIsolationBitmapProvider GetIsolationBitmapProvider(ITfsGitRepository repository) => repository is TfsGitRepository tfsGitRepository ? tfsGitRepository.IsolationBitmapProvider : throw new InvalidOperationException("Invalid repository instance, expected TfsGitRepository.");

    internal static HashSet<Sha1Id> ExpandReachableObjects(
      ITfsGitRepository sourceRepo,
      IIsolationBitmapProvider targetIsolationProvider,
      ICollection<Sha1Id> objectsToExpand)
    {
      return GitServerUtils.ExpandReachableObjects(sourceRepo, targetIsolationProvider, (ICollection<TfsGitObject>) objectsToExpand.Select<Sha1Id, TfsGitObject>((Func<Sha1Id, TfsGitObject>) (x => sourceRepo.LookupObject(x))).ToList<TfsGitObject>());
    }

    internal static HashSet<Sha1Id> ExpandReachableObjects(
      ITfsGitRepository sourceRepo,
      IIsolationBitmapProvider targetIsolationProvider,
      ICollection<TfsGitObject> objectsToExpand)
    {
      HashSet<Sha1Id> addToIsolation = new HashSet<Sha1Id>();
      if (objectsToExpand.Count == 0)
        return addToIsolation;
      List<Sha1Id> sha1IdList = new List<Sha1Id>();
      HashSet<TfsGitObject> second = new HashSet<TfsGitObject>(TfsGitObjectEqualityComparer.Instance);
      foreach (TfsGitObject tag in (IEnumerable<TfsGitObject>) objectsToExpand)
      {
        switch (tag.ObjectType)
        {
          case GitObjectType.Commit:
            sha1IdList.Add(tag.ObjectId);
            continue;
          case GitObjectType.Tag:
            List<TfsGitObject> seenObjects;
            if (((TfsGitTag) tag).TryPeelToNonTag(out seenObjects))
            {
              using (List<TfsGitObject>.Enumerator enumerator = seenObjects.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  TfsGitObject current = enumerator.Current;
                  if (current.ObjectType == GitObjectType.Commit)
                    sha1IdList.Add(current.ObjectId);
                  else
                    second.Add(current);
                }
                continue;
              }
            }
            else
              continue;
          default:
            continue;
        }
      }
      IEnumerable<TfsGitObject> list = (IEnumerable<TfsGitObject>) new AncestralGraphAlgorithm<int, Sha1Id>().GetReachable((IDirectedGraph<int, Sha1Id>) sourceRepo.GetCommitGraph((IEnumerable<Sha1Id>) sha1IdList), (IEnumerable<Sha1Id>) sha1IdList).Select<Sha1Id, TfsGitObject>(new Func<Sha1Id, TfsGitObject>(((GitOdbExtensions) sourceRepo).LookupObject)).Concat<TfsGitObject>((IEnumerable<TfsGitObject>) objectsToExpand).Concat<TfsGitObject>((IEnumerable<TfsGitObject>) second).ToList<TfsGitObject>();
      ObjectWalker objectWalker = new ObjectWalker();
      IReadOnlyBitmap<Sha1Id> odbIsolation = targetIsolationProvider.GetOdb();
      IEnumerable<TfsGitObject> startObjects = list;
      Func<Sha1Id, GitObjectType, bool> walkCondition = (Func<Sha1Id, GitObjectType, bool>) ((id, objectType) => addToIsolation.Add(id) && !odbIsolation.Contains(id));
      objectWalker.Walk(startObjects, walkCondition);
      addToIsolation.AddRange<Sha1Id, HashSet<Sha1Id>>(list.Select<TfsGitObject, Sha1Id>((Func<TfsGitObject, Sha1Id>) (x => x.ObjectId)));
      return addToIsolation;
    }

    internal static ContentDB GetContentDB(ITfsGitRepository repository) => GitServerUtils.GetContentDB(repository.Objects);

    internal static ContentDB GetContentDB(IGitObjectSet odb) => odb is GitObjectSet gitObjectSet ? gitObjectSet.ContentDB : throw new InvalidOperationException("Invalid ODB instance, expected ContentDB.");

    internal static GitGraphProvider GetGraphProvider(ITfsGitRepository repo)
    {
      if (!(repo is TfsGitRepository tfsGitRepository))
        throw new InvalidOperationException("Invalid ODB instance, expected TfsGitRepository.");
      return tfsGitRepository.Odb.GraphProvider;
    }

    internal static byte[] CreateObjectHeader(GitPackObjectType objectType, long length)
    {
      string gitString = objectType.ToGitString();
      StringBuilder stringBuilder = new StringBuilder(gitString.Length + 10);
      stringBuilder.Append(gitString);
      stringBuilder.Append(' ');
      stringBuilder.Append(length.ToString("d"));
      string s = stringBuilder.ToString();
      byte[] bytes = new byte[Encoding.UTF8.GetByteCount(s) + 1];
      Encoding.UTF8.GetBytes(s, 0, s.Length, bytes, 0);
      return bytes;
    }

    internal static (GitPackObjectType type, long length) ReadLooseObjectHeader(
      Stream inflatedStream)
    {
      using (ByteArray byteArray = new ByteArray(32))
      {
        int count = 0;
        int num;
        while ((num = inflatedStream.ReadByte()) != 0 && num != -1)
        {
          if (count == byteArray.SizeRequested)
            throw new InvalidOperationException("Loose object header is not valid");
          byteArray.Bytes[count] = (byte) num;
          ++count;
        }
        string[] strArray = GitEncodingUtil.SafeUtf8NoBom.GetString(byteArray.Bytes, 0, count).Split(' ');
        return strArray.Length == 2 ? (GitObjectTypeExtensions.GetPackType(strArray[0]), long.Parse(strArray[1])) : throw new InvalidOperationException("Loose object header is not in the form: \"<type> <length>\"");
      }
    }

    internal static int ReadPackHeader(Stream stream)
    {
      byte[] numArray = new byte[4];
      if (GitStreamUtil.TryReadGreedy(stream, numArray, 0, numArray.Length) < numArray.Length || !string.Equals(Encoding.UTF8.GetString(numArray), "PACK"))
        throw new InvalidGitPackHeaderException();
      if (GitStreamUtil.TryReadGreedy(stream, numArray, 0, numArray.Length) < numArray.Length || 2 != IPAddress.NetworkToHostOrder(BitConverter.ToInt32(numArray, 0)))
        throw new InvalidGitPackHeaderException();
      if (GitStreamUtil.TryReadGreedy(stream, numArray, 0, numArray.Length) < numArray.Length)
        throw new InvalidGitPackHeaderException();
      int hostOrder = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(numArray, 0));
      return hostOrder >= 0 ? hostOrder : throw new InvalidGitPackHeaderException();
    }

    internal static void ReadPackEntryHeader(
      Stream stream,
      out GitPackObjectType type,
      out long uncompressedSize)
    {
      int num1 = stream.ReadByte();
      if (num1 < 0)
        throw new InvalidGitPackEntryHeaderException("EOF at first byte of pack entry header");
      type = (GitPackObjectType) ((num1 & 112) >> 4);
      uncompressedSize = (long) (num1 & 15);
      if (type == GitPackObjectType.None || GitPackObjectType.Reserved == type)
        throw new InvalidGitPackEntryHeaderException(string.Format("Invalid GitPackObjectType {0}", (object) type));
      int num2 = 0;
      while ((num1 & 128) != 0)
      {
        num1 = stream.ReadByte();
        if (num1 < 0)
          throw new InvalidGitPackEntryHeaderException(string.Format("EOF during variable-length header read. {0} bytes read.", (object) num2));
        uncompressedSize += (long) (num1 & (int) sbyte.MaxValue) << 4 + 7 * num2;
        if (uncompressedSize < 0L)
          throw new InvalidGitPackEntryHeaderException("Pack entry header's uncompressed size overflowed");
        ++num2;
      }
    }

    internal static long ReadOfsDeltaOffset(Stream stream)
    {
      int num1 = stream.ReadByte();
      if (num1 < 0)
        throw new InvalidGitPackEntryHeaderException(FormattableString.Invariant(FormattableStringFactory.Create("{0} < 0", (object) "headerByte")));
      long num2 = (long) (num1 & (int) sbyte.MaxValue);
      int num3 = 0;
      while ((num1 & 128) != 0)
      {
        num1 = stream.ReadByte();
        if (num1 < 0)
          throw new InvalidGitPackEntryHeaderException(FormattableString.Invariant(FormattableStringFactory.Create("{0} < 0", (object) "headerByte")));
        num2 = (num2 + 1L << 7) + (long) (num1 & (int) sbyte.MaxValue);
        if (num2 < 0L)
          throw new InvalidGitPackEntryHeaderException(FormattableString.Invariant(FormattableStringFactory.Create("{0} < 0", (object) "offset")));
        ++num3;
      }
      return num2;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string TraceArea => "Git";

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string GitPolicyArea => "GitPolicy";

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string GitPolicyGenericLayer => "GitPolicyGeneric";

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string GitCodeReviewArea => "GitCodeReview";

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string GitImportLayer => "GitImport";

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string FileContainerArtifactUriPrefix => "vstfs:///Git/GitOdbStorage/";

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string LegacyFileContainerArtifactUriPrefix => "vstfs:///Git/GitStorage/";

    public static bool IsConnected(
      IVssRequestContext requestContext,
      ITfsGitRepository objectDb,
      IEnumerable<Sha1Id> ancestors,
      Sha1Id objectId)
    {
      CommitIdSet ancestors1 = new CommitIdSet();
      foreach (Sha1Id ancestor in ancestors)
      {
        TfsGitCommit commit = objectDb.TryLookupObject(ancestor).TryResolveToCommit();
        if (commit != null)
          ancestors1.Add(ancestor, commit.GetCommitter().Time);
      }
      return GitServerUtils.IsConnected(requestContext, objectDb, ancestors1, objectId);
    }

    internal static bool IsConnected(
      IVssRequestContext requestContext,
      ITfsGitRepository objectDb,
      CommitIdSet ancestors,
      Sha1Id objectId)
    {
      if (ancestors.Count == 0)
        return false;
      TfsGitCommit commit = objectDb.LookupObject(objectId).TryResolveToCommit();
      if (commit == null)
        return false;
      IDirectedGraph<int, Sha1Id> graph = (IDirectedGraph<int, Sha1Id>) objectDb.GetCommitGraph((IEnumerable<Sha1Id>) new Sha1Id[1]
      {
        commit.ObjectId
      });
      List<Sha1Id> list = ancestors.Where<Sha1Id>((Func<Sha1Id, bool>) (id => graph.HasVertex(id))).ToList<Sha1Id>();
      return new AncestralGraphAlgorithm<int, Sha1Id>().CanReach(graph, commit.ObjectId, (IEnumerable<Sha1Id>) list);
    }

    public static bool TreesMatch(
      IVssRequestContext requestContext,
      ITfsGitRepository objectDb,
      Sha1Id? commitIdA,
      Sha1Id? commitIdB)
    {
      Sha1Id objectId1 = commitIdA ?? Sha1Id.Empty;
      Sha1Id objectId2 = commitIdB ?? Sha1Id.Empty;
      if (objectId1 == objectId2)
        return true;
      TfsGitCommit tfsGitCommit1 = objectDb.TryLookupObject<TfsGitCommit>(objectId1);
      TfsGitCommit tfsGitCommit2 = objectDb.TryLookupObject<TfsGitCommit>(objectId2);
      return tfsGitCommit1 != null && tfsGitCommit2 != null && tfsGitCommit1.GetTree().ObjectId == tfsGitCommit2.GetTree().ObjectId;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetProjectUriFilter(
      IVssRequestContext requestContext,
      string projFilter,
      bool throwOnProjectNotFound = false)
    {
      return GitServerUtils.GetProjectUriFilter(requestContext, projFilter, out ProjectInfo _, throwOnProjectNotFound);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetProjectUriFilter(
      IVssRequestContext requestContext,
      string projFilter,
      out ProjectInfo projectInfo,
      bool throwOnProjectNotFound = false)
    {
      projectInfo = GitServerUtils.GetProjectInfo(requestContext, projFilter, throwOnProjectNotFound);
      return projectInfo != null ? projectInfo.Uri : (string) null;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ProjectInfo GetProjectInfo(
      IVssRequestContext requestContext,
      string projFilter,
      bool throwOnProjectNotFound)
    {
      if (projFilter == null)
        return (ProjectInfo) null;
      IProjectService service = requestContext.GetService<IProjectService>();
      ProjectInfo projectInfo = (ProjectInfo) null;
      Guid result;
      if (!Guid.TryParse(projFilter, out result))
      {
        try
        {
          projectInfo = service.GetProject(requestContext, projFilter);
        }
        catch (ProjectDoesNotExistWithNameException ex)
        {
          if (throwOnProjectNotFound)
            throw;
        }
      }
      else
      {
        try
        {
          projectInfo = service.GetProject(requestContext, result);
        }
        catch (ProjectDoesNotExistException ex)
        {
          if (throwOnProjectNotFound)
            throw;
        }
      }
      return projectInfo;
    }

    public static ITfsGitRepository FindRepositoryByFilters(
      IVssRequestContext requestContext,
      string repositoryFilter,
      string projectUriFilter = null,
      bool allowReadByAdmins = false)
    {
      ITeamFoundationGitRepositoryService service = requestContext.GetService<ITeamFoundationGitRepositoryService>();
      Guid result;
      ITfsGitRepository repositoryByFilters;
      if (Guid.TryParse(repositoryFilter, out result) && string.IsNullOrEmpty(projectUriFilter))
      {
        repositoryByFilters = service.FindRepositoryById(requestContext, result, allowReadByAdmins);
      }
      else
      {
        if (string.IsNullOrEmpty(projectUriFilter))
          throw new ArgumentException(Resources.Get("GitRepositoryProjectNameRequired")).Expected("git");
        repositoryByFilters = service.FindRepositoryByNameAndUri(requestContext, projectUriFilter, repositoryFilter, allowReadByAdmins);
      }
      return repositoryByFilters;
    }

    public static string GetTagWebUrl(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string tagName)
    {
      string publicBaseUrl = GitServerUtils.GetPublicBaseUrl(requestContext);
      return GitServerUtils.GetRepositoryWebUrl(requestContext, publicBaseUrl, projectName, repositoryName) + "#version=GT" + Uri.EscapeDataString(tagName.Substring("refs/tags/".Length));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetRepositoryCloneUrl(
      IVssRequestContext requestContext,
      string baseUrl,
      string projectName,
      string repositoryName,
      string gitArea = "_git")
    {
      string repositoryCloneUrl = GitServerUtils.GetRepositoryWebUrl(requestContext, baseUrl, projectName, repositoryName, gitArea);
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsFeatureEnabled("Git.HttpCloneUrl.PrependCodexWithOrgName"))
      {
        string locationServiceUrl = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, Guid.Empty, AccessMappingConstants.DevOpsAccessMapping);
        Uri result = (Uri) null;
        if (!Uri.TryCreate(locationServiceUrl, UriKind.Absolute, out result))
          requestContext.TraceAlways(1013842, TraceLevel.Error, GitServerUtils.TraceArea, "CloneUrls", "LocationService returned an invalid URI: " + locationServiceUrl);
        Uri uri = new Uri(repositoryCloneUrl);
        if (result?.Host == uri.Host)
          repositoryCloneUrl = new UriBuilder(repositoryCloneUrl)
          {
            UserName = requestContext.ServiceHost.Name
          }.Uri.AbsoluteUri;
      }
      return repositoryCloneUrl;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetRepositoryWebUrl(
      IVssRequestContext requestContext,
      string baseUrl,
      string projectName,
      string repositoryName,
      string gitArea = "_git")
    {
      string part2 = UriUtility.CombinePath(Uri.EscapeDataString(projectName), UriUtility.CombinePath(gitArea, Uri.EscapeDataString(repositoryName)));
      return UriUtility.CombinePath(baseUrl, part2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetSshUrl(
      IVssRequestContext requestContext,
      string repositoryRemoteUrl,
      out bool sshEnabled)
    {
      SshSettings sshSettings = SshSettings.TryLoadIfEnabled(requestContext);
      if (sshSettings == null)
      {
        sshEnabled = false;
        return (string) null;
      }
      sshEnabled = true;
      int port = sshSettings.Port;
      Uri uri = new Uri(repositoryRemoteUrl);
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      string empty3 = string.Empty;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        string locationServiceUrl = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, Guid.Empty, AccessMappingConstants.DevOpsAccessMapping);
        Uri result = (Uri) null;
        if (!Uri.TryCreate(locationServiceUrl, UriKind.Absolute, out result))
          requestContext.TraceAlways(1013842, TraceLevel.Error, GitServerUtils.TraceArea, "CloneUrls", "LocationService returned an invalid URI: " + locationServiceUrl);
        int num = result?.Host == uri.Host ? 1 : 0;
        string str1 = Regex.Replace(Regex.Replace(uri.LocalPath, "/_git/", "/", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant), "/DefaultCollection/", "/", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        string empty4 = string.Empty;
        string str2 = string.Empty;
        string str3;
        string str4;
        if (num != 0)
        {
          str3 = "ssh." + uri.Host;
          str4 = "git";
        }
        else
        {
          str2 = "/" + requestContext.ServiceHost.Name;
          str3 = sshSettings.Host;
          str4 = requestContext.ServiceHost.Name;
        }
        string stringToEscape;
        if (port == 22)
          stringToEscape = string.Format("{0}@{1}:v{2}{3}{4}", (object) str4, (object) str3, (object) 3, (object) str2, (object) str1);
        else
          stringToEscape = string.Format("ssh://{0}@{1}:{2}/v{3}{4}{5}", (object) str4, (object) str3, (object) port, (object) 3, (object) str2, (object) str1);
        return Uri.EscapeUriString(stringToEscape);
      }
      string host = uri.Host;
      string localPath = uri.LocalPath;
      return new UriBuilder(uri)
      {
        Host = host,
        Port = port,
        Scheme = "ssh",
        Path = localPath,
        UserName = empty3
      }.Uri.AbsoluteUri;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetPublicBaseUrl(
      IVssRequestContext requestContext,
      bool appendDefaultCollection = false)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      string locationServiceUrl = service.GetLocationServiceUrl(requestContext, Guid.Empty, AccessMappingConstants.DevOpsAccessMapping);
      string accessMappingMoniker = AccessMappingConstants.ClientAccessMappingMoniker;
      string uriString = service.GetLocationServiceUrl(requestContext, Guid.Empty, accessMappingMoniker);
      Uri result = (Uri) null;
      if (!Uri.TryCreate(uriString, UriKind.Absolute, out result))
        requestContext.TraceAlways(1013842, TraceLevel.Error, GitServerUtils.TraceArea, "CloneUrls", "LocationService returned an invalid URI: '" + (uriString ?? "<null>") + "' for access mapping '" + accessMappingMoniker + "'");
      if (appendDefaultCollection && (uriString == null || !uriString.Equals(locationServiceUrl, StringComparison.OrdinalIgnoreCase)) && requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, in UrlHostResolutionConstants.VsUsesLegacyDefaultCollectionRoutingQuery, false))
      {
        AccessMapping accessMapping = service.GetAccessMapping(requestContext, AccessMappingConstants.VstsAccessMapping);
        if (accessMapping != null)
        {
          accessMapping.VirtualDirectory = "DefaultCollection";
          uriString = service.GetSelfReferenceUrl(requestContext, accessMapping);
        }
        else
          requestContext.TraceAlways(1013843, TraceLevel.Error, GitServerUtils.TraceArea, nameof (GetPublicBaseUrl), "VSTS Access Mapping not found");
      }
      if (uriString == null)
        uriString = service.GetLocationServiceUrl(requestContext, Guid.Empty, AccessMappingConstants.PublicAccessMappingMoniker);
      if (uriString == null)
      {
        requestContext.TraceAlways(1013842, TraceLevel.Error, GitServerUtils.TraceArea, "CloneUrls", "Couldn't get a URL from location service, falling back to constructing it ourselves.");
        Uri uri = requestContext.RequestUri();
        uriString = new UriBuilder()
        {
          Host = uri.Host,
          Path = requestContext.VirtualPath()
        }.Uri.AbsoluteUri;
      }
      return uriString;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetCacheDirectory(IVssRequestContext rc, Guid repoOrOdbId)
    {
      GitServerUtils.CheckOdbRequestContext(rc);
      string path = Path.Combine(!GitServerUtils.CanUseFileCachingService(rc) ? FileSpec.GetTempDirectory() : rc.GetService<ITeamFoundationFileCacheService>().Configuration.CacheRoot, rc.ServiceHost.InstanceId.ToString(), "git-" + repoOrOdbId.ToString("N"));
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
      return path;
    }

    internal static bool CanUseFileCachingService(IVssRequestContext requestContext)
    {
      if (!GitServerUtils.s_calculatedCanUseFileCachingService)
      {
        lock (GitServerUtils.s_cachingServiceTraceLayer)
        {
          if (!GitServerUtils.s_calculatedCanUseFileCachingService)
          {
            requestContext.Trace(1013143, TraceLevel.Verbose, GitServerUtils.TraceArea, GitServerUtils.s_cachingServiceTraceLayer, "Determining if we can use the file caching service.");
            ITeamFoundationFileCacheService service = requestContext.GetService<ITeamFoundationFileCacheService>();
            if (service == null || !service.IsGitCacheEnabled || service.Configuration == null || string.IsNullOrEmpty(service.Configuration.CacheRoot))
            {
              GitServerUtils.s_calculatedCanUseFileCachingService = true;
              return false;
            }
            int num = 131;
            string fullPath = Path.GetFullPath(service.Configuration.CacheRoot.TrimEnd('/', '\\'));
            try
            {
              if (fullPath.Length <= num)
              {
                PermissionSet permissionSet = new PermissionSet(PermissionState.None);
                permissionSet.AddPermission((IPermission) new FileIOPermission(FileIOPermissionAccess.Write, fullPath));
                if (permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet))
                {
                  GitServerUtils.s_canUseFileCachingService = true;
                  requestContext.Trace(1013143, TraceLevel.Verbose, GitServerUtils.TraceArea, GitServerUtils.s_cachingServiceTraceLayer, "We will use the file caching service for the lifetime of this process.");
                }
                else
                  requestContext.Trace(1013170, TraceLevel.Error, GitServerUtils.TraceArea, GitServerUtils.s_cachingServiceTraceLayer, "Permission is denied to the current process for path {0}.", (object) fullPath);
              }
              else
                requestContext.Trace(1013142, TraceLevel.Error, GitServerUtils.TraceArea, GitServerUtils.s_cachingServiceTraceLayer, "CachingDirectoryTooLong", (object) num);
            }
            catch (IOException ex)
            {
              requestContext.TraceException(1013171, TraceLevel.Error, GitServerUtils.TraceArea, GitServerUtils.s_cachingServiceTraceLayer, (Exception) ex);
            }
            catch (UnauthorizedAccessException ex)
            {
              requestContext.TraceException(1013172, TraceLevel.Error, GitServerUtils.TraceArea, GitServerUtils.s_cachingServiceTraceLayer, (Exception) ex);
            }
            GitServerUtils.s_calculatedCanUseFileCachingService = true;
          }
        }
      }
      return GitServerUtils.s_canUseFileCachingService;
    }

    private static DateTime GetMaxDateTimeOfCommit(TfsGitCommit commit)
    {
      DateTime dateTimeOfCommit = commit.GetCommitter().Time;
      foreach (TfsGitCommit parent in (IEnumerable<TfsGitCommit>) commit.GetParents())
      {
        DateTime time = parent.GetCommitter().Time;
        dateTimeOfCommit = dateTimeOfCommit > time ? dateTimeOfCommit : time.AddMilliseconds(1.0);
      }
      return dateTimeOfCommit;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static int CompareByCommitDate(
      IVssRequestContext requestContext,
      TfsGitObject x,
      TfsGitObject y)
    {
      TfsGitCommit commit1 = x.TryResolveToCommit();
      TfsGitCommit commit2 = y.TryResolveToCommit();
      return commit1 == null ? (commit2 == null ? 0 : -1) : (commit2 == null ? 1 : commit1.GetCommitter().Time.CompareTo(commit2.GetCommitter().Time));
    }

    public static HashSet<TfsGitObject> ExpandHavesAndWants(
      IVssRequestContext requestContext,
      ITfsGitRepository repo,
      Sha1Id have,
      Sha1Id want,
      out HashSet<TfsGitObject> baseHaves)
    {
      Sha1Id[] shallows = Array.Empty<Sha1Id>();
      Sha1Id[] wants = new Sha1Id[1]{ want };
      return GitServerUtils.ExpandHavesAndWants(requestContext, repo, (ISet<Sha1Id>) new HashSet<Sha1Id>()
      {
        have
      }, (IEnumerable<Sha1Id>) wants, (ICollection<Sha1Id>) shallows, false, out baseHaves);
    }

    public static string GetAuthorizationHeader(string username, string password)
    {
      string authorizationHeader = (string) null;
      if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(password))
        authorizationHeader = string.Format("Basic {0}", (object) Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", (object) username, (object) password))));
      return authorizationHeader;
    }

    internal static HashSet<TfsGitObject> ExpandHavesAndWants(
      IVssRequestContext requestContext,
      ITfsGitRepository objectDb,
      ISet<Sha1Id> haves,
      IEnumerable<Sha1Id> wants,
      ICollection<Sha1Id> shallows,
      bool wantsPreExpanded,
      out HashSet<TfsGitObject> baseHaves)
    {
      baseHaves = new HashSet<TfsGitObject>(TfsGitObjectEqualityComparer.Instance);
      HashSet<TfsGitObject> tfsGitObjectSet = new HashSet<TfsGitObject>(TfsGitObjectEqualityComparer.Instance);
      HashSet<Sha1Id> reachableFrom = new HashSet<Sha1Id>();
      HashSet<Sha1Id> first = new HashSet<Sha1Id>();
      HashSet<Sha1Id> sha1IdSet = new HashSet<Sha1Id>();
      HashSet<Sha1Id> requiredCommits = new HashSet<Sha1Id>();
      foreach (Sha1Id have in (IEnumerable<Sha1Id>) haves)
      {
        TfsGitObject gitObject = objectDb.LookupObject(have);
        if (gitObject is TfsGitCommit tfsGitCommit)
        {
          requiredCommits.Add(tfsGitCommit.ObjectId);
          first.Add(tfsGitCommit.ObjectId);
        }
        else if (gitObject.TryResolveToCommit() != null)
          baseHaves.Add(gitObject);
      }
      foreach (Sha1Id want in wants)
      {
        TfsGitObject tfsGitObject = objectDb.LookupObject(want);
        bool flag;
        for (flag = haves.Contains(tfsGitObject.ObjectId); !flag && tfsGitObject is TfsGitTag tfsGitTag; flag = haves.Contains(tfsGitObject.ObjectId))
        {
          tfsGitObjectSet.Add(tfsGitObject);
          tfsGitObject = tfsGitTag.GetReferencedObject();
        }
        if (!flag)
        {
          if (tfsGitObject is TfsGitCommit tfsGitCommit1)
          {
            reachableFrom.Add(tfsGitCommit1.ObjectId);
            requiredCommits.Add(tfsGitCommit1.ObjectId);
          }
          else
            tfsGitObjectSet.Add(tfsGitObject);
        }
        else if (tfsGitObject is TfsGitCommit tfsGitCommit2)
          baseHaves.Add((TfsGitObject) tfsGitCommit2);
      }
      if (!wantsPreExpanded)
      {
        foreach (Sha1Id shallow in (IEnumerable<Sha1Id>) shallows)
        {
          if (objectDb.TryLookupObject(shallow) is TfsGitCommit)
          {
            requiredCommits.Add(shallow);
            sha1IdSet.Add(shallow);
          }
        }
        ReachableSetAndBoundary<Sha1Id> reachableWithBoundary = new AncestralGraphAlgorithm<int, Sha1Id>().GetReachableWithBoundary((IDirectedGraph<int, Sha1Id>) new ShallowCommitGraph((IDirectedGraph<int, Sha1Id>) objectDb.GetCommitGraph((IEnumerable<Sha1Id>) requiredCommits), (IEnumerable<Sha1Id>) sha1IdSet), (IEnumerable<Sha1Id>) reachableFrom, first.Concat<Sha1Id>((IEnumerable<Sha1Id>) sha1IdSet));
        foreach (TfsGitObject tfsGitObject in reachableWithBoundary.Boundary.Select<Sha1Id, TfsGitObject>((Func<Sha1Id, TfsGitObject>) (id => objectDb.LookupObject(id))))
          baseHaves.Add(tfsGitObject);
        foreach (Sha1Id reachable in reachableWithBoundary.ReachableSet)
          tfsGitObjectSet.Add((TfsGitObject) objectDb.LookupObject<TfsGitCommit>(reachable));
      }
      else
      {
        foreach (Sha1Id objectId in reachableFrom)
        {
          TfsGitCommit tfsGitCommit = objectDb.LookupObject<TfsGitCommit>(objectId);
          if (!first.Contains(objectId) && !shallows.Contains(objectId))
            tfsGitObjectSet.Add((TfsGitObject) tfsGitCommit);
          else
            baseHaves.Add((TfsGitObject) tfsGitCommit);
        }
      }
      return tfsGitObjectSet;
    }

    internal static void ConfigureTeamProjectForGit(
      IVssRequestContext requestContext,
      string teamProjectUri,
      bool setGitEnabled = true,
      bool setCapability = false,
      IEnumerable<IAccessControlEntry> permissions = null)
    {
      if (!setGitEnabled & setCapability)
        throw new ArgumentException("setCapability can be true only when setGitEnabled is true as SourceControlCapabilityFlags is set only while enabling git for the project");
      IVssRequestContext vssRequestContext1 = requestContext.Elevate();
      List<ProjectProperty> projectPropertyList = new List<ProjectProperty>();
      IProjectService service = vssRequestContext1.GetService<IProjectService>();
      ProjectInfo project = service.GetProject(vssRequestContext1, ProjectInfo.GetProjectId(teamProjectUri));
      project.PopulateProperties(vssRequestContext1, "System.SourceControlGitEnabled", "System.SourceControlGitPermissionsInitialized");
      ProjectProperty projectProperty1 = project.Properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (x => x.Name.Equals("System.SourceControlGitEnabled")));
      ProjectProperty projectProperty2 = project.Properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (x => x.Name.Equals("System.SourceControlGitPermissionsInitialized")));
      bool flag1 = projectProperty1 != null && string.Equals((string) projectProperty1.Value, bool.TrueString);
      bool flag2 = projectProperty2 != null && string.Equals((string) projectProperty2.Value, bool.TrueString);
      if (!flag2 & flag1)
      {
        flag2 = true;
        projectPropertyList.Add(new ProjectProperty("System.SourceControlGitPermissionsInitialized", (object) bool.TrueString));
      }
      if (setGitEnabled)
      {
        if (setCapability)
          projectPropertyList.Add(new ProjectProperty("System.SourceControlCapabilityFlags", (object) GitConstants.SourceControlCapabilityFlag.ToString()));
        if (!flag1)
          projectPropertyList.Add(new ProjectProperty("System.SourceControlGitEnabled", (object) bool.TrueString));
        IVssRequestContext vssRequestContext2 = vssRequestContext1.To(TeamFoundationHostType.Application);
        CatalogTransactionContext transactionContext = vssRequestContext2.GetService<ITeamFoundationCatalogService>().CreateTransactionContext();
        if (!transactionContext.IsReadOnly(vssRequestContext2))
        {
          CatalogNode catalogNode = vssRequestContext1.GetService<ICommonStructureService>().QueryProjectCatalogNode(vssRequestContext1, teamProjectUri);
          if (catalogNode == null)
            throw new CatalogNodeDoesNotExistException();
          if (catalogNode.Resource == null)
            throw new CatalogResourceDoesNotExistException(FrameworkResources.CatalogResourceMustBePassedWithNode());
          if (setCapability)
            catalogNode.Resource.Properties["SourceControlCapabilityFlags"] = GitConstants.SourceControlCapabilityFlag.ToString();
          catalogNode.Resource.Properties["SourceControlGitEnabled"] = bool.TrueString;
          transactionContext.AttachResource(catalogNode.Resource);
          transactionContext.Save(vssRequestContext2, false);
        }
      }
      if (!flag2)
      {
        if (permissions == null || !permissions.Any<IAccessControlEntry>())
          permissions = (IEnumerable<IAccessControlEntry>) new AccessControlEntry[1]
          {
            new AccessControlEntry()
            {
              Descriptor = GitServerUtils.GetProjectAdminGroup(vssRequestContext1, teamProjectUri),
              Allow = 491382,
              Deny = 0
            }
          };
        SecurityHelper.Instance.SetPermissions(vssRequestContext1, new RepoScope(project.Id, Guid.Empty), (string) null, permissions);
        projectPropertyList.Add(new ProjectProperty("System.SourceControlGitPermissionsInitialized", (object) bool.TrueString));
      }
      if (projectPropertyList.Count <= 0)
        return;
      service.SetProjectProperties(vssRequestContext1, project.Id, (IEnumerable<ProjectProperty>) projectPropertyList);
    }

    internal static IdentityDescriptor GetProjectAdminGroup(
      IVssRequestContext requestContext,
      string teamProjectUri)
    {
      return requestContext.GetService<ITeamFoundationIdentityService>().ReadIdentity(requestContext, IdentitySearchFactor.AdministratorsGroup, teamProjectUri)?.Descriptor;
    }

    public static string GetAbbreviatedCommitId(byte[] commitId)
    {
      if (commitId == null)
        throw new ArgumentNullException("gitObjectIdBytes");
      if (commitId.Length < 3)
        throw new ArgumentException("gitObjectIdBytes");
      int num1 = 0;
      char[] chArray1 = new char[6];
      for (int index1 = 0; index1 < 3; ++index1)
      {
        int num2 = (int) commitId[index1];
        char ch1 = (char) ((num2 >> 4 & 15) + 48);
        char ch2 = (char) ((num2 & 15) + 48);
        char[] chArray2 = chArray1;
        int index2 = num1;
        int num3 = index2 + 1;
        int num4 = ch1 >= ':' ? (int) (ushort) ((uint) ch1 + 39U) : (int) ch1;
        chArray2[index2] = (char) num4;
        char[] chArray3 = chArray1;
        int index3 = num3;
        num1 = index3 + 1;
        int num5 = ch2 >= ':' ? (int) (ushort) ((uint) ch2 + 39U) : (int) ch2;
        chArray3[index3] = (char) num5;
      }
      return new string(chArray1);
    }

    internal static bool IsWindowsLibGit2Client(string userAgent) => !string.IsNullOrWhiteSpace(userAgent) && userAgent.StartsWith("git/1.0", StringComparison.Ordinal) && (userAgent.IndexOf("Microsoft", StringComparison.Ordinal) > 0 || userAgent.IndexOf("12.0.0.0", StringComparison.Ordinal) > 0 || userAgent.IndexOf("libgit2", StringComparison.Ordinal) > 0 && (userAgent.IndexOf("Team Foundation", StringComparison.Ordinal) > 0 || userAgent.IndexOf("CloudBuild", StringComparison.Ordinal) > 0 || userAgent.IndexOf("14.0.", StringComparison.Ordinal) > 0 || userAgent.IndexOf("15.0.", StringComparison.Ordinal) > 0));

    internal static bool GitClientNeedsUpdate(
      string userAgent,
      Version minimumRecommendedGitVersion,
      IReadOnlyCollection<string> userAgentExemptions = null)
    {
      if (string.IsNullOrEmpty(userAgent) || !userAgent.StartsWith("git/") || userAgent.StartsWith("git/1.0 ") || userAgent.StartsWith("git/2.0 ") || userAgent.Contains("VS15/15.0.0") || userAgentExemptions != null && userAgentExemptions.Any<string>((Func<string, bool>) (s => userAgent.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0)))
        return false;
      userAgent = userAgent.Substring("git/".Length);
      int num = 0;
      while (num < userAgent.Length && (char.IsDigit(userAgent[num]) || num >= userAgent.Length - 1 || char.IsDigit(userAgent[num + 1])))
        ++num;
      userAgent = userAgent.Substring(0, num);
      Version result;
      return Version.TryParse(userAgent, out result) && result < minimumRecommendedGitVersion;
    }

    internal static Stream CreateInflateStream(
      Stream stream,
      bool leaveOpen = false,
      long uncompressedLength = -1)
    {
      return (Stream) LengthCountingStream.CreateForRead((Stream) new ZlibStream(stream, CompressionMode.Decompress, leaveOpen), expectedLength: new long?(uncompressedLength));
    }

    public static void ValidateAndNormalizeGitRepositoryName(
      ref string repositoryName,
      string parameterName)
    {
      try
      {
        repositoryName = ProjectInfo.NormalizeProjectName(repositoryName, parameterName, true, true);
      }
      catch (InvalidProjectNameException ex)
      {
        throw new InvalidGitRepositoryNameException(parameterName);
      }
    }

    public static bool IsRequestOptimized(HttpContextBase httpContext, GitRepoSettings repoSettings)
    {
      switch (httpContext.Request.RequestContext.RouteData.GetRouteValue<string>("GitArea"))
      {
        case "_git/_optimized":
          return true;
        case "_git":
          return repoSettings.OptimizedByDefault;
        default:
          return false;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void TranslateLegacyPermissionsToCurrentPermissions(
      IEnumerable<IAccessControlEntry> aces)
    {
      foreach (IAccessControlEntry ace in aces)
      {
        if ((ace.Allow & 1) == 1)
        {
          ace.Allow |= 16128;
          ace.Allow &= -2;
        }
        if ((ace.Deny & 1) == 1)
        {
          ace.Deny |= 16128;
          ace.Deny &= -2;
        }
        if (((ace.Allow & 4) == 4 || (ace.Allow & 2) == 2) && (ace.Deny & 16384) != 16384)
          ace.Allow |= 16384;
        if ((ace.Deny & 4) == 4 && (ace.Allow & 16384) != 16384)
          ace.Deny |= 16384;
        if ((ace.Allow & 128) == 128 && (ace.Deny & 32768) != 32768)
          ace.Allow |= 32768;
        if ((ace.Deny & 128) == 128 && (ace.Allow & 32768) != 32768)
          ace.Deny |= 32768;
        if ((ace.Allow & 16384) == 16384 && (ace.Deny & 65536) != 65536)
          ace.Allow |= 65536;
        if ((ace.Deny & 16384) == 16384 && (ace.Allow & 65536) != 65536)
          ace.Deny |= 65536;
        if ((ace.Allow & 512) == 512 && (ace.Deny & 131072) != 131072)
          ace.Allow |= 131072;
        if ((ace.Deny & 512) == 512 && (ace.Allow & 131072) != 131072)
          ace.Deny |= 131072;
      }
    }

    public static GitUserDate CreateUserDate(
      IVssRequestContext rc,
      string name,
      string email,
      DateTime date,
      bool includeImageUrl)
    {
      return rc.GetService<ITeamFoundationGitCommitService>().CreateUserDate(rc, name, email, date, includeImageUrl);
    }

    public static bool UseGravatarForExternalIdentities(IVssRequestContext requestContext) => !requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) "/WebAccess/IdentityImage/DisableGravatar", false);

    public static Microsoft.VisualStudio.Services.Identity.Identity GetIdentityForEmail(
      IVssRequestContext rc,
      string email)
    {
      return IdentityImageServiceUtil.GetIdentityImageService(rc).GetIdentityFromEmail(rc, ref email);
    }

    private class DateTimeObjectComparer : IComparer<Tuple<DateTime, bool, Sha1Id>>
    {
      private static readonly GitServerUtils.DateTimeObjectComparer s_instance = new GitServerUtils.DateTimeObjectComparer();

      public int Compare(Tuple<DateTime, bool, Sha1Id> a, Tuple<DateTime, bool, Sha1Id> b)
      {
        int num = b.Item1.CompareTo(a.Item1);
        if (num == 0)
          num = a.Item3.CompareTo(b.Item3);
        return num;
      }

      public static IComparer<Tuple<DateTime, bool, Sha1Id>> Instance => (IComparer<Tuple<DateTime, bool, Sha1Id>>) GitServerUtils.DateTimeObjectComparer.s_instance;
    }
  }
}
