// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.IHostMigrationExtension
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  [InheritedExport]
  public interface IHostMigrationExtension
  {
    void PrepareSource(IVssRequestContext requestContext, SourceHostMigration migrationEntry);

    void PrepareSourceDatabase(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties dbProperties,
      TeamFoundationDatabaseCredential credential,
      ITFLogger logger);

    void FinalizeSource(
      IVssRequestContext requestContext,
      SourceHostMigration migrationEntry,
      bool rollback);

    void FinalizeTarget(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry,
      bool rollback);

    void CleanupTarget(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry,
      bool rolledback);

    void CleanupSource(
      IVssRequestContext requestContext,
      SourceHostMigration migrationEntry,
      bool rolledback);
  }
}
