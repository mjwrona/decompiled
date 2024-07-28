// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ProtocolBase
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using System;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public abstract class ProtocolBase : IProtocol
  {
    protected ProtocolBase(
      string correctlyCasedName,
      string commitLogItemType,
      IntegerRange tracePointRange,
      IOrgLevelPackagingSettingDefinition<bool> enabledSettingDefinition,
      string readOnlyFeatureFlagName,
      string disasterRecoveryChangeProcessingBypassFeatureFlagName,
      BookmarkTokenKey changeProcessingBookmarkTokenKey,
      BookmarkTokenKey? deleteProcessingBookmarkTokenKey,
      BookmarkTokenKey? contentVerificationBookmarkTokenKey,
      bool isMultiFile)
    {
      this.CorrectlyCasedName = correctlyCasedName ?? throw new ArgumentNullException(nameof (correctlyCasedName));
      this.CommitLogItemType = commitLogItemType ?? throw new ArgumentNullException(nameof (commitLogItemType));
      this.TracePointRange = tracePointRange ?? throw new ArgumentNullException(nameof (tracePointRange));
      this.LowercasedName = this.CorrectlyCasedName.ToLowerInvariant();
      this.EnabledSettingDefinition = enabledSettingDefinition ?? throw new ArgumentNullException(nameof (enabledSettingDefinition));
      this.ReadOnlyFeatureFlagName = readOnlyFeatureFlagName ?? throw new ArgumentNullException(nameof (readOnlyFeatureFlagName));
      this.DisasterRecoveryChangeProcessingBypassFeatureFlagName = disasterRecoveryChangeProcessingBypassFeatureFlagName;
      this.ChangeProcessingBookmarkTokenKey = changeProcessingBookmarkTokenKey ?? throw new ArgumentNullException(nameof (changeProcessingBookmarkTokenKey));
      this.DeleteProcessingBookmarkTokenKey = deleteProcessingBookmarkTokenKey;
      this.ContentVerificationBookmarkTokenKey = contentVerificationBookmarkTokenKey;
      this.IsMultiFile = isMultiFile;
    }

    public override string ToString() => this.CorrectlyCasedName;

    public bool IsMultiFile { get; }

    public string CorrectlyCasedName { get; }

    public string LowercasedName { get; }

    public string CommitLogItemType { get; }

    public IntegerRange TracePointRange { get; }

    public IOrgLevelPackagingSettingDefinition<bool> EnabledSettingDefinition { get; }

    public string ReadOnlyFeatureFlagName { get; }

    public string DisasterRecoveryChangeProcessingBypassFeatureFlagName { get; }

    public abstract IReadOnlyList<BlockedPackageIdentity> PermanentlyBlockedPackageIdentities { get; }

    public BookmarkTokenKey ChangeProcessingBookmarkTokenKey { get; }

    public BookmarkTokenKey? DeleteProcessingBookmarkTokenKey { get; }

    public BookmarkTokenKey? ContentVerificationBookmarkTokenKey { get; }
  }
}
