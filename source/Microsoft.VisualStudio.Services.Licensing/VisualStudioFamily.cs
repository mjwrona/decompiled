// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.VisualStudioFamily
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

namespace Microsoft.VisualStudio.Services.Licensing
{
  public enum VisualStudioFamily
  {
    Invalid = -1, // 0xFFFFFFFF
    VisualStudio = 2,
    VisualStudioExpressWindows = 3,
    VisualStudioExpressWeb = 4,
    VisualStudioExpressPhone = 5,
    VisualStudioTestProfessional = 9,
    VisualStudioExpressDesktop = 11, // 0x0000000B
    VisualStudioTeamExplorer = 17, // 0x00000011
    TeamFoundationServer = 20, // 0x00000014
    TeamFoundationServerExpress = 21, // 0x00000015
    VisualStudioMac = 22, // 0x00000016
    VisualStudioCExpress = 110, // 0x0000006E
    VisualStudioCSharpExpress = 111, // 0x0000006F
    VisualBasicExpress = 112, // 0x00000070
    Update = 113, // 0x00000071
    TestManager = 200, // 0x000000C8
    VisualStudioEmulatorAndroid = 300, // 0x0000012C
  }
}
