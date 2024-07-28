// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Deserialization.ODataEnumDeserializer
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData;
using Microsoft.OData.Edm;
using System;
using System.Linq;

namespace Microsoft.AspNet.OData.Formatter.Deserialization
{
  public class ODataEnumDeserializer : ODataEdmTypeDeserializer
  {
    public ODataEnumDeserializer()
      : base(ODataPayloadKind.Property)
    {
    }

    public override object Read(
      ODataMessageReader messageReader,
      Type type,
      ODataDeserializerContext readContext)
    {
      if (messageReader == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (messageReader));
      if (type == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (type));
      IEdmTypeReference edmTypeReference = readContext != null ? readContext.GetEdmType(type) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (readContext));
      return this.ReadInline((object) messageReader.ReadProperty(edmTypeReference), edmTypeReference, readContext);
    }

    public override object ReadInline(
      object item,
      IEdmTypeReference edmType,
      ODataDeserializerContext readContext)
    {
      if (item == null)
        return (object) null;
      if (item is ODataProperty odataProperty)
        item = odataProperty.Value;
      IEdmEnumTypeReference enumTypeReference = edmType.AsEnum();
      ODataEnumValue enumValue = item as ODataEnumValue;
      if (readContext.IsUntyped)
        return (object) new EdmEnumObject(enumTypeReference, enumValue.Value);
      IEdmEnumType enumType = enumTypeReference.EnumDefinition();
      ClrEnumMemberAnnotation memberAnnotation = readContext.Model.GetClrEnumMemberAnnotation(enumType);
      if (memberAnnotation != null && enumValue != null)
      {
        IEdmEnumMember edmEnumMember = enumType.Members.FirstOrDefault<IEdmEnumMember>((Func<IEdmEnumMember, bool>) (m => m.Name == enumValue.Value));
        if (edmEnumMember != null)
        {
          Enum clrEnumMember = memberAnnotation.GetClrEnumMember(edmEnumMember);
          if (clrEnumMember != null)
            return (object) clrEnumMember;
        }
      }
      Type clrType = EdmLibHelpers.GetClrType(edmType, readContext.Model);
      return EnumDeserializationHelpers.ConvertEnumValue(item, clrType);
    }
  }
}
