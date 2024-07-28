// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Session
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.Content.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts
{
  public class Session
  {
    public const int DefaultExpirationTimeInHour = 2;
    private readonly IClock Clock = UtcClock.Instance;

    [JsonConverter(typeof (StringEnumConverter))]
    public SessionState Status { get; set; }

    public DedupIdentifier ContentId { get; set; }

    public List<Part> Parts { get; set; }

    public bool ShouldSerializeContentId() => this.Status == SessionState.Completed;

    public bool ShouldSerializeParts() => this.Status == SessionState.Completed;

    [JsonIgnore]
    public Guid Id { get; }

    [JsonIgnore]
    public Guid Owner { get; }

    [JsonIgnore]
    public DateTime Expiration { get; set; }

    [JsonIgnore]
    public bool IsValid
    {
      get
      {
        if (this.Status == SessionState.Completed)
          return true;
        return this.Status == SessionState.Active && this.Clock.Now.UtcDateTime <= this.Expiration;
      }
    }

    public Session()
    {
    }

    public Session(Guid owner)
    {
      this.Id = Guid.NewGuid();
      this.Owner = owner;
      this.Status = SessionState.Active;
      this.Expiration = this.Clock.Now.UtcDateTime.AddHours(2.0);
      this.Parts = new List<Part>();
    }

    public Session(
      Guid id,
      Guid owner,
      SessionState status,
      DateTime expiration,
      DedupIdentifier contentId,
      List<Part> parts)
    {
      this.Id = id;
      this.Owner = owner;
      this.Status = status;
      this.Expiration = expiration;
      this.ContentId = contentId;
      this.Parts = parts;
    }

    public bool CanAccess(Guid userName) => this.Owner.Equals(userName);
  }
}
