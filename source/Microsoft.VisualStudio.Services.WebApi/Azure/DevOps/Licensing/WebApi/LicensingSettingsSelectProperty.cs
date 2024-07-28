// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.Licensing.WebApi.LicensingSettingsSelectProperty
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.DevOps.Licensing.WebApi
{
  [DataContract]
  [Flags]
  public enum LicensingSettingsSelectProperty
  {
    DefaultAccessLevel = 1,
    AccessLevelOptions = 2,
    All = AccessLevelOptions | DefaultAccessLevel, // 0x00000003
  }
}
