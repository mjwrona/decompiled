// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.AppConfigElement
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public abstract class AppConfigElement
  {
    internal AppConfigElement(XElement root) => this.Root = root;

    public string this[string name]
    {
      get => this.Root.Attribute(this.MakeName(name))?.Value;
      set
      {
        XAttribute xattribute = this.Root.Attribute(this.MakeName(name));
        if (value == null)
          xattribute?.Remove();
        else if (xattribute != null)
          xattribute.Value = value;
        else
          this.Root.Add((object) this.MakeAttribute(name, value));
      }
    }

    public void Clear() => this.Root.Add((object) this.MakeElement("clear"));

    public void AddCondition(ICondition condition) => this.Root.AddAnnotation((object) condition);

    protected XElement Root { get; private set; }

    protected XElement GetOrCreateElement(string name)
    {
      XElement content = this.Root.Element(this.MakeName(name));
      if (content == null)
      {
        content = this.MakeElement(name);
        this.Root.Add((object) content);
      }
      return content;
    }

    protected XElement GetOrCreateElement(string name, string attributeName, string attributeValue) => this.GetOrCreateElement(this.MakeName(name), this.MakeName(attributeName), attributeValue);

    protected XElement GetOrCreateElement(XName name, XName attributeName, string attributeValue)
    {
      XElement content = this.Root.Elements(name).SingleOrDefault<XElement>((Func<XElement, bool>) (x => x.Attribute(attributeName)?.Value == attributeValue));
      if (content == null)
      {
        content = this.MakeElement(name);
        if (attributeValue != null)
          content.Add((object) this.MakeAttribute(attributeName, attributeValue));
        this.Root.Add((object) content);
      }
      return content;
    }

    protected virtual XElement MakeElement(string name) => this.MakeElement(this.MakeName(name));

    protected virtual XElement MakeElement(XName name) => new XElement(name);

    protected virtual XAttribute MakeAttribute(string name, string value) => this.MakeAttribute(this.MakeName(name), value);

    protected virtual XAttribute MakeAttribute(XName name, string value) => new XAttribute(name, (object) value);

    protected virtual XName MakeName(string name) => XNamespace.None.GetName(name);

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      using (Utf8StringWriter utf8StringWriter = new Utf8StringWriter(sb))
        this.GetXml().Save((TextWriter) utf8StringWriter);
      return sb.ToString();
    }

    internal XElement GetXml()
    {
      XElement xml = new XElement(this.Root);
      List<Tuple<XElement, XElement>> tupleList1 = new List<Tuple<XElement, XElement>>();
      foreach (Tuple<XElement, XElement> tuple in this.Root.Descendants().Zip<XElement, XElement, Tuple<XElement, XElement>>(xml.Descendants(), (Func<XElement, XElement, Tuple<XElement, XElement>>) ((x, y) => Tuple.Create<XElement, XElement>(x, y))))
      {
        if (tuple.Item1.Annotations<ICondition>().Any<ICondition>())
          tupleList1.Add(tuple);
      }
      foreach (Tuple<XElement, XElement> tuple in tupleList1)
      {
        XElement root = tuple.Item2;
        foreach (ICondition annotation in tuple.Item1.Annotations<ICondition>())
          annotation.Materialize(ref root);
      }
      bool flag1;
      do
      {
        List<Tuple<XElement, ICondition>> tupleList2 = new List<Tuple<XElement, ICondition>>();
        foreach (XElement descendant in xml.Descendants())
        {
          ICondition condition;
          if (When.TryParse(descendant, out condition))
            tupleList2.Add(Tuple.Create<XElement, ICondition>(descendant, condition));
        }
        flag1 = false;
        foreach (Tuple<XElement, ICondition> tuple in tupleList2)
        {
          bool? nullable1 = tuple.Item2.Evaluate();
          if (nullable1.HasValue)
          {
            bool? nullable2 = nullable1;
            bool flag2 = true;
            if (nullable2.GetValueOrDefault() == flag2 & nullable2.HasValue)
            {
              tuple.Item1.AddAfterSelf((object) tuple.Item1.Elements());
              flag1 = true;
            }
            tuple.Item1.Remove();
          }
        }
      }
      while (flag1);
      return xml;
    }
  }
}
