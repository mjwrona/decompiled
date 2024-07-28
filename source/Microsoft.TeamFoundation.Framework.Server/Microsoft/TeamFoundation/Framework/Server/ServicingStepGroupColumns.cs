// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingStepGroupColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingStepGroupColumns : ObjectBinder<ServicingStepGroup>
  {
    private SqlColumnBinder m_nameColumn = new SqlColumnBinder("GroupName");
    private SqlColumnBinder m_handlersColumn = new SqlColumnBinder("Handlers");

    protected override ServicingStepGroup Bind()
    {
      ServicingStepGroup servicingStepGroup = new ServicingStepGroup(this.m_nameColumn.GetString((IDataReader) this.Reader, false), (List<ServicingStep>) null);
      string str1 = this.m_handlersColumn.GetString((IDataReader) this.Reader, true);
      if (!string.IsNullOrEmpty(str1))
      {
        string str2 = str1;
        string[] separator = new string[1]{ ";" };
        foreach (string str3 in str2.Split(separator, StringSplitOptions.RemoveEmptyEntries))
          servicingStepGroup.ExecutionHandlers.Add(new ServicingExecutionHandlerData()
          {
            HandlerType = str3
          });
      }
      return servicingStepGroup;
    }
  }
}
