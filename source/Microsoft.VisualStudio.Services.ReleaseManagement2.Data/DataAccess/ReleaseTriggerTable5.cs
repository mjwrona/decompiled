// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.ReleaseTriggerTable5
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  public static class ReleaseTriggerTable5
  {
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[4]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("TriggerType", SqlDbType.TinyInt),
      new Microsoft.SqlServer.Server.SqlMetaData("Alias", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("TargetEnvironmentName", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("TriggerContent", SqlDbType.NVarChar, -1L)
    };
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData6 = new Microsoft.SqlServer.Server.SqlMetaData[5]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("TriggerType", SqlDbType.TinyInt),
      new Microsoft.SqlServer.Server.SqlMetaData("Alias", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("TargetEnvironmentName", SqlDbType.NVarChar, 256L),
      new Microsoft.SqlServer.Server.SqlMetaData("TriggerContent", SqlDbType.NVarChar, -1L),
      new Microsoft.SqlServer.Server.SqlMetaData("DefinitionGuid", SqlDbType.UniqueIdentifier)
    };

    public static void BindReleaseTriggerTable5(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ReleaseTriggerBase> releaseTriggers)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseTriggerTableV5", ReleaseTriggerTable5.GetSqlDataRecords5(releaseTriggers));
    }

    private static IEnumerable<SqlDataRecord> GetSqlDataRecords5(
      IEnumerable<ReleaseTriggerBase> rows)
    {
      rows = rows ?? Enumerable.Empty<ReleaseTriggerBase>();
      foreach (ReleaseTriggerBase trigger in rows.Where<ReleaseTriggerBase>((System.Func<ReleaseTriggerBase, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ReleaseTriggerTable5.SqlMetaData);
        sqlDataRecord.SetByte(ordinal, (byte) trigger.TriggerType);
        int num1;
        sqlDataRecord.SetString(num1 = ordinal + 1, ReleaseTriggerTable5.GetAlias(trigger));
        int num2;
        sqlDataRecord.SetString(num2 = num1 + 1, ReleaseTriggerTable5.GetTargetEnvironmentName(trigger));
        int num3;
        sqlDataRecord.SetString(num3 = num2 + 1, ReleaseTriggerTable5.GetTriggerContent(trigger));
        yield return sqlDataRecord;
      }
    }

    private static string GetAlias(ReleaseTriggerBase trigger)
    {
      if (trigger.TriggerType == ReleaseTriggerType.ArtifactSource)
      {
        ArtifactSourceTrigger artifactSourceTrigger = trigger.ToArtifactSourceTrigger();
        return artifactSourceTrigger != null ? artifactSourceTrigger.Alias : string.Empty;
      }
      if (trigger.TriggerType == ReleaseTriggerType.SourceRepo)
      {
        SourceRepoTrigger sourceRepoTrigger = trigger.ToSourceRepoTrigger();
        return sourceRepoTrigger != null ? sourceRepoTrigger.Alias : string.Empty;
      }
      if (trigger.TriggerType == ReleaseTriggerType.ContainerImage)
      {
        ContainerImageTrigger containerImageTrigger = trigger.ToContainerImageTrigger();
        return containerImageTrigger != null ? containerImageTrigger.Alias : string.Empty;
      }
      if (trigger.TriggerType == ReleaseTriggerType.Package)
      {
        PackageTrigger packageTrigger = trigger.ToPackageTrigger();
        return packageTrigger != null ? packageTrigger.Alias : string.Empty;
      }
      if (trigger.TriggerType != ReleaseTriggerType.PullRequest)
        return string.Empty;
      PullRequestTrigger pullRequestTrigger = trigger.ToPullRequestTrigger();
      return pullRequestTrigger != null ? pullRequestTrigger.ArtifactAlias : string.Empty;
    }

    private static string GetTargetEnvironmentName(ReleaseTriggerBase trigger)
    {
      ArtifactSourceTrigger artifactSourceTrigger = trigger.ToArtifactSourceTrigger();
      return artifactSourceTrigger == null || string.IsNullOrEmpty(artifactSourceTrigger.TargetEnvironmentName) ? string.Empty : artifactSourceTrigger.TargetEnvironmentName;
    }

    private static string GetTriggerContent(ReleaseTriggerBase trigger)
    {
      string str1 = string.Empty;
      string str2 = string.Empty;
      if (trigger.TriggerType == ReleaseTriggerType.Schedule)
      {
        ScheduledReleaseTrigger scheduledReleaseTrigger = trigger.ToScheduledReleaseTrigger();
        if (scheduledReleaseTrigger != null && !string.IsNullOrEmpty(scheduledReleaseTrigger.TriggerContent))
          str1 = scheduledReleaseTrigger.TriggerContent;
      }
      else if (trigger.TriggerType == ReleaseTriggerType.ArtifactSource)
      {
        ArtifactSourceTrigger artifactSourceTrigger = trigger.ToArtifactSourceTrigger();
        if (artifactSourceTrigger != null && artifactSourceTrigger.TriggerConditions != null)
        {
          str2 = artifactSourceTrigger.Alias;
          str1 = JsonConvert.SerializeObject((object) artifactSourceTrigger.TriggerConditions);
        }
      }
      else if (trigger.TriggerType == ReleaseTriggerType.SourceRepo)
      {
        SourceRepoTrigger sourceRepoTrigger = trigger.ToSourceRepoTrigger();
        if (sourceRepoTrigger != null)
        {
          str2 = sourceRepoTrigger.Alias;
          str1 = JsonConvert.SerializeObject((object) sourceRepoTrigger);
        }
      }
      else if (trigger.TriggerType == ReleaseTriggerType.ContainerImage)
      {
        ContainerImageTrigger containerImageTrigger = trigger.ToContainerImageTrigger();
        if (containerImageTrigger != null)
        {
          str2 = containerImageTrigger.Alias;
          str1 = JsonConvert.SerializeObject((object) containerImageTrigger);
        }
      }
      else if (trigger.TriggerType == ReleaseTriggerType.Package)
      {
        PackageTrigger packageTrigger = trigger.ToPackageTrigger();
        if (packageTrigger != null)
        {
          str2 = packageTrigger.Alias;
          str1 = JsonConvert.SerializeObject((object) packageTrigger);
        }
      }
      else if (trigger.TriggerType == ReleaseTriggerType.PullRequest)
      {
        PullRequestTrigger pullRequestTrigger = trigger.ToPullRequestTrigger();
        if (pullRequestTrigger != null)
        {
          str2 = pullRequestTrigger.ArtifactAlias;
          str1 = JsonConvert.SerializeObject((object) pullRequestTrigger);
        }
      }
      return str1.Length <= 4000 ? str1 : throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TriggerContentExceededLimit, (object) str2));
    }

    public static void BindReleaseTriggerTable6(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ReleaseTriggerBase> releaseTriggers)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseTriggerTableV6", ReleaseTriggerTable5.GetSqlDataRecords6(releaseTriggers));
    }

    private static IEnumerable<SqlDataRecord> GetSqlDataRecords6(
      IEnumerable<ReleaseTriggerBase> rows)
    {
      rows = rows ?? Enumerable.Empty<ReleaseTriggerBase>();
      foreach (ReleaseTriggerBase trigger in rows.Where<ReleaseTriggerBase>((System.Func<ReleaseTriggerBase, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord record = new SqlDataRecord(ReleaseTriggerTable5.SqlMetaData6);
        record.SetByte(ordinal, (byte) trigger.TriggerType);
        int num1;
        record.SetString(num1 = ordinal + 1, ReleaseTriggerTable5.GetAlias(trigger));
        int num2;
        record.SetString(num2 = num1 + 1, ReleaseTriggerTable5.GetTargetEnvironmentName(trigger));
        int num3;
        record.SetString(num3 = num2 + 1, ReleaseTriggerTable5.GetTriggerContent(trigger));
        int num4;
        record.SetNullableGuid(num4 = num3 + 1, new Guid?());
        yield return record;
      }
    }

    public static void BindReleaseTriggerTable7(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ReleaseTriggerBase> releaseTriggers)
    {
      if (component == null)
        throw new ArgumentNullException(nameof (component));
      component.BindTable(parameterName, "Release.typ_ReleaseTriggerTableV7", ReleaseTriggerTable5.GetSqlDataRecords6(releaseTriggers));
    }
  }
}
