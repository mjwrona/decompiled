// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.OutputVariableSecret
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal class OutputVariableSecret : VariableSecret
  {
    public Guid TimelineId;
    public Guid RecordId;
    private const string c_secretOutputvariableStrongBoxLookupKeyFormat = "{0}/{1}/variables/{2}";

    public override string GetLookupKey() => OutputVariableSecret.GetLookupKey(this.TimelineId, this.RecordId, this.Name);

    public static string GetLookupKey(Guid timelineId, Guid recordId, string name) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/variables/{2}", (object) timelineId, (object) recordId, (object) name);
  }
}
