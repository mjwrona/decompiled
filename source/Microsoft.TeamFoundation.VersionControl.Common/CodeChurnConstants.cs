// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.CodeChurnConstants
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  public static class CodeChurnConstants
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly Guid CodeChurnJobGuid = new Guid("3B74723B-5E93-4bc2-BD52-E39734F0E677");
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const int DefaultBatchSize = 10000;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const long DefaultMaxFileSizeToDiff = 2097152;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const int DefaultRetryCount = 3;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const int DefaultRetryInterval = 3600;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly string CodeChurnRegistryPath = "/Configuration/VersionControl/CodeChurn";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly string BatchSizeRegistryKey = CodeChurnConstants.CodeChurnRegistryPath + "/BatchSize";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly string MaxFileSizeToChurn = CodeChurnConstants.CodeChurnRegistryPath + "/MaxFileSizeToChurn";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly string MaxFileSizeToDiff = CodeChurnConstants.CodeChurnRegistryPath + "/MaxFileSizeToDiff";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly string ItemRetryCount = CodeChurnConstants.CodeChurnRegistryPath + "/ItemRetryCount";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly string ItemRetryInterval = CodeChurnConstants.CodeChurnRegistryPath + "/ItemRetryInterval";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly string NextChangeSetToProcessRegistryKey = CodeChurnConstants.CodeChurnRegistryPath + "/NextChangeSetToProcess";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly string LastItemProcessedRegistryKey = CodeChurnConstants.CodeChurnRegistryPath + "/LastItemProcessed";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly string LastItemRetryTime = CodeChurnConstants.CodeChurnRegistryPath + "/LastItemRetryTime";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly string InUpgrade = CodeChurnConstants.CodeChurnRegistryPath + "/InUpgrade";
    public static readonly string DetailsPropertyName = "Microsoft.TeamFoundation.VersionControl.CodeChurn.Details";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly string RetryCountPropertyName = "Microsoft.TeamFoundation.VersionControl.CodeChurn.RetryCount";
  }
}
