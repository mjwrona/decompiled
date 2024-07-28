// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Authorization.WebApi.Permission
// Assembly: Microsoft.Azure.Pipelines.Authorization.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4807FD31-F2A4-4329-AA76-35B262BDA671
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Authorization.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Authorization.WebApi
{
  [DataContract]
  public class Permission
  {
    [DataMember(EmitDefaultValue = false)]
    public bool Authorized { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef AuthorizedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? AuthorizedOn { get; set; }
  }
}
