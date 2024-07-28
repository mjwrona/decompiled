// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfsVersionControlProvider
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi.Legacy;
using Microsoft.TeamFoundation.SourceControl.WebServer.Legacy;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class TfsVersionControlProvider : VersionControlProvider
  {
    private const int c_maxChangesetsForAuthorCountsQuery = 1000;
    private static readonly string[] s_defaultPropertyFilters = new string[1]
    {
      "Microsoft.TeamFoundation.VersionControl.SymbolicLink"
    };

    public TfsVersionControlProvider(IVssRequestContext requestContext)
      : base(requestContext, (string) null)
    {
      this.RepositoryName = "$/" + requestContext.GetService<IRequestProjectService>().GetProject(requestContext)?.Name;
    }

    public void CreateProjectFolder(string teamProjectUri)
    {
      ProjectInfo project = this.RequestContext.GetService<IProjectService>().GetProject(this.RequestContext, ProjectInfo.GetProjectId(teamProjectUri));
      project.PopulateProperties(this.RequestContext, ProcessTemplateIdPropertyNames.ProcessTemplateType);
      TeamProjectFolderPermission[] folderPermissions = VersionControlProcessTemplateUtility.GetTfvcProjectFolderPermissions(this.RequestContext, project, true);
      TeamFoundationVersionControlService service = this.RequestContext.GetService<TeamFoundationVersionControlService>();
      TeamProjectFolderOptions projectFolderOptions = new TeamProjectFolderOptions()
      {
        TeamProject = project.Name,
        Comment = string.Format("CreateProjectFolderComment", (object) project.Name),
        Permissions = folderPermissions,
        KeepExistingPermissions = true
      };
      IVssRequestContext requestContext = this.RequestContext;
      TeamProjectFolderOptions teamProjectOptions = projectFolderOptions;
      service.CreateTeamProjectFolder(requestContext, teamProjectOptions);
    }

    public override Stream GetFileContentStream(Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel file)
    {
      TfsItem tfsItem = (TfsItem) file;
      this.CheckReadPermission(tfsItem);
      if (file.IsFolder)
        throw new TeamFoundationServiceException("ErrorInvalidFileType");
      return this.RequestContext.GetService<TeamFoundationFileService>().RetrieveFile(this.RequestContext, (long) tfsItem.FileId, false, out byte[] _, out long _, out CompressionType _) ?? throw new TeamFoundationServiceException(string.Format("DestroyedFileContentUnavailableException", (object) tfsItem.ChangesetVersion, (object) file.ServerItem), TeamFoundationEventId.ServerItemException);
    }

    public override StoredFile GetFileContentStreamWithMetadata(Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel file)
    {
      TfsItem tfsItem = (TfsItem) file;
      this.CheckReadPermission(tfsItem);
      if (file.IsFolder)
        throw new TeamFoundationServiceException("ErrorInvalidFileType");
      byte[] hashValue;
      return new StoredFile(this.RequestContext.GetService<TeamFoundationFileService>().RetrieveFile(this.RequestContext, (long) tfsItem.FileId, false, out hashValue, out long _, out CompressionType _) ?? throw new TeamFoundationServiceException(string.Format("DestroyedFileContentUnavailableException", (object) tfsItem.ChangesetVersion, (object) file.ServerItem), TeamFoundationEventId.ServerItemException), hashValue);
    }

    public override byte[] GetFileHashValueNoContent(Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel file)
    {
      TfsItem tfsItem = (TfsItem) file;
      this.CheckReadPermission(tfsItem);
      if (file.IsFolder)
        throw new TeamFoundationServiceException("ErrorInvalidFileType");
      return (this.RequestContext.GetService<TeamFoundationFileService>().GetFileStatistics(this.RequestContext, (long) tfsItem.FileId) ?? throw new TeamFoundationServiceException(string.Format("DestroyedFileContentUnavailableException", (object) tfsItem.ChangesetVersion, (object) file.ServerItem), TeamFoundationEventId.ServerItemException)).HashValue;
    }

    public override Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel GetItem(
      string path,
      string version,
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemDetailsOptions details)
    {
      VersionControlPath.ValidatePath(path);
      VersionSpec versionSpec = (VersionSpec) new LatestVersionSpec();
      if (!string.IsNullOrEmpty(version))
      {
        versionSpec = LegacyTfsModelExtensions.ParseVersionSpecString(version, (VersionSpec) null);
        if (versionSpec == null)
          throw new InvalidArgumentValueException(nameof (version));
      }
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlRecursionType recursionLevel = details.RecursionLevel;
      IEnumerable<TfsItem> items = this.GetItems(path, versionSpec, TfsVersionControlProvider.ConvertVersionControlRecursionType(recursionLevel), DeletedState.NonDeleted, ItemType.Any, TfsVersionControlProvider.s_defaultPropertyFilters);
      TfsItem file = items != null && items.Any<TfsItem>() ? items.First<TfsItem>() : throw new ItemNotFoundException(this.RequestContext, new ItemSpec(path, RecursionType.None), versionSpec);
      if (details.RecursionLevel == Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlRecursionType.OneLevel)
        file.ChildItems = (IList<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel>) ((IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel>) items.Skip<TfsItem>(1)).ToList<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel>();
      else if (details.RecursionLevel == Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlRecursionType.Full)
      {
        Dictionary<string, TfsItem> dictionary = new Dictionary<string, TfsItem>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (TfsItem tfsItem in items)
          dictionary[tfsItem.ServerItem] = tfsItem;
        foreach (TfsItem tfsItem1 in items.Skip<TfsItem>(1))
        {
          string folderName = VersionControlPath.GetFolderName(tfsItem1.ServerItem);
          TfsItem tfsItem2;
          if (dictionary.TryGetValue(folderName, out tfsItem2))
          {
            if (tfsItem2.ChildItems == null)
              tfsItem2.ChildItems = (IList<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel>) new List<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel>();
            tfsItem2.ChildItems.Add((Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel) tfsItem1);
          }
        }
      }
      if (details.IncludeContentMetadata)
      {
        int encoding = 0;
        if (!file.IsFolder)
          encoding = file.Encoding != -1 ? this.TryDetectFileEncoding((Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel) file, file.Encoding, details.ScanBytesForEncoding) : (!file.IsSymbolicLink ? -1 : 0);
        file.ContentMetadata = this.GetFileContentMetadata(file.ServerItem, file.IsFolder, encoding);
        if (!file.IsFolder)
          file.ContentMetadata.VisualStudioWebLink = this.GetVisualStudioWebLink(path);
      }
      if (details.IncludeVersionDescription)
        file.VersionDescription = this.GetVersionDescription((Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel) file);
      return (Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel) file;
    }

    public string GetVisualStudioWebLink(string path)
    {
      ProjectInfo project = this.RequestContext.GetService<IRequestProjectService>().GetProject(this.RequestContext);
      ArtifactId artifactId = new ArtifactId("VersionControl", "LaunchLatestVersionedItem", path);
      return VisualStudioLinkingUtility.GetProjectArtifactLink(this.RequestContext, project != null ? project.Id : Guid.Empty, artifactId);
    }

    public IEnumerable<TfsItem> GetItemsById(int[] ids, int changeset, bool generateDownloadUrls) => TfsVersionControlProvider.GetItemsById(this.RequestContext, ids, changeset, generateDownloadUrls);

    public static IEnumerable<TfsItem> GetItemsById(
      IVssRequestContext requestContext,
      int[] ids,
      int changeset,
      bool generateDownloadUrls)
    {
      try
      {
        requestContext.TraceEnter(513040, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetItemsById));
        TeamFoundationVersionControlService service = requestContext.GetService<TeamFoundationVersionControlService>();
        if (changeset < 1)
          changeset = int.MaxValue;
        IVssRequestContext requestContext1 = requestContext;
        int[] itemIds = ids;
        int changeSet = changeset;
        int num = generateDownloadUrls ? 1 : 0;
        return (IEnumerable<TfsItem>) ((IEnumerable<Microsoft.TeamFoundation.VersionControl.Server.Item>) service.QueryItemsById(requestContext1, itemIds, changeSet, num != 0, 0)).Select<Microsoft.TeamFoundation.VersionControl.Server.Item, TfsItem>((Func<Microsoft.TeamFoundation.VersionControl.Server.Item, int, TfsItem>) ((item, index) =>
        {
          return item != null ? item.ToWebApiItem() : throw new ItemNotFoundException(ids[index].ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
        })).ToArray<TfsItem>();
      }
      finally
      {
        requestContext.TraceLeave(513045, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetItemsById));
      }
    }

    public TfsItem GetItem(string path, VersionSpec version) => this.GetItems(path, version, RecursionType.None, DeletedState.Any, ItemType.Any, (string[]) null).FirstOrDefault<TfsItem>() ?? throw new ItemNotFoundException(this.RequestContext, new ItemSpec(path, RecursionType.None), version);

    public IEnumerable<TfsItem> GetItems(
      string path,
      VersionSpec version,
      RecursionType recursion,
      DeletedState deletedState,
      ItemType itemType,
      string[] propertyFilters)
    {
      try
      {
        this.RequestContext.TraceEnter(513055, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetItems));
        TeamFoundationVersionControlService service = this.RequestContext.GetService<TeamFoundationVersionControlService>();
        path = VersionControlPath.GetFullPath(path);
        this.RequestContext.Trace(513060, TraceLevel.Verbose, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, "retrieving item for path {0}", (object) path);
        ItemSpec itemSpec1 = new ItemSpec(path, RecursionType.None, 0);
        if (version is TipVersionSpec && ((TipVersionSpec) version).Version is LatestVersionSpec)
          version = (VersionSpec) new LatestVersionSpec();
        if (version is TipVersionSpec)
        {
          this.RequestContext.Trace(513065, TraceLevel.Verbose, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, "Retrieving tip version of item");
          TipVersionSpec tipVersionSpec = (TipVersionSpec) version;
          ChangesetVersionSpec changesetVersionSpec = new ChangesetVersionSpec(this.GetItem(path, tipVersionSpec.Version).ChangesetVersion);
          TfsItem tfsItem = (TfsItem) null;
          using (TeamFoundationDataReader foundationDataReader = service.QueryHistory(this.RequestContext, (string) null, (string) null, itemSpec1, (VersionSpec) changesetVersionSpec, string.Empty, (VersionSpec) changesetVersionSpec, (VersionSpec) new LatestVersionSpec(), 1, true, false, true, false))
          {
            Changeset changeset = foundationDataReader.CurrentEnumerable<Changeset>().FirstOrDefault<Changeset>();
            tfsItem = this.GetItemFromChangeset(service, changeset, propertyFilters);
          }
          if (tfsItem == null)
            throw new ItemNotFoundException(this.RequestContext, itemSpec1, version);
          if (tfsItem.IsFolder && recursion != RecursionType.None)
            return this.GetItems(tfsItem.ServerItem, (VersionSpec) new LatestVersionSpec(), recursion, deletedState, itemType, propertyFilters);
          return (IEnumerable<TfsItem>) new TfsItem[1]
          {
            tfsItem
          };
        }
        if (version is PreviousVersionSpec)
        {
          PreviousVersionSpec previousVersionSpec = (PreviousVersionSpec) version;
          if (previousVersionSpec.Version is ShelvesetVersionSpec)
          {
            ShelvesetVersionSpec version1 = (ShelvesetVersionSpec) previousVersionSpec.Version;
            TfsChange tfsChange = this.GetShelvedChanges(path, RecursionType.Full, 0, version1, int.MaxValue, 0, (string[]) null, out bool _).FirstOrDefault<TfsChange>();
            if (tfsChange == null)
              throw new ItemNotFoundException(this.RequestContext, itemSpec1, version);
            VersionSpec version2 = tfsChange.PendingVersion != 0 ? (VersionSpec) new ChangesetVersionSpec(tfsChange.PendingVersion) : (VersionSpec) version1;
            string path1 = tfsChange.Item.ServerItem;
            if (!string.IsNullOrEmpty(tfsChange.SourceServerItem))
              path1 = tfsChange.SourceServerItem;
            return this.GetItems(path1, version2, recursion, deletedState, itemType, propertyFilters);
          }
          ChangesetVersionSpec changesetVersionSpec = new ChangesetVersionSpec(this.GetItem(path, previousVersionSpec.Version).ChangesetVersion);
          TfsItem tfsItem = (TfsItem) null;
          using (TeamFoundationDataReader foundationDataReader = service.QueryHistory(this.RequestContext, (string) null, (string) null, itemSpec1, (VersionSpec) changesetVersionSpec, string.Empty, (VersionSpec) new ChangesetVersionSpec(1), (VersionSpec) changesetVersionSpec, 2, true, false, true, false))
          {
            Changeset changeset = foundationDataReader.CurrentEnumerable<Changeset>().LastOrDefault<Changeset>();
            tfsItem = this.GetItemFromChangeset(service, changeset, propertyFilters);
          }
          if (tfsItem == null)
            throw new ItemNotFoundException(this.RequestContext, itemSpec1, version);
          if (tfsItem.IsFolder && recursion != RecursionType.None)
            return this.GetItems(tfsItem.ServerItem, (VersionSpec) new ChangesetVersionSpec(tfsItem.ChangesetVersion), recursion, deletedState, itemType, propertyFilters);
          return (IEnumerable<TfsItem>) new TfsItem[1]
          {
            tfsItem
          };
        }
        if (version is ShelvesetVersionSpec)
        {
          ShelvesetVersionSpec shelvesetVersionSpec = version as ShelvesetVersionSpec;
          this.RequestContext.Trace(513075, TraceLevel.Verbose, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, "Retrieving shelveset version of item. {0};{1}", (object) shelvesetVersionSpec.Name, (object) shelvesetVersionSpec.Owner);
          RecursionType recursion1 = recursion;
          if (recursion == RecursionType.OneLevel)
            recursion1 = RecursionType.Full;
          IEnumerable<TfsChange> shelvedChanges = this.GetShelvedChanges(path, recursion1, 0, shelvesetVersionSpec, int.MaxValue, 0, propertyFilters, out bool _);
          return recursion == RecursionType.OneLevel ? this.NormalizeSparseTree(path, (VersionSpec) shelvesetVersionSpec, shelvedChanges).Select<TfsChange, TfsItem>((Func<TfsChange, TfsItem>) (c => (TfsItem) c.Item)) : shelvedChanges.Select<TfsChange, Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel>((Func<TfsChange, Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel>) (pc => pc.Item)).OfType<TfsItem>();
        }
        if (version is ChangeVersionSpec)
        {
          ChangeVersionSpec version3 = version as ChangeVersionSpec;
          this.RequestContext.Trace(513075, TraceLevel.Verbose, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, "Retrieving changeset version of item. Changeset: {0}", (object) version3.ChangesetId);
          using (TeamFoundationDataReader foundationDataReader = service.QueryChangesForChangeset(this.RequestContext, version3.ChangesetId, false, int.MaxValue, (ItemSpec) null, propertyFilters, false))
          {
            TfsChange[] array = foundationDataReader.CurrentEnumerable<Microsoft.TeamFoundation.VersionControl.Server.Change>().Where<Microsoft.TeamFoundation.VersionControl.Server.Change>((Func<Microsoft.TeamFoundation.VersionControl.Server.Change, bool>) (change => VersionControlPath.IsSubItem(change.Item.ServerItem, path))).Select<Microsoft.TeamFoundation.VersionControl.Server.Change, TfsChange>((Func<Microsoft.TeamFoundation.VersionControl.Server.Change, TfsChange>) (change => LegacyTfsModelExtensions.ToWebApiChangeModel(change))).ToArray<TfsChange>();
            if (array.Length == 0)
              throw new ItemNotFoundException(this.RequestContext, itemSpec1, version);
            return recursion == RecursionType.OneLevel ? this.NormalizeSparseTree(path, (VersionSpec) version3, (IEnumerable<TfsChange>) array).Select<TfsChange, TfsItem>((Func<TfsChange, TfsItem>) (c => (TfsItem) c.Item)) : ((IEnumerable<TfsChange>) array).Select<TfsChange, TfsItem>((Func<TfsChange, TfsItem>) (c => (TfsItem) c.Item));
          }
        }
        else
        {
          if (version is MergeSourceVersionSpec)
            return (IEnumerable<TfsItem>) TfvcItemUtility.GetMergeSourceChanges<TfsItem>(this.RequestContext, version as MergeSourceVersionSpec, path, (Func<ExtendedMerge, TfsItem>) (merge => merge.SourceItem.Item.ToWebApiItem()));
          ItemSpec itemSpec2 = new ItemSpec(path, recursion);
          TfsItem[] array;
          using (TeamFoundationDataReader foundationDataReader = service.QueryItems(this.RequestContext, (string) null, (string) null, new ItemSpec[1]
          {
            itemSpec2
          }, version, deletedState, itemType, false, 12, propertyFilters, (string[]) null))
            array = foundationDataReader.CurrentEnumerable<ItemSet>().SelectMany<ItemSet, Microsoft.TeamFoundation.VersionControl.Server.Item>((Func<ItemSet, IEnumerable<Microsoft.TeamFoundation.VersionControl.Server.Item>>) (itemset => (IEnumerable<Microsoft.TeamFoundation.VersionControl.Server.Item>) itemset.Items)).Select<Microsoft.TeamFoundation.VersionControl.Server.Item, TfsItem>((Func<Microsoft.TeamFoundation.VersionControl.Server.Item, TfsItem>) (item => item.ToWebApiItem())).ToArray<TfsItem>();
          return array.Length != 0 ? (IEnumerable<TfsItem>) array : throw new ItemNotFoundException(this.RequestContext, itemSpec2, version);
        }
      }
      finally
      {
        this.RequestContext.TraceLeave(513080, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetItems));
      }
    }

    private TfsItem GetItemFromChangeset(
      TeamFoundationVersionControlService vcService,
      Changeset changeset,
      string[] propertyFilters)
    {
      TfsItem tfsItem = (TfsItem) null;
      if (changeset != null)
      {
        Microsoft.TeamFoundation.VersionControl.Server.Change change = changeset.Changes.FirstOrDefault<Microsoft.TeamFoundation.VersionControl.Server.Change>();
        if (change != null)
        {
          tfsItem = change.Item.ToWebApiItem();
          if (propertyFilters != null)
          {
            using (TeamFoundationDataReader foundationDataReader = vcService.QueryItems(this.RequestContext, (string) null, (string) null, new ItemSpec[1]
            {
              new ItemSpec(tfsItem.ServerItem, RecursionType.None)
            }, (VersionSpec) new ChangesetVersionSpec(changeset.ChangesetId), DeletedState.Any, ItemType.Any, false, 0, propertyFilters, (string[]) null))
            {
              Microsoft.TeamFoundation.VersionControl.Server.Item obj = foundationDataReader.CurrentEnumerable<ItemSet>().SelectMany<ItemSet, Microsoft.TeamFoundation.VersionControl.Server.Item>((Func<ItemSet, IEnumerable<Microsoft.TeamFoundation.VersionControl.Server.Item>>) (set => (IEnumerable<Microsoft.TeamFoundation.VersionControl.Server.Item>) set.Items)).FirstOrDefault<Microsoft.TeamFoundation.VersionControl.Server.Item>();
              if (obj != null)
                tfsItem.ParsePropertyValues((IEnumerable<PropertyValue>) obj.PropertyValues);
            }
          }
        }
      }
      return tfsItem;
    }

    public static bool HasItemWithPath(IVssRequestContext requestContext, string path)
    {
      TeamFoundationVersionControlService service = requestContext.GetService<TeamFoundationVersionControlService>();
      path = VersionControlPath.GetFullPath(path);
      IVssRequestContext requestContext1 = requestContext;
      ItemSpec[] items = new ItemSpec[1]
      {
        new ItemSpec(path, RecursionType.None)
      };
      LatestVersionSpec version = new LatestVersionSpec();
      using (TeamFoundationDataReader foundationDataReader = service.QueryItems(requestContext1, (string) null, (string) null, items, (VersionSpec) version, DeletedState.NonDeleted, ItemType.Any, false, 0))
        return foundationDataReader.CurrentEnumerable<ItemSet>().SelectMany<ItemSet, Microsoft.TeamFoundation.VersionControl.Server.Item>((Func<ItemSet, IEnumerable<Microsoft.TeamFoundation.VersionControl.Server.Item>>) (itemset => (IEnumerable<Microsoft.TeamFoundation.VersionControl.Server.Item>) itemset.Items)).Any<Microsoft.TeamFoundation.VersionControl.Server.Item>();
    }

    private IEnumerable<TfsChange> NormalizeSparseTree(
      string path,
      VersionSpec version,
      IEnumerable<TfsChange> changes)
    {
      List<TfsChange> list = changes.GroupBy<TfsChange, string>((Func<TfsChange, string>) (change =>
      {
        string str = VersionControlPath.MakeRelative(change.Item.ServerItem, path);
        if (str.Length > 0)
        {
          int length = str.IndexOf('/');
          if (length >= 0)
            str = str.Substring(0, length);
        }
        return str;
      })).Select<IGrouping<string, TfsChange>, TfsChange>((Func<IGrouping<string, TfsChange>, TfsChange>) (g =>
      {
        string str = VersionControlPath.Combine(path, g.Key);
        Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlChangeType changeType = Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlChangeType.None;
        int val1_1 = int.MaxValue;
        DateTime dateTime = DateTime.MaxValue;
        bool flag = true;
        int val1_2 = int.MaxValue;
        foreach (TfsChange tfsChange in (IEnumerable<TfsChange>) g)
        {
          changeType |= tfsChange.ChangeType;
          val1_1 = Math.Min(val1_1, ((TfsItem) tfsChange.Item).ChangesetVersion);
          dateTime = new DateTime(Math.Min(dateTime.Ticks, tfsChange.Item.ChangeDate.Ticks));
          if (StringComparer.OrdinalIgnoreCase.Equals(str, tfsChange.Item.ServerItem))
            flag = tfsChange.Item.IsFolder;
          val1_2 = Math.Min(val1_2, ((TfsItem) tfsChange.Item).DeletionId);
        }
        TfsItem webApiItem = LegacyTfsModelExtensions.CreateWebApiItem(str, version);
        webApiItem.ChangesetVersion = val1_1;
        webApiItem.IsFolder = flag;
        webApiItem.DeletionId = val1_2;
        webApiItem.ChangeDate = dateTime;
        return new TfsChange(webApiItem, changeType, (string) null);
      })).ToList<TfsChange>();
      if (list.Count > 0 && !StringComparer.OrdinalIgnoreCase.Equals(path, list[0].Item.ServerItem))
      {
        TfsItem webApiItem = LegacyTfsModelExtensions.CreateWebApiItem(path, version);
        webApiItem.IsFolder = true;
        webApiItem.ChangesetVersion = int.MaxValue;
        webApiItem.ChangeDate = DateTime.MaxValue;
        webApiItem.DeletionId = int.MaxValue;
        TfsChange tfsChange1 = new TfsChange(webApiItem, Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlChangeType.None, (string) null);
        foreach (TfsChange tfsChange2 in list)
        {
          tfsChange1.ChangeType |= tfsChange2.ChangeType;
          webApiItem.ChangesetVersion = Math.Min(webApiItem.ChangesetVersion, ((TfsItem) tfsChange2.Item).ChangesetVersion);
          TfsItem tfsItem = webApiItem;
          DateTime changeDate = webApiItem.ChangeDate;
          long ticks1 = changeDate.Ticks;
          changeDate = tfsChange2.Item.ChangeDate;
          long ticks2 = changeDate.Ticks;
          DateTime dateTime = new DateTime(Math.Min(ticks1, ticks2));
          tfsItem.ChangeDate = dateTime;
          webApiItem.DeletionId = Math.Min(webApiItem.DeletionId, ((TfsItem) tfsChange2.Item).DeletionId);
        }
        list.Insert(0, tfsChange1);
      }
      return (IEnumerable<TfsChange>) list;
    }

    private string GetVersionDescription(Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel item)
    {
      TfsItem tfsItem1 = (TfsItem) item;
      string empty = string.Empty;
      string versionDescription;
      if (tfsItem1.IsPendingChange)
      {
        versionDescription = "Pending";
      }
      else
      {
        versionDescription = tfsItem1.ChangesetVersion.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        TfsItem tfsItem2 = (TfsItem) null;
        try
        {
          tfsItem2 = this.GetItem(tfsItem1.ServerItem, (VersionSpec) new TipVersionSpec(tfsItem1.VersionString));
        }
        catch (ItemNotFoundException ex)
        {
        }
        List<string> values = new List<string>();
        if (tfsItem1.DeletionId > 0)
          values.Add("SccItemDeleted");
        if (tfsItem2 != null && tfsItem2.ChangesetVersion == tfsItem1.ChangesetVersion)
          values.Add("SccItemLatest");
        if (values.Count > 0)
          versionDescription = versionDescription + " (" + string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, (IEnumerable<string>) values) + ")";
      }
      return versionDescription;
    }

    private void CheckReadPermission(TfsItem item)
    {
      if (!this.CheckReadPermission(item.ServerItem))
        throw new ItemNotFoundException(this.RequestContext, new ItemSpec(item.ServerItem, RecursionType.None), LegacyTfsModelExtensions.ParseVersionSpecString(item.VersionString, (VersionSpec) null));
    }

    private bool CheckReadPermission(string serverItem) => new VersionControlRequestContext(this.RequestContext, this.RequestContext.GetService<TeamFoundationVersionControlService>()).GetRepositorySecurity().HasPermission(this.RequestContext, serverItem, 1, false);

    public override ChangeList GetChangeList(string version, int maxNumberOfChanges)
    {
      VersionSpec versionSpec = LegacyTfsModelExtensions.ParseVersionSpecString(version, (VersionSpec) null);
      if (versionSpec == null || versionSpec is LatestVersionSpec)
        versionSpec = (VersionSpec) new ChangesetVersionSpec(new LatestVersionSpec().ToChangeset(this.RequestContext));
      switch (versionSpec)
      {
        case ShelvesetVersionSpec _:
          ShelvesetVersionSpec shelvesetVersionSpec = (ShelvesetVersionSpec) versionSpec;
          return (ChangeList) this.GetShelveset(shelvesetVersionSpec.Name, shelvesetVersionSpec.Owner, maxNumberOfChanges);
        case ChangesetVersionSpec _:
          return (ChangeList) this.GetChangeset(((ChangesetVersionSpec) versionSpec).ChangesetId, maxNumberOfChanges, true);
        default:
          throw new TeamFoundationServiceException(string.Format("InvalidVersionSpecForChangeModel", (object) version));
      }
    }

    public override ChangeQueryResults GetChangeListChanges(
      string version,
      int maxNumberOfChanges,
      int skipCount)
    {
      VersionSpec versionSpecString = LegacyTfsModelExtensions.ParseVersionSpecString(version, (VersionSpec) null);
      ChangeQueryResults changeListChanges = new ChangeQueryResults();
      switch (versionSpecString)
      {
        case ShelvesetVersionSpec _:
          bool allChangesIncluded1;
          IEnumerable<TfsChange> shelvedChanges = this.GetShelvedChanges("$/", RecursionType.Full, 2, (ShelvesetVersionSpec) versionSpecString, maxNumberOfChanges, skipCount, (string[]) null, out allChangesIncluded1);
          changeListChanges.Results = (IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.Change>) shelvedChanges;
          changeListChanges.MoreResultsAvailable = !allChangesIncluded1;
          break;
        case ChangesetVersionSpec _:
          bool allChangesIncluded2;
          IEnumerable<Microsoft.TeamFoundation.VersionControl.Server.Change> changesetChanges = this.GetChangesetChanges(((ChangesetVersionSpec) versionSpecString).ChangesetId, maxNumberOfChanges, out allChangesIncluded2);
          changeListChanges.Results = (IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.Change>) changesetChanges.Skip<Microsoft.TeamFoundation.VersionControl.Server.Change>(skipCount).Select<Microsoft.TeamFoundation.VersionControl.Server.Change, TfsChange>((Func<Microsoft.TeamFoundation.VersionControl.Server.Change, TfsChange>) (c => LegacyTfsModelExtensions.ToWebApiChangeModel(c))).ToList<TfsChange>();
          changeListChanges.MoreResultsAvailable = !allChangesIncluded2;
          break;
        default:
          throw new TeamFoundationServiceException(string.Format("InvalidVersionSpecForChangeModel", (object) version));
      }
      changeListChanges.ChangeCounts = Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ChangeListHelpers.ComputeChangeCounts(changeListChanges.Results);
      return changeListChanges;
    }

    public override HistoryQueryResults QueryHistory(ChangeListSearchCriteria searchCriteria)
    {
      int valueOrDefault = searchCriteria.Top.GetValueOrDefault(10000);
      if (valueOrDefault == int.MaxValue)
        --valueOrDefault;
      return searchCriteria.ItemPaths != null && searchCriteria.ItemPaths.Any<string>() ? this.GetMultipleItemPathsHistory(searchCriteria, valueOrDefault) : this.GetSingleItemPathHistory(searchCriteria, valueOrDefault);
    }

    protected override void SetSecuredObject(VersionControlSecuredObject securableObject)
    {
    }

    private HistoryQueryResults GetSingleItemPathHistory(
      ChangeListSearchCriteria searchCriteria,
      int maxLength)
    {
      if (string.IsNullOrEmpty(searchCriteria.ItemPath))
        searchCriteria.ItemPath = "$/";
      else
        VersionControlPath.ValidatePath(searchCriteria.ItemPath);
      if (!string.IsNullOrEmpty(searchCriteria.ItemVersion) && LegacyTfsModelExtensions.ParseVersionSpecString(searchCriteria.ItemVersion, (VersionSpec) null) is MergeSourceVersionSpec versionSpecString)
        return this.GetMergeSourceHistory(versionSpecString, searchCriteria.ItemPath);
      TfsItem tfsItem = (TfsItem) this.GetItem(searchCriteria.ItemPath, searchCriteria.ItemVersion);
      bool flag = !tfsItem.IsFolder;
      RecursionType recursionType = flag ? RecursionType.None : RecursionType.Full;
      ItemSpec itemSpec = new ItemSpec(tfsItem.ServerItem, recursionType);
      VersionSpec versionItem = string.IsNullOrEmpty(searchCriteria.ItemVersion) ? (VersionSpec) new LatestVersionSpec() : (VersionSpec) new ChangesetVersionSpec(tfsItem.ChangesetVersion);
      VersionSpec versionFrom = this.GetVersionFrom(searchCriteria);
      VersionSpec versionTo = this.GetVersionTo(searchCriteria);
      using (TeamFoundationDataReader source = this.RequestContext.GetService<TeamFoundationVersionControlService>().QueryHistory(this.RequestContext, (string) null, (string) null, itemSpec, versionItem, searchCriteria.User, versionFrom, versionTo, maxLength + 1, flag, false, !searchCriteria.FollowRenames, false))
        return this.GetHistoryEntriesFromChangesets(source.OfType<Changeset>().ToList<Changeset>(), maxLength, flag, tfsItem);
    }

    private HistoryQueryResults GetMultipleItemPathsHistory(
      ChangeListSearchCriteria searchCriteria,
      int maxLength)
    {
      VersionSpec versionFrom = this.GetVersionFrom(searchCriteria);
      VersionSpec versionTo = this.GetVersionTo(searchCriteria);
      List<TfvcMappingFilter> mappings = new List<TfvcMappingFilter>();
      ItemSpec itemSpec = new ItemSpec();
      if (searchCriteria.ItemPaths != null && searchCriteria.ItemPaths.Any<string>())
      {
        foreach (string itemPath in searchCriteria.ItemPaths)
        {
          if (!string.IsNullOrEmpty(itemPath))
          {
            VersionControlPath.ValidatePath(itemPath);
            mappings.Add(new TfvcMappingFilter()
            {
              Exclude = false,
              ServerPath = itemPath
            });
          }
        }
      }
      else
        mappings.Add(new TfvcMappingFilter()
        {
          Exclude = false,
          ServerPath = "$/"
        });
      return this.GetHistoryEntriesFromChangesets(this.RequestContext.GetService<TeamFoundationVersionControlService>().QueryChangesetRange(this.RequestContext, (IEnumerable<TfvcMappingFilter>) mappings, versionFrom, versionTo, maxLength + 1, false), maxLength, false);
    }

    private VersionSpec GetVersionFrom(ChangeListSearchCriteria searchCriteria)
    {
      VersionSpec versionFrom = (VersionSpec) null;
      if (!string.IsNullOrEmpty(searchCriteria.FromVersion))
      {
        versionFrom = LegacyTfsModelExtensions.ParseVersionSpecString(searchCriteria.FromVersion, (VersionSpec) null) ?? (VersionSpec) new ChangesetVersionSpec(searchCriteria.FromVersion);
      }
      else
      {
        DateTime? fromDate = this.RequestContext.ParseFromDate(searchCriteria.FromDate);
        if (fromDate.HasValue)
          versionFrom = (VersionSpec) new DateVersionSpec()
          {
            Date = fromDate.Value
          };
      }
      return versionFrom;
    }

    private VersionSpec GetVersionTo(ChangeListSearchCriteria searchCriteria)
    {
      VersionSpec versionTo = (VersionSpec) null;
      if (!string.IsNullOrEmpty(searchCriteria.ToVersion))
      {
        versionTo = LegacyTfsModelExtensions.ParseVersionSpecString(searchCriteria.ToVersion, (VersionSpec) null) ?? (VersionSpec) new ChangesetVersionSpec(searchCriteria.ToVersion);
      }
      else
      {
        DateTime? toDate = this.RequestContext.ParseToDate(searchCriteria.ToDate);
        if (toDate.HasValue)
          versionTo = (VersionSpec) new DateVersionSpec()
          {
            Date = toDate.Value
          };
      }
      return versionTo;
    }

    private HistoryQueryResults GetHistoryEntriesFromChangesets(
      List<Changeset> changesets,
      int maxLength,
      bool singleFileMode,
      TfsItem item = null)
    {
      List<HistoryEntry> historyEntryList = new List<HistoryEntry>();
      HistoryQueryResults entriesFromChangesets = new HistoryQueryResults();
      entriesFromChangesets.Results = (IEnumerable<HistoryEntry>) historyEntryList;
      foreach (Changeset changeset in changesets)
      {
        if (historyEntryList.Count == maxLength)
        {
          entriesFromChangesets.MoreResultsAvailable = true;
          break;
        }
        TfsHistoryEntry tfsHistoryEntry = new TfsHistoryEntry();
        tfsHistoryEntry.ChangeList = (ChangeList) this.GetChangesetModel(changeset, 0, false);
        if (item != null)
          tfsHistoryEntry.ServerItem = item.ServerItem;
        tfsHistoryEntry.ItemChangeType = Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlChangeType.None;
        if (singleFileMode)
        {
          Microsoft.TeamFoundation.VersionControl.Server.Change change = changeset.Changes.FirstOrDefault<Microsoft.TeamFoundation.VersionControl.Server.Change>();
          if (change != null)
          {
            tfsHistoryEntry.ItemChangeType = (Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlChangeType) change.ChangeType;
            tfsHistoryEntry.ServerItem = change.Item.ServerItem;
            tfsHistoryEntry.FileId = change.Item.fileId;
            tfsHistoryEntry.Encoding = change.Item.Encoding;
          }
        }
        historyEntryList.Add((HistoryEntry) tfsHistoryEntry);
      }
      return entriesFromChangesets;
    }

    private HistoryQueryResults GetMergeSourceHistory(
      MergeSourceVersionSpec versionSpec,
      string itemPath)
    {
      List<HistoryEntry> historyEntryList = new List<HistoryEntry>();
      HistoryQueryResults mergeSourceHistory1 = new HistoryQueryResults();
      mergeSourceHistory1.Results = (IEnumerable<HistoryEntry>) historyEntryList;
      IList<TfsHistoryEntry> mergeSourceChanges = TfvcItemUtility.GetMergeSourceChanges<TfsHistoryEntry>(this.RequestContext, versionSpec, itemPath, (Func<ExtendedMerge, TfsHistoryEntry>) (merge =>
      {
        TfsHistoryEntry mergeSourceHistory2 = new TfsHistoryEntry()
        {
          ChangeList = (ChangeList) this.GetChangesetModel(merge.SourceChangeset),
          ServerItem = merge.SourceItem.Item.ServerItem,
          ItemChangeType = Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlChangeType.None
        };
        if (!merge.SourceItem.Item.IsFolder)
        {
          mergeSourceHistory2.ItemChangeType = (Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlChangeType) merge.SourceItem.ChangeType;
          mergeSourceHistory2.FileId = merge.SourceItem.Item.fileId;
          mergeSourceHistory2.Encoding = merge.SourceItem.Item.Encoding;
        }
        return mergeSourceHistory2;
      }));
      historyEntryList.AddRange((IEnumerable<HistoryEntry>) mergeSourceChanges);
      historyEntryList.Sort((Comparison<HistoryEntry>) ((x, y) => -((TfsChangeList) x.ChangeList).ChangesetId.CompareTo(((TfsChangeList) y.ChangeList).ChangesetId)));
      return mergeSourceHistory1;
    }

    public List<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.FileDiff> GetAnnotateDiffModels(
      TfsAnnotateDiffParameters[] annotateDiffParametersArray)
    {
      using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(this.RequestContext, "TfsVersionControlProvider.GetAnnotateDiffModels"))
      {
        performanceTimer.AddProperty("VersionCount", (object) annotateDiffParametersArray.Length);
        FileDiffParameters diffParameters = new FileDiffParameters()
        {
          IgnoreTrimmedWhitespace = new bool?(true),
          LineNumbersOnly = true
        };
        TeamFoundationFileService fileService = this.RequestContext.GetService<TeamFoundationFileService>();
        List<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.FileDiff> annotateDiffModels = new List<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.FileDiff>();
        foreach (TfsAnnotateDiffParameters annotateDiffParameters1 in annotateDiffParametersArray)
        {
          TfsAnnotateDiffParameters annotateDiffParameters = annotateDiffParameters1;
          Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.FileDiff model = new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.FileDiff();
          this.PopulateFileDiffModelBlocks(model, diffParameters, (Func<StoredFile>) (() =>
          {
            if (annotateDiffParameters.oFileId == 0)
              return (StoredFile) null;
            if (!this.CheckReadPermission(annotateDiffParameters.oServerItem))
              throw new ItemNotFoundException(annotateDiffParameters.oServerItem);
            return new StoredFile(fileService.RetrieveFile(this.RequestContext, (long) annotateDiffParameters.oFileId, false, out hashValue, out contentLength, out compressionType), hashValue);
          }), (Func<StoredFile>) (() =>
          {
            if (annotateDiffParameters.mFileId == 0)
              return (StoredFile) null;
            if (!this.CheckReadPermission(annotateDiffParameters.mServerItem))
              throw new ItemNotFoundException(annotateDiffParameters.mServerItem);
            return new StoredFile(fileService.RetrieveFile(this.RequestContext, (long) annotateDiffParameters.mFileId, false, out hashValue, out contentLength, out compressionType), hashValue);
          }), annotateDiffParameters.oEncoding, annotateDiffParameters.mEncoding);
          annotateDiffModels.Add(model);
        }
        return annotateDiffModels;
      }
    }

    private IEnumerable<TfsChange> GetShelvedChanges(
      string itemPath,
      RecursionType recursion,
      int deletionId,
      ShelvesetVersionSpec versionSpec,
      int maxCount,
      int skipCount,
      string[] propertyFilters,
      out bool allChangesIncluded)
    {
      return TfsVersionControlProvider.GetShelvedChanges(this.RequestContext, itemPath, recursion, deletionId, versionSpec, maxCount, skipCount, propertyFilters, out allChangesIncluded);
    }

    public static IEnumerable<TfsChange> GetShelvedChanges(
      IVssRequestContext requestContext,
      string itemPath,
      RecursionType recursion,
      int deletionId,
      ShelvesetVersionSpec versionSpec,
      int maxCount,
      int skipCount,
      string[] propertyFilters,
      out bool allChangesIncluded)
    {
      int num = 0;
      try
      {
        requestContext.TraceEnter(513115, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetShelvedChanges));
        ItemSpec itemSpec = new ItemSpec(itemPath, recursion, deletionId);
        TeamFoundationVersionControlService service = requestContext.GetService<TeamFoundationVersionControlService>();
        allChangesIncluded = true;
        IVssRequestContext requestContext1 = requestContext;
        string name = versionSpec.Name;
        string owner = versionSpec.Owner;
        ItemSpec[] itemSpecs = new ItemSpec[1]{ itemSpec };
        string[] itemPropertyFilters = propertyFilters;
        using (TeamFoundationDataReader foundationDataReader = service.QueryShelvedChanges(requestContext1, (string) null, (string) null, name, owner, itemSpecs, false, itemPropertyFilters))
        {
          List<TfsChange> shelvedChanges = new List<TfsChange>();
          foreach (PendingSet current in foundationDataReader.CurrentEnumerable<PendingSet>())
          {
            foreach (PendingChange pendingChange in current.PendingChanges)
            {
              if (num < skipCount)
              {
                ++num;
              }
              else
              {
                if (shelvedChanges.Count == maxCount)
                {
                  allChangesIncluded = false;
                  break;
                }
                shelvedChanges.Add(LegacyTfsModelExtensions.CreateWebApiChangeModel(pendingChange, versionSpec));
              }
            }
            if (!allChangesIncluded)
              break;
          }
          return (IEnumerable<TfsChange>) shelvedChanges;
        }
      }
      finally
      {
        requestContext.TraceLeave(513120, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetShelvedChanges));
      }
    }

    public static string TryParseShelvesetId(
      IVssRequestContext context,
      NameValueCollection requestParams)
    {
      string shelvesetName;
      string shelvesetOwner;
      return TfsVersionControlProvider.TryParseShelvesetName(context, requestParams, string.Empty, true, out shelvesetName, out shelvesetOwner) ? shelvesetName + ";" + shelvesetOwner : (string) null;
    }

    private static bool TryParseShelvesetName(
      IVssRequestContext context,
      NameValueCollection requestParams,
      string prefix,
      bool specificShelveset,
      out string shelvesetName,
      out string shelvesetOwner)
    {
      string requestParam = requestParams[prefix + "ss"];
      if (string.IsNullOrEmpty(requestParam))
        requestParam = requestParams[prefix + "shelveset"];
      shelvesetName = requestParams[prefix + "ssname"];
      shelvesetOwner = requestParams[prefix + "ssowner"];
      if (!string.IsNullOrEmpty(requestParam))
      {
        string[] strArray = requestParam.Split(new char[1]
        {
          ';'
        }, StringSplitOptions.None);
        if (strArray.Length != 0)
          shelvesetName = strArray[0];
        if (specificShelveset && string.IsNullOrEmpty(shelvesetName))
          return false;
        shelvesetOwner = strArray.Length <= 1 ? context.AuthenticatedUserName : strArray[1];
        if (string.IsNullOrEmpty(shelvesetOwner))
          return false;
      }
      else if (string.IsNullOrEmpty(shelvesetName) || string.IsNullOrEmpty(shelvesetOwner))
        return false;
      return true;
    }

    public virtual TfsChangeList GetShelveset(string shelvesetId, int maxChanges)
    {
      string shelvesetName = (string) null;
      string shelvesetOwner = (string) null;
      if (!string.IsNullOrEmpty(shelvesetId))
      {
        string[] strArray = shelvesetId.Split(new char[1]
        {
          ';'
        }, 2, StringSplitOptions.None);
        shelvesetName = strArray[0];
        if (strArray.Length > 1)
          shelvesetOwner = strArray[1];
        if (string.IsNullOrEmpty(shelvesetOwner))
          shelvesetOwner = this.RequestContext.AuthenticatedUserName;
      }
      return this.GetShelveset(shelvesetName, shelvesetOwner, maxChanges);
    }

    public TfsChangeList GetShelveset(string shelvesetName, string shelvesetOwner, int maxChanges) => this.GetShelvesets(shelvesetName, shelvesetOwner, maxChanges).FirstOrDefault<TfsChangeList>() ?? throw new ShelvesetNotFoundException(shelvesetName, shelvesetOwner);

    public IEnumerable<TfsChangeList> GetShelvesets(
      string shelvesetName,
      string shelvesetOwner,
      int maxChanges)
    {
      List<TfsChangeList> shelvesets = new List<TfsChangeList>();
      foreach (Shelveset shelveset in this.GetShelvesets(shelvesetName, shelvesetOwner))
      {
        bool allChangesIncluded = false;
        IEnumerable<TfsChange> changes = (IEnumerable<TfsChange>) null;
        if (maxChanges > 0)
          changes = this.GetShelvedChanges("$/", RecursionType.Full, 2, new ShelvesetVersionSpec(shelveset.Name, shelveset.Owner), maxChanges, 0, TfsVersionControlProvider.s_defaultPropertyFilters, out allChangesIncluded);
        TfsChangeList webApiChangeList = shelveset.ToWebApiChangeList(changes, allChangesIncluded);
        webApiChangeList.OwnerRef = this.GetOwnerRef(webApiChangeList.OwnerId);
        shelvesets.Add(webApiChangeList);
      }
      return (IEnumerable<TfsChangeList>) shelvesets;
    }

    private IEnumerable<Shelveset> GetShelvesets(string shelvesetName, string shelvesetOwner)
    {
      try
      {
        this.RequestContext.TraceEnter(513135, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetShelvesets));
        return (IEnumerable<Shelveset>) this.RequestContext.GetService<TeamFoundationVersionControlService>().QueryShelvesets(this.RequestContext, shelvesetName, shelvesetOwner);
      }
      finally
      {
        this.RequestContext.TraceLeave(513140, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetShelvesets));
      }
    }

    public static int GetChangesetId(
      IVssRequestContext requestContext,
      NameValueCollection requestParams,
      bool required)
    {
      string requestParam = requestParams["id"];
      if (string.IsNullOrEmpty(requestParam))
      {
        requestParam = requestParams["cs"];
        if (string.IsNullOrEmpty(requestParam))
          requestParam = requestParams["changeset"];
      }
      if (required)
        return VersionSpecCommon.ParseChangesetNumber((VersionSpecFactory) VersionSpec.ServerVersionSpecFactory, requestParam);
      int result;
      return !string.IsNullOrEmpty(requestParam) && int.TryParse(requestParam, out result) ? result : 0;
    }

    public IEnumerable<Microsoft.TeamFoundation.VersionControl.Server.Change> GetChangesetChanges(
      int changesetId,
      int maxCount,
      out bool allChangesIncluded)
    {
      return TfsVersionControlProvider.GetChangesetChanges(this.RequestContext, changesetId, maxCount, out allChangesIncluded);
    }

    public static IEnumerable<Microsoft.TeamFoundation.VersionControl.Server.Change> GetChangesetChanges(
      IVssRequestContext requestContext,
      int changesetId,
      int maxCount,
      out bool allChangesIncluded)
    {
      requestContext.TraceEnter(513160, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetChangesetChanges));
      try
      {
        allChangesIncluded = true;
        using (TeamFoundationDataReader foundationDataReader = requestContext.GetService<TeamFoundationVersionControlService>().QueryChangesForChangeset(requestContext, changesetId, false, Math.Min(maxCount + 1, int.MaxValue), (ItemSpec) null, (string[]) null, true))
        {
          List<Microsoft.TeamFoundation.VersionControl.Server.Change> changesetChanges = new List<Microsoft.TeamFoundation.VersionControl.Server.Change>();
          foreach (Microsoft.TeamFoundation.VersionControl.Server.Change current in foundationDataReader.CurrentEnumerable<Microsoft.TeamFoundation.VersionControl.Server.Change>())
          {
            if (changesetChanges.Count == maxCount)
            {
              allChangesIncluded = false;
              break;
            }
            changesetChanges.Add(current);
          }
          return (IEnumerable<Microsoft.TeamFoundation.VersionControl.Server.Change>) changesetChanges;
        }
      }
      finally
      {
        requestContext.TraceLeave(513165, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetChangesetChanges));
      }
    }

    public virtual TfsChangeList GetChangeset(int changesetId, int maxChanges, bool includeDetails)
    {
      Changeset changeset;
      using (TeamFoundationDataReader foundationDataReader = this.RequestContext.GetService<TeamFoundationVersionControlService>().QueryChangeset(this.RequestContext, changesetId, false, false, true))
        changeset = foundationDataReader.Current<Changeset>();
      return this.GetChangesetModel(changeset, maxChanges, includeDetails);
    }

    private TfsChangeList GetChangesetModel(
      Changeset changeset,
      int maxChanges,
      bool includeDetails)
    {
      bool allChangesIncluded = false;
      IEnumerable<TfsChange> changes = (IEnumerable<TfsChange>) null;
      if (maxChanges > 0)
        changes = this.GetChangesetChanges(changeset.ChangesetId, maxChanges, out allChangesIncluded).Select<Microsoft.TeamFoundation.VersionControl.Server.Change, TfsChange>((Func<Microsoft.TeamFoundation.VersionControl.Server.Change, TfsChange>) (change => LegacyTfsModelExtensions.ToWebApiChangeModel(change)));
      TfsChangeList webApiChangeList = changeset.ToWebApiChangeList(changes, allChangesIncluded, includeDetails);
      webApiChangeList.OwnerRef = this.GetOwnerRef(changeset.OwnerId);
      return webApiChangeList;
    }

    private TfsChangeList GetChangesetModel(ChangesetSummary changesetSummary)
    {
      TfsChangeList changesetModel = new TfsChangeList();
      changesetModel.ChangesetId = changesetSummary.ChangesetId;
      changesetModel.Comment = changesetSummary.Comment;
      changesetModel.CreationDate = changesetSummary.CreationDate;
      changesetModel.Owner = changesetSummary.Owner;
      changesetModel.OwnerDisplayName = changesetSummary.OwnerDisplayName;
      changesetModel.OwnerId = changesetSummary.ownerId;
      changesetModel.OwnerRef = this.GetOwnerRef(changesetSummary.ownerId);
      changesetModel.SortDate = changesetSummary.CreationDate;
      changesetModel.Version = new ChangesetVersionSpec(changesetSummary.ChangesetId).ToString();
      return changesetModel;
    }

    private IdentityRef GetOwnerRef(Guid ownerId)
    {
      try
      {
        return TfvcIdentityHelper.FindIdentity(this.RequestContext, ownerId).ToIdentityRef(this.RequestContext);
      }
      catch
      {
        return (IdentityRef) null;
      }
    }

    public IEnumerable<TfsChangeList> GetChangesetsByIds(int[] changesetIds)
    {
      IEnumerable<Changeset> source = this.RequestContext.GetService<TeamFoundationVersionControlService>().QueryChangesets(this.RequestContext, changesetIds);
      VersionControlRequestContext versionControlRequestContext = new VersionControlRequestContext(this.RequestContext);
      foreach (Changeset changeset in source)
        changeset.LookupDisplayNames(versionControlRequestContext);
      return source.Select<Changeset, TfsChangeList>((Func<Changeset, TfsChangeList>) (cs => cs.ToTfsChangeList(false)));
    }

    public TeamFoundationDataReader GetChangesets(
      string serverItem,
      VersionSpec version,
      bool includeFiles)
    {
      try
      {
        this.RequestContext.TraceEnter(513170, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetChangesets));
        TeamFoundationVersionControlService service = this.RequestContext.GetService<TeamFoundationVersionControlService>();
        ItemSpec itemSpec1 = new ItemSpec(serverItem, RecursionType.None, 0);
        IVssRequestContext requestContext = this.RequestContext;
        ItemSpec itemSpec2 = itemSpec1;
        VersionSpec versionItem = version;
        string empty = string.Empty;
        int num = includeFiles ? 1 : 0;
        return service.QueryHistory(requestContext, (string) null, (string) null, itemSpec2, versionItem, empty, (VersionSpec) null, (VersionSpec) null, int.MaxValue, num != 0, false, true, false);
      }
      finally
      {
        this.RequestContext.TraceLeave(513175, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetChangesets));
      }
    }

    public void UpdateChangeset(int changesetId, string comment, IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.CheckinNote> notes)
    {
      Microsoft.TeamFoundation.VersionControl.Server.CheckinNote checkinNote = (Microsoft.TeamFoundation.VersionControl.Server.CheckinNote) null;
      if (notes != null)
        checkinNote = new Microsoft.TeamFoundation.VersionControl.Server.CheckinNote()
        {
          Values = notes.Select<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.CheckinNote, CheckinNoteFieldValue>((Func<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.CheckinNote, CheckinNoteFieldValue>) (note => note.ToWebApiCheckinNoteFieldValue())).ToArray<CheckinNoteFieldValue>()
        };
      Changeset.UpdateChangeset(new VersionControlRequestContext(this.RequestContext), changesetId, comment, checkinNote);
    }

    public override IEnumerable<TeamIdentityReference> GetAuthors()
    {
      using (TeamFoundationDataReader foundationDataReader = this.RequestContext.GetService<TeamFoundationVersionControlService>().QueryChangeSetOwners(this.RequestContext, false))
        return (IEnumerable<TeamIdentityReference>) foundationDataReader.CurrentEnumerable<ChangeSetOwner>().Where<ChangeSetOwner>((Func<ChangeSetOwner, bool>) (owner => owner.Identity != null && !owner.Identity.IsContainer)).Select<ChangeSetOwner, TfsIdentityReference>((Func<ChangeSetOwner, TfsIdentityReference>) (owner => owner.Identity.ToWebApiIdentityReference())).OrderBy<TfsIdentityReference, string>((Func<TfsIdentityReference, string>) (owner => owner.DisplayName), (IComparer<string>) StringComparer.CurrentCultureIgnoreCase).ToArray<TfsIdentityReference>();
    }

    public override IEnumerable<int> GetLinkedWorkItemIds(string[] versions)
    {
      IEnumerable<int> ints = (IEnumerable<int>) new List<int>();
      foreach (string version in versions)
      {
        if (!string.IsNullOrEmpty(version))
        {
          IEnumerable<int> linkedWorkItemIds = this.GetLinkedWorkItemIds(version);
          if (linkedWorkItemIds.Any<int>())
            ints = ints.Concat<int>(linkedWorkItemIds);
        }
      }
      return ints.Distinct<int>();
    }

    private IEnumerable<int> GetLinkedWorkItemIds(string version)
    {
      VersionSpec versionSpecString;
      try
      {
        versionSpecString = LegacyTfsModelExtensions.ParseVersionSpecString(version, (VersionSpec) null);
      }
      catch (Exception ex)
      {
        throw new InvalidArgumentValueException("versionSpec", ex);
      }
      switch (versionSpecString)
      {
        case ShelvesetVersionSpec _:
          ShelvesetVersionSpec shelvesetVersionSpec = versionSpecString as ShelvesetVersionSpec;
          return this.GetLinkedWorkItemIds(shelvesetVersionSpec.Name, shelvesetVersionSpec.Owner);
        case ChangesetVersionSpec _:
          return this.GetLinkedWorkItemIds(((ChangesetVersionSpec) versionSpecString).ChangesetId);
        case ChangeVersionSpec _:
          return this.GetLinkedWorkItemIds(((ChangeVersionSpec) versionSpecString).ChangesetId);
        default:
          throw new NotSupportedException();
      }
    }

    private IEnumerable<int> GetLinkedWorkItemIds(int changesetId)
    {
      this.RequestContext.GetService<TeamFoundationLinkingService>();
      return this.GetLinkedWorkItemIdsFromArtifactUri(LinkingUtilities.EncodeUri(new ArtifactId()
      {
        Tool = "VersionControl",
        ArtifactType = "Changeset",
        ToolSpecificId = changesetId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
      }));
    }

    private IEnumerable<int> GetLinkedWorkItemIds(string shelvesetName, string shelvesetOwner)
    {
      Shelveset shelveset = this.GetShelvesets(shelvesetName, shelvesetOwner).FirstOrDefault<Shelveset>();
      if (shelveset == null)
        throw new ShelvesetNotFoundException(shelvesetName, shelvesetOwner);
      try
      {
        this.RequestContext.TraceEnter(513190, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetLinkedWorkItemIds));
        return shelveset.Links == null ? Enumerable.Empty<int>() : ((IEnumerable<VersionControlLink>) shelveset.Links).Where<VersionControlLink>((Func<VersionControlLink, bool>) (link => (link.LinkType & 1024) == 1024)).Select<VersionControlLink, int>((Func<VersionControlLink, int>) (link => int.Parse(LinkingUtilities.DecodeUri(link.Url).ToolSpecificId, (IFormatProvider) CultureInfo.InvariantCulture)));
      }
      finally
      {
        this.RequestContext.TraceLeave(513195, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetLinkedWorkItemIds));
      }
    }

    public static RecursionType ConvertVersionControlRecursionType(
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlRecursionType vcRecursionType)
    {
      switch (vcRecursionType)
      {
        case Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlRecursionType.OneLevel:
          return RecursionType.OneLevel;
        case Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlRecursionType.Full:
          return RecursionType.Full;
        default:
          return RecursionType.None;
      }
    }
  }
}
