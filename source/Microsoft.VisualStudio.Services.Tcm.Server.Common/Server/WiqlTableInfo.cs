// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.WiqlTableInfo
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class WiqlTableInfo
  {
    private string m_wiqlTableName;
    private string m_sqlTableName;
    private Type m_objectType;
    private Dictionary<string, WiqlFieldInfo> m_fields;

    internal WiqlTableInfo(Type objectType, string sqlTableName, string wiqlTableName)
    {
      this.m_objectType = objectType;
      this.m_sqlTableName = sqlTableName;
      this.m_wiqlTableName = wiqlTableName;
      this.m_fields = new Dictionary<string, WiqlFieldInfo>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (PropertyInfo property in objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        QueryMappingAttribute customAttribute = (QueryMappingAttribute) Attribute.GetCustomAttribute((MemberInfo) property, typeof (QueryMappingAttribute));
        if (customAttribute != null)
        {
          string str = customAttribute.WiqlFieldName ?? property.Name;
          string sqlFieldName = customAttribute.SqlFieldName ?? str;
          DataType dataType = customAttribute.DataType == DataType.Unknown ? (!(customAttribute.EnumType != (Type) null) ? WiqlTableInfo.DetectDataType(property) : DataType.String) : customAttribute.DataType;
          TcmTrace.TraceAndDebugAssert("QueryEngine", !this.m_fields.ContainsKey(str), "Duplicate field name " + str);
          this.m_fields.Add(str, new WiqlFieldInfo(str, sqlFieldName, dataType, customAttribute.EnumType));
        }
      }
    }

    internal Dictionary<string, WiqlFieldInfo> Fields => this.m_fields;

    internal string WiqlTableName => this.m_wiqlTableName;

    internal string SqlTableName => this.m_sqlTableName;

    internal Type ObjectType => this.m_objectType;

    private static DataType DetectDataType(PropertyInfo property)
    {
      if (property.PropertyType == typeof (string) || property.PropertyType == typeof (Guid))
        return DataType.String;
      if (property.PropertyType == typeof (bool))
        return DataType.Bool;
      if (property.PropertyType == typeof (DateTime))
        return DataType.Date;
      if (property.PropertyType == typeof (byte) || property.PropertyType == typeof (int) || property.PropertyType == typeof (long))
        return DataType.Numeric;
      TcmTrace.TraceAndDebugFail("QueryEngine", "Cannot auto-detect data type");
      throw new NotSupportedException();
    }
  }
}
