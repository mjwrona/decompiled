// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.SqlOperationData
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  internal class SqlOperationData : SQLTableData
  {
    internal SqlOperationData(
      List<SqlOperation> operations,
      List<SqlEntity> entities,
      List<SqlProperty> props)
      : base(entities, props)
    {
      this.Operations = operations ?? new List<SqlOperation>();
      this.Count = this.Operations.Count;
    }

    internal int Count { get; private set; }

    internal List<SqlOperation> Operations { get; private set; }
  }
}
