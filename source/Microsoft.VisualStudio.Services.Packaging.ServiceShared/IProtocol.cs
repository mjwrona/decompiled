// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.IProtocol
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using Newtonsoft.Json;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public interface IProtocol
  {
    string CorrectlyCasedName { get; }

    string LowercasedName { get; }

    string CommitLogItemType { get; }

    IntegerRange TracePointRange { get; }

    IOrgLevelPackagingSettingDefinition<bool> EnabledSettingDefinition { get; }

    string ReadOnlyFeatureFlagName { get; }

    string DisasterRecoveryChangeProcessingBypassFeatureFlagName { get; }

    [JsonIgnore]
    IReadOnlyList<BlockedPackageIdentity> PermanentlyBlockedPackageIdentities { get; }

    BookmarkTokenKey ChangeProcessingBookmarkTokenKey { get; }

    BookmarkTokenKey? DeleteProcessingBookmarkTokenKey { get; }

    BookmarkTokenKey? ContentVerificationBookmarkTokenKey { get; }

    string ToString();

    bool IsMultiFile { get; }
  }
}
