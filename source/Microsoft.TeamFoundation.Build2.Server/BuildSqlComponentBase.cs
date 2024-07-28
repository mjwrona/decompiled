// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildSqlComponentBase
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [SupportedSqlAccessIntent(SqlAccessIntent.ReadWrite | SqlAccessIntent.ReadOnly, null)]
  internal abstract class BuildSqlComponentBase : TeamFoundationSqlResourceComponent
  {
    private IDictionary<int, SqlExceptionFactory> m_translatedExceptions;
    private static readonly SqlMetaData[] typ_BranchNameTable = new SqlMetaData[1]
    {
      new SqlMetaData("Name", SqlDbType.NVarChar, 400L)
    };

    protected BuildSqlComponentBase()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    protected override void Initialize(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
      base.Initialize(requestContext, dataspaceCategory, dataspaceIdentifier, serviceVersion, connectionType, logger);
    }

    protected override string TraceArea => "Build2";

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions
    {
      get
      {
        if (this.m_translatedExceptions == null)
          this.m_translatedExceptions = this.CreateExceptionMap();
        return this.m_translatedExceptions;
      }
    }

    protected override SqlParameter BindNullableGuid(string parameterName, Guid? parameterValue)
    {
      SqlParameter sqlParameter = this.Command.Parameters.Add(parameterName, SqlDbType.UniqueIdentifier);
      if (parameterValue.HasValue)
        sqlParameter.Value = (object) parameterValue.Value;
      else
        sqlParameter.Value = (object) DBNull.Value;
      return sqlParameter;
    }

    protected SqlParameter BindNullableInt32(string parameterName, int? parameterValue)
    {
      SqlParameter sqlParameter = this.Command.Parameters.Add(parameterName, SqlDbType.Int);
      if (parameterValue.HasValue)
        sqlParameter.Value = (object) parameterValue.Value;
      else
        sqlParameter.Value = (object) DBNull.Value;
      return sqlParameter;
    }

    protected override SqlParameter BindDateTime2(string parameterName, DateTime parameterValue)
    {
      SqlParameter sqlParameter = this.Command.Parameters.Add(parameterName, SqlDbType.DateTime2);
      sqlParameter.Size = 7;
      sqlParameter.Value = (object) parameterValue;
      return sqlParameter;
    }

    protected override SqlParameter BindNullableDateTime2(
      string parameterName,
      DateTime? parameterValue)
    {
      SqlParameter sqlParameter = this.Command.Parameters.Add(parameterName, SqlDbType.DateTime2);
      sqlParameter.Size = 7;
      if (parameterValue.HasValue)
        sqlParameter.Value = (object) parameterValue.Value;
      else
        sqlParameter.Value = (object) DBNull.Value;
      return sqlParameter;
    }

    protected DateTime EnsureUtc(DateTime value) => value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(value, DateTimeKind.Utc) : value.ToUniversalTime();

    protected DateTime? EnsureUtc(DateTime? value) => value.HasValue ? new DateTime?(this.EnsureUtc(value.Value)) : value;

    protected SqlParameter BindUtcDateTime2(string parameterName, DateTime parameterValue)
    {
      parameterValue = DBHelper.GetDateTime(this.EnsureUtc(parameterValue));
      SqlParameter sqlParameter = this.Command.Parameters.Add(parameterName, SqlDbType.DateTime2);
      sqlParameter.Size = 7;
      sqlParameter.Value = (object) parameterValue;
      return sqlParameter;
    }

    protected SqlParameter BindNullableUtcDateTime2(string parameterName, DateTime? parameterValue)
    {
      parameterValue = DBHelper.GetDateTime(this.EnsureUtc(parameterValue));
      SqlParameter sqlParameter = this.Command.Parameters.Add(parameterName, SqlDbType.DateTime2);
      sqlParameter.Size = 7;
      if (parameterValue.HasValue)
        sqlParameter.Value = (object) parameterValue.Value;
      else
        sqlParameter.Value = (object) DBNull.Value;
      return sqlParameter;
    }

    protected SqlParameter BindBranchNameTable(string parameterName, IEnumerable<string> rows)
    {
      rows = rows ?? Enumerable.Empty<string>();
      System.Func<string, SqlDataRecord> selector = (System.Func<string, SqlDataRecord>) (name =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(BuildSqlComponentBase.typ_BranchNameTable);
        name = name ?? string.Empty;
        if (name != null && name.Length > 400)
          name = name.Substring(0, 400);
        sqlDataRecord.SetString(0, name);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Build.typ_BranchNameTable", rows.Select<string, SqlDataRecord>(selector));
    }

    protected SqlParameter BindRecursiveFolderPath(string parameterName, string folderPath)
    {
      string path = string.Empty;
      if (!string.IsNullOrWhiteSpace(folderPath))
      {
        FolderValidator.CheckValidItemPath(ref folderPath, true, true);
        string str = folderPath;
        if (!str.EndsWith("\\", StringComparison.OrdinalIgnoreCase))
          str += "\\";
        path = str + "**\\*\\";
      }
      return this.BindString(parameterName, DBHelper.UserToDBPath(path), 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
    }

    protected string ToString<T>(IList<T> value)
    {
      List<T> list = ((IEnumerable<T>) value ?? Enumerable.Empty<T>()).Where<T>((System.Func<T, bool>) (item => (object) item != null)).ToList<T>();
      return list.Count > 0 ? JsonUtility.ToString<T>((IList<T>) list) : (string) null;
    }

    protected string ToString<TKey, TValue>(IDictionary<TKey, TValue> value) => value == null || value.Count == 0 ? (string) null : JsonUtility.ToString((object) value);

    protected IDisposable TraceScope(int tracepoint = 0, [CallerMemberName] string method = null) => (IDisposable) new BuildSqlComponentBase.SqlMethodScope(this, tracepoint, method);

    private Exception CreateException(
      IVssRequestContext requestContext,
      int errorNumber,
      SqlException sqlException,
      SqlError sqlError)
    {
      Exception exception = (Exception) null;
      switch (errorNumber)
      {
        case 901005:
          exception = (Exception) new DefinitionExistsException(BuildServerResources.DefinitionExists((object) DBHelper.DBPathToServerPath(sqlError.ExtractString("definitionName")), (object) this.ExtractProject(requestContext, sqlError)));
          break;
        case 901006:
          exception = (Exception) new DefinitionNotFoundException(BuildServerResources.DefinitionNotFound((object) sqlError.ExtractInt("definitionId")));
          break;
        case 901007:
          exception = (Exception) new QueueExistsException(BuildServerResources.QueueExists((object) sqlError.ExtractString("queueName")));
          break;
        case 901008:
          exception = (Exception) new QueueNotFoundException(BuildServerResources.QueueNotFound((object) sqlError.ExtractInt("queueId")));
          break;
        case 901009:
          exception = (Exception) new BuildExistsException(BuildServerResources.BuildExists((object) DBHelper.DBPathToServerPath(sqlError.ExtractString("buildNumber")), (object) DBHelper.DBPathToServerPath(sqlError.ExtractString("definitionName")), (object) this.ExtractProject(requestContext, sqlError)));
          break;
        case 901010:
          exception = (Exception) new BuildNotFoundException(BuildServerResources.BuildNotFound((object) sqlError.ExtractInt("buildId")));
          break;
        case 901011:
          exception = (Exception) new DefinitionDisabledException(BuildServerResources.DefinitionDisabled((object) DBHelper.DBPathToServerPath(sqlError.ExtractString("definitionName")), (object) this.ExtractProject(requestContext, sqlError)));
          break;
        case 901012:
          exception = (Exception) new ArtifactExistsException(BuildServerResources.ArtifactExists((object) sqlError.ExtractString("artifactName"), (object) sqlError.ExtractInt("buildId")));
          break;
        case 901015:
          exception = (Exception) new BuildStatusInvalidChangeException(BuildServerResources.BuildStatusInvalidChange((object) sqlError.ExtractInt("buildId"), (object) BuildSqlComponentBase.ToBuildStatusString(sqlError.ExtractInt("oldBuildStatus")), (object) BuildSqlComponentBase.ToBuildStatusString(sqlError.ExtractInt("newBuildStatus"))));
          break;
        case 901016:
          exception = (Exception) new BuildNumberFormatException(BuildServerResources.BuildNumberFormatInvalidOutput((object) sqlError.ExtractString("buildNumberFormat"), (object) DBHelper.DBPathToServerPath(sqlError.ExtractString("buildNumber"))));
          break;
        case 901017:
          exception = (Exception) new FolderExistsException(BuildServerResources.FolderExists((object) sqlError.ExtractString("folderPath"), (object) this.ExtractProject(requestContext, sqlError)));
          break;
        case 901018:
          exception = (Exception) new FolderNotFoundException(BuildServerResources.FolderNotFound((object) sqlError.ExtractInt("folderId")));
          break;
        case 901019:
          exception = (Exception) new DefinitionTriggerAlreadyExistsException(BuildServerResources.DefinitionTriggerAlreadyExistsException((object) sqlError.ExtractInt("definitionId")));
          break;
        case 901020:
          exception = (Exception) new InvalidDefinitionInTriggerSourceException(BuildServerResources.InvalidDefinitionTriggerSource((object) sqlError.ExtractInt("definitionId")));
          break;
        case 901021:
          exception = (Exception) new CycleDetectedInProvidedBuildCompletionTriggersException(BuildServerResources.CycleDetectedInProvidedBuildCompletionTriggers());
          break;
        case 901022:
          exception = (Exception) new UnsupportedBuildCompletionTriggerChainException(BuildServerResources.UnsupportedBuildCompletionTriggerChain((object) sqlError.ExtractInt("definitionId")));
          break;
        case 901023:
          exception = (Exception) new CannotDeleteDefinitionWithRetainedBuildsException(BuildServerResources.CannotDeleteDefinitionWithRetainedBuilds());
          break;
        case 901024:
          exception = (Exception) new CannotRestoreDeletedDraftWithoutRestoringParentException(BuildServerResources.CannotRestoreDeletedDraftWithoutRestoringParent());
          break;
        case 901025:
          exception = (Exception) new BuildEventNotFoundException(BuildServerResources.BuildEventNotFound((object) sqlError.ExtractInt("buildId"), (object) sqlError.ExtractInt("eventType")));
          break;
        case 901026:
          exception = (Exception) new BuildEventStatusInvalidChangeException(BuildServerResources.InvalidBuildEventStatusUpdate((object) sqlError.ExtractInt("buildId"), (object) sqlError.ExtractInt("eventType"), (object) sqlError.ExtractInt("eventStatus")));
          break;
        case 901027:
          exception = (Exception) new BuildOrchestrationExistsException(BuildServerResources.BuildOrchestrationExists((object) sqlError.ExtractInt("existingBuildId"), (object) this.ExtractProject(requestContext, sqlError)));
          break;
      }
      return exception;
    }

    private string ExtractProject(IVssRequestContext requestContext, SqlError error)
    {
      string str = error.ExtractString("projectId");
      int result1;
      Guid result2;
      if (int.TryParse(str, out result1))
        result2 = this.GetDataspaceIdentifier(result1);
      else
        Guid.TryParse(str, out result2);
      using (requestContext.AcquireExemptionLock())
        return requestContext.GetService<IProjectService>().GetProjectName(requestContext, result2);
    }

    private IDictionary<int, SqlExceptionFactory> CreateExceptionMap() => (IDictionary<int, SqlExceptionFactory>) new Dictionary<int, SqlExceptionFactory>()
    {
      {
        901009,
        new SqlExceptionFactory(typeof (BuildExistsException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(this.CreateException))
      },
      {
        901010,
        new SqlExceptionFactory(typeof (BuildNotFoundException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(this.CreateException))
      },
      {
        901016,
        new SqlExceptionFactory(typeof (BuildNumberFormatException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(this.CreateException))
      },
      {
        901005,
        new SqlExceptionFactory(typeof (DefinitionExistsException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(this.CreateException))
      },
      {
        901006,
        new SqlExceptionFactory(typeof (DefinitionNotFoundException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(this.CreateException))
      },
      {
        901011,
        new SqlExceptionFactory(typeof (DefinitionDisabledException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(this.CreateException))
      },
      {
        901017,
        new SqlExceptionFactory(typeof (FolderExistsException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(this.CreateException))
      },
      {
        901018,
        new SqlExceptionFactory(typeof (FolderNotFoundException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(this.CreateException))
      },
      {
        901007,
        new SqlExceptionFactory(typeof (QueueExistsException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(this.CreateException))
      },
      {
        901008,
        new SqlExceptionFactory(typeof (QueueNotFoundException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(this.CreateException))
      },
      {
        901012,
        new SqlExceptionFactory(typeof (ArtifactExistsException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(this.CreateException))
      },
      {
        901014,
        new SqlExceptionFactory(typeof (DefinitionTemplateExistsException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(this.CreateException))
      },
      {
        901015,
        new SqlExceptionFactory(typeof (BuildStatusInvalidChangeException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(this.CreateException))
      },
      {
        901021,
        new SqlExceptionFactory(typeof (CycleDetectedInProvidedBuildCompletionTriggersException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(this.CreateException))
      },
      {
        901019,
        new SqlExceptionFactory(typeof (DefinitionTriggerAlreadyExistsException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(this.CreateException))
      },
      {
        901020,
        new SqlExceptionFactory(typeof (InvalidDefinitionInTriggerSourceException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(this.CreateException))
      },
      {
        901022,
        new SqlExceptionFactory(typeof (UnsupportedBuildCompletionTriggerChainException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(this.CreateException))
      },
      {
        901023,
        new SqlExceptionFactory(typeof (CannotDeleteDefinitionWithRetainedBuildsException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(this.CreateException))
      },
      {
        901024,
        new SqlExceptionFactory(typeof (CannotRestoreDeletedDraftWithoutRestoringParentException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(this.CreateException))
      },
      {
        901025,
        new SqlExceptionFactory(typeof (BuildEventNotFoundException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(this.CreateException))
      },
      {
        901026,
        new SqlExceptionFactory(typeof (BuildEventStatusInvalidChangeException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(this.CreateException))
      },
      {
        901027,
        new SqlExceptionFactory(typeof (BuildOrchestrationExistsException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(this.CreateException))
      }
    };

    private static string ToBuildStatusString(int status) => ((BuildStatus) status).ToString();

    private struct SqlMethodScope : IDisposable
    {
      private readonly int m_tracepoint;
      private readonly string m_method;
      private readonly BuildSqlComponentBase m_component;

      public SqlMethodScope(BuildSqlComponentBase component, int tracepoint, string method)
      {
        this.m_tracepoint = tracepoint;
        this.m_method = method;
        this.m_component = component;
        this.m_component.TraceEnter(this.m_tracepoint, this.m_method);
      }

      public void Dispose() => this.m_component.TraceLeave(this.m_tracepoint, this.m_method);
    }
  }
}
