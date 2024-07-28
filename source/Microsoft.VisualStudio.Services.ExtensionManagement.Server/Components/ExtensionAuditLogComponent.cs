// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Components.ExtensionAuditLogComponent
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Components
{
  internal class ExtensionAuditLogComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<ExtensionAuditLogComponent>(1)
    }, "ExtensionAuditLog");
    private const string s_area = "ExtensionAuditLogComponent";

    public ExtensionAuditLogComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    protected override string TraceArea => nameof (ExtensionAuditLogComponent);

    public virtual ExtensionAuditLog GetExtensionAuditLog(
      string publisherName,
      string extensionName)
    {
      try
      {
        this.TraceEnter(70210, nameof (GetExtensionAuditLog));
        this.PrepareStoredProcedure("Extension.prc_GetExtensionAuditLog");
        this.BindString("@publisherName", publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
        this.BindString("@extensionName", extensionName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Extension.prc_GetExtensionAuditLog", this.RequestContext))
        {
          resultCollection.AddBinder<ExtensionAuditLogEntry>((ObjectBinder<ExtensionAuditLogEntry>) this.GetExtensionAuditLogColumns());
          return this.ProcessAuditLogEntries((IEnumerable<ExtensionAuditLogEntry>) resultCollection.GetCurrent<ExtensionAuditLogEntry>()).FirstOrDefault<ExtensionAuditLog>();
        }
      }
      catch (Exception ex)
      {
        this.TraceException(70215, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(70220, nameof (GetExtensionAuditLog));
      }
    }

    protected List<ExtensionAuditLog> ProcessAuditLogEntries(
      IEnumerable<ExtensionAuditLogEntry> entries)
    {
      Dictionary<string, ExtensionAuditLog> dictionary = new Dictionary<string, ExtensionAuditLog>();
      foreach (ExtensionAuditLogEntry entry in entries)
      {
        string key = string.Format("{0}.{1}", (object) entry.PublisherName, (object) entry.ExtensionName);
        ExtensionAuditLog extensionAuditLog;
        if (!dictionary.TryGetValue(key, out extensionAuditLog))
        {
          extensionAuditLog = new ExtensionAuditLog();
          extensionAuditLog.PublisherName = entry.PublisherName;
          extensionAuditLog.ExtensionName = entry.ExtensionName;
          dictionary.Add(key, extensionAuditLog);
        }
        extensionAuditLog.Entries.Add(entry);
      }
      return dictionary.Values.ToList<ExtensionAuditLog>();
    }

    protected virtual ExtensionAuditLogColumns GetExtensionAuditLogColumns() => new ExtensionAuditLogColumns(this.RequestContext);
  }
}
