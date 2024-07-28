// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.SqlDatabaseBackup
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class SqlDatabaseBackup
  {
    private readonly string m_backupFilePath;

    public SqlDatabaseBackup(string backupFilePath)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(backupFilePath, nameof (backupFilePath));
      this.PercentCompleteNotification = 10;
      this.Initialize = true;
      this.Skip = false;
      this.Rewind = true;
      this.BackupType = BackupType.Database;
      this.m_backupFilePath = backupFilePath;
    }

    public bool ContinueAfterError { get; set; }

    public bool CopyOnly { get; set; }

    public bool? CompressionOption { get; set; }

    public bool Initialize { get; set; }

    public bool FormatMedia { get; set; }

    public string Database { get; set; }

    public int PercentCompleteNotification { get; set; }

    public bool Skip { get; set; }

    public bool Rewind { get; set; }

    public bool Unload { get; set; }

    public BackupType BackupType { get; set; }

    public IVssRequestContext RequestContext { get; set; }

    public ITFLogger Logger { get; set; }

    public event EventHandler<PercentCompleteEventArgs> PercentComplete;

    public event EventHandler<ServerMessageEventArgs> Information;

    public event EventHandler<ServerMessageEventArgs> Complete;

    public void Backup(ISqlConnectionInfo connectionInfo)
    {
      string sqlStatement = this.Script();
      using (SqlDatabaseBackupRestoreComponent componentRaw = connectionInfo.CreateComponentRaw<SqlDatabaseBackupRestoreComponent>(0))
      {
        componentRaw.Information += (EventHandler<ServerMessageEventArgs>) ((s, e) => this.RaiseEvent<ServerMessageEventArgs>(this.Information, e));
        componentRaw.Complete += (EventHandler<ServerMessageEventArgs>) ((s, e) => this.RaiseEvent<ServerMessageEventArgs>(this.Complete, e));
        componentRaw.PercentComplete += (EventHandler<PercentCompleteEventArgs>) ((s, e) => this.RaiseEvent<PercentCompleteEventArgs>(this.PercentComplete, e));
        componentRaw.Execute(sqlStatement);
      }
    }

    private void RaiseEvent<TEventArgs>(EventHandler<TEventArgs> eventHandler, TEventArgs args) where TEventArgs : EventArgs
    {
      if (eventHandler == null)
        return;
      eventHandler((object) this, args);
    }

    public string Script()
    {
      if (string.IsNullOrEmpty(this.Database))
        throw new InvalidOperationException("Database property not set.");
      StringBuilder stringBuilder = new StringBuilder();
      if (this.BackupType == BackupType.TransactionLog)
      {
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "BACKUP LOG [{0}] ", (object) TFCommonUtil.EscapeString(this.Database, '['));
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "TO DISK = N'{0}' WITH", (object) TFCommonUtil.EscapeString(this.m_backupFilePath, '\''));
      }
      else
      {
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "BACKUP DATABASE [{0}] ", (object) TFCommonUtil.EscapeString(this.Database, '['));
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "TO DISK = N'{0}' WITH", (object) TFCommonUtil.EscapeString(this.m_backupFilePath, '\''));
      }
      if (this.BackupType == BackupType.DifferentialDatabase)
        stringBuilder.Append(" DIFFERENTIAL,");
      stringBuilder.Append(this.FormatMedia ? " FORMAT," : " NOFORMAT,");
      stringBuilder.Append(this.Initialize ? "  INIT," : " NOINIT,");
      if (!this.FormatMedia)
        stringBuilder.Append(this.Skip ? " SKIP," : " NOSKIP,");
      stringBuilder.Append(this.Rewind ? " REWIND," : " NOREWIND,");
      stringBuilder.Append(this.Unload ? " UNLOAD," : " NOUNLOAD,");
      if (this.ContinueAfterError)
        stringBuilder.Append(" CONTINUE_AFTER_ERROR,");
      if (this.CopyOnly)
        stringBuilder.Append(" COPY_ONLY,");
      bool? compressionOption1 = this.CompressionOption;
      bool flag1 = true;
      if (compressionOption1.GetValueOrDefault() == flag1 & compressionOption1.HasValue)
      {
        stringBuilder.Append(" COMPRESSION,");
      }
      else
      {
        bool? compressionOption2 = this.CompressionOption;
        bool flag2 = false;
        if (compressionOption2.GetValueOrDefault() == flag2 & compressionOption2.HasValue)
          stringBuilder.Append(" NO_COMPRESSION,");
      }
      if (this.PercentCompleteNotification > 0 && this.PercentCompleteNotification <= 100)
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, " STATS = {0},", (object) this.PercentCompleteNotification);
      return stringBuilder.ToString().TrimEnd(',');
    }
  }
}
