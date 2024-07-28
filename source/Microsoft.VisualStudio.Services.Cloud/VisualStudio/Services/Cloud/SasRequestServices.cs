// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SasRequestServices
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [DataContract]
  [Flags]
  public enum SasRequestServices
  {
    None = 0,
    Blob = 1,
    Queue = 2,
    File = 4,
    Table = 8,
    All = Table | File | Queue | Blob, // 0x0000000F
  }
}
