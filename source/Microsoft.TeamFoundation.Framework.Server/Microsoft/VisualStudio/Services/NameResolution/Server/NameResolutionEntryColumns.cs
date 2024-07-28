// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NameResolution.Server.NameResolutionEntryColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.NameResolution.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class NameResolutionEntryColumns : ObjectBinder<NameResolutionEntry>
  {
    private SqlColumnBinder NamespaceColumn = new SqlColumnBinder("Namespace");
    private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder ValueColumn = new SqlColumnBinder("Value");
    private SqlColumnBinder IsPrimaryColumn = new SqlColumnBinder("IsPrimary");
    private SqlColumnBinder IsEnabledColumn = new SqlColumnBinder("IsEnabled");
    private SqlColumnBinder TTLColumn = new SqlColumnBinder("TTL");
    private SqlColumnBinder RevisionColumn = new SqlColumnBinder("Revision");

    protected override NameResolutionEntry Bind() => this.Bind(this.Reader);

    internal virtual NameResolutionEntry Bind(SqlDataReader reader)
    {
      NameResolutionEntry nameResolutionEntry = new NameResolutionEntry()
      {
        Namespace = string.Intern(this.NamespaceColumn.GetString((IDataReader) reader, false)),
        Name = this.NameColumn.GetString((IDataReader) reader, false),
        Value = this.ValueColumn.GetGuid((IDataReader) reader, true)
      };
      if (reader.FieldCount > 3)
      {
        nameResolutionEntry.IsPrimary = this.IsPrimaryColumn.GetBoolean((IDataReader) reader);
        nameResolutionEntry.IsEnabled = this.IsEnabledColumn.GetBoolean((IDataReader) reader);
        nameResolutionEntry.TTL = this.TTLColumn.IsNull((IDataReader) reader) ? new int?() : new int?(this.TTLColumn.GetInt32((IDataReader) reader));
        nameResolutionEntry.Revision = this.RevisionColumn.GetInt32((IDataReader) reader);
      }
      return nameResolutionEntry;
    }
  }
}
