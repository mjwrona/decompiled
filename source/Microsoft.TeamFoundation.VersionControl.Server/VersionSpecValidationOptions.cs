// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.VersionSpecValidationOptions
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Flags]
  internal enum VersionSpecValidationOptions
  {
    None = 0,
    Date = 1,
    Label = 2,
    Workspace = 4,
    Changeset = 8,
    Latest = 16, // 0x00000010
    All = Latest | Changeset | Workspace | Label | Date, // 0x0000001F
  }
}
