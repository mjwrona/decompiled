// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.GalleryAuditLogComponent
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class GalleryAuditLogComponent : TeamFoundationSqlResourceComponent
  {
    [StaticSafe]
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    private const string s_area = "GalleryExtensionAuditLogComponent";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<GalleryAuditLogComponent>(1),
      (IComponentCreator) new ComponentCreator<GalleryAuditLogComponent2>(2),
      (IComponentCreator) new ComponentCreator<GalleryAuditLogComponent3>(3)
    }, "GalleryAuditLog");

    static GalleryAuditLogComponent() => GalleryAuditLogComponent.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) GalleryAuditLogComponent.s_sqlExceptionFactories;

    protected override string TraceArea => "GalleryExtensionAuditLogComponent";

    public virtual void LogAuditEntry(
      Guid changedBy,
      string auditAction,
      string resourceId,
      string resourceType,
      string data)
    {
      try
      {
        this.TraceEnter(12061036, "Enter LogAuditEntry");
        ArgumentUtility.CheckStringForNullOrEmpty(resourceType, "resourceName");
        ArgumentUtility.CheckStringForNullOrEmpty(resourceId, nameof (resourceId));
        ArgumentUtility.CheckStringForNullOrEmpty(auditAction, "AuditAction");
        this.PrepareStoredProcedure("Gallery.prc_AddGalleryAuditLog");
        this.BindGuid("ChangedBy", changedBy);
        this.BindString("AuditAction", auditAction, 100, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
        this.BindString("ResourceId", resourceId, 100, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
        this.BindString("ResourceType", resourceType, 100, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
        this.BindString("Data", data, 2048, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.Trace(12061038, TraceLevel.Info, string.Format("InsertAudit row where changedBy={0}, AuditAction={1}, ResourceId={2}, ResourceName={3}, data={4}", (object) changedBy, (object) auditAction, (object) resourceId, (object) resourceType, data == null ? (object) string.Empty : (object) data));
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(12061038, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(12061037, "Leave LogAuditEntry");
      }
    }

    public virtual void CleanUpAuditLog(int noOfDays) => throw new NotImplementedException();
  }
}
