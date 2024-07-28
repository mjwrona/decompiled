// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Zeus.DatabaseMigration
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Zeus
{
  [DataContract]
  public class DatabaseMigration
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int MigrationId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string SqlInstance { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string UserId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Password { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string StorageAccount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Container { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Databases { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DatabaseMigrationType MigrationType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int DatabasesMigrated { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DatabaseMigrationStatus Status { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? StartTime { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? QueuedTime { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? EndTime { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string StatusMessage { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid JobId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string DatabasePrefix { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DatabaseMigration\r\n[\r\n    MigrationId:           {0}\r\n    JobId:                 {1}\r\n    SqlInstance:           {2}\r\n    UserId:                {3}\r\n    DatabasePrefix         {4}\r\n    Container:             {5}\r\n    Databases:             {6}\r\n    MigrationType:         {7}\r\n    DatabasesMigrated:     {8}\r\n    QueuedTime:            {9}\r\n    StartTime:             {10}\r\n    EndTime:               {11}\r\n    Status:                {12}\r\n    StatusMessage:         {13}\r\n]", (object) this.MigrationId, (object) this.JobId, (object) this.SqlInstance, (object) this.UserId, (object) this.DatabasePrefix, (object) this.Container, (object) this.Databases, (object) this.MigrationType, (object) this.DatabasesMigrated, (object) this.QueuedTime, (object) this.StartTime, (object) this.EndTime, (object) this.Status, (object) this.StatusMessage);
  }
}
