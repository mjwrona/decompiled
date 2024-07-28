// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.SQLTableData
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  internal class SQLTableData
  {
    internal SQLTableData(List<SqlEntity> entities, List<SqlProperty> props)
    {
      this.Entities = entities ?? new List<SqlEntity>();
      this.Properties = props ?? new List<SqlProperty>();
    }

    internal List<SqlEntity> Entities { get; private set; }

    internal List<SqlProperty> Properties { get; private set; }
  }
}
