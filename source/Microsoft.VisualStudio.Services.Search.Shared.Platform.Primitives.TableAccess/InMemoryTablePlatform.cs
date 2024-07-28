// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.InMemoryTablePlatform
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  public sealed class InMemoryTablePlatform : TableAccessPlatform
  {
    private Dictionary<string, object> m_tableDictionary = new Dictionary<string, object>();

    public override ITable<T> GetTable<T>(IVssRequestContext requestContext)
    {
      string fullName = typeof (T).FullName;
      if (!this.m_tableDictionary.ContainsKey(fullName))
        this.m_tableDictionary[fullName] = (object) new TableSimulator<T>();
      return (ITable<T>) this.m_tableDictionary[fullName];
    }
  }
}
