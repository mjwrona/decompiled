// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.SequenceIdColumns
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class SequenceIdColumns : ObjectBinder<int>
  {
    private SqlColumnBinder sequenceId = new SqlColumnBinder("node_sequence_id");

    protected override int Bind() => this.sequenceId.GetInt32((IDataReader) this.Reader);
  }
}
