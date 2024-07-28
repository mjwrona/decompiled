// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Common.XmlElementValue
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Csdl.Parsing.Common
{
  internal abstract class XmlElementValue
  {
    internal XmlElementValue(string elementName, CsdlLocation elementLocation)
    {
      this.Name = elementName;
      this.Location = elementLocation;
    }

    internal string Name { get; private set; }

    internal CsdlLocation Location { get; private set; }

    internal abstract object UntypedValue { get; }

    internal abstract bool IsUsed { get; }

    internal virtual bool IsText => false;

    internal virtual string TextValue => this.ValueAs<string>();

    internal virtual TValue ValueAs<TValue>() where TValue : class => this.UntypedValue as TValue;
  }
}
