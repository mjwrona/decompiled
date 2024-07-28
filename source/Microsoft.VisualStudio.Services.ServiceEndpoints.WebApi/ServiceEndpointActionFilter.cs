// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointActionFilter
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [Flags]
  [DataContract]
  public enum ServiceEndpointActionFilter
  {
    [EnumMember] None = 0,
    [EnumMember] Manage = 2,
    [EnumMember] Use = 16, // 0x00000010
    [EnumMember] View = 32, // 0x00000020
  }
}
