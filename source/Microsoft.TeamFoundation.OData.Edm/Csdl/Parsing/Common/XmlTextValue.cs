// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Common.XmlTextValue
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Csdl.Parsing.Common
{
  internal class XmlTextValue : XmlElementValue<string>
  {
    internal static readonly XmlTextValue Missing = new XmlTextValue((CsdlLocation) null, (string) null);
    internal const string ElementName = "<\"Text\">";

    internal XmlTextValue(CsdlLocation textLocation, string textValue)
      : base("<\"Text\">", textLocation, textValue)
    {
    }

    internal override bool IsText => true;

    internal override string TextValue => this.Value;
  }
}
