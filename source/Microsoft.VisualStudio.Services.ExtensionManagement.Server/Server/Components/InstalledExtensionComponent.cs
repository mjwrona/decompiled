// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components.InstalledExtensionComponent
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components
{
  internal class InstalledExtensionComponent : TeamFoundationSqlResourceComponent
  {
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        1660007,
        new SqlExceptionFactory(typeof (ExtensionAlreadyInstalledException))
      }
    };
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[8]
    {
      (IComponentCreator) new ComponentCreator<InstalledExtensionComponent>(1),
      (IComponentCreator) new ComponentCreator<InstalledExtensionComponent2>(2),
      (IComponentCreator) new ComponentCreator<InstalledExtensionComponent3>(3),
      (IComponentCreator) new ComponentCreator<InstalledExtensionComponent4>(4),
      (IComponentCreator) new ComponentCreator<InstalledExtensionComponent5>(5),
      (IComponentCreator) new ComponentCreator<InstalledExtensionComponent6>(6),
      (IComponentCreator) new ComponentCreator<InstalledExtensionComponent7>(7),
      (IComponentCreator) new ComponentCreator<InstalledExtensionComponent8>(8)
    }, "InstalledExtensions");
    private const string s_area = "InstalledExtensionComponent";

    public InstalledExtensionComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    protected override string TraceArea => nameof (InstalledExtensionComponent);

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) InstalledExtensionComponent.s_sqlExceptionFactories;

    public virtual void InstallExtensions(IEnumerable<ExtensionState> states)
    {
    }

    public virtual ExtensionState InstallExtension(
      string version,
      ExtensionStateFlags flags,
      DateTime? lastVersionCheck,
      string publisherName,
      string extensionName,
      bool failIfInstalled,
      Guid installedBy,
      out string previousVersion,
      out ExtensionStateFlags? previousFlags)
    {
      this.PrepareStoredProcedure("Extension.prc_UpsertInstalledExtension");
      this.BindGuid("@extensionId", Guid.NewGuid());
      this.BindString("@version", version, 25, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindInt("@flags", (int) flags);
      this.BindNullableDateTime("@lastVersionCheck", lastVersionCheck);
      if (this is InstalledExtensionComponent2)
      {
        this.BindString("@publisherName", publisherName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindString("@extensionName", extensionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      }
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Extension.prc_UpsertInstalledExtension", this.RequestContext);
      resultCollection.AddBinder<ExtensionState>((ObjectBinder<ExtensionState>) this.GetInstalledExtensionColumns());
      previousVersion = (string) null;
      previousFlags = new ExtensionStateFlags?();
      return resultCollection.GetCurrent<ExtensionState>().Items[0];
    }

    public virtual void UninstallExtension(
      string publisherName = null,
      string extensionName = null,
      Guid uninstalledBy = default (Guid))
    {
      try
      {
        this.TraceEnter(70210, nameof (UninstallExtension));
        this.PrepareStoredProcedure("Extension.prc_UninstallExtension");
        this.BindGuid("@extensionId", Guid.NewGuid());
        if (this is InstalledExtensionComponent2)
        {
          this.BindString("@publisherName", publisherName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
          this.BindString("@extensionName", extensionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        }
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(70215, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(70220, nameof (UninstallExtension));
      }
    }

    public virtual ResultCollection QueryInstalledExtensions()
    {
      try
      {
        this.TraceEnter(70210, nameof (QueryInstalledExtensions));
        this.PrepareStoredProcedure("Extension.prc_QueryInstalledExtensions");
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<ExtensionState>((ObjectBinder<ExtensionState>) this.GetInstalledExtensionColumns());
        return resultCollection;
      }
      catch (Exception ex)
      {
        this.TraceException(70215, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(70220, nameof (QueryInstalledExtensions));
      }
    }

    public virtual void DeleteExtensionStates(List<ExtensionState> extensionStates)
    {
    }

    protected virtual InstalledExtensionColumns GetInstalledExtensionColumns() => new InstalledExtensionColumns();
  }
}
