// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationDatabaseCopyStatus
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TeamFoundationDatabaseCopyStatus
  {
    internal DateTimeOffset StartDate { get; set; }

    internal DateTimeOffset ModifyDate { get; set; }

    internal double PercentComplete { get; set; }

    internal int ErrorCode { get; set; }

    internal string ErrorDescription { get; set; }

    internal int ErrorSeverity { get; set; }

    internal int ErrorState { get; set; }

    internal ReplicationState ReplicationState { get; set; }

    internal string ReplicationStateDescription { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[ReplicationState: {0}, PercentComplete: {1}, ErrorCode: {2}, ErrorDesc: {3}]", (object) this.ReplicationStateDescription, (object) this.PercentComplete, (object) this.ErrorCode, (object) this.ErrorDescription);
  }
}
