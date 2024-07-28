// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.RaiseEventBinder
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [ExcludeFromCodeCoverage]
  internal class RaiseEventBinder : ObjectBinder<bool>
  {
    private SqlColumnBinder RaiseEventColumn = new SqlColumnBinder("RaiseEvent");

    protected override bool Bind() => this.RaiseEventColumn.GetBoolean((IDataReader) this.Reader);
  }
}
