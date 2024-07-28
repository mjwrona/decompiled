// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TypeField
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Public)]
  public class TypeField : Field
  {
    internal const string DefaultFormat = "{0}";

    [XmlAttribute("type")]
    public FieldTypeEnum Type { get; set; }

    [XmlAttribute("format")]
    [DefaultValue("{0}")]
    public string Format { get; set; }

    public TypeFieldValue[] TypeFieldValues { get; set; }
  }
}
