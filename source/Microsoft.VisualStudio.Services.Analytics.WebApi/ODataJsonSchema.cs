// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WebApi.ODataJsonSchema
// Assembly: Microsoft.VisualStudio.Services.Analytics.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F858B048-FFE8-4E5F-8EBC-9B25DAC23DF8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.WebApi
{
  [DataContract]
  public class ODataJsonSchema
  {
    [DataMember(EmitDefaultValue = false)]
    public ODataJsonSchemaType Type { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ODataJsonSchemaFormat Format { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ODataJsonSchema Items { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, ODataJsonSchema> Properties { get; set; }

    [DataMember(Name = "@Display.DisplayName", EmitDefaultValue = false)]
    public string DisplayNameAnnotation { get; set; }

    [IgnoreDataMember]
    public string ReferenceNameAnnotation { get; set; }

    [DataMember(Name = "@Mashup.Transform", EmitDefaultValue = false)]
    public ODataMashupFunction[] TransformationsAnnotation { get; set; }
  }
}
