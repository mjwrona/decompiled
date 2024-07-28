// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.SchemasHrSchema
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas
{
  internal static class SchemasHrSchema
  {
    public static readonly Namespace Namespace = SchemasHrSchema.CreateSchema();
    public static readonly LayoutResolver LayoutResolver = SchemasHrSchema.LoadSchema();

    private static Namespace CreateSchema()
    {
      Namespace schema1 = new Namespace();
      schema1.Name = "Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas";
      schema1.Version = SchemaLanguageVersion.V2;
      schema1.CppNamespace = "cdb_hr";
      schema1.Enums = new List<EnumSchema>()
      {
        new EnumSchema()
        {
          Name = "SchemaLanguageVersion",
          Comment = "Versions of the HybridRow Schema Description Language.",
          Type = TypeKind.UInt8,
          Values = new List<EnumValue>()
          {
            new EnumValue()
            {
              Name = "V1",
              Comment = "Initial version of the HybridRow Schema Description Language.",
              Value = 0L
            },
            new EnumValue()
            {
              Name = "V2",
              Comment = "Introduced Enums, Inheritance.",
              Value = 2L
            },
            new EnumValue()
            {
              Name = "Unspecified",
              Comment = "No version is specified.",
              Value = (long) byte.MaxValue
            }
          }
        },
        new EnumSchema()
        {
          Name = "TypeKind",
          Comment = "Describes the logical type of a property.",
          Type = TypeKind.UInt8
        },
        new EnumSchema()
        {
          Name = "StorageKind",
          Comment = "Describes the storage placement for primitive properties.",
          Type = TypeKind.UInt8
        },
        new EnumSchema()
        {
          Name = "SortDirection",
          Comment = "Describes the sort order direction.",
          Type = TypeKind.UInt8
        },
        new EnumSchema()
        {
          Name = "AllowEmptyKind",
          Comment = "Describes the empty canonicalization for properties.",
          Type = TypeKind.UInt8
        }
      };
      List<Schema> schemaList = new List<Schema>();
      schemaList.Add(new Schema()
      {
        Name = "EmptySchema",
        SchemaId = new SchemaId(2147473650),
        Options = new SchemaOptions()
      });
      Schema schema2 = new Schema();
      schema2.Name = "Segment";
      schema2.SchemaId = new SchemaId(2147473648);
      schema2.Options = new SchemaOptions();
      List<Property> propertyList1 = new List<Property>();
      Property property1 = new Property();
      property1.Path = "length";
      PrimitivePropertyType primitivePropertyType1 = new PrimitivePropertyType();
      primitivePropertyType1.Type = TypeKind.Int32;
      primitivePropertyType1.Storage = StorageKind.Fixed;
      primitivePropertyType1.RowBufferSize = true;
      property1.PropertyType = (PropertyType) primitivePropertyType1;
      property1.Comment = "(Required) length (in bytes) of this RecordIO segment header itself.  Does NOT include the length of the records that follow.";
      propertyList1.Add(property1);
      Property property2 = new Property();
      property2.Path = "comment";
      PrimitivePropertyType primitivePropertyType2 = new PrimitivePropertyType();
      primitivePropertyType2.Type = TypeKind.Utf8;
      property2.PropertyType = (PropertyType) primitivePropertyType2;
      property2.Comment = "A comment describing the data in this RecordIO segment.";
      propertyList1.Add(property2);
      Property property3 = new Property();
      property3.Path = "sdl";
      PrimitivePropertyType primitivePropertyType3 = new PrimitivePropertyType();
      primitivePropertyType3.Type = TypeKind.Utf8;
      property3.PropertyType = (PropertyType) primitivePropertyType3;
      property3.Comment = "A HybridRow Schema in SDL (json-format).";
      property3.ApiName = "SDL";
      propertyList1.Add(property3);
      propertyList1.Add(new Property()
      {
        Path = "schema",
        PropertyType = (PropertyType) new UdtPropertyType()
        {
          Name = "Namespace",
          SchemaId = new SchemaId(2147473651)
        },
        Comment = "A HybridRow Schema."
      });
      schema2.Properties = propertyList1;
      schemaList.Add(schema2);
      Schema schema3 = new Schema();
      schema3.Name = "Record";
      schema3.SchemaId = new SchemaId(2147473649);
      schema3.Options = new SchemaOptions()
      {
        DisallowUnschematized = true
      };
      List<Property> propertyList2 = new List<Property>();
      Property property4 = new Property();
      property4.Path = "length";
      PrimitivePropertyType primitivePropertyType4 = new PrimitivePropertyType();
      primitivePropertyType4.Type = TypeKind.Int32;
      primitivePropertyType4.Storage = StorageKind.Fixed;
      primitivePropertyType4.Nullable = false;
      property4.PropertyType = (PropertyType) primitivePropertyType4;
      property4.Comment = "(Required) length (in bytes) of the HybridRow value that follows this record header.";
      propertyList2.Add(property4);
      Property property5 = new Property();
      property5.Path = "crc32";
      PrimitivePropertyType primitivePropertyType5 = new PrimitivePropertyType();
      primitivePropertyType5.Type = TypeKind.UInt32;
      primitivePropertyType5.Storage = StorageKind.Fixed;
      primitivePropertyType5.Nullable = false;
      property5.PropertyType = (PropertyType) primitivePropertyType5;
      property5.Comment = "(Optional) CRC-32 as described in ISO 3309.";
      propertyList2.Add(property5);
      schema3.Properties = propertyList2;
      schemaList.Add(schema3);
      Schema schema4 = new Schema();
      schema4.Name = "Namespace";
      schema4.SchemaId = new SchemaId(2147473651);
      schema4.Options = new SchemaOptions()
      {
        DisallowUnschematized = true
      };
      List<Property> propertyList3 = new List<Property>();
      Property property6 = new Property();
      property6.Path = "version";
      PrimitivePropertyType primitivePropertyType6 = new PrimitivePropertyType();
      primitivePropertyType6.Type = TypeKind.Enum;
      primitivePropertyType6.Storage = StorageKind.Fixed;
      primitivePropertyType6.Enum = "SchemaLanguageVersion";
      primitivePropertyType6.Nullable = false;
      property6.PropertyType = (PropertyType) primitivePropertyType6;
      property6.Comment = "(Required) SDL language version.";
      propertyList3.Add(property6);
      Property property7 = new Property();
      property7.Path = "name";
      PrimitivePropertyType primitivePropertyType7 = new PrimitivePropertyType();
      primitivePropertyType7.Type = TypeKind.Utf8;
      primitivePropertyType7.Storage = StorageKind.Variable;
      property7.PropertyType = (PropertyType) primitivePropertyType7;
      property7.Comment = "(Optional) Name of the namespace.";
      propertyList3.Add(property7);
      Property property8 = new Property();
      property8.Path = "comment";
      PrimitivePropertyType primitivePropertyType8 = new PrimitivePropertyType();
      primitivePropertyType8.Type = TypeKind.Utf8;
      property8.PropertyType = (PropertyType) primitivePropertyType8;
      property8.Comment = "(Optional) Comment field describing the namespace.";
      propertyList3.Add(property8);
      Property property9 = new Property();
      property9.Path = "schemas";
      property9.AllowEmpty = AllowEmptyKind.Both;
      ArrayPropertyType arrayPropertyType1 = new ArrayPropertyType();
      UdtPropertyType udtPropertyType1 = new UdtPropertyType();
      udtPropertyType1.Name = "Schema";
      udtPropertyType1.SchemaId = new SchemaId(2147473652);
      udtPropertyType1.Nullable = false;
      arrayPropertyType1.Items = (PropertyType) udtPropertyType1;
      property9.PropertyType = (PropertyType) arrayPropertyType1;
      property9.Comment = "The set of schemas that make up the namespace.";
      propertyList3.Add(property9);
      Property property10 = new Property();
      property10.Path = "enums";
      property10.AllowEmpty = AllowEmptyKind.Both;
      ArrayPropertyType arrayPropertyType2 = new ArrayPropertyType();
      UdtPropertyType udtPropertyType2 = new UdtPropertyType();
      udtPropertyType2.Name = "EnumSchema";
      udtPropertyType2.SchemaId = new SchemaId(2147473668);
      udtPropertyType2.Nullable = false;
      arrayPropertyType2.Items = (PropertyType) udtPropertyType2;
      property10.PropertyType = (PropertyType) arrayPropertyType2;
      property10.Comment = "The set of enums defined in the namespace.";
      propertyList3.Add(property10);
      Property property11 = new Property();
      property11.Path = "cppNamespace";
      PrimitivePropertyType primitivePropertyType9 = new PrimitivePropertyType();
      primitivePropertyType9.Type = TypeKind.Utf8;
      property11.PropertyType = (PropertyType) primitivePropertyType9;
      property11.Comment = "An (optional) namespace to use when performing C++ codegen.";
      propertyList3.Add(property11);
      schema4.Properties = propertyList3;
      schemaList.Add(schema4);
      Schema schema5 = new Schema();
      schema5.Name = "Schema";
      schema5.SchemaId = new SchemaId(2147473652);
      schema5.Options = new SchemaOptions()
      {
        DisallowUnschematized = true
      };
      List<Property> propertyList4 = new List<Property>();
      Property property12 = new Property();
      property12.Path = "version";
      PrimitivePropertyType primitivePropertyType10 = new PrimitivePropertyType();
      primitivePropertyType10.Type = TypeKind.Enum;
      primitivePropertyType10.Storage = StorageKind.Fixed;
      primitivePropertyType10.Enum = "SchemaLanguageVersion";
      primitivePropertyType10.Nullable = false;
      property12.PropertyType = (PropertyType) primitivePropertyType10;
      property12.Comment = "(Optional) SDL language version.";
      propertyList4.Add(property12);
      Property property13 = new Property();
      property13.Path = "type";
      PrimitivePropertyType primitivePropertyType11 = new PrimitivePropertyType();
      primitivePropertyType11.Type = TypeKind.Enum;
      primitivePropertyType11.Storage = StorageKind.Fixed;
      primitivePropertyType11.Enum = "TypeKind";
      primitivePropertyType11.Nullable = false;
      property13.PropertyType = (PropertyType) primitivePropertyType11;
      property13.Comment = "(Required) Type of the schema element.";
      propertyList4.Add(property13);
      Property property14 = new Property();
      property14.Path = "id";
      PrimitivePropertyType primitivePropertyType12 = new PrimitivePropertyType();
      primitivePropertyType12.Type = TypeKind.Int32;
      primitivePropertyType12.Storage = StorageKind.Fixed;
      primitivePropertyType12.Nullable = false;
      primitivePropertyType12.ApiType = "SchemaId";
      property14.PropertyType = (PropertyType) primitivePropertyType12;
      property14.Comment = "(Required) Globally unique id of the schema.";
      property14.ApiName = "SchemaId";
      propertyList4.Add(property14);
      Property property15 = new Property();
      property15.Path = "name";
      PrimitivePropertyType primitivePropertyType13 = new PrimitivePropertyType();
      primitivePropertyType13.Type = TypeKind.Utf8;
      primitivePropertyType13.Storage = StorageKind.Variable;
      property15.PropertyType = (PropertyType) primitivePropertyType13;
      property15.Comment = "(Optional) Name of the schema.";
      propertyList4.Add(property15);
      Property property16 = new Property();
      property16.Path = "comment";
      PrimitivePropertyType primitivePropertyType14 = new PrimitivePropertyType();
      primitivePropertyType14.Type = TypeKind.Utf8;
      property16.PropertyType = (PropertyType) primitivePropertyType14;
      property16.Comment = "(Optional) Comment field describing the schema.";
      propertyList4.Add(property16);
      propertyList4.Add(new Property()
      {
        Path = "options",
        PropertyType = (PropertyType) new UdtPropertyType()
        {
          Name = "SchemaOptions",
          SchemaId = new SchemaId(2147473653)
        },
        Comment = "(Optional) Schema options."
      });
      Property property17 = new Property();
      property17.Path = "partitionKeys";
      property17.AllowEmpty = AllowEmptyKind.Both;
      ArrayPropertyType arrayPropertyType3 = new ArrayPropertyType();
      UdtPropertyType udtPropertyType3 = new UdtPropertyType();
      udtPropertyType3.Name = "PartitionKey";
      udtPropertyType3.SchemaId = new SchemaId(2147473654);
      udtPropertyType3.Nullable = false;
      arrayPropertyType3.Items = (PropertyType) udtPropertyType3;
      property17.PropertyType = (PropertyType) arrayPropertyType3;
      property17.Comment = "(Optional) List of zero or more logical paths that form the partition key.";
      propertyList4.Add(property17);
      Property property18 = new Property();
      property18.Path = "primaryKeys";
      property18.AllowEmpty = AllowEmptyKind.Both;
      ArrayPropertyType arrayPropertyType4 = new ArrayPropertyType();
      UdtPropertyType udtPropertyType4 = new UdtPropertyType();
      udtPropertyType4.Name = "PrimarySortKey";
      udtPropertyType4.SchemaId = new SchemaId(2147473655);
      udtPropertyType4.Nullable = false;
      arrayPropertyType4.Items = (PropertyType) udtPropertyType4;
      property18.PropertyType = (PropertyType) arrayPropertyType4;
      property18.Comment = "(Optional) List of zero or more logical paths that form the primary sort key.";
      propertyList4.Add(property18);
      Property property19 = new Property();
      property19.Path = "staticKeys";
      property19.AllowEmpty = AllowEmptyKind.Both;
      ArrayPropertyType arrayPropertyType5 = new ArrayPropertyType();
      UdtPropertyType udtPropertyType5 = new UdtPropertyType();
      udtPropertyType5.Name = "StaticKey";
      udtPropertyType5.SchemaId = new SchemaId(2147473656);
      udtPropertyType5.Nullable = false;
      arrayPropertyType5.Items = (PropertyType) udtPropertyType5;
      property19.PropertyType = (PropertyType) arrayPropertyType5;
      property19.Comment = "(Optional) List of zero or more logical paths that hold data shared by all documents that have the same partition key.";
      propertyList4.Add(property19);
      Property property20 = new Property();
      property20.Path = "properties";
      property20.AllowEmpty = AllowEmptyKind.Both;
      ArrayPropertyType arrayPropertyType6 = new ArrayPropertyType();
      UdtPropertyType udtPropertyType6 = new UdtPropertyType();
      udtPropertyType6.Name = "Property";
      udtPropertyType6.SchemaId = new SchemaId(2147473657);
      udtPropertyType6.Nullable = false;
      arrayPropertyType6.Items = (PropertyType) udtPropertyType6;
      property20.PropertyType = (PropertyType) arrayPropertyType6;
      property20.Comment = "(Optional) List of zero or more property definitions that define the columns within the schema.";
      propertyList4.Add(property20);
      Property property21 = new Property();
      property21.Path = "baseName";
      PrimitivePropertyType primitivePropertyType15 = new PrimitivePropertyType();
      primitivePropertyType15.Type = TypeKind.Utf8;
      property21.PropertyType = (PropertyType) primitivePropertyType15;
      property21.Comment = "The name of the schema this schema derives from.";
      propertyList4.Add(property21);
      Property property22 = new Property();
      property22.Path = "baseId";
      PrimitivePropertyType primitivePropertyType16 = new PrimitivePropertyType();
      primitivePropertyType16.Type = TypeKind.Int32;
      primitivePropertyType16.ApiType = "SchemaId";
      property22.PropertyType = (PropertyType) primitivePropertyType16;
      property22.Comment = "The unique identifier of the schema this schema derives from.";
      property22.ApiName = "BaseSchemaId";
      propertyList4.Add(property22);
      schema5.Properties = propertyList4;
      schemaList.Add(schema5);
      Schema schema6 = new Schema();
      schema6.Name = "SchemaOptions";
      schema6.SchemaId = new SchemaId(2147473653);
      schema6.Comment = "Describes the set of options that apply to the entire schema and the way it is validated.";
      schema6.Options = new SchemaOptions()
      {
        DisallowUnschematized = true
      };
      List<Property> propertyList5 = new List<Property>();
      Property property23 = new Property();
      property23.Path = "disallowUnschematized";
      PrimitivePropertyType primitivePropertyType17 = new PrimitivePropertyType();
      primitivePropertyType17.Type = TypeKind.Boolean;
      property23.PropertyType = (PropertyType) primitivePropertyType17;
      propertyList5.Add(property23);
      Property property24 = new Property();
      property24.Path = "enablePropertyLevelTimestamp";
      PrimitivePropertyType primitivePropertyType18 = new PrimitivePropertyType();
      primitivePropertyType18.Type = TypeKind.Boolean;
      property24.PropertyType = (PropertyType) primitivePropertyType18;
      propertyList5.Add(property24);
      Property property25 = new Property();
      property25.Path = "disableSystemPrefix";
      PrimitivePropertyType primitivePropertyType19 = new PrimitivePropertyType();
      primitivePropertyType19.Type = TypeKind.Boolean;
      property25.PropertyType = (PropertyType) primitivePropertyType19;
      propertyList5.Add(property25);
      Property property26 = new Property();
      property26.Path = "abstract";
      PrimitivePropertyType primitivePropertyType20 = new PrimitivePropertyType();
      primitivePropertyType20.Type = TypeKind.Boolean;
      property26.PropertyType = (PropertyType) primitivePropertyType20;
      property26.Comment = "If true then instances of this schema cannot be created directly, only through subtypes.";
      propertyList5.Add(property26);
      schema6.Properties = propertyList5;
      schemaList.Add(schema6);
      Schema schema7 = new Schema();
      schema7.Name = "PartitionKey";
      schema7.SchemaId = new SchemaId(2147473654);
      schema7.Options = new SchemaOptions()
      {
        DisallowUnschematized = true
      };
      List<Property> propertyList6 = new List<Property>();
      Property property27 = new Property();
      property27.Path = "path";
      PrimitivePropertyType primitivePropertyType21 = new PrimitivePropertyType();
      primitivePropertyType21.Type = TypeKind.Utf8;
      primitivePropertyType21.Storage = StorageKind.Variable;
      property27.PropertyType = (PropertyType) primitivePropertyType21;
      propertyList6.Add(property27);
      schema7.Properties = propertyList6;
      schemaList.Add(schema7);
      Schema schema8 = new Schema();
      schema8.Name = "PrimarySortKey";
      schema8.SchemaId = new SchemaId(2147473655);
      schema8.Options = new SchemaOptions()
      {
        DisallowUnschematized = true
      };
      List<Property> propertyList7 = new List<Property>();
      Property property28 = new Property();
      property28.Path = "path";
      PrimitivePropertyType primitivePropertyType22 = new PrimitivePropertyType();
      primitivePropertyType22.Type = TypeKind.Utf8;
      primitivePropertyType22.Storage = StorageKind.Variable;
      property28.PropertyType = (PropertyType) primitivePropertyType22;
      propertyList7.Add(property28);
      Property property29 = new Property();
      property29.Path = "direction";
      PrimitivePropertyType primitivePropertyType23 = new PrimitivePropertyType();
      primitivePropertyType23.Type = TypeKind.Enum;
      primitivePropertyType23.Storage = StorageKind.Fixed;
      primitivePropertyType23.Enum = "SortDirection";
      primitivePropertyType23.Nullable = false;
      property29.PropertyType = (PropertyType) primitivePropertyType23;
      propertyList7.Add(property29);
      schema8.Properties = propertyList7;
      schemaList.Add(schema8);
      Schema schema9 = new Schema();
      schema9.Name = "StaticKey";
      schema9.SchemaId = new SchemaId(2147473656);
      schema9.Options = new SchemaOptions()
      {
        DisallowUnschematized = true
      };
      List<Property> propertyList8 = new List<Property>();
      Property property30 = new Property();
      property30.Path = "path";
      PrimitivePropertyType primitivePropertyType24 = new PrimitivePropertyType();
      primitivePropertyType24.Type = TypeKind.Utf8;
      primitivePropertyType24.Storage = StorageKind.Variable;
      property30.PropertyType = (PropertyType) primitivePropertyType24;
      propertyList8.Add(property30);
      schema9.Properties = propertyList8;
      schemaList.Add(schema9);
      Schema schema10 = new Schema();
      schema10.Name = "Property";
      schema10.SchemaId = new SchemaId(2147473657);
      schema10.Options = new SchemaOptions()
      {
        DisallowUnschematized = true
      };
      List<Property> propertyList9 = new List<Property>();
      Property property31 = new Property();
      property31.Path = "path";
      PrimitivePropertyType primitivePropertyType25 = new PrimitivePropertyType();
      primitivePropertyType25.Type = TypeKind.Utf8;
      primitivePropertyType25.Storage = StorageKind.Variable;
      property31.PropertyType = (PropertyType) primitivePropertyType25;
      propertyList9.Add(property31);
      Property property32 = new Property();
      property32.Path = "comment";
      PrimitivePropertyType primitivePropertyType26 = new PrimitivePropertyType();
      primitivePropertyType26.Type = TypeKind.Utf8;
      property32.PropertyType = (PropertyType) primitivePropertyType26;
      propertyList9.Add(property32);
      propertyList9.Add(new Property()
      {
        Path = "type",
        PropertyType = (PropertyType) new UdtPropertyType()
        {
          Name = "PropertyType",
          SchemaId = new SchemaId(2147473658)
        },
        Comment = "The type of the property. This field is polymorphic and may contain any defined subtype of PropertyType.",
        ApiName = "PropertyType"
      });
      Property property33 = new Property();
      property33.Path = "apiname";
      PrimitivePropertyType primitivePropertyType27 = new PrimitivePropertyType();
      primitivePropertyType27.Type = TypeKind.Utf8;
      property33.PropertyType = (PropertyType) primitivePropertyType27;
      property33.ApiName = "ApiName";
      propertyList9.Add(property33);
      Property property34 = new Property();
      property34.Path = "allowEmpty";
      property34.AllowEmpty = AllowEmptyKind.Both;
      PrimitivePropertyType primitivePropertyType28 = new PrimitivePropertyType();
      primitivePropertyType28.Type = TypeKind.Enum;
      primitivePropertyType28.Enum = "AllowEmptyKind";
      property34.PropertyType = (PropertyType) primitivePropertyType28;
      propertyList9.Add(property34);
      schema10.Properties = propertyList9;
      schemaList.Add(schema10);
      Schema schema11 = new Schema();
      schema11.Name = "PropertyType";
      schema11.SchemaId = new SchemaId(2147473658);
      schema11.Options = new SchemaOptions()
      {
        DisallowUnschematized = true,
        Abstract = true
      };
      List<Property> propertyList10 = new List<Property>();
      Property property35 = new Property();
      property35.Path = "apitype";
      PrimitivePropertyType primitivePropertyType29 = new PrimitivePropertyType();
      primitivePropertyType29.Type = TypeKind.Utf8;
      primitivePropertyType29.Storage = StorageKind.Variable;
      property35.PropertyType = (PropertyType) primitivePropertyType29;
      property35.ApiName = "ApiType";
      propertyList10.Add(property35);
      Property property36 = new Property();
      property36.Path = "type";
      PrimitivePropertyType primitivePropertyType30 = new PrimitivePropertyType();
      primitivePropertyType30.Type = TypeKind.Enum;
      primitivePropertyType30.Storage = StorageKind.Fixed;
      primitivePropertyType30.Enum = "TypeKind";
      primitivePropertyType30.Nullable = false;
      property36.PropertyType = (PropertyType) primitivePropertyType30;
      propertyList10.Add(property36);
      Property property37 = new Property();
      property37.Path = "nullable";
      PrimitivePropertyType primitivePropertyType31 = new PrimitivePropertyType();
      primitivePropertyType31.Type = TypeKind.Boolean;
      primitivePropertyType31.Storage = StorageKind.Fixed;
      primitivePropertyType31.Nullable = false;
      property37.PropertyType = (PropertyType) primitivePropertyType31;
      propertyList10.Add(property37);
      schema11.Properties = propertyList10;
      schemaList.Add(schema11);
      Schema schema12 = new Schema();
      schema12.Name = "PrimitivePropertyType";
      schema12.SchemaId = new SchemaId(2147473659);
      schema12.BaseName = "PropertyType";
      schema12.BaseSchemaId = new SchemaId(2147473658);
      schema12.Options = new SchemaOptions()
      {
        DisallowUnschematized = true
      };
      List<Property> propertyList11 = new List<Property>();
      Property property38 = new Property();
      property38.Path = "length";
      PrimitivePropertyType primitivePropertyType32 = new PrimitivePropertyType();
      primitivePropertyType32.Type = TypeKind.Int32;
      primitivePropertyType32.Storage = StorageKind.Fixed;
      primitivePropertyType32.Nullable = false;
      property38.PropertyType = (PropertyType) primitivePropertyType32;
      propertyList11.Add(property38);
      Property property39 = new Property();
      property39.Path = "storage";
      PrimitivePropertyType primitivePropertyType33 = new PrimitivePropertyType();
      primitivePropertyType33.Type = TypeKind.Enum;
      primitivePropertyType33.Storage = StorageKind.Fixed;
      primitivePropertyType33.Enum = "StorageKind";
      primitivePropertyType33.Nullable = false;
      property39.PropertyType = (PropertyType) primitivePropertyType33;
      propertyList11.Add(property39);
      Property property40 = new Property();
      property40.Path = "enum";
      property40.AllowEmpty = AllowEmptyKind.EmptyAsNull;
      PrimitivePropertyType primitivePropertyType34 = new PrimitivePropertyType();
      primitivePropertyType34.Type = TypeKind.Utf8;
      property40.PropertyType = (PropertyType) primitivePropertyType34;
      propertyList11.Add(property40);
      Property property41 = new Property();
      property41.Path = "rowBufferSize";
      PrimitivePropertyType primitivePropertyType35 = new PrimitivePropertyType();
      primitivePropertyType35.Type = TypeKind.Boolean;
      property41.PropertyType = (PropertyType) primitivePropertyType35;
      propertyList11.Add(property41);
      schema12.Properties = propertyList11;
      schemaList.Add(schema12);
      Schema schema13 = new Schema();
      schema13.Name = "ScopePropertyType";
      schema13.SchemaId = new SchemaId(2147473660);
      schema13.BaseName = "PropertyType";
      schema13.BaseSchemaId = new SchemaId(2147473658);
      schema13.Options = new SchemaOptions()
      {
        DisallowUnschematized = true,
        Abstract = true
      };
      List<Property> propertyList12 = new List<Property>();
      Property property42 = new Property();
      property42.Path = "immutable";
      PrimitivePropertyType primitivePropertyType36 = new PrimitivePropertyType();
      primitivePropertyType36.Type = TypeKind.Boolean;
      primitivePropertyType36.Storage = StorageKind.Fixed;
      primitivePropertyType36.Nullable = false;
      property42.PropertyType = (PropertyType) primitivePropertyType36;
      propertyList12.Add(property42);
      schema13.Properties = propertyList12;
      schemaList.Add(schema13);
      schemaList.Add(new Schema()
      {
        Name = "ArrayPropertyType",
        SchemaId = new SchemaId(2147473661),
        BaseName = "ScopePropertyType",
        BaseSchemaId = new SchemaId(2147473660),
        Options = new SchemaOptions()
        {
          DisallowUnschematized = true
        },
        Properties = new List<Property>()
        {
          new Property()
          {
            Path = "items",
            PropertyType = (PropertyType) new UdtPropertyType()
            {
              Name = "PropertyType",
              SchemaId = new SchemaId(2147473658)
            },
            Comment = "The type of the property. This field is polymorphic and may contain any defined subtype of PropertyType."
          }
        }
      });
      Schema schema14 = new Schema();
      schema14.Name = "ObjectPropertyType";
      schema14.SchemaId = new SchemaId(2147473662);
      schema14.BaseName = "ScopePropertyType";
      schema14.BaseSchemaId = new SchemaId(2147473660);
      schema14.Options = new SchemaOptions()
      {
        DisallowUnschematized = true
      };
      List<Property> propertyList13 = new List<Property>();
      Property property43 = new Property();
      property43.Path = "properties";
      property43.AllowEmpty = AllowEmptyKind.Both;
      ArrayPropertyType arrayPropertyType7 = new ArrayPropertyType();
      UdtPropertyType udtPropertyType7 = new UdtPropertyType();
      udtPropertyType7.Name = "Property";
      udtPropertyType7.SchemaId = new SchemaId(2147473657);
      udtPropertyType7.Nullable = false;
      arrayPropertyType7.Items = (PropertyType) udtPropertyType7;
      property43.PropertyType = (PropertyType) arrayPropertyType7;
      property43.Comment = "(Optional) List of zero or more property definitions that define the columns within the schema.";
      propertyList13.Add(property43);
      schema14.Properties = propertyList13;
      schemaList.Add(schema14);
      Schema schema15 = new Schema();
      schema15.Name = "UdtPropertyType";
      schema15.SchemaId = new SchemaId(2147473663);
      schema15.BaseName = "ScopePropertyType";
      schema15.BaseSchemaId = new SchemaId(2147473660);
      schema15.Options = new SchemaOptions()
      {
        DisallowUnschematized = true
      };
      List<Property> propertyList14 = new List<Property>();
      Property property44 = new Property();
      property44.Path = "name";
      PrimitivePropertyType primitivePropertyType37 = new PrimitivePropertyType();
      primitivePropertyType37.Type = TypeKind.Utf8;
      primitivePropertyType37.Storage = StorageKind.Variable;
      property44.PropertyType = (PropertyType) primitivePropertyType37;
      propertyList14.Add(property44);
      Property property45 = new Property();
      property45.Path = "id";
      PrimitivePropertyType primitivePropertyType38 = new PrimitivePropertyType();
      primitivePropertyType38.Type = TypeKind.Int32;
      primitivePropertyType38.Storage = StorageKind.Fixed;
      primitivePropertyType38.Nullable = false;
      primitivePropertyType38.ApiType = "SchemaId";
      property45.PropertyType = (PropertyType) primitivePropertyType38;
      property45.ApiName = "SchemaId";
      propertyList14.Add(property45);
      schema15.Properties = propertyList14;
      schemaList.Add(schema15);
      schemaList.Add(new Schema()
      {
        Name = "SetPropertyType",
        SchemaId = new SchemaId(2147473664),
        BaseName = "ScopePropertyType",
        BaseSchemaId = new SchemaId(2147473660),
        Options = new SchemaOptions()
        {
          DisallowUnschematized = true
        },
        Properties = new List<Property>()
        {
          new Property()
          {
            Path = "items",
            PropertyType = (PropertyType) new UdtPropertyType()
            {
              Name = "PropertyType",
              SchemaId = new SchemaId(2147473658)
            },
            Comment = "The type of the property. This field is polymorphic and may contain any defined subtype of PropertyType."
          }
        }
      });
      schemaList.Add(new Schema()
      {
        Name = "MapPropertyType",
        SchemaId = new SchemaId(2147473665),
        BaseName = "ScopePropertyType",
        BaseSchemaId = new SchemaId(2147473660),
        Options = new SchemaOptions()
        {
          DisallowUnschematized = true
        },
        Properties = new List<Property>()
        {
          new Property()
          {
            Path = "keys",
            PropertyType = (PropertyType) new UdtPropertyType()
            {
              Name = "PropertyType",
              SchemaId = new SchemaId(2147473658)
            },
            Comment = "The type of the property. This field is polymorphic and may contain any defined subtype of PropertyType."
          },
          new Property()
          {
            Path = "values",
            PropertyType = (PropertyType) new UdtPropertyType()
            {
              Name = "PropertyType",
              SchemaId = new SchemaId(2147473658)
            },
            Comment = "The type of the property. This field is polymorphic and may contain any defined subtype of PropertyType."
          }
        }
      });
      Schema schema16 = new Schema();
      schema16.Name = "TuplePropertyType";
      schema16.SchemaId = new SchemaId(2147473666);
      schema16.BaseName = "ScopePropertyType";
      schema16.BaseSchemaId = new SchemaId(2147473660);
      schema16.Options = new SchemaOptions()
      {
        DisallowUnschematized = true
      };
      List<Property> propertyList15 = new List<Property>();
      Property property46 = new Property();
      property46.Path = "items";
      property46.AllowEmpty = AllowEmptyKind.Both;
      ArrayPropertyType arrayPropertyType8 = new ArrayPropertyType();
      UdtPropertyType udtPropertyType8 = new UdtPropertyType();
      udtPropertyType8.Name = "PropertyType";
      udtPropertyType8.SchemaId = new SchemaId(2147473658);
      udtPropertyType8.Nullable = false;
      arrayPropertyType8.Items = (PropertyType) udtPropertyType8;
      property46.PropertyType = (PropertyType) arrayPropertyType8;
      property46.Comment = "The type of the properties. This field is polymorphic and may contain any defined subtype of PropertyType.";
      propertyList15.Add(property46);
      schema16.Properties = propertyList15;
      schemaList.Add(schema16);
      Schema schema17 = new Schema();
      schema17.Name = "TaggedPropertyType";
      schema17.SchemaId = new SchemaId(2147473667);
      schema17.BaseName = "ScopePropertyType";
      schema17.BaseSchemaId = new SchemaId(2147473660);
      schema17.Options = new SchemaOptions()
      {
        DisallowUnschematized = true
      };
      List<Property> propertyList16 = new List<Property>();
      Property property47 = new Property();
      property47.Path = "items";
      property47.AllowEmpty = AllowEmptyKind.Both;
      ArrayPropertyType arrayPropertyType9 = new ArrayPropertyType();
      UdtPropertyType udtPropertyType9 = new UdtPropertyType();
      udtPropertyType9.Name = "PropertyType";
      udtPropertyType9.SchemaId = new SchemaId(2147473658);
      udtPropertyType9.Nullable = false;
      arrayPropertyType9.Items = (PropertyType) udtPropertyType9;
      property47.PropertyType = (PropertyType) arrayPropertyType9;
      property47.Comment = "The type of the properties. This field is polymorphic and may contain any defined subtype of PropertyType.";
      propertyList16.Add(property47);
      schema17.Properties = propertyList16;
      schemaList.Add(schema17);
      Schema schema18 = new Schema();
      schema18.Name = "EnumSchema";
      schema18.SchemaId = new SchemaId(2147473668);
      schema18.Options = new SchemaOptions()
      {
        DisallowUnschematized = true
      };
      List<Property> propertyList17 = new List<Property>();
      Property property48 = new Property();
      property48.Path = "type";
      PrimitivePropertyType primitivePropertyType39 = new PrimitivePropertyType();
      primitivePropertyType39.Type = TypeKind.Enum;
      primitivePropertyType39.Storage = StorageKind.Fixed;
      primitivePropertyType39.Enum = "TypeKind";
      primitivePropertyType39.Nullable = false;
      property48.PropertyType = (PropertyType) primitivePropertyType39;
      property48.Comment = "(Required) Type of the schema element.";
      propertyList17.Add(property48);
      Property property49 = new Property();
      property49.Path = "name";
      PrimitivePropertyType primitivePropertyType40 = new PrimitivePropertyType();
      primitivePropertyType40.Type = TypeKind.Utf8;
      primitivePropertyType40.Storage = StorageKind.Variable;
      property49.PropertyType = (PropertyType) primitivePropertyType40;
      property49.Comment = "(Optional) Name of the schema.";
      propertyList17.Add(property49);
      Property property50 = new Property();
      property50.Path = "comment";
      PrimitivePropertyType primitivePropertyType41 = new PrimitivePropertyType();
      primitivePropertyType41.Type = TypeKind.Utf8;
      property50.PropertyType = (PropertyType) primitivePropertyType41;
      property50.Comment = "(Optional) Comment field describing the schema.";
      propertyList17.Add(property50);
      Property property51 = new Property();
      property51.Path = "apitype";
      PrimitivePropertyType primitivePropertyType42 = new PrimitivePropertyType();
      primitivePropertyType42.Type = TypeKind.Utf8;
      property51.PropertyType = (PropertyType) primitivePropertyType42;
      property51.Comment = "Api-specific type annotations for the property.";
      property51.ApiName = "ApiType";
      propertyList17.Add(property51);
      Property property52 = new Property();
      property52.Path = "values";
      property52.AllowEmpty = AllowEmptyKind.Both;
      ArrayPropertyType arrayPropertyType10 = new ArrayPropertyType();
      UdtPropertyType udtPropertyType10 = new UdtPropertyType();
      udtPropertyType10.Name = "EnumValue";
      udtPropertyType10.SchemaId = new SchemaId(2147473669);
      udtPropertyType10.Nullable = false;
      arrayPropertyType10.Items = (PropertyType) udtPropertyType10;
      property52.PropertyType = (PropertyType) arrayPropertyType10;
      property52.Comment = "(Optional) List of zero or more values.";
      propertyList17.Add(property52);
      schema18.Properties = propertyList17;
      schemaList.Add(schema18);
      Schema schema19 = new Schema();
      schema19.Name = "EnumValue";
      schema19.SchemaId = new SchemaId(2147473669);
      schema19.Options = new SchemaOptions()
      {
        DisallowUnschematized = true
      };
      List<Property> propertyList18 = new List<Property>();
      Property property53 = new Property();
      property53.Path = "name";
      PrimitivePropertyType primitivePropertyType43 = new PrimitivePropertyType();
      primitivePropertyType43.Type = TypeKind.Utf8;
      primitivePropertyType43.Storage = StorageKind.Variable;
      property53.PropertyType = (PropertyType) primitivePropertyType43;
      property53.Comment = "(Optional) Name of the schema.";
      propertyList18.Add(property53);
      Property property54 = new Property();
      property54.Path = "value";
      PrimitivePropertyType primitivePropertyType44 = new PrimitivePropertyType();
      primitivePropertyType44.Type = TypeKind.Int64;
      primitivePropertyType44.Storage = StorageKind.Fixed;
      property54.PropertyType = (PropertyType) primitivePropertyType44;
      property54.Comment = "The numerical value of the enum value.";
      propertyList18.Add(property54);
      Property property55 = new Property();
      property55.Path = "comment";
      PrimitivePropertyType primitivePropertyType45 = new PrimitivePropertyType();
      primitivePropertyType45.Type = TypeKind.Utf8;
      property55.PropertyType = (PropertyType) primitivePropertyType45;
      property55.Comment = "(Optional) Comment field describing the schema.";
      propertyList18.Add(property55);
      schema19.Properties = propertyList18;
      schemaList.Add(schema19);
      schema1.Schemas = schemaList;
      return schema1;
    }

    private static LayoutResolver LoadSchema() => (LayoutResolver) new LayoutResolverNamespace(SchemasHrSchema.Namespace);
  }
}
