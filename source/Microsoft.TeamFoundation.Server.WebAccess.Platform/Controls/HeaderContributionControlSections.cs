// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controls.HeaderContributionControlSections
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controls
{
  [Flags]
  public enum HeaderContributionControlSections
  {
    Left = 1,
    Center = 2,
    Right = 4,
    Justified = 8,
    L1Only = Right | Center | Left, // 0x00000007
    All = L1Only | Justified, // 0x0000000F
  }
}
