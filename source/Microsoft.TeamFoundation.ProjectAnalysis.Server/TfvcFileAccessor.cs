// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.TfvcFileAccessor
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  internal sealed class TfvcFileAccessor : 
    TfsFileAccessorBase<TfvcRepositoryDescriptor>,
    IFileAccessor
  {
    private static readonly int s_queryItemsOptions = 14;
    private static readonly string[] s_symbolicLinkPropertyFilter = new string[1]
    {
      "Microsoft.TeamFoundation.VersionControl.SymbolicLink"
    };
    private TeamFoundationVersionControlService m_versionControl;
    private IVssRegistryService m_registryService;
    private const string c_layer = "TfvcFileAccessor";
    private const string c_tpAttributesFile = ".tpattributes";

    public TfvcFileAccessor(
      IVssRequestContext requestContext,
      TfvcRepositoryDescriptor repositoryDescriptor)
      : base(requestContext, repositoryDescriptor)
    {
    }

    private TeamFoundationVersionControlService VersionControl => this.m_versionControl ?? (this.m_versionControl = this.TfsRequestContext.GetService<TeamFoundationVersionControlService>());

    private IVssRegistryService RegistryService => this.m_registryService ?? (this.m_registryService = this.TfsRequestContext.GetService<IVssRegistryService>());

    public Stream GetContent(string filePath)
    {
      ArgumentUtility.CheckForNull<string>(filePath, nameof (filePath));
      Item obj = this.GetFileItems(new ItemSpec(VersionControlPath.GetFullPath(filePath), RecursionType.None, 0)).FirstOrDefault<Item>();
      if (obj != null && obj.ItemType == ItemType.File && obj.GetFileId(0) > 0)
      {
        if (!obj.IsContentDestroyed)
        {
          try
          {
            return this.TfsRequestContext.GetService<TeamFoundationFileService>().RetrieveFile(this.TfsRequestContext, (long) obj.GetFileId(0), false, out byte[] _, out long _, out CompressionType _);
          }
          catch (Exception ex)
          {
            this.TfsRequestContext.Trace(15281504, TraceLevel.Verbose, "ProjectAnalysisService", nameof (TfvcFileAccessor), ex.Message);
          }
        }
      }
      return (Stream) null;
    }

    protected override void InitializeDescriptor()
    {
      if (this.m_repositoryDescriptor.ChangesetId >= 0)
        return;
      this.m_repositoryDescriptor.ChangesetId = this.VersionControl.GetLatestChangeset(this.TfsRequestContext);
    }

    public IEnumerable<string> GetFilePaths() => this.GetFolderSubitems(this.m_repositoryDescriptor.TfvcRootFolder).Select<Item, string>((Func<Item, string>) (x => x.ServerItem));

    public ILanguageConfigurator GetConfigurator()
    {
      if (this.RepositoryDescriptor is TfvcRepositoryDescriptor repositoryDescriptor)
      {
        try
        {
          using (Stream content = this.GetContent(Path.Combine(repositoryDescriptor.TfvcRootFolder, ".tpattributes")))
          {
            if (content != null)
              return (ILanguageConfigurator) new TfvcAttributesLanguageConfigurator(this.TfsRequestContext, content);
          }
        }
        catch (Exception ex)
        {
          this.TfsRequestContext.TraceException(15281509, "ProjectAnalysisService", nameof (TfvcFileAccessor), ex);
        }
      }
      return (ILanguageConfigurator) new DefaultLanguageConfigurator(this.TfsRequestContext);
    }

    public List<Item> GetFolderSubitems(string parentFolder) => this.GetFileItems(new ItemSpec(parentFolder, RecursionType.Full)).ToList<Item>();

    public List<Item> GetFileItems(ItemSpec itemSpec)
    {
      string startFrom = (string) null;
      bool flag = true;
      int pageSize = this.GetPageSize();
      int sleepTime = this.GetSleepTime();
      List<Item> fileItems = new List<Item>();
      do
      {
        try
        {
          List<Item> fileItemsBatch = this.GetFileItemsBatch(itemSpec, pageSize, ref startFrom);
          List<Item> list = fileItemsBatch.Where<Item>((Func<Item, bool>) (item => !IsSymLink(item))).ToList<Item>();
          fileItems.AddRange((IEnumerable<Item>) list);
          if (fileItemsBatch.Count < pageSize)
            flag = false;
          else
            Thread.Sleep(sleepTime);
        }
        catch (Exception ex)
        {
          this.TfsRequestContext.TraceException(15281511, TraceLevel.Info, "ProjectAnalysisService", nameof (TfvcFileAccessor), ex);
          break;
        }
      }
      while (flag);
      return fileItems;

      static bool IsSymLink(Item item)
      {
        StreamingCollection<PropertyValue> attributes = item.Attributes;
        return string.Equals((attributes != null ? attributes.FirstOrDefault<PropertyValue>((Func<PropertyValue, bool>) (p => string.Equals(TfvcFileAccessor.s_symbolicLinkPropertyFilter[0], p.PropertyName, StringComparison.Ordinal))) : (PropertyValue) null)?.Value?.ToString(), "true", StringComparison.OrdinalIgnoreCase);
      }
    }

    public IEnumerable<List<Item>> GetFolderSubitemsIterator(string parentFolder)
    {
      ItemSpec itemSpec = new ItemSpec(parentFolder, RecursionType.Full);
      string lastItem = (string) null;
      bool haveMoreItems = true;
      int batchSize = this.GetPageSize();
      int sleepTime = this.GetSleepTime();
      do
      {
        yield return this.GetFileItemsBatch(itemSpec, batchSize, ref lastItem).Where<Item>((Func<Item, bool>) (item => !IsSymLink(item))).ToList<Item>();
        if (lastItem == null)
          haveMoreItems = false;
        else
          Thread.Sleep(sleepTime);
      }
      while (haveMoreItems);

      static bool IsSymLink(Item item)
      {
        StreamingCollection<PropertyValue> attributes = item.Attributes;
        return string.Equals((attributes != null ? attributes.FirstOrDefault<PropertyValue>((Func<PropertyValue, bool>) (p => string.Equals(TfvcFileAccessor.s_symbolicLinkPropertyFilter[0], p.PropertyName, StringComparison.Ordinal))) : (PropertyValue) null)?.Value?.ToString(), "true", StringComparison.OrdinalIgnoreCase);
      }
    }

    private List<Item> GetFileItemsBatch(ItemSpec itemSpec, int batchSize, ref string startFrom)
    {
      List<Item> objList = new List<Item>();
      ItemSpec continuationItem = string.IsNullOrEmpty(startFrom) ? (ItemSpec) null : new ItemSpec(startFrom, RecursionType.Full);
      using (TeamFoundationDataReader foundationDataReader = this.VersionControl.QueryItemsPaged(this.TfsRequestContext, itemSpec, this.VersionControl.GetLatestChangeset(this.TfsRequestContext), batchSize, continuationItem, TfvcFileAccessor.s_queryItemsOptions, out startFrom))
        return foundationDataReader.CurrentEnumerable<Item>().ToList<Item>();
    }

    public IDictionary<int, long> GetFileSizes(List<Item> items, string projectRootFolder)
    {
      // ISSUE: unable to decompile the method.
    }

    private void TraceTfvcDatabaseIssue(
      List<Item> items,
      string projectRootFolder,
      List<Guid> dataspaceIds,
      string title)
    {
      string str = title + string.Format("\r\n  project root: {0}, items.Count {1}", (object) projectRootFolder, (object) items.Count) + string.Format("\r\n  dataspaceIds.Count: {0}", (object) dataspaceIds.Count);
      List<IGrouping<Guid, Item>> list = items.GroupBy<Item, Guid>((Func<Item, Guid>) (i => i.ItemDataspaceId)).ToList<IGrouping<Guid, Item>>();
      list.Sort((Comparison<IGrouping<Guid, Item>>) ((g, h) => Math.Sign(h.Count<Item>() - g.Count<Item>())));
      string message = str + string.Format("\r\n  dataspaceIds: {0}", (object) string.Join(", ", list.Take<IGrouping<Guid, Item>>(10).Select<IGrouping<Guid, Item>, string>((Func<IGrouping<Guid, Item>, string>) (g => g.Key.ToString()))));
      foreach (IGrouping<Guid, Item> source in list.Take<IGrouping<Guid, Item>>(10))
      {
        message += string.Format("\r\n  {0}:", (object) source.Key);
        foreach (Item obj in source.Take<Item>(10))
        {
          message += string.Format("\r\n    {0}", (object) obj.ItemPathPair.ProjectNamePath);
          message += string.Format("\r\n      {0}", (object) obj.ItemPathPair.ProjectGuidPath);
          message += string.Format("\r\n        deletionId: {0}, isFolder: {1}, isBranch: {2}, isContentDestroyed: {3}", (object) obj.DeletionId, (object) obj.IsFolder, (object) obj.IsBranch, (object) obj.IsContentDestroyed);
        }
      }
      this.TfsRequestContext.Trace(15280027, TraceLevel.Info, "ProjectAnalysisService", nameof (TfvcFileAccessor), message);
    }

    private int GetPageSize() => Math.Max(1, Math.Min(this.RegistryService.GetValue<int>(this.TfsRequestContext, (RegistryQuery) ProjectAnalysisConstants.Settings.TfvcPageSizeKey, true, 10000), 10000));

    private int GetSleepTime() => Math.Max(1, Math.Min(this.RegistryService.GetValue<int>(this.TfsRequestContext, (RegistryQuery) ProjectAnalysisConstants.Settings.TfvcSleepTimeKey, true, 1000), 10000));
  }
}
