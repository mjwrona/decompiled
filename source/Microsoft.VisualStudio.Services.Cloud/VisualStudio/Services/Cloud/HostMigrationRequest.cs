// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigrationRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public sealed class HostMigrationRequest
  {
    public Guid HostId { get; set; }

    public string TargetInstanceName { get; set; }

    public byte Priority { get; set; }

    public HostMigrationOptions Options { get; set; }

    public Guid DriverJobId { get; set; }

    public Guid MigrationId { get; set; }

    public int TargetDatabaseId { get; set; }

    public int HostsAffectedByTheMove { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Host Id: {0}, Target Instance: {1}, Priority: {2}, Options: {3}, MigrationId: {4}, TargetDatabaseId: {5}", (object) this.HostId, (object) this.TargetInstanceName, (object) this.Priority, (object) this.Options, (object) this.MigrationId, (object) this.TargetDatabaseId);
  }
}
