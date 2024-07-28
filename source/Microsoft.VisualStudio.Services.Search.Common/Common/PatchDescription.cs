// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.PatchDescription
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class PatchDescription
  {
    public string FilePath { get; private set; }

    public MetaDataStoreUpdateType PatchOperation { get; private set; }

    public string ContentHash { get; private set; }

    public string OldContentHash { get; private set; }

    public long? ContentSize { get; private set; }

    public string Branch { get; private set; }

    public bool IsContentAvailable { get; private set; }

    public byte[] Content { get; private set; }

    public VersionControlType VersionControlType { get; private set; }

    public string LastIndexedChangeId { get; private set; }

    public DateTime LastIndexedChangeUtcTime { get; private set; }

    public int AttemptCount { get; set; }

    internal PatchDescription(string filePath) => this.FilePath = filePath;

    internal PatchDescription(
      string filePath,
      MetaDataStoreUpdateType patchOperation,
      VersionControlType versionControlType,
      string lastIndexedChangeId,
      DateTime lastIndexedChangeUtcTime)
    {
      this.FilePath = filePath;
      this.PatchOperation = patchOperation;
      this.VersionControlType = versionControlType;
      this.IsContentAvailable = false;
      this.Content = (byte[]) null;
      this.LastIndexedChangeId = lastIndexedChangeId;
      this.LastIndexedChangeUtcTime = lastIndexedChangeUtcTime;
      this.ContentHash = (string) null;
      this.OldContentHash = (string) null;
      this.Branch = (string) null;
      this.ContentSize = new long?();
      this.AttemptCount = 0;
    }

    internal PatchDescription(
      string filePath,
      MetaDataStoreUpdateType patchOperation,
      VersionControlType versionControlType,
      string branchName,
      string contentHash,
      string oldContentHash,
      string lastIndexedChangeId,
      DateTime lastIndexedChangeUtcTime,
      long? contentSize = null)
      : this(filePath, patchOperation, versionControlType, lastIndexedChangeId, lastIndexedChangeUtcTime)
    {
      PatchDescription.ValidateParams(filePath, patchOperation, branchName, contentHash, oldContentHash, lastIndexedChangeId, lastIndexedChangeUtcTime, contentSize);
      this.Branch = branchName;
      this.ContentHash = contentHash;
      this.OldContentHash = oldContentHash;
      this.ContentSize = contentSize;
    }

    private static void ValidateParams(
      string filePath,
      MetaDataStoreUpdateType patchOperation,
      string branchName,
      string contentHash,
      string oldContentHash,
      string lastIndexedChangeId,
      DateTime lastIndexedChangeUtcTime,
      long? contentSize)
    {
      if (string.IsNullOrWhiteSpace(filePath))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0} cannot be null or whitespace.", (object) nameof (filePath))));
      if (branchName == null)
        throw new ArgumentNullException(nameof (branchName));
      if (patchOperation != MetaDataStoreUpdateType.Add && patchOperation != MetaDataStoreUpdateType.Edit && patchOperation != MetaDataStoreUpdateType.Delete)
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0} should be in Add/Edit/Delete.", (object) nameof (patchOperation))));
      if ((patchOperation == MetaDataStoreUpdateType.Add || patchOperation == MetaDataStoreUpdateType.Edit) && string.IsNullOrWhiteSpace(contentHash))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0} should not be null or whitespace for operation type = {1}.", (object) nameof (contentHash), (object) patchOperation)));
      if ((patchOperation == MetaDataStoreUpdateType.Delete || patchOperation == MetaDataStoreUpdateType.Edit) && string.IsNullOrWhiteSpace(oldContentHash))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0} should not be null or whitespace for operation type = {1}.", (object) nameof (oldContentHash), (object) patchOperation)));
      if (string.IsNullOrWhiteSpace(lastIndexedChangeId) || lastIndexedChangeId == RepositoryConstants.DefaultLastIndexCommitId || lastIndexedChangeId == "-1")
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0} should be valid.", (object) nameof (lastIndexedChangeId))));
      if (lastIndexedChangeUtcTime == RepositoryConstants.DefaultLastIndexChangeUtcTime || lastIndexedChangeUtcTime == DateTime.MinValue)
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0} should be valid.", (object) nameof (lastIndexedChangeUtcTime))));
      if (contentSize.HasValue && contentSize.Value < 0L)
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0} should not be negative.", (object) nameof (contentSize))));
    }

    public static PatchDescription CreatePathDescription(
      string filePath,
      MetaDataStoreUpdateType metaDataStoreUpdateType,
      VersionControlType vcType,
      string lastIndexedChangeId,
      DateTime lastIndexedChangeUtcTime)
    {
      return new PatchDescription(filePath, metaDataStoreUpdateType, vcType, lastIndexedChangeId, lastIndexedChangeUtcTime);
    }

    public static PatchDescription CreateEditPatchDescription(
      string filePath,
      string branchName,
      string contentHash,
      string oldContentHash,
      VersionControlType vcType,
      string lastIndexedChangeId,
      DateTime lastIndexedChangeUtcTime,
      long? contentSize = null)
    {
      return new PatchDescription(filePath, MetaDataStoreUpdateType.Edit, vcType, branchName, contentHash, oldContentHash, lastIndexedChangeId, lastIndexedChangeUtcTime, contentSize);
    }

    public static PatchDescription CreateAddPatchDescription(
      string filePath,
      string branchName,
      string contentHash,
      VersionControlType vcType,
      string lastIndexedChangeId,
      DateTime lastIndexedChangeUtcTime,
      long? contentSize = null)
    {
      return new PatchDescription(filePath, MetaDataStoreUpdateType.Add, vcType, branchName, contentHash, (string) null, lastIndexedChangeId, lastIndexedChangeUtcTime, contentSize);
    }

    public static PatchDescription CreateDeletePatchDescription(
      string filePath,
      string branchName,
      string oldContentHash,
      VersionControlType vcType,
      string lastIndexedChangeId,
      DateTime lastIndexedChangeUtcTime,
      long? contentSize = null)
    {
      return new PatchDescription(filePath, MetaDataStoreUpdateType.Delete, vcType, branchName, (string) null, oldContentHash, lastIndexedChangeId, lastIndexedChangeUtcTime, contentSize);
    }

    public override bool Equals(object obj)
    {
      if (obj == null || this.GetType() != obj.GetType())
        return false;
      PatchDescription patchDescription = (PatchDescription) obj;
      return string.Equals(this.FilePath, patchDescription.FilePath) && this.PatchOperation == patchDescription.PatchOperation && string.Equals(this.Branch, patchDescription.Branch) && string.Equals(this.OldContentHash, patchDescription.OldContentHash) && string.Equals(this.ContentHash, patchDescription.ContentHash);
    }

    public override int GetHashCode() => ((((17 * 23 + this.FilePath.GetHashCode()) * 23 + this.PatchOperation.GetHashCode()) * 23 + this.Branch.GetHashCode()) * 23 + this.OldContentHash.GetHashCode()) * 23 + this.ContentHash.GetHashCode();
  }
}
