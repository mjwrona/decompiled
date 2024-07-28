// Decompiled with JetBrains decompiler
// Type: WebGrease.CacheDependencyGraph
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WebGrease
{
  internal class CacheDependencyGraph
  {
    private readonly List<KeyValuePair<Guid, Guid>> links = new List<KeyValuePair<Guid, Guid>>();
    private readonly IDictionary<string, Guid> nodes = (IDictionary<string, Guid>) new Dictionary<string, Guid>();

    internal void AddDependencyLink(string label1, string label2)
    {
      KeyValuePair<Guid, Guid> keyValuePair = new KeyValuePair<Guid, Guid>(this.AddDependencyNode(label1), this.AddDependencyNode(label2));
      if (this.links.Contains(keyValuePair))
        return;
      this.links.Add(keyValuePair);
    }

    internal void Save(string path)
    {
      XNamespace xmlns = XNamespace.Get("http://schemas.microsoft.com/vs/2009/dgml");
      new XDocument(new XDeclaration("1.0", "utf-8", "no"), new object[1]
      {
        (object) new XElement(xmlns + "DirectedGraph", new object[5]
        {
          (object) new XAttribute((XName) "GraphDirection", (object) "TopToBottom"),
          (object) new XAttribute((XName) "Layout", (object) "Sugiyama"),
          (object) new XElement(xmlns + "Nodes", (object) this.nodes.Select<KeyValuePair<string, Guid>, XElement>((Func<KeyValuePair<string, Guid>, XElement>) (kvp => new XElement(xmlns + "Node", new object[2]
          {
            (object) new XAttribute((XName) "Id", (object) kvp.Value),
            (object) new XAttribute((XName) "Label", (object) kvp.Key)
          })))),
          (object) new XElement(xmlns + "Links", (object) this.links.Select<KeyValuePair<Guid, Guid>, XElement>((Func<KeyValuePair<Guid, Guid>, XElement>) (kvp => new XElement(xmlns + "Link", new object[2]
          {
            (object) new XAttribute((XName) "Source", (object) kvp.Key),
            (object) new XAttribute((XName) "Target", (object) kvp.Value)
          })))),
          (object) new XElement(xmlns + "Properties", new object[4]
          {
            (object) new XElement(xmlns + "Property", new object[2]
            {
              (object) new XAttribute((XName) "Id", (object) "GraphDirection"),
              (object) new XAttribute((XName) "DataType", (object) "Microsoft.VisualStudio.Diagrams.Layout.LayoutOrientation")
            }),
            (object) new XElement(xmlns + "Property", new object[2]
            {
              (object) new XAttribute((XName) "Id", (object) "Layout"),
              (object) new XAttribute((XName) "DataType", (object) "System.String")
            }),
            (object) new XElement(xmlns + "Property", new object[2]
            {
              (object) new XAttribute((XName) "Id", (object) "Bounds"),
              (object) new XAttribute((XName) "DataType", (object) "System.String")
            }),
            (object) new XElement(xmlns + "Property", new object[4]
            {
              (object) new XAttribute((XName) "Id", (object) "Label"),
              (object) new XAttribute((XName) "Label", (object) "Label"),
              (object) new XAttribute((XName) "Description", (object) "Displayable label of an Annotatable object"),
              (object) new XAttribute((XName) "DataType", (object) "System.String")
            })
          })
        })
      }).Save(path);
    }

    private Guid AddDependencyNode(string label)
    {
      string lowerInvariant = label.ToLowerInvariant();
      if (!this.nodes.ContainsKey(lowerInvariant))
        this.nodes.Add(lowerInvariant, Guid.NewGuid());
      return this.nodes[lowerInvariant];
    }
  }
}
