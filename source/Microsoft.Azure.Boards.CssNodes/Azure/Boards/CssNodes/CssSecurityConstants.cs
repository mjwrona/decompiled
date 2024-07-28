// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.CssNodes.CssSecurityConstants
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using System;

namespace Microsoft.Azure.Boards.CssNodes
{
  [Flags]
  public enum CssSecurityConstants
  {
    None = 0,
    GENERIC_READ = 1,
    GENERIC_WRITE = 2,
    CREATE_CHILDREN = 4,
    DELETE = 8,
    WORK_ITEM_READ = 16, // 0x00000010
    WORK_ITEM_WRITE = 32, // 0x00000020
    MANAGE_TEST_PLANS = 64, // 0x00000040
    MANAGE_TEST_SUITES = 128, // 0x00000080
    ViewEmailAddress = 256, // 0x00000100
    WORK_ITEM_SAVE_COMMENT = 512, // 0x00000200
  }
}
