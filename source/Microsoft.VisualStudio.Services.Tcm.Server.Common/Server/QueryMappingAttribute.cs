// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.QueryMappingAttribute
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  internal sealed class QueryMappingAttribute : Attribute
  {
    private string m_wiqlFieldName;
    private string m_sqlFieldName;
    private DataType m_dataType;
    private Type m_enumType;

    internal QueryMappingAttribute()
      : this((string) null, (string) null, DataType.Unknown)
    {
    }

    internal QueryMappingAttribute(string wiqlFieldName, string sqlFieldName, DataType dataType)
    {
      this.m_wiqlFieldName = wiqlFieldName;
      this.m_sqlFieldName = sqlFieldName;
      this.m_dataType = dataType;
    }

    public DataType DataType
    {
      get => this.m_dataType;
      set => this.m_dataType = value;
    }

    public string SqlFieldName
    {
      get => this.m_sqlFieldName;
      set => this.m_sqlFieldName = value;
    }

    public string WiqlFieldName
    {
      get => this.m_wiqlFieldName;
      set => this.m_wiqlFieldName = value;
    }

    public Type EnumType
    {
      get => this.m_enumType;
      set => this.m_enumType = value;
    }
  }
}
