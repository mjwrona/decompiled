// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitRepository
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Diff;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Bitmap;
using Microsoft.TeamFoundation.Git.Server.Graph;
using Microsoft.TeamFoundation.Git.Server.Protocol;
using Microsoft.TeamFoundation.Git.Server.Storage;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Core.Security;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class TfsGitRepository : ITfsGitRepository, IDisposable
  {
    private readonly ICherryPickRelationships m_cherryPicks;
    private readonly TfsGitRepositoryRefsCollection m_refs;
    private readonly IIsolationBitmapProvider m_isolationPrv;
    private readonly IGitObjectSet m_objectSet;
    private Odb m_odb;
    private readonly IVssRequestContext m_requestContext;
    private bool? m_disposed;
    private readonly Lazy<GitRepoSettings> m_repoSettings;
    private readonly IRepoPermissionsManager m_repoPermissionsManager;
    private readonly StackTrace m_constructorStackTrace;
    private static readonly char[] s_slashSplit = new char[1]
    {
      '/'
    };
    private const string s_Layer = "TfsGitRepository";

    internal TfsGitRepository(
      string name,
      RepoKey key,
      bool isFork,
      long size,
      bool isDisabled,
      bool isInMaintenance,
      ICherryPickRelationships cherryPickRelationships,
      IIsolationBitmapProvider isolationPrv,
      GitObjectSet objectSet,
      Odb odb,
      IVssRequestContext requestContext,
      Func<GitRepoSettings> repoSettingsFactory)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(name, nameof (name));
      ArgumentUtility.CheckForNull<RepoKey>(key, nameof (key));
      ArgumentUtility.CheckForNull<IIsolationBitmapProvider>(isolationPrv, nameof (isolationPrv));
      ArgumentUtility.CheckForNull<GitObjectSet>(objectSet, nameof (objectSet));
      ArgumentUtility.CheckForNull<Odb>(odb, nameof (odb));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Func<GitRepoSettings>>(repoSettingsFactory, nameof (repoSettingsFactory));
      this.Name = name;
      this.Key = key;
      this.IsFork = isFork;
      this.Size = size;
      this.IsDisabled = isDisabled;
      this.IsInMaintenance = isInMaintenance;
      this.m_isolationPrv = isolationPrv;
      this.m_objectSet = (IGitObjectSet) objectSet;
      this.m_odb = odb;
      this.m_refs = new TfsGitRepositoryRefsCollection(requestContext, (ITfsGitRepository) this);
      this.m_cherryPicks = cherryPickRelationships;
      this.m_requestContext = requestContext;
      this.m_repoSettings = new Lazy<GitRepoSettings>(repoSettingsFactory, false);
      if (TeamFoundationTracingService.IsRawTracingEnabled(640381591, TraceLevel.Info, GitServerUtils.TraceArea, nameof (TfsGitRepository), (string[]) null))
        this.m_constructorStackTrace = new StackTrace(1);
      this.m_repoPermissionsManager = (IRepoPermissionsManager) new RepoPermissionsManager(requestContext, (RepoScope) this.Key);
      this.m_disposed = new bool?(false);
    }

    ~TfsGitRepository()
    {
      bool? disposed = this.m_disposed;
      bool flag = false;
      if (!(disposed.GetValueOrDefault() == flag & disposed.HasValue))
        return;
      TeamFoundationTracingService.TraceRaw(1013122, TraceLevel.Error, GitServerUtils.TraceArea, nameof (TfsGitRepository), "TfsGitRepository finalizer without dispose - call stack: {0}", (object) (this.m_constructorStackTrace?.ToString() ?? "Disabled"));
    }

    public void Dispose()
    {
      if (this.m_odb != null)
      {
        this.m_odb.Dispose();
        this.m_odb = (Odb) null;
      }
      this.m_disposed = new bool?(true);
      GC.SuppressFinalize((object) this);
    }

    public string Name { get; }

    public bool IsFork { get; }

    public long Size { get; }

    public RepoKey Key { get; }

    public bool IsDisabled { get; }

    public bool IsInMaintenance { get; }

    public ITfsGitRepositoryRefsCollection Refs
    {
      get
      {
        this.EnsureNotDisposed();
        return (ITfsGitRepositoryRefsCollection) this.m_refs;
      }
    }

    public ICherryPickRelationships CherryPickRelationships
    {
      get
      {
        this.EnsureNotDisposed();
        return this.m_cherryPicks;
      }
    }

    public IGitObjectSet Objects
    {
      get
      {
        this.EnsureNotDisposed();
        return this.m_objectSet;
      }
    }

    public IRepoPermissionsManager Permissions
    {
      get
      {
        this.EnsureNotDisposed();
        return this.m_repoPermissionsManager;
      }
    }

    public GitRepoSettings Settings
    {
      get
      {
        this.EnsureNotDisposed();
        return this.m_repoSettings.Value;
      }
    }

    public GitOdbSettings OdbSettings
    {
      get
      {
        this.EnsureNotDisposed();
        return this.m_odb.Settings;
      }
    }

    internal Odb Odb
    {
      get
      {
        this.EnsureNotDisposed();
        return this.m_odb;
      }
    }

    public IObjectMetadata ObjectMetadata
    {
      get
      {
        this.EnsureNotDisposed();
        return this.m_odb.ObjectMetadata;
      }
    }

    public IIsolationBitmapProvider IsolationBitmapProvider
    {
      get
      {
        this.EnsureNotDisposed();
        return this.m_isolationPrv;
      }
    }

    public PackAndRefIngester CreatePackAndRefIngester(out Stream tempPackStream) => PackAndRefIngester.CreateWithWriteableStream(this.m_requestContext, (ITfsGitRepository) this, out tempPackStream);

    public PackAndRefIngester CreatePackAndRefIngester(
      Stream inputPackStream,
      ClientTraceData ctData)
    {
      return new PackAndRefIngester(this.m_requestContext, (ITfsGitRepository) this, inputPackStream, ctData);
    }

    public IGitCommitGraph GetCommitGraph(IEnumerable<Sha1Id> requiredCommits)
    {
      this.EnsureNotDisposed();
      return this.m_odb.GraphProvider.Get(requiredCommits);
    }

    public string GetRepositoryWebUri()
    {
      this.EnsureNotDisposed();
      return this.GetRepositoryAreaWebUri("_git");
    }

    public string GetRepositoryCloneUri()
    {
      this.EnsureNotDisposed();
      return this.GetRepositoryAreaCloneUri("_git");
    }

    public string GetRepositoryFullUri()
    {
      this.EnsureNotDisposed();
      return this.Settings.OptimizedByDefault ? this.GetRepositoryAreaCloneUri("_git/_full") : this.GetRepositoryAreaCloneUri("_git");
    }

    private string GetRepositoryAreaWebUri(string area)
    {
      ProjectInfo project = this.m_requestContext.GetService<IProjectService>().GetProject(this.m_requestContext, this.Key.ProjectId);
      return GitServerUtils.GetRepositoryWebUrl(this.m_requestContext, GitServerUtils.GetPublicBaseUrl(this.m_requestContext), project.Name, this.Name, area);
    }

    private string GetRepositoryAreaCloneUri(string area)
    {
      ProjectInfo project = this.m_requestContext.GetService<IProjectService>().GetProject(this.m_requestContext, this.Key.ProjectId);
      return GitServerUtils.GetRepositoryCloneUrl(this.m_requestContext, GitServerUtils.GetPublicBaseUrl(this.m_requestContext), project.Name, this.Name, area);
    }

    private void EnsureNotDisposed()
    {
      if (this.m_disposed.HasValue && this.m_disposed.Value)
        throw new ObjectDisposedException(nameof (TfsGitRepository));
    }

    public TfsGitRefUpdateResultSet ModifyPaths(
      string refName,
      Sha1Id baseCommitId,
      string comment,
      IEnumerable<GitChange> changes,
      GitUserDate author,
      GitUserDate committer,
      TimeSpan? authorOffset = null,
      TimeSpan? committerOffset = null)
    {
      this.EnsureNotDisposed();
      if (string.IsNullOrEmpty(refName) || string.IsNullOrEmpty(comment) || changes == null || !changes.Any<GitChange>())
        throw new ArgumentException(Resources.Get("InvalidParameters"));
      TfsGitRef tfsGitRef = this.Refs.MatchingName(refName);
      Sha1Id oldObjectId;
      if (tfsGitRef == null)
      {
        oldObjectId = Sha1Id.Empty;
        if (!RefUtil.IsValidRefName(refName, true))
          throw new ArgumentException(Resources.Format("InvalidGitRefName", (object) refName));
      }
      else
      {
        oldObjectId = baseCommitId;
        if (tfsGitRef.ObjectId != baseCommitId)
          throw new GitReferenceStaleException(refName);
      }
      bool flag = !baseCommitId.IsEmpty;
      TreeBuilder treeBuilder1 = !flag ? new TreeBuilder() : new TreeBuilder(this.LookupObject<TfsGitCommit>(baseCommitId).GetTree());
      Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
      Stack<Tuple<TreeBuilder, Dictionary<string, object>>> tupleStack = new Stack<Tuple<TreeBuilder, Dictionary<string, object>>>();
      tupleStack.Push(new Tuple<TreeBuilder, Dictionary<string, object>>(treeBuilder1, dictionary1));
      int objectCount = 2;
      List<GitChange> gitChangeList1 = new List<GitChange>();
      foreach (GitChange change in changes)
      {
        if (change.Item == null || string.IsNullOrWhiteSpace(change.Item.Path))
          throw new ArgumentException(Resources.Get("InvalidParameters"));
        if (change.ChangeType != VersionControlChangeType.Add && change.ChangeType != VersionControlChangeType.Edit && change.ChangeType != VersionControlChangeType.Delete && change.ChangeType != VersionControlChangeType.Rename)
          throw new ArgumentException(Resources.Get("InvalidParameters"));
        int num = change.NewContent == null ? 0 : (change.NewContent.Content != null ? 1 : 0);
        if (num != (change.ChangeType == VersionControlChangeType.Add ? (true ? 1 : 0) : (change.ChangeType == VersionControlChangeType.Edit ? 1 : 0)))
          throw new ArgumentException(Resources.Get("InvalidParameters"));
        if (num != 0 && change.NewContent.ContentType != ItemContentType.RawText && change.NewContent.ContentType != ItemContentType.Base64Encoded)
          throw new ArgumentException(Resources.Get("InvalidParameters"));
        if (string.IsNullOrWhiteSpace(change.SourceServerItem) != (change.ChangeType != VersionControlChangeType.Rename))
          throw new ArgumentException(Resources.Get("InvalidParameters"));
        if (change.ChangeType == VersionControlChangeType.Rename)
        {
          List<GitChange> gitChangeList2 = gitChangeList1;
          GitChange gitChange1 = new GitChange();
          gitChange1.ChangeType = VersionControlChangeType.Delete;
          GitItem gitItem1 = new GitItem();
          gitItem1.Path = change.SourceServerItem;
          gitChange1.Item = gitItem1;
          gitChangeList2.Add(gitChange1);
          List<GitChange> gitChangeList3 = gitChangeList1;
          GitChange gitChange2 = new GitChange();
          gitChange2.ChangeType = VersionControlChangeType.Add;
          GitItem gitItem2 = new GitItem();
          gitItem2.Path = change.Item.Path;
          gitChange2.Item = gitItem2;
          gitChange2.SourceServerItem = change.SourceServerItem;
          gitChangeList3.Add(gitChange2);
        }
        else
          gitChangeList1.Add(change);
      }
      Dictionary<string, TreeBuilderEntry> dictionary2 = new Dictionary<string, TreeBuilderEntry>();
      foreach (GitChange gitChange in gitChangeList1)
      {
        string path = gitChange.Item.Path;
        string[] pathElements = path.Split(TfsGitRepository.s_slashSplit, StringSplitOptions.RemoveEmptyEntries);
        Dictionary<string, object> dictionary3 = dictionary1;
        TreeBuilder treeBuilder2 = treeBuilder1;
        if (pathElements.Length == 0 || path.Contains<char>(char.MinValue))
          throw new ArgumentException(Resources.Get("InvalidParameters"));
        for (int index = 0; index < pathElements.Length - 1; ++index)
        {
          object obj;
          if (!dictionary3.TryGetValue(pathElements[index], out obj))
          {
            TreeBuilder treeBuilder3 = (TreeBuilder) null;
            ++objectCount;
            foreach (TreeBuilderEntry entry in (IEnumerable<TreeBuilderEntry>) treeBuilder2.Entries)
            {
              if (pathElements[index].Equals(entry.Name))
              {
                if (!(this.LookupObject(entry.ObjectId) is TfsGitTree tree))
                  throw new ArgumentException(Resources.Get("GitUpdatedTreatedBlobLikeTree"));
                treeBuilder3 = new TreeBuilder(tree);
                break;
              }
            }
            if (treeBuilder3 == null)
            {
              if (gitChange.ChangeType == VersionControlChangeType.Add)
                treeBuilder3 = new TreeBuilder();
              else
                throw new ArgumentException(Resources.Format("GitPathDoesNotExistAtCommit", (object) path, (object) baseCommitId));
            }
            tuple = new Tuple<TreeBuilder, Dictionary<string, object>>(treeBuilder3, new Dictionary<string, object>());
            tupleStack.Push(new Tuple<TreeBuilder, Dictionary<string, object>>(treeBuilder3, tuple.Item2));
            dictionary3[pathElements[index]] = (object) tuple;
          }
          else if (!(obj is Tuple<TreeBuilder, Dictionary<string, object>> tuple))
            throw new ArgumentException(Resources.Format("BlobTreatedAsTree", (object) path));
          treeBuilder2 = tuple.Item1;
          dictionary3 = tuple.Item2;
        }
        if (dictionary3.ContainsKey(pathElements[pathElements.Length - 1]))
          throw new ArgumentException(Resources.Format("MultipleOperationsOnSameFile", (object) path));
        TreeBuilderEntry treeBuilderEntry1 = treeBuilder2.Entries.FirstOrDefault<TreeBuilderEntry>((Func<TreeBuilderEntry, bool>) (entry => string.Equals(entry.Name, pathElements[pathElements.Length - 1])));
        TreeBuilderEntry treeBuilderEntry2 = (TreeBuilderEntry) null;
        if (gitChange.ChangeType == VersionControlChangeType.Delete)
        {
          dictionary2[path] = treeBuilderEntry1 != null ? treeBuilderEntry1 : throw new ArgumentException(Resources.Format("GitPathDoesNotExistAtCommit", (object) path, (object) baseCommitId));
        }
        else
        {
          if (gitChange.ChangeType == VersionControlChangeType.Add && treeBuilderEntry1 != null)
          {
            if (string.IsNullOrEmpty(gitChange.SourceServerItem))
              throw new ArgumentException(Resources.Format("AddTargetAlreadyExists", (object) path));
            throw new ArgumentException(Resources.Format("RenameTargetItemExists", (object) path));
          }
          if (gitChange.ChangeType == VersionControlChangeType.Edit && treeBuilderEntry1 == null)
            throw new ArgumentException(Resources.Format("GitPathDoesNotExistAtCommit", (object) path, (object) baseCommitId));
          if (gitChange.ChangeType == VersionControlChangeType.Edit && treeBuilderEntry1.PackType != GitPackObjectType.Blob)
            throw new ArgumentException(Resources.Format("UnexpectedPathObjectType", (object) GitPackObjectType.Blob, (object) treeBuilderEntry1.ObjectId, (object) treeBuilderEntry1.PackType, (object) path));
          if (gitChange.ChangeType == VersionControlChangeType.Add && !string.IsNullOrEmpty(gitChange.SourceServerItem))
          {
            if (!dictionary2.TryGetValue(gitChange.SourceServerItem, out treeBuilderEntry2))
              throw new ArgumentException("Could not find rename source");
          }
          else
            ++objectCount;
          gitChange.Item.ContentMetadata = new FileContentMetadata();
          if (gitChange.ChangeType == VersionControlChangeType.Edit && gitChange.NewContent.ContentType == ItemContentType.RawText)
          {
            bool containsPreamble = false;
            int codePage;
            using (Stream content = this.GetContent(treeBuilderEntry1.ObjectId, out GitObjectType _))
            {
              Encoding encoding = FileTypeUtil.DetermineEncoding(content, true, GitEncodingUtil.SafeUtf8NoBom, 0L, out containsPreamble);
              codePage = encoding != null ? encoding.CodePage : 0;
            }
            if (codePage > 0)
            {
              gitChange.Item.ContentMetadata.Encoding = codePage;
              gitChange.Item.ContentMetadata.EncodingWithBom = containsPreamble;
            }
          }
        }
        dictionary3[pathElements[pathElements.Length - 1]] = (object) new Tuple<GitChange, TreeBuilderEntry>(gitChange, treeBuilderEntry2);
      }
      string path1 = Path.Combine(GitServerUtils.GetCacheDirectory(this.m_requestContext, this.Key.RepoId), Path.GetRandomFileName());
      DirectoryInfo directoryInfo = (DirectoryInfo) null;
      int num1 = 0;
      try
      {
        directoryInfo = Directory.CreateDirectory(path1);
        Stream tempPackStream;
        PackAndRefIngester packAndRefIngester = this.CreatePackAndRefIngester(out tempPackStream);
        using (tempPackStream)
        {
          Sha1Id commit;
          using (GitPackSerializer gitPackSerializer = new GitPackSerializer(tempPackStream, objectCount, true))
          {
            while (tupleStack.Count > 0)
            {
              Tuple<TreeBuilder, Dictionary<string, object>> tuple1 = tupleStack.Pop();
              TreeBuilder treeBuilder4 = tuple1.Item1;
              foreach (KeyValuePair<string, object> keyValuePair in tuple1.Item2)
              {
                if (keyValuePair.Value is Tuple<TreeBuilder, Dictionary<string, object>> tuple3)
                {
                  if (tuple3.Item1.Entries.Count == 0)
                    treeBuilder4.RemoveTreeEntry(keyValuePair.Key);
                  else
                    treeBuilder4.UpdateTreeEntry(this.Settings.ToTreeFsckOptions(), keyValuePair.Key, tuple3.Item1.ObjectId, GitPackObjectType.Tree);
                }
                else
                {
                  Tuple<GitChange, TreeBuilderEntry> tuple2 = (Tuple<GitChange, TreeBuilderEntry>) keyValuePair.Value;
                  if (tuple2.Item1.ChangeType == VersionControlChangeType.Delete)
                    tuple1.Item1.RemoveTreeEntry(keyValuePair.Key);
                  else if (tuple2.Item2 != null)
                  {
                    treeBuilder4.UpdateTreeEntry(this.Settings.ToTreeFsckOptions(), keyValuePair.Key, tuple2.Item2.ObjectId, tuple2.Item2.PackType);
                  }
                  else
                  {
                    string str = Path.Combine(directoryInfo.FullName, num1++.ToString());
                    using (FileStream fileStream = File.Create(str))
                    {
                      if (tuple2.Item1.NewContent.ContentType == ItemContentType.Base64Encoded)
                      {
                        byte[] buffer = Convert.FromBase64String(tuple2.Item1.NewContent.Content);
                        fileStream.Write(buffer, 0, buffer.Length);
                      }
                      else
                      {
                        GitChange gitChange = tuple2.Item1;
                        FileContentMetadata contentMetadata1 = gitChange.Item.ContentMetadata;
                        bool byteOrderMark = contentMetadata1 != null && contentMetadata1.EncodingWithBom;
                        Encoding encoding1 = byteOrderMark ? GitEncodingUtil.SafeUtf8WithBom : GitEncodingUtil.SafeUtf8NoBom;
                        FileContentMetadata contentMetadata2 = gitChange.Item.ContentMetadata;
                        int encoding2 = contentMetadata2 != null ? contentMetadata2.Encoding : 0;
                        Encoding encoding3 = encoding2 != 0 ? GitEncodingUtil.GetSafeEncoding(encoding2, byteOrderMark) : encoding1;
                        using (StreamWriter streamWriter = new StreamWriter((Stream) fileStream, encoding3))
                          streamWriter.Write(tuple2.Item1.NewContent.Content);
                      }
                    }
                    FileInfo fileInfo = new FileInfo(str);
                    using (FileStream fileStream = fileInfo.OpenRead())
                    {
                      using (HashingStream<SHA1Cng2> sourceStream = new HashingStream<SHA1Cng2>())
                      {
                        sourceStream.Setup((Stream) fileStream, FileAccess.Read);
                        long length = fileInfo.Length;
                        byte[] objectHeader = GitServerUtils.CreateObjectHeader(GitPackObjectType.Blob, length);
                        sourceStream.AddToHash(objectHeader);
                        gitPackSerializer.AddInflatedStreamWithTypeAndSize((Stream) sourceStream, GitPackObjectType.Blob, length);
                        treeBuilder4.UpdateTreeEntry(this.Settings.ToTreeFsckOptions(), keyValuePair.Key, new Sha1Id(sourceStream.Hash), GitPackObjectType.Blob);
                      }
                    }
                  }
                }
              }
              byte[] finalTreeBytes;
              treeBuilder4.CreateTree(out finalTreeBytes);
              gitPackSerializer.AddInflatedStreamWithTypeAndSize((Stream) new MemoryStream(finalTreeBytes), GitPackObjectType.Tree, (long) finalTreeBytes.Length);
            }
            Sha1Id[] parents;
            if (!flag)
              parents = new Sha1Id[0];
            else
              parents = new Sha1Id[1]{ baseCommitId };
            DateTime utcNow = DateTime.UtcNow;
            string gitUserString1 = this.GetGitUserString(author, utcNow, authorOffset);
            string gitUserString2 = this.GetGitUserString(committer, utcNow, committerOffset);
            byte[] commitBytes;
            commit = CommitBuilder.CreateCommit((IEnumerable<Sha1Id>) parents, treeBuilder1.ObjectId, gitUserString1, gitUserString2, comment, out commitBytes);
            gitPackSerializer.AddInflatedStreamWithTypeAndSize((Stream) new MemoryStream(commitBytes), GitPackObjectType.Commit, (long) commitBytes.Length);
            gitPackSerializer.Complete();
          }
          tempPackStream.Seek(0L, SeekOrigin.Begin);
          this.m_requestContext.Trace(1013248, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (TfsGitRepository), "Completing the push from passed in content");
          packAndRefIngester.AddRefUpdateRequest(refName, oldObjectId, commit);
          return packAndRefIngester.Ingest();
        }
      }
      finally
      {
        try
        {
          directoryInfo?.Delete(true);
        }
        catch (IOException ex)
        {
          this.m_requestContext.TraceCatch(1013249, TraceLevel.Warning, GitServerUtils.TraceArea, nameof (TfsGitRepository), (Exception) ex);
        }
      }
    }

    private string GetGitUserString(GitUserDate user, DateTime utcCurrentTime, TimeSpan? utcOffset)
    {
      utcOffset = new TimeSpan?(utcOffset ?? TimeSpan.Zero);
      string name;
      string email;
      if (user != null && !string.IsNullOrEmpty(user.Name) && !string.IsNullOrEmpty(user.Email))
      {
        name = user.Name;
        email = user.Email;
      }
      else
      {
        ITeamFoundationIdentityService service = this.m_requestContext.GetService<ITeamFoundationIdentityService>();
        TeamFoundationIdentity foundationIdentity = service.ReadIdentity(this.m_requestContext, this.m_requestContext.UserContext, MembershipQuery.None, ReadIdentityOptions.None);
        name = foundationIdentity.DisplayName;
        email = service.GetPreferredEmailAddress(this.m_requestContext, foundationIdentity.TeamFoundationId);
      }
      DateTime dateTime = user == null || !(user.Date != new DateTime()) ? utcCurrentTime : user.Date;
      DateTime time = dateTime;
      if (dateTime.Kind != DateTimeKind.Utc)
        time = DateTime.SpecifyKind(dateTime.Add(utcOffset.Value), DateTimeKind.Utc);
      return IdentityAndDate.CreateIdentityString(name, email, time, utcOffset.Value);
    }

    public List<TfsGitPushMetadata> QueryPushHistory(
      bool includeRefUpdates,
      DateTime? fromDate,
      DateTime? toDate,
      IEnumerable<Guid> pusherIds,
      bool excludePushers,
      int? skip,
      int? take,
      string refName)
    {
      this.EnsureNotDisposed();
      int? nullable = take;
      int num = 0;
      if (nullable.GetValueOrDefault() == num & nullable.HasValue)
        return new List<TfsGitPushMetadata>(0);
      this.m_requestContext.Trace(1013327, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (TfsGitRepository), "QueryPushHistory repoId:{0} refName {1} fromDate:{2} toDate:{3} pusherIds:{4} excludePushers:{5} skip:{6} take:{7}", (object) this.Key.RepoId, (object) refName, (object) fromDate, (object) toDate, (object) string.Join<Guid>(";", pusherIds), (object) excludePushers, (object) skip, (object) take);
      if (fromDate.HasValue)
        fromDate = new DateTime?(fromDate.Value.ToUniversalTime());
      if (toDate.HasValue)
        toDate = new DateTime?(toDate.Value.ToUniversalTime());
      using (GitCoreComponent gitCoreComponent = this.m_requestContext.CreateGitCoreComponent())
      {
        using (ResultCollection resultCollection = gitCoreComponent.QueryPushHistory(this.Key, refName, includeRefUpdates, fromDate, toDate, pusherIds, excludePushers, skip, take))
        {
          List<TfsGitPushMetadata> items1 = resultCollection.GetCurrent<TfsGitPushMetadata>().Items;
          if (includeRefUpdates)
          {
            resultCollection.NextResult();
            List<TfsGitRefLogEntry> items2 = resultCollection.GetCurrent<TfsGitRefLogEntry>().Items;
            if (items2.Count > 0)
              TfsGitPushMetadata.AssignRefLogsToMetadata(items1, (IEnumerable<TfsGitRefLogEntry>) items2);
          }
          return items1;
        }
      }
    }
  }
}
