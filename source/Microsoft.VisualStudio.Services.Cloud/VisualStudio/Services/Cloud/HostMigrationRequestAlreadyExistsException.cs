// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigrationRequestAlreadyExistsException
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Security;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [Serializable]
  public class HostMigrationRequestAlreadyExistsException : TeamFoundationServiceException
  {
    public string ExistingRecordTargetInstanceName { get; private set; }

    public byte ExistingRecordPriority { get; private set; }

    public HostMigrationRequestAlreadyExistsException()
    {
    }

    public HostMigrationRequestAlreadyExistsException(
      Guid hostId,
      string targetInstanceName,
      byte priority)
      : base(HostingResources.HostMigrateRequestAlreadyExistsException((object) hostId, (object) targetInstanceName, (object) priority))
    {
      this.ExistingRecordTargetInstanceName = targetInstanceName;
      this.ExistingRecordPriority = priority;
    }

    public HostMigrationRequestAlreadyExistsException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(new Guid(TeamFoundationServiceException.ExtractString(sqlError, "hostId")), TeamFoundationServiceException.ExtractString(sqlError, "targetInstanceName"), Convert.ToByte(TeamFoundationServiceException.ExtractString(sqlError, "priority")))
    {
    }

    protected HostMigrationRequestAlreadyExistsException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
      this.ExistingRecordTargetInstanceName = info.GetString(nameof (ExistingRecordTargetInstanceName));
      this.ExistingRecordPriority = info.GetByte(nameof (ExistingRecordPriority));
    }

    [SecurityCritical]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("ExistingRecordTargetInstanceName", (object) this.ExistingRecordTargetInstanceName);
      info.AddValue("ExistingRecordPriority", this.ExistingRecordPriority);
    }
  }
}
