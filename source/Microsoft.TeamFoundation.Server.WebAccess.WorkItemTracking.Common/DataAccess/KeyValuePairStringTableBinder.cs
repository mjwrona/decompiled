// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.KeyValuePairStringTableBinder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class KeyValuePairStringTableBinder : ObjectBinder<KeyValuePair<string, string>>
  {
    private SqlColumnBinder KeyColumn = new SqlColumnBinder("Key");
    private SqlColumnBinder ValueColumn = new SqlColumnBinder("Value");

    protected override KeyValuePair<string, string> Bind() => new KeyValuePair<string, string>(this.KeyColumn.GetString((IDataReader) this.Reader, false), this.ValueColumn.GetString((IDataReader) this.Reader, false));
  }
}
