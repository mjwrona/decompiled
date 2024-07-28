// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.Orchestration.InMemoryASTableRetriever
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure.Orchestration
{
  internal class InMemoryASTableRetriever : IASTableRetriever
  {
    internal IDictionary<string, SQLTableData> data = (IDictionary<string, SQLTableData>) new Dictionary<string, SQLTableData>();

    public InMemoryASTableRetriever(string experienceName)
    {
    }

    public List<string> ListPrimaryKeys(int total, string startExclusive) => this.data.Keys.OrderBy<string, string>((Func<string, string>) (s => s)).Where<string>((Func<string, bool>) (s => string.CompareOrdinal(s, startExclusive) > 0)).Take<string>(total).ToList<string>();

    public IEnumerable<SqlTableEntity> RetrieveEntities(string pk) => SqlDataConverter.FromTableData(this.data[pk]);
  }
}
