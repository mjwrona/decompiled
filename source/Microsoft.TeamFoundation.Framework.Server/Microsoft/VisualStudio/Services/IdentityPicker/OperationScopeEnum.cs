// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.OperationScopeEnum
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.IdentityPicker
{
  [Flags]
  public enum OperationScopeEnum
  {
    None = 0,
    [JsonProperty] Source = 1,
    [JsonProperty] IMS = 2,
    [JsonProperty] AAD = 4,
    [JsonProperty] AD = 8,
    [JsonProperty] WMD = 16, // 0x00000010
    [JsonProperty] GHB = 32, // 0x00000020
  }
}
