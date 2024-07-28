// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandGet
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandGet : VersionControlCommand
  {
    private Workspace m_workspace;
    private List<GetRequest> m_requests;
    private GetOptions m_options;
    private int m_maxResults;
    private List<StreamingCollection<GetOperation>> m_getOpsCollections;
    private bool m_throttle;
    private int m_loopCount;
    private UrlSigner m_signer;
    private VersionedItemComponent m_db;
    private ObjectBinder<GetOperation> m_getBinder;
    private ResultCollection m_resultCollection;
    private List<string> m_requestLocalItems;
    private int m_requestIndex;
    private int m_returnedResults;
    private PropertyMerger<GetOperation> m_attributeMerger;
    private PropertyMerger<GetOperation> m_propertyMerger;
    private CommandGet.State m_state;
    private bool m_hasMoreData;
    private bool m_hasMoreProperties;
    private bool m_hasMoreAttributes;

    public CommandGet(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public void Execute(
      Workspace workspace,
      GetRequest[] requests,
      GetOptions options,
      int maxResults,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters)
    {
      this.SecurityWrapper.CheckWorkspacePermission(this.m_versionControlRequestContext, 2, workspace);
      if ((options & GetOptions.Remap) == GetOptions.Remap && (options & GetOptions.GetAll) == GetOptions.GetAll)
        throw new ArgumentException(Resources.Format("InvalidOptionCombination", (object) GetOptions.Remap, (object) GetOptions.Overwrite), nameof (options));
      if ((options & GetOptions.Remap) == GetOptions.Remap && (options & GetOptions.Preview) == GetOptions.Preview)
        throw new ArgumentException(Resources.Format("InvalidOptionCombination", (object) GetOptions.Remap, (object) GetOptions.Preview), nameof (options));
      CustomerIntelligenceData ciData = new CustomerIntelligenceData();
      ciData.Add(nameof (options), (object) options);
      ciData.Add("requestsLength", (object) requests?.Length);
      ciData.Add(nameof (maxResults), (double) maxResults);
      CustomerIntelligence.Publish(this.RequestContext, "Get", ciData);
      ciData.Add("workspaceName", workspace?.Name);
      if (requests != null)
        ciData.Add("items", ((IEnumerable<GetRequest>) requests).Take<GetRequest>(5).Select<GetRequest, string>((Func<GetRequest, string>) (x => string.Format("{0};{1}", (object) x.ItemSpec, (object) x.VersionSpec))).ToList<string>());
      ClientTrace.Publish(this.RequestContext, "Get", ciData);
      this.m_signer = new UrlSigner(this.m_versionControlRequestContext.RequestContext);
      this.m_workspace = workspace;
      this.m_requests = new List<GetRequest>((IEnumerable<GetRequest>) requests);
      this.m_options = options;
      this.m_maxResults = maxResults;
      this.m_throttle = this.RequestContext.IsFeatureEnabled("Tfvc.SelfThrottle");
      this.m_getOpsCollections = new List<StreamingCollection<GetOperation>>(requests.Length);
      this.m_requestLocalItems = new List<string>(requests.Length);
      this.ExpandServerRequests(requests);
      if (itemPropertyFilters != null && itemPropertyFilters.Length != 0)
        this.m_propertyMerger = new PropertyMerger<GetOperation>(this.m_versionControlRequestContext, itemPropertyFilters, (VersionControlCommand) this, VersionControlPropertyKinds.ImmutableVersionedItem);
      if (itemAttributeFilters != null && itemAttributeFilters.Length != 0)
        this.m_attributeMerger = new PropertyMerger<GetOperation>(this.m_versionControlRequestContext, itemAttributeFilters, (VersionControlCommand) this, VersionControlPropertyKinds.VersionedItem);
      this.m_state = CommandGet.State.GetOps;
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      this.ContinueExecution();
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    private void ExpandServerRequests(GetRequest[] requests)
    {
      for (int index = 0; index < requests.Length; ++index)
      {
        if (requests[index].ItemSpec != null && requests[index].ItemSpec.isServerItem && requests[index].ItemSpec.RecursionType != RecursionType.None)
        {
          foreach (WorkingFolder upToDateFolder in this.m_workspace.GetUpToDateFolders(this.RequestContext))
          {
            if (VersionControlPath.IsSubItem(upToDateFolder.ServerItem, requests[index].ItemSpec.Item) && !VersionControlPath.Equals(upToDateFolder.ServerItem, requests[index].ItemSpec.Item) && (requests[index].ItemSpec.RecursionType == RecursionType.Full || VersionControlPath.IsImmediateChild(upToDateFolder.ServerItem, requests[index].ItemSpec.Item)) && upToDateFolder.Type != WorkingFolderType.Cloak)
            {
              string localItem = this.m_workspace.ServerToLocalItem(this.RequestContext, requests[index].ItemSpec.Item, false);
              if (localItem != null)
              {
                string relative = VersionControlPath.MakeRelative(upToDateFolder.ServerItem, requests[index].ItemSpec.Item);
                if (localItem.Length + relative.Length > 248 || FileSpec.Compare(FileSpec.Combine(localItem, relative), upToDateFolder.LocalItem) != 0)
                  this.m_requests.Add(new GetRequest()
                  {
                    ItemSpec = new ItemSpec(upToDateFolder.ServerItem, requests[index].ItemSpec.RecursionType == RecursionType.Full ? RecursionType.Full : RecursionType.None),
                    VersionSpec = requests[index].VersionSpec
                  });
              }
            }
          }
        }
      }
      if (this.m_requests.Count != requests.Length)
      {
        for (int length1 = requests.Length; length1 < this.m_requests.Count; ++length1)
        {
          for (int length2 = requests.Length; length2 < this.m_requests.Count; ++length2)
          {
            if (this.m_requests[length1] != this.m_requests[length2] && this.m_requests[length1].VersionSpec == this.m_requests[length2].VersionSpec && this.m_requests[length1].ItemSpec.RecursionType == RecursionType.Full && VersionControlPath.IsSubItem(this.m_requests[length2].ItemSpec.Item, this.m_requests[length1].ItemSpec.Item))
            {
              string localItem1 = this.m_workspace.ServerToLocalItem(this.RequestContext, this.m_requests[length1].ItemSpec.Item, false);
              string localItem2 = this.m_workspace.ServerToLocalItem(this.RequestContext, this.m_requests[length2].ItemSpec.Item, false);
              if (localItem1 != null && localItem2 != null)
              {
                string relative = VersionControlPath.MakeRelative(this.m_requests[length2].ItemSpec.Item, this.m_requests[length1].ItemSpec.Item);
                if (FileSpec.Compare(FileSpec.Combine(localItem1, relative), localItem2) == 0)
                  this.m_requests.RemoveAt(length2);
              }
            }
          }
        }
      }
      for (int index = 0; index < this.m_requests.Count; ++index)
      {
        this.m_getOpsCollections.Add(new StreamingCollection<GetOperation>((Command) this)
        {
          HandleExceptions = false
        });
        if (this.m_requests[index].ItemSpec != null)
        {
          string localItem = ItemSpec.toLocalItem(this.RequestContext, this.m_requests[index].ItemSpec.Item, this.m_workspace, false);
          if (localItem != null && localItem.Length > 259)
            throw new RepositoryPathTooLongException(localItem, 259);
          this.m_requestLocalItems.Add(localItem);
        }
        else
          this.m_requestLocalItems.Add((string) null);
      }
    }

    public override void ContinueExecution()
    {
      Dictionary<string, Tuple<ItemPathPair, GetOperation>> dictionary = new Dictionary<string, Tuple<ItemPathPair, GetOperation>>((IEqualityComparer<string>) VersionControlPath.FullPathComparer);
      if (this.m_state == CommandGet.State.GetOps)
      {
        for (; this.m_requestIndex < this.m_requests.Count; ++this.m_requestIndex)
        {
          GetRequest request = this.m_requests[this.m_requestIndex];
          if (this.m_getBinder == null)
          {
            string localItem = (string) null;
            RecursionType recursive = RecursionType.Full;
            if (request.ItemSpec != null)
            {
              localItem = this.m_requestLocalItems[this.m_requestIndex];
              if (localItem == null)
              {
                string folderName = request.ItemSpec.Item;
                string parent = (string) null;
                while (!VersionControlPath.Equals(folderName, "$/"))
                {
                  folderName = VersionControlPath.GetFolderName(folderName);
                  parent = this.m_workspace.ServerToLocalItem(this.RequestContext, folderName, false);
                  if (parent != null)
                  {
                    string relative = VersionControlPath.MakeRelative(request.ItemSpec.Item, folderName);
                    localItem = FileSpec.Combine(parent, relative);
                    break;
                  }
                }
                if (parent == null)
                  continue;
              }
              recursive = request.ItemSpec.RecursionType;
            }
            this.m_resultCollection = this.m_db.Get(this.m_workspace, localItem, request.VersionSpec, recursive, this.m_options, this.m_versionControlRequestContext.MaxSupportedServerPathLength);
            this.m_getBinder = this.m_resultCollection.GetCurrent<GetOperation>();
            ++this.m_loopCount;
            if (this.m_throttle && this.m_loopCount >= 300 && this.m_loopCount % 100 == 0)
              Thread.Sleep(500);
          }
          bool flag = false;
          while (!this.IsCacheFull && this.m_getBinder.MoveNext())
          {
            GetOperation current = this.m_getBinder.Current;
            ItemPathPair sourceItemPathPair = current.SourceItemPathPair;
            if (VersionControlPath.Equals(current.SourceServerItem, current.TargetServerItem))
              current.SourceItemPathPair = ItemPathPair.FromServerItem(GetOperationColumns.EmptySourceItem);
            if (current.TargetServerItem != null && (!this.SecurityWrapper.HasItemPermissionExpect(this.m_versionControlRequestContext, VersionedItemPermissions.Read, current.TargetItemPathPair, false, true) || !string.Equals(current.SourceServerItem, GetOperationColumns.EmptySourceItem, StringComparison.OrdinalIgnoreCase) && !VersionControlPath.Equals(current.SourceServerItem, current.TargetServerItem) && !this.SecurityWrapper.HasItemPermissionExpect(this.m_versionControlRequestContext, VersionedItemPermissions.Read, current.SourceItemPathPair, false, true)))
            {
              if (current.SourceLocalItem != null && current.PendingChangeId == 0)
              {
                current.VersionServer = 0;
                current.TargetLocalItem = (string) null;
                current.fileId = 0;
              }
              else
                continue;
            }
            if (current.TargetLocalItem == null || this.m_requests[this.m_requestIndex].ItemSpec == null || this.m_requests[this.m_requestIndex].ItemSpec.postMatch(current.TargetLocalItem))
            {
              if (current.SourceServerItem != null && (PathLength) current.SourceServerItem.Length > this.m_versionControlRequestContext.MaxSupportedServerPathLength + 1)
                throw new RepositoryPathTooLongDetailedException(current.SourceServerItem, (int) this.m_versionControlRequestContext.MaxSupportedServerPathLength);
              if (current.TargetServerItem != null && (PathLength) current.TargetServerItem.Length > this.m_versionControlRequestContext.MaxSupportedServerPathLength + 1)
                throw new RepositoryPathTooLongDetailedException(current.TargetServerItem, (int) this.m_versionControlRequestContext.MaxSupportedServerPathLength);
              if (current.ItemId == -6 && sourceItemPathPair.ProjectNamePath != null)
                dictionary[sourceItemPathPair.ProjectGuidPath ?? sourceItemPathPair.ProjectNamePath] = new Tuple<ItemPathPair, GetOperation>(sourceItemPathPair, current);
              this.m_signer.SignObject((ISignable) current);
              this.m_getOpsCollections[this.m_requestIndex].Enqueue(current);
              ++this.m_returnedResults;
              flag = this.m_maxResults > 0 && this.m_returnedResults >= this.m_maxResults;
              if (flag)
              {
                for (int requestIndex = this.m_requestIndex; requestIndex < this.m_requests.Count; ++requestIndex)
                  this.m_getOpsCollections[requestIndex].IsComplete = true;
                break;
              }
            }
          }
          if (this.IsCacheFull | flag)
          {
            this.m_hasMoreData = true;
            break;
          }
          this.m_getBinder = (ObjectBinder<GetOperation>) null;
          this.m_resultCollection.Dispose();
          this.m_resultCollection = (ResultCollection) null;
          this.m_getOpsCollections[this.m_requestIndex].IsComplete = true;
          this.m_hasMoreData = false;
        }
        if (dictionary.Count > 0)
        {
          using (VersionedItemComponent versionedItemComponent = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext))
          {
            using (ResultCollection extantItemIds = versionedItemComponent.GenerateExtantItemIds(this.m_workspace, dictionary.Values.Select<Tuple<ItemPathPair, GetOperation>, ItemPathPair>((Func<Tuple<ItemPathPair, GetOperation>, ItemPathPair>) (di => di.Item1))))
            {
              foreach (Item obj in extantItemIds.GetCurrent<Item>())
              {
                Tuple<ItemPathPair, GetOperation> tuple;
                if (dictionary.TryGetValue(obj.ItemPathPair.ProjectGuidPath ?? obj.ServerItem, out tuple))
                  tuple.Item2.ItemId = obj.ItemId;
              }
            }
          }
        }
        this.m_state = CommandGet.State.Properties;
      }
      this.m_signer.FlushDeferredSignatures();
      if (this.m_state == CommandGet.State.Properties)
      {
        if (this.m_propertyMerger != null)
        {
          if (!this.m_hasMoreProperties)
            this.m_propertyMerger.Execute(this.m_getOpsCollections);
          this.m_hasMoreProperties = this.m_propertyMerger.TryMergeNextPage();
        }
        this.m_state = CommandGet.State.Attributes;
      }
      if (this.m_state == CommandGet.State.Attributes && this.m_attributeMerger != null)
      {
        if (!this.m_hasMoreAttributes)
          this.m_attributeMerger.Execute(this.m_getOpsCollections);
        this.m_hasMoreAttributes = this.m_attributeMerger.TryMergeNextPage();
      }
      this.m_state = this.m_hasMoreProperties ? CommandGet.State.Properties : (this.m_hasMoreAttributes ? CommandGet.State.Attributes : (this.m_hasMoreData ? CommandGet.State.GetOps : CommandGet.State.Complete));
    }

    public List<StreamingCollection<GetOperation>> GetOperations => this.m_getOpsCollections;

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.m_db != null)
      {
        this.m_db.Cancel();
        this.m_db.Dispose();
        this.m_db = (VersionedItemComponent) null;
      }
      if (this.m_signer != null)
      {
        this.m_signer.Dispose();
        this.m_signer = (UrlSigner) null;
      }
      if (this.m_resultCollection != null)
      {
        this.m_resultCollection.Dispose();
        this.m_resultCollection = (ResultCollection) null;
      }
      if (this.m_propertyMerger != null)
      {
        this.m_propertyMerger.Dispose();
        this.m_propertyMerger = (PropertyMerger<GetOperation>) null;
      }
      if (this.m_attributeMerger == null)
        return;
      this.m_attributeMerger.Dispose();
      this.m_attributeMerger = (PropertyMerger<GetOperation>) null;
    }

    private enum State
    {
      GetOps,
      Attributes,
      Properties,
      Complete,
    }
  }
}
