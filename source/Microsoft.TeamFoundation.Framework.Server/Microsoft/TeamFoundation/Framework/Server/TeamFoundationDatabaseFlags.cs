// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationDatabaseFlags
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Flags]
  public enum TeamFoundationDatabaseFlags
  {
    None = 0,
    CleanupCandidate = 1,
    [Obsolete("This flag has been deprecated. Do not reuse the field 0x2 in future.", true)] AutoCreated = 2,
    RlsEnabled = 4,
    DataspaceRlsEnabled = 8,
    IntentReadOnlyOnSqlAzureDatabase = 16, // 0x00000010
    PrecreationInProgress = 32, // 0x00000020
  }
}
