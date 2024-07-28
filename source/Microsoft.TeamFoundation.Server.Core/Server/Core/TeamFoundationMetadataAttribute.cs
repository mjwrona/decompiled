// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationMetadataAttribute
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;

namespace Microsoft.TeamFoundation.Server.Core
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
  public class TeamFoundationMetadataAttribute : Attribute
  {
    public TeamFoundationMetadataAttribute(short ordinal, string sqlType, string columnName)
      : this(ordinal, sqlType, columnName, (short) 0)
    {
    }

    public TeamFoundationMetadataAttribute(
      short ordinal,
      string sqlType,
      string columnName,
      short maxLength)
    {
      this.SqlType = sqlType;
      this.Ordinal = ordinal;
      this.ColumnName = columnName;
      this.MaxLength = maxLength;
    }

    public TeamFoundationMetadataAttribute(bool ignore) => this.Ignore = ignore;

    public TeamFoundationMetadataAttribute(string typeName) => this.TypeName = typeName;

    public string SqlType { get; private set; }

    public short MaxLength { get; private set; }

    public short Ordinal { get; private set; }

    public string ColumnName { get; private set; }

    public bool Ignore { get; private set; }

    public string TypeName { get; private set; }
  }
}
