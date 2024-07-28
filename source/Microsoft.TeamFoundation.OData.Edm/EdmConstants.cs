// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmConstants
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;

namespace Microsoft.OData.Edm
{
  public static class EdmConstants
  {
    public static readonly Version EdmVersion4 = new Version(4, 0);
    public static readonly Version EdmVersion401 = new Version(4, 1);
    public static readonly Version EdmVersionLatest = EdmConstants.EdmVersion401;
    public static Version EdmVersionDefault = EdmConstants.EdmVersion4;
    internal const string EdmVersion401String = "4.01";
    internal const string EdmNamespace = "Edm";
    internal const string TransientNamespace = "Transient";
    internal const string XmlPrefix = "xml";
    internal const string XmlNamespacePrefix = "xmlns";
    internal const string InternalUri = "http://schemas.microsoft.com/ado/2011/04/edm/internal";
    internal const string EdmVersionAnnotation = "EdmVersion";
    internal const string FacetName_Nullable = "Nullable";
    internal const string FacetName_Precision = "Precision";
    internal const string FacetName_Scale = "Scale";
    internal const string FacetName_MaxLength = "MaxLength";
    internal const string FacetName_Unicode = "Unicode";
    internal const string FacetName_Collation = "Collation";
    internal const string FacetName_Srid = "SRID";
    internal const string Value_UnknownType = "UnknownType";
    internal const string Value_UnnamedType = "UnnamedType";
    internal const string Value_Max = "max";
    internal const string Value_SridVariable = "Variable";
    internal const string Value_ScaleVariable = "Variable";
    internal const string Type_Collection = "Collection";
    internal const string Type_Complex = "Complex";
    internal const string Type_Entity = "Entity";
    internal const string Type_EntityReference = "EntityReference";
    internal const string Type_Enum = "Enum";
    internal const string Type_TypeDefinition = "TypeDefinition";
    internal const string Type_Path = "Path";
    internal const string Type_Primitive = "Primitive";
    internal const string Type_Binary = "Binary";
    internal const string Type_Decimal = "Decimal";
    internal const string Type_String = "String";
    internal const string Type_Stream = "Stream";
    internal const string Type_Spatial = "Spatial";
    internal const string Type_Temporal = "Temporal";
    internal const string Type_Structured = "Structured";
    internal const int Max_Precision = 2147483647;
    internal const int Min_Precision = 0;
    internal const string MimeTypeAttributeName = "MimeType";
  }
}
