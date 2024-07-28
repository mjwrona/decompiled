// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlScriptException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class SqlScriptException : Exception
  {
    private readonly SqlErrorCollection m_errors;
    private readonly SqlBatch m_batch;

    protected SqlScriptException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public SqlScriptException(ICollection<SqlError> sqlErrors, SqlBatch batch)
      : base(SqlScriptException.CreateMessage(sqlErrors, batch))
    {
      this.m_errors = SqlScriptException.CreateSqlErrrorCollection((IEnumerable<SqlError>) sqlErrors);
      this.m_batch = batch;
    }

    public static string CreateMessage(ICollection<SqlError> sqlErrors, SqlBatch batch)
    {
      ArgumentUtility.CheckForNull<ICollection<SqlError>>(sqlErrors, nameof (sqlErrors));
      StringBuilder stringBuilder = new StringBuilder();
      if (batch != null && !string.IsNullOrEmpty(batch.ScriptName))
        stringBuilder.AppendFormat("{0} error(s) occurred while executing {1} script.", (object) sqlErrors.Count, (object) batch.ScriptName).AppendLine();
      int num = 0;
      if (batch != null)
      {
        stringBuilder.AppendFormat("Failed batch starts on line: {0}.", (object) batch.LineNumber).AppendLine();
        num = batch.LineNumber - 1 - batch.HeaderLinesCount;
        if (num < 0)
          num = 0;
      }
      foreach (SqlError sqlError in (IEnumerable<SqlError>) sqlErrors)
      {
        string str = string.Format("Error: {0}, Level: {1}, State: {2}, Batch Line: {3}, Script Line: {4}\r\nMessage: {5}", (object) sqlError.Number, (object) sqlError.Class, (object) sqlError.State, (object) (sqlError.LineNumber - (batch != null ? batch.HeaderLinesCount : 0)), (object) (sqlError.LineNumber + num), (object) sqlError.Message);
        stringBuilder.AppendLine().AppendLine(str);
      }
      if (batch != null)
      {
        stringBuilder.AppendLine("================ Failed batch begin ==========================");
        List<string> lines = batch.GetLines();
        for (int headerLinesCount = batch.HeaderLinesCount; headerLinesCount < lines.Count; ++headerLinesCount)
          stringBuilder.AppendLine(lines[headerLinesCount]);
        stringBuilder.AppendLine("================ Failed batch end ============================");
      }
      return stringBuilder.ToString();
    }

    public SqlErrorCollection Errors => this.m_errors;

    public SqlBatch Batch => this.m_batch;

    private static SqlErrorCollection CreateSqlErrrorCollection(IEnumerable<SqlError> sqlErrors)
    {
      SqlErrorCollection errrorCollection = (SqlErrorCollection) typeof (SqlErrorCollection).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, (Binder) null, Type.EmptyTypes, (ParameterModifier[]) null).Invoke((object[]) null);
      MethodInfo method = typeof (SqlErrorCollection).GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic);
      foreach (SqlError sqlError in sqlErrors)
        method.Invoke((object) errrorCollection, new object[1]
        {
          (object) sqlError
        });
      return errrorCollection;
    }
  }
}
