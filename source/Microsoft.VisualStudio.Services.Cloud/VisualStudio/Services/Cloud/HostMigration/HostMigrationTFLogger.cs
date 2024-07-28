// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.HostMigrationTFLogger
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  internal class HostMigrationTFLogger : ITFLogger
  {
    private readonly IVssRequestContext RequestContext;
    private readonly IMigrationEntry MigrationEntry;
    private readonly string Operation;

    public HostMigrationTFLogger(
      IVssRequestContext requestContext,
      IMigrationEntry migrationEntry,
      string operation)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IMigrationEntry>(migrationEntry, nameof (migrationEntry));
      ArgumentUtility.CheckStringForNullOrEmpty(operation, nameof (operation));
      this.RequestContext = requestContext;
      this.MigrationEntry = migrationEntry;
      this.Operation = operation;
    }

    public void Error(string message) => this.Log(message, ServicingStepLogEntryKind.Error);

    public void Error(string message, params object[] args) => this.Error(string.Format(message, args));

    public void Error(Exception exception) => this.Error("Exception: " + exception.ToReadableStackTrace());

    public void Info(string message) => this.Log(message, ServicingStepLogEntryKind.Informational);

    public void Info(string message, params object[] args) => this.Info(string.Format(message, args));

    public void Warning(string message) => this.Log(message, ServicingStepLogEntryKind.Warning);

    public void Warning(string message, params object[] args) => this.Warning(string.Format(message, args));

    public void Warning(Exception exception) => this.Warning("[WARN] Exception: " + exception.ToReadableStackTrace());

    private void Log(string message, ServicingStepLogEntryKind entryKind) => HostMigrationLogger.LogInfo(this.RequestContext, this.MigrationEntry, this.Operation, message, entryKind);
  }
}
