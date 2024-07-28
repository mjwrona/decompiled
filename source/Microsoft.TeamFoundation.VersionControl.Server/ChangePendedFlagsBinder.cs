// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ChangePendedFlagsBinder
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ChangePendedFlagsBinder : VersionControlObjectBinder<ChangePendedFlags>
  {
    private SqlColumnBinder flags = new SqlColumnBinder("Flags");

    protected override ChangePendedFlags Bind()
    {
      ChangePendedFlags int32 = (ChangePendedFlags) this.flags.GetInt32((IDataReader) this.Reader, 0);
      return int32 == ChangePendedFlags.Unknown ? ChangePendedFlags.None : int32;
    }
  }
}
