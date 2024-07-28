// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryEntityResult
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Directories
{
  [DataContract]
  public class DirectoryEntityResult
  {
    [DataMember]
    [JsonConverter(typeof (DirectoryEntityJsonConverter))]
    public IDirectoryEntity Entity { get; }

    [DataMember]
    public string EntityState { get; }

    [DataMember]
    public string Status { get; }

    [DataMember(EmitDefaultValue = false)]
    public Exception Exception { get; }

    public DirectoryEntityResult(
      IDirectoryEntity entity,
      string entityState,
      string status,
      Exception exception = null)
    {
      this.Entity = entity;
      this.EntityState = entityState;
      this.Status = status;
      this.Exception = exception;
    }
  }
}
