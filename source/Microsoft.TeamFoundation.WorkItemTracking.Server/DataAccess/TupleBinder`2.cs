// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.TupleBinder`2
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class TupleBinder<T1, T2> : ObjectBinder<Tuple<T1, T2>>
  {
    protected IDataReader Reader => this.BaseReader;

    protected override Tuple<T1, T2> Bind() => new Tuple<T1, T2>((T1) this.Reader.GetValue(0), (T2) this.Reader.GetValue(1));
  }
}
