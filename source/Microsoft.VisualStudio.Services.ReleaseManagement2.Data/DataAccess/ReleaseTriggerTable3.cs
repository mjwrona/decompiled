// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.ReleaseTriggerTable3
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  public static class ReleaseTriggerTable3
  {
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[4]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("TriggerType", SqlDbType.TinyInt),
      new Microsoft.SqlServer.Server.SqlMetaData("TriggerEntityId", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("TargetEnvironmentName", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("TriggerContent", SqlDbType.NVarChar, -1L)
    };

    public static void BindReleaseTriggerTable3(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ReleaseTriggerBase> releaseTriggers,
      IEnumerable<ArtifactSource> artifactSources)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseTriggerTableV3", ReleaseTriggerTable3.GetSqlDataRecords(releaseTriggers, artifactSources));
    }

    public static void BindReleaseTriggerTable4(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ReleaseTriggerBase> releaseTriggers,
      IEnumerable<ArtifactSource> artifactSources)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseTriggerTableV4", ReleaseTriggerTable3.GetSqlDataRecords(releaseTriggers, artifactSources));
    }

    private static IEnumerable<SqlDataRecord> GetSqlDataRecords(
      IEnumerable<ReleaseTriggerBase> rows,
      IEnumerable<ArtifactSource> artifactSources)
    {
      rows = rows ?? Enumerable.Empty<ReleaseTriggerBase>();
      foreach (ReleaseTriggerBase trigger in rows.Where<ReleaseTriggerBase>((System.Func<ReleaseTriggerBase, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ReleaseTriggerTable3.SqlMetaData);
        sqlDataRecord.SetByte(ordinal, (byte) trigger.TriggerType);
        int num1;
        sqlDataRecord.SetInt32(num1 = ordinal + 1, ReleaseTriggerTable3.GetArtifactSourceId(trigger, artifactSources));
        int num2;
        sqlDataRecord.SetString(num2 = num1 + 1, ReleaseTriggerTable3.GetTargetEnvironmentName(trigger));
        int num3;
        sqlDataRecord.SetString(num3 = num2 + 1, ReleaseTriggerTable3.GetTriggerContent(trigger));
        yield return sqlDataRecord;
      }
    }

    private static int GetArtifactSourceId(
      ReleaseTriggerBase trigger,
      IEnumerable<ArtifactSource> artifactSources)
    {
      ArtifactSourceTrigger artifactSourceTrigger = trigger.ToArtifactSourceTrigger();
      string artifactAlias = artifactSourceTrigger == null ? string.Empty : artifactSourceTrigger.Alias;
      return !string.IsNullOrEmpty(artifactAlias) ? ReleaseTriggerTable3.ToArtifactSourceId(artifactAlias, artifactSources) : 0;
    }

    private static string GetTargetEnvironmentName(ReleaseTriggerBase trigger)
    {
      ArtifactSourceTrigger artifactSourceTrigger = trigger.ToArtifactSourceTrigger();
      return artifactSourceTrigger == null || string.IsNullOrEmpty(artifactSourceTrigger.TargetEnvironmentName) ? string.Empty : artifactSourceTrigger.TargetEnvironmentName;
    }

    private static string GetTriggerContent(ReleaseTriggerBase trigger)
    {
      string triggerContent = string.Empty;
      ScheduledReleaseTrigger scheduledReleaseTrigger = trigger.ToScheduledReleaseTrigger();
      if (scheduledReleaseTrigger != null && !string.IsNullOrEmpty(scheduledReleaseTrigger.TriggerContent))
        triggerContent = scheduledReleaseTrigger.TriggerContent;
      return triggerContent;
    }

    private static int ToArtifactSourceId(
      string artifactAlias,
      IEnumerable<ArtifactSource> artifactSources)
    {
      foreach (ArtifactSource artifactSource in artifactSources)
      {
        if (artifactAlias.Equals(artifactSource.Alias, StringComparison.OrdinalIgnoreCase))
          return artifactSource.Id;
      }
      throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ArtifactWithAliasNotFound, (object) artifactAlias));
    }
  }
}
