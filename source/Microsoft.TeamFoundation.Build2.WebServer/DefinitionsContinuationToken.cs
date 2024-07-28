// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.DefinitionsContinuationToken
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using System;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  public class DefinitionsContinuationToken
  {
    private DefinitionsContinuationToken(DateTime createdTime) => this.LastModifiedTime = new DateTime?(createdTime);

    private DefinitionsContinuationToken(string name) => this.Name = name;

    public DefinitionsContinuationToken(
      string nextDefinitionName,
      DateTime? nextDefinitionCreatedDate,
      Microsoft.TeamFoundation.Build2.Server.DefinitionQueryOrder queryOrder)
    {
      if (queryOrder == Microsoft.TeamFoundation.Build2.Server.DefinitionQueryOrder.DefinitionNameAscending || queryOrder == Microsoft.TeamFoundation.Build2.Server.DefinitionQueryOrder.DefinitionNameDescending)
        this.Name = nextDefinitionName;
      else
        this.LastModifiedTime = nextDefinitionCreatedDate;
    }

    public DateTime? LastModifiedTime { get; private set; }

    public string Name { get; private set; }

    public override string ToString()
    {
      if (this.LastModifiedTime.HasValue)
        return this.LastModifiedTime.Value.ToString("o");
      return !string.IsNullOrEmpty(this.Name) ? this.Name : string.Empty;
    }

    public static bool TryParse(
      string value,
      Microsoft.TeamFoundation.Build.WebApi.DefinitionQueryOrder queryOrder,
      out DefinitionsContinuationToken token)
    {
      token = (DefinitionsContinuationToken) null;
      if (!string.IsNullOrEmpty(value))
      {
        switch (queryOrder)
        {
          case Microsoft.TeamFoundation.Build.WebApi.DefinitionQueryOrder.LastModifiedAscending:
          case Microsoft.TeamFoundation.Build.WebApi.DefinitionQueryOrder.LastModifiedDescending:
            DateTime result;
            if (!DateTime.TryParse(value, out result))
              return false;
            token = new DefinitionsContinuationToken(result);
            return true;
          case Microsoft.TeamFoundation.Build.WebApi.DefinitionQueryOrder.DefinitionNameAscending:
          case Microsoft.TeamFoundation.Build.WebApi.DefinitionQueryOrder.DefinitionNameDescending:
            token = new DefinitionsContinuationToken(value);
            return true;
        }
      }
      return false;
    }
  }
}
