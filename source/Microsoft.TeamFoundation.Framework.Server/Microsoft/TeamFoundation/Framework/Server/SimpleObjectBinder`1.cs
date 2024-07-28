// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SimpleObjectBinder`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SimpleObjectBinder<T> : ObjectBinder<T>
  {
    private readonly System.Func<IDataReader, T> m_binder;

    public SimpleObjectBinder(System.Func<IDataReader, T> binder)
    {
      ArgumentUtility.CheckForNull<System.Func<IDataReader, T>>(binder, nameof (binder));
      this.m_binder = binder;
    }

    protected override T Bind() => this.m_binder((IDataReader) this.Reader);
  }
}
