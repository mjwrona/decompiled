// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HostManagement.ServiceHostType
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.HostManagement
{
  [Flags]
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public enum ServiceHostType : byte
  {
    [EnumMember] Unknown = 0,
    [EnumMember] Deployment = 1,
    [EnumMember] Application = 2,
    [EnumMember] Collection = 4,
  }
}
