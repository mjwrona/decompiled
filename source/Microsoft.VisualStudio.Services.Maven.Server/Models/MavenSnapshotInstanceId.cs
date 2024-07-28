// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Models.MavenSnapshotInstanceId
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Maven.Server.Models
{
  public class MavenSnapshotInstanceId : IEquatable<MavenSnapshotInstanceId>
  {
    public int BuildId { get; set; }

    [JsonConverter(typeof (LongDateConverter))]
    public DateTime Timestamp { get; set; }

    public bool Equals(MavenSnapshotInstanceId other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return this.BuildId == other.BuildId && this.Timestamp.Equals(other.Timestamp);
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if (this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((MavenSnapshotInstanceId) obj);
    }

    public override int GetHashCode() => this.BuildId * 397 ^ this.Timestamp.GetHashCode();
  }
}
