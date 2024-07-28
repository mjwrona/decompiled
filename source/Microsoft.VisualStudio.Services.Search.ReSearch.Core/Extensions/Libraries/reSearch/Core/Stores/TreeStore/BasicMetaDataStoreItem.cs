// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore.BasicMetaDataStoreItem
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint;
using Microsoft.VisualStudio.Services.Search.Common.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore
{
  public class BasicMetaDataStoreItem : IMetaDataStoreItem
  {
    public const long UnknownSize = -1;

    public Hash ContentId { get; private set; }

    public string Path { get; private set; }

    public string FileName { get; private set; }

    public string Extension { get; private set; }

    public long Size { get; set; }

    public MetaDataStoreUpdateType UpdateType { get; set; }

    public string DocumentId { get; set; }

    public List<BranchInfo> BranchesInfo { get; private set; }

    public long? FilePathId { get; set; }

    public string ContentKey { get; set; }

    public BasicMetaDataStoreItem()
    {
    }

    public BasicMetaDataStoreItem(Hash contentId, string path)
      : this(contentId, path, MetaDataStoreUpdateType.None, -1L, string.Empty)
    {
    }

    public BasicMetaDataStoreItem(Hash contentId, string path, MetaDataStoreUpdateType updateType)
      : this(contentId, path, updateType, -1L, string.Empty)
    {
    }

    public BasicMetaDataStoreItem(
      Hash contentId,
      string path,
      MetaDataStoreUpdateType updateType,
      string documentId)
      : this(contentId, path, updateType, -1L, documentId)
    {
    }

    public BasicMetaDataStoreItem(
      Hash contentId,
      string path,
      string documentId,
      string contentKey,
      MetaDataStoreUpdateType updateType)
      : this(contentId, path, documentId, contentKey, updateType, -1L)
    {
    }

    public BasicMetaDataStoreItem(Hash contentId, string path, long size)
      : this(contentId, path, MetaDataStoreUpdateType.None, size, string.Empty)
    {
    }

    public BasicMetaDataStoreItem(
      Hash contentId,
      string path,
      MetaDataStoreUpdateType updateType,
      long size)
    {
      this.Initialize(contentId, path, updateType, size, string.Empty, new long?(), new List<BranchInfo>());
    }

    public BasicMetaDataStoreItem(
      Hash contentId,
      string path,
      MetaDataStoreUpdateType updateType,
      long size,
      string documentId)
    {
      this.Initialize(contentId, path, updateType, size, documentId, new long?(), new List<BranchInfo>());
    }

    public BasicMetaDataStoreItem(
      Hash contentId,
      string path,
      MetaDataStoreUpdateType updateType,
      string documentId,
      List<BranchInfo> branches,
      long? filePathId)
    {
      this.Initialize(contentId, path, updateType, -1L, documentId, filePathId, branches);
    }

    public BasicMetaDataStoreItem(
      Hash contentId,
      string path,
      string documentId,
      string contentKey,
      MetaDataStoreUpdateType updateType,
      long size)
    {
      this.Initialize(contentId, path, updateType, size, documentId, contentKey);
    }

    public override bool Equals(object obj) => obj is BasicMetaDataStoreItem metaDataStoreItem && !(this.ContentId == (Hash) null) && this.Path != null && this.ContentId.Equals((object) metaDataStoreItem.ContentId) && this.Path.Equals(metaDataStoreItem.Path);

    public override int GetHashCode() => this.Path.GetHashCode();

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ContentHash: {0} ", (object) this.ContentId.HexHash));
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "FilePath: {0} ", (object) this.Path));
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "UpdateType: {0} ", (object) this.UpdateType));
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Branches: "));
      foreach (BranchInfo branchInfo in this.BranchesInfo)
        stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) Serializers.ToXmlString((object) branchInfo, typeof (BranchInfo))));
      return stringBuilder.ToString();
    }

    public void Initialize(
      Hash contentId,
      string path,
      MetaDataStoreUpdateType updateType,
      long size,
      string docId,
      long? filePathId,
      List<BranchInfo> branchesInfo)
    {
      if (branchesInfo != null && branchesInfo.Count > 0 && contentId.HexHash.Length < 2)
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("If branchesInfo is populated, even ContentId should be populated. branchesInfoCount = {0}. contentId = {1}", (object) branchesInfo.Count, (object) contentId.HexHash)));
      if (branchesInfo == null)
        branchesInfo = new List<BranchInfo>();
      if (!filePathId.HasValue)
        filePathId = new long?(-1L);
      if (docId == null)
        docId = string.Empty;
      this.ContentId = contentId;
      this.Path = path;
      this.FileName = FilePathUtils.GetFileName(path);
      this.Extension = FilePathUtils.GetFileExtension(path);
      this.Size = size;
      this.UpdateType = updateType;
      this.DocumentId = docId;
      this.FilePathId = filePathId;
      this.BranchesInfo = branchesInfo;
    }

    public void Initialize(
      Hash contentId,
      string path,
      MetaDataStoreUpdateType updateType,
      long size,
      string documentId,
      string contentKey)
    {
      this.ContentId = contentId;
      this.Path = path;
      this.FileName = FilePathUtils.GetFileName(path);
      this.Extension = FilePathUtils.GetFileExtension(path);
      this.Size = size;
      this.UpdateType = updateType;
      if (documentId == null)
        documentId = string.Empty;
      this.DocumentId = documentId;
      this.ContentKey = contentKey;
      this.FilePathId = new long?(-1L);
      this.BranchesInfo = new List<BranchInfo>();
    }

    public virtual HashSet<string> GetBranches(IMetaDataStore metadataStore)
    {
      HashSet<string> branches = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (this.BranchesInfo != null && this.BranchesInfo.Count > 0)
        branches.UnionWith(this.BranchesInfo.Select<BranchInfo, string>((Func<BranchInfo, string>) (x => x.BranchName)));
      else
        branches.Add(metadataStore["BranchName"] ?? throw new SearchServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Found metadatastore item for failed item DocId: {0} UpdateType: {1} but branch name is not present in the property bag.", (object) this.DocumentId, (object) this.UpdateType)));
      return branches;
    }
  }
}
