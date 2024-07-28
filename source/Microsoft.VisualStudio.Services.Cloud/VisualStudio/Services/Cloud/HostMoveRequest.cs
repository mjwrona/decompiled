// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMoveRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public sealed class HostMoveRequest
  {
    public Guid HostId { get; set; }

    public int TargetDatabaseId { get; set; }

    public byte Priority { get; set; }

    public HostMoveOptions Options { get; set; }

    public Guid JobId { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Host Id: {0}, Target Database: {1}, Priority: {2}, Options: {3}, JobId: {4}", (object) this.HostId, (object) this.TargetDatabaseId, (object) this.Priority, (object) this.Options, (object) this.JobId);
  }
}
