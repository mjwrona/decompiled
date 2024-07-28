// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MigrationRegistryUtil
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class MigrationRegistryUtil
  {
    private static readonly string s_area = "HostMigrationCertificate";
    private static readonly string s_layer = nameof (MigrationRegistryUtil);

    public static HostMigrationSigningState GetMigrationSigningState(
      IVssRequestContext requestContext)
    {
      return MigrationRegistryUtil.GetMigrationSigningState(requestContext, out bool _);
    }

    public static HostMigrationSigningState GetMigrationSigningState(
      IVssRequestContext requestContext,
      out bool inheritedFromDatabase)
    {
      requestContext.CheckProjectCollectionOrOrganizationRequestContext();
      IVssServiceHost serviceHost = requestContext.ServiceHost;
      return MigrationRegistryUtil.GetMigrationSigningState(requestContext.To(TeamFoundationHostType.Deployment), serviceHost.InstanceId, serviceHost.ServiceHostInternal().DatabaseId, out inheritedFromDatabase);
    }

    public static HostMigrationSigningState GetMigrationSigningState(
      IVssRequestContext requestContext,
      Guid hostId,
      int databaseId,
      out bool inheritedFromDatabase)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string str = FrameworkServerConstants.MakeMigrationSigningDatabasePath<int>(databaseId);
      string state = service.GetValue<string>(requestContext, (RegistryQuery) str, string.Empty);
      inheritedFromDatabase = true;
      if (string.IsNullOrEmpty(state))
      {
        str = FrameworkServerConstants.MakeMigrationSigningHostPath<Guid>(hostId);
        state = service.GetValue<string>(requestContext, (RegistryQuery) str, string.Empty);
        inheritedFromDatabase = false;
      }
      return string.IsNullOrEmpty(state) ? HostMigrationSigningState.NotMigrating : MigrationRegistryUtil.RegistryStringToHostMigrationSigningState(state, str);
    }

    public static void SetDatabaseMigrationSigningState(
      IVssRequestContext requestContext,
      int databaseId,
      HostMigrationSigningState newState)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForNonnegativeInt(databaseId, nameof (databaseId));
      if (!MigrationRegistryUtil.SetOrDeleteMigrationState(requestContext, FrameworkServerConstants.MakeMigrationSigningDatabasePath<int>(databaseId), newState))
        return;
      requestContext.TraceAlways(27890230, TraceLevel.Info, MigrationRegistryUtil.s_area, MigrationRegistryUtil.s_layer, string.Format("Migration signing state for database {0} updated to {1}", (object) databaseId, (object) newState));
    }

    public static void SetHostMigrationSigningState(
      IVssRequestContext hostRequestContext,
      HostMigrationSigningState newState)
    {
      hostRequestContext.CheckProjectCollectionRequestContext();
      MigrationRegistryUtil.SetHostMigrationSigningState(hostRequestContext.To(TeamFoundationHostType.Deployment), hostRequestContext.ServiceHost.InstanceId, newState);
    }

    public static void SetHostMigrationSigningState(
      IVssRequestContext requestContext,
      Guid hostId,
      HostMigrationSigningState newState)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      if (!MigrationRegistryUtil.SetOrDeleteMigrationState(requestContext, FrameworkServerConstants.MakeMigrationSigningHostPath<Guid>(hostId), newState))
        return;
      requestContext.TraceAlways(27890231, TraceLevel.Info, MigrationRegistryUtil.s_area, MigrationRegistryUtil.s_layer, string.Format("Migration signing state for host {0} updated to {1}", (object) hostId, (object) newState));
    }

    private static bool SetOrDeleteMigrationState(
      IVssRequestContext requestContext,
      string registryPath,
      HostMigrationSigningState newState)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      if (newState == HostMigrationSigningState.NotMigrating)
        return service.DeleteEntries(requestContext, registryPath) > 0;
      service.SetValue<string>(requestContext, registryPath, MigrationRegistryUtil.HostMigrationSigningStateToRegistryString(newState));
      return true;
    }

    public static string HostMigrationSigningStateToRegistryString(HostMigrationSigningState state)
    {
      switch (state)
      {
        case HostMigrationSigningState.Preparing:
          return "Preparing";
        case HostMigrationSigningState.Migrating:
          return "Migrating";
        case HostMigrationSigningState.CleaningUp:
          return "CleaningUp";
        default:
          throw new ArgumentException(string.Format("Cannot convert {0} to a string value", (object) state));
      }
    }

    public static HostMigrationSigningState RegistryStringToHostMigrationSigningState(
      string state,
      string registryPath)
    {
      switch (state)
      {
        case "Preparing":
          return HostMigrationSigningState.Preparing;
        case "Migrating":
          return HostMigrationSigningState.Migrating;
        case "CleaningUp":
          return HostMigrationSigningState.CleaningUp;
        default:
          throw new FormatException("Cannot convert registry value " + state + " at path " + registryPath + " to HostMigrationSigningState");
      }
    }

    public static void SetDefaultMigrationCertificate(
      IVssRequestContext requestContext,
      string thumbprint)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IVssRegistryService>().SetValue<string>(vssRequestContext, FrameworkServerConstants.MigrationDefaultSigningKeyPath, thumbprint);
    }

    public static string GetDefaultMigrationCertificate(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<ISqlRegistryService>().GetValue<string>(vssRequestContext, (RegistryQuery) FrameworkServerConstants.MigrationDefaultSigningKeyPath, false, (string) null);
    }
  }
}
