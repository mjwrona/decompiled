// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Common.XmlElementValueCollection
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl.Parsing.Common
{
  internal class XmlElementValueCollection : IEnumerable<XmlElementValue>, IEnumerable
  {
    private static readonly XmlElementValueCollection empty = new XmlElementValueCollection((IList<XmlElementValue>) new XmlElementValue[0], ((IEnumerable<XmlElementValue>) new XmlElementValue[0]).ToLookup<XmlElementValue, string>((Func<XmlElementValue, string>) (value => value.Name)));
    private readonly IList<XmlElementValue> values;
    private ILookup<string, XmlElementValue> nameLookup;

    private XmlElementValueCollection(
      IList<XmlElementValue> list,
      ILookup<string, XmlElementValue> nameMap)
    {
      this.values = list;
      this.nameLookup = nameMap;
    }

    internal XmlTextValue FirstText => this.values.OfText().FirstOrDefault<XmlTextValue>() ?? XmlTextValue.Missing;

    internal XmlElementValue this[string elementName] => this.EnsureLookup()[elementName].FirstOrDefault<XmlElementValue>() ?? (XmlElementValue) XmlElementValueCollection.MissingXmlElementValue.Instance;

    public IEnumerator<XmlElementValue> GetEnumerator() => this.values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.values.GetEnumerator();

    internal bool Remove(XmlElementValue value) => value != null && this.values.Remove(value);

    internal static XmlElementValueCollection FromList(IList<XmlElementValue> values) => values == null || values.Count == 0 ? XmlElementValueCollection.empty : new XmlElementValueCollection(values, (ILookup<string, XmlElementValue>) null);

    internal IEnumerable<XmlElementValue> FindByName(string elementName) => this.EnsureLookup()[elementName];

    internal IEnumerable<XmlElementValue<TResult>> FindByName<TResult>(string elementName) where TResult : class => this.FindByName(elementName).OfResultType<TResult>();

    private ILookup<string, XmlElementValue> EnsureLookup() => this.nameLookup ?? (this.nameLookup = this.values.ToLookup<XmlElementValue, string>((Func<XmlElementValue, string>) (value => value.Name)));

    internal sealed class MissingXmlElementValue : XmlElementValue
    {
      internal static readonly XmlElementValueCollection.MissingXmlElementValue Instance = new XmlElementValueCollection.MissingXmlElementValue();

      private MissingXmlElementValue()
        : base((string) null, (CsdlLocation) null)
      {
      }

      internal override object UntypedValue => (object) null;

      internal override bool IsUsed => false;
    }
  }
}
