// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.TableAccessPlatform
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using System;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  public class TableAccessPlatform : ITableAccessPlatform
  {
    public virtual ITable<T> GetTable<T>(IVssRequestContext requestContext) => this.CreateTable<T>(requestContext);

    private ITable<T> CreateTable<T>(IVssRequestContext requestContext)
    {
      if (typeof (T) == typeof (DisabledFile))
        return (ITable<T>) requestContext.CreateComponent<DisabledFilesTable>();
      if (typeof (T) == typeof (ReindexingStatusEntry))
        return (ITable<T>) requestContext.CreateComponent<ReindexingStatusTable>();
      if (typeof (T) == typeof (ClassificationNode))
        return (ITable<T>) requestContext.CreateComponent<ClassificationNodeComponent>();
      throw new NotImplementedException();
    }
  }
}
