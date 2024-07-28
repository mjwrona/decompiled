// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.IdentityDirectoryEntityResult`1
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Identity;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Directories
{
  [DataContract]
  public class IdentityDirectoryEntityResult<TIdentity> : DirectoryEntityResult where TIdentity : IVssIdentity
  {
    [DataMember]
    public TIdentity Identity { get; }

    public IdentityDirectoryEntityResult(DirectoryEntityResult result, TIdentity identity)
      : this(identity, result.Entity, result.EntityState, result.Status, result.Exception)
    {
    }

    [JsonConstructor]
    public IdentityDirectoryEntityResult(
      TIdentity identity,
      IDirectoryEntity entity,
      string entityState,
      string status,
      Exception exception = null)
      : base(entity, entityState, status, exception)
    {
      this.Identity = identity;
    }
  }
}
