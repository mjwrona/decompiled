// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Common.Contracts.AuthorizationHeader
// Assembly: Microsoft.TeamFoundation.DistributedTask.Common.Contracts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F0CB8220-D93B-49F7-B603-A5F8DA1FAAC3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Common.Contracts.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Common.Contracts
{
  [DataContract]
  public class AuthorizationHeader : BaseSecuredObject
  {
    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Value { get; set; }
  }
}
