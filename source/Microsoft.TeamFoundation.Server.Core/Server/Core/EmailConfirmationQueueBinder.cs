// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.EmailConfirmationQueueBinder
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class EmailConfirmationQueueBinder : ObjectBinder<PreferredEmailConfirmationEntry>
  {
    private SqlColumnBinder TfIdColumn = new SqlColumnBinder("TfId");
    private SqlColumnBinder PreferredEmailAddressColumn = new SqlColumnBinder("PreferredEmailAddress");

    protected override PreferredEmailConfirmationEntry Bind() => new PreferredEmailConfirmationEntry(this.TfIdColumn.GetGuid((IDataReader) this.Reader), this.PreferredEmailAddressColumn.GetString((IDataReader) this.Reader, true));
  }
}
