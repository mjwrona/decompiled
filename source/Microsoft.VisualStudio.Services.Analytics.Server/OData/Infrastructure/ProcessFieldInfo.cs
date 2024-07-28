// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.ProcessFieldInfo
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public class ProcessFieldInfo
  {
    private static readonly Dictionary<string, Type> _typeMappings = new Dictionary<string, Type>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase)
    {
      {
        "String",
        typeof (string)
      },
      {
        "Integer",
        typeof (long?)
      },
      {
        "Double",
        typeof (double?)
      },
      {
        "DateTime",
        typeof (DateTimeOffset?)
      },
      {
        "Boolean",
        typeof (bool?)
      }
    };
    private static readonly Dictionary<string, EdmPrimitiveTypeKind> _edmTypeMappings = new Dictionary<string, EdmPrimitiveTypeKind>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase)
    {
      {
        "String",
        EdmPrimitiveTypeKind.String
      },
      {
        "Integer",
        EdmPrimitiveTypeKind.Int64
      },
      {
        "Double",
        EdmPrimitiveTypeKind.Double
      },
      {
        "DateTime",
        EdmPrimitiveTypeKind.DateTimeOffset
      },
      {
        "Boolean",
        EdmPrimitiveTypeKind.Boolean
      },
      {
        "Identity",
        EdmPrimitiveTypeKind.Guid
      }
    };
    private static readonly Regex ValidPropertyNameRegex = new Regex("[^\\w\\s\\.]", RegexOptions.Compiled);
    private static readonly Regex ValidatePropertyNamePrefixRegex = new Regex("^(\\d+)", RegexOptions.Compiled);
    private static readonly Regex _extractTableLevel = new Regex("(\\d+)$", RegexOptions.Compiled);
    private string _name;

    public string ReferenceName { get; set; }

    public string PropertyName
    {
      get
      {
        if (this.ReferenceName == null)
          return (string) null;
        string input = ProcessFieldInfo.ValidatePropertyNamePrefixRegex.Replace(this.ReferenceName, "_$1");
        return ProcessFieldInfo.ValidPropertyNameRegex.Replace(input, new MatchEvaluator(this.TextToReplace)).Replace('.', '_');
      }
    }

    private string TextToReplace(Match m)
    {
      string replace = (string) null;
      foreach (int num in m.Value)
        replace = "__" + num.ToString("X4");
      return replace;
    }

    public Type ClrType
    {
      get
      {
        Type type;
        return ProcessFieldInfo._typeMappings.TryGetValue(this.FieldType, out type) ? type : (Type) null;
      }
    }

    public EdmPrimitiveTypeKind EdmType
    {
      get
      {
        EdmPrimitiveTypeKind edmType;
        if (ProcessFieldInfo._edmTypeMappings.TryGetValue(this.FieldType, out edmType))
          return edmType;
        throw new NotSupportedException(AnalyticsResources.UNSUPPORTED_CUSTOM_FIELD((object) this.FieldType));
      }
    }

    public string Name
    {
      get => this._name ?? this.ReferenceName;
      set => this._name = value;
    }

    public string Description { get; set; }

    public string FieldType { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsSystem { get; set; }

    public string TableName { get; set; }

    public string ColumnName { get; set; }

    public bool IsHistoryEnabled { get; set; }

    public bool IsPerson { get; set; }

    public string SourceFieldName { get; set; }

    public string SourceKeyFieldName { get; set; }

    public int TableLevel
    {
      get
      {
        int result;
        return this.TableName == null || !int.TryParse(ProcessFieldInfo._extractTableLevel.Match(this.TableName).Value, out result) ? 0 : result;
      }
    }

    public override bool Equals(object obj) => obj is ProcessFieldInfo processFieldInfo && processFieldInfo.ReferenceName == this.ReferenceName && this.Name == this.Name && this.FieldType == this.FieldType && this.ColumnName == this.ColumnName;

    public override int GetHashCode() => (this.ReferenceName, this.Name, this.FieldType, this.ColumnName).GetHashCode();
  }
}
