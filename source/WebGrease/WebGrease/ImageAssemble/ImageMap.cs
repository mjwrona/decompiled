// Decompiled with JetBrains decompiler
// Type: WebGrease.ImageAssemble.ImageMap
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace WebGrease.ImageAssemble
{
  internal sealed class ImageMap
  {
    private const string XmlVersion = "1.0";
    private const string XmlEncoding = "UTF-8";
    private const string RootNode = "images";
    private const string ImageNode = "input";
    private const string OriginalFile = "originalfile";
    private const string GeneratedFile = "file";
    private const string Width = "width";
    private const string Height = "height";
    private const string XPosition = "xposition";
    private const string YPosition = "yposition";
    private const string PositionInSprite = "positioninsprite";
    private const string InputNode = "input";
    private const string OutputNode = "output";
    private const string CommentNode = "comment";
    private const string Padding = "padding";
    private readonly string mapFileName;
    private readonly XElement root;
    private readonly XDocument xdoc;
    private readonly IList<string> spritedFiles = (IList<string>) new List<string>();
    private XElement currentOutputNode;
    private XElement notAssembledNode;

    internal ImageMap(string mapFileName)
    {
      this.xdoc = new XDocument();
      this.Document.Declaration = new XDeclaration("1.0", "UTF-8", "UTF-8");
      this.root = new XElement((XName) "images");
      this.Document.AddFirst((object) this.root);
      this.mapFileName = mapFileName;
    }

    internal XDocument Document => this.xdoc;

    internal IList<string> SpritedFiles => this.spritedFiles;

    internal void AppendToXml(string notAssembledFile, string comment)
    {
      if (this.notAssembledNode == null)
      {
        this.notAssembledNode = new XElement((XName) "output");
        this.notAssembledNode.SetAttributeValue((XName) "file", (object) string.Empty);
        this.root.Add((object) this.notAssembledNode);
      }
      XElement content = new XElement((XName) "input");
      content.Add((object) new XElement((XName) "originalfile", (object) notAssembledFile.ToLowerInvariant()));
      content.Add((object) new XElement((XName) nameof (comment), (object) comment));
      this.notAssembledNode.Add((object) content);
    }

    internal void AppendToXml(
      string originalFile,
      string genFile,
      int width,
      int height,
      int posX,
      int posY,
      string comment,
      bool addOutputNode,
      ImagePosition? posSprite)
    {
      if (addOutputNode)
      {
        this.SpritedFiles.Add(genFile);
        this.currentOutputNode = new XElement((XName) "output");
        this.currentOutputNode.SetAttributeValue((XName) "file", (object) genFile);
        this.root.Add((object) this.currentOutputNode);
      }
      XElement content = new XElement((XName) "input");
      content.Add((object) new XElement((XName) "originalfile", (object) originalFile.ToLowerInvariant()));
      content.Add((object) new XElement((XName) nameof (width), (object) width.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      content.Add((object) new XElement((XName) nameof (height), (object) height.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      content.Add((object) new XElement((XName) "xposition", (object) posX.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      content.Add((object) new XElement((XName) "yposition", (object) posY.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      if (!string.IsNullOrEmpty(comment))
        content.Add((object) new XElement((XName) nameof (comment), (object) comment));
      if (posSprite.HasValue)
        content.Add((object) new XElement((XName) "positioninsprite", (object) posSprite.ToString()));
      this.currentOutputNode.Add((object) content);
    }

    internal void AppendPadding(string padding) => this.root.SetAttributeValue((XName) nameof (padding), (object) padding);

    private void SaveXmlMap()
    {
      if (string.IsNullOrWhiteSpace(this.mapFileName))
        return;
      this.Document.Save(this.mapFileName);
    }

    internal bool UpdateAssembledImageName(string oldName, string newName)
    {
      bool flag = false;
      IEnumerable<XElement> source = this.root.Elements((XName) "output").Where<XElement>((Func<XElement, bool>) (outNode => (string) outNode.Attribute((XName) "file") == oldName));
      if (source.Count<XElement>() > 0)
      {
        source.First<XElement>().Attribute((XName) "file").Value = newName;
        this.SaveXmlMap();
        flag = true;
      }
      return flag;
    }

    public void UpdateSize(string file, int width, int height)
    {
      this.UpdateOrSetOutputAttribute(file, nameof (width), width.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.UpdateOrSetOutputAttribute(file, nameof (height), height.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    private void UpdateOrSetOutputAttribute(string file, string attributeName, string value)
    {
      XElement xelement = this.root.Elements((XName) "output").FirstOrDefault<XElement>((Func<XElement, bool>) (e => (string) e.Attribute((XName) nameof (file)) == file));
      if (xelement == null)
        return;
      XAttribute xattribute = xelement.Attribute((XName) attributeName);
      if (xattribute == null)
      {
        XAttribute content = new XAttribute((XName) attributeName, (object) value);
        xelement.Add((object) content);
      }
      else
        xattribute.Value = value;
    }
  }
}
