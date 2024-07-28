// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.RegistryMigrator
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class RegistryMigrator
  {
    private readonly IVssRequestContext m_rc;

    public RegistryMigrator(IVssRequestContext rc) => this.m_rc = rc;

    public string Migrate(Guid sourceRepo, Guid targetRepo)
    {
      int num = 0;
      List<RegistryItem> items = new List<RegistryItem>();
      IVssRegistryService service = this.m_rc.GetService<IVssRegistryService>();
      foreach (RegistryItem registryItem in service.Read(this.m_rc, new RegistryQuery(string.Format("{0}/**/{1}", (object) "/Service/Git", (object) sourceRepo))))
      {
        string[] source = registryItem.Path.Split('/');
        string str = string.Format("{0}/{1}", (object) string.Join("/", ((IEnumerable<string>) source).Take<string>(source.Length - 1)), (object) targetRepo);
        if (service.Read(this.m_rc, new RegistryQuery(str)).Count<RegistryItem>() == 0)
          items.Add(new RegistryItem(str, registryItem.Value));
        else
          ++num;
      }
      if (items.Count > 0)
        service.Write(this.m_rc, (IEnumerable<RegistryItem>) items);
      return string.Format("Copied {0} registry items. Skipped {1} duplicates.", (object) items.Count, (object) num);
    }
  }
}
