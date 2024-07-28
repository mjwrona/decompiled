// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BadgeSvgGenerator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class BadgeSvgGenerator
  {
    private static readonly BadgeSvgGenerator.ProductLogo s_ArtifactsLogo = new BadgeSvgGenerator.ProductLogo("AzureDevOpsArtifacts.svg");
    private static readonly BadgeSvgGenerator.ProductLogo s_PipelineLogo = new BadgeSvgGenerator.ProductLogo("AzureDevOpsPipelines.svg");
    private static readonly BadgeSvgGenerator.ProductLogo s_BoardsLogo = new BadgeSvgGenerator.ProductLogo("AzureDevOpsBoards.svg");
    private const string c_AzDevOpsArtifactsResourceName = "AzureDevOpsArtifacts.svg";
    private const string c_AzDevOpsPipelinesResourceName = "AzureDevOpsPipelines.svg";
    private const string c_AzDevOpsBoardsResourceName = "AzureDevOpsBoards.svg";
    private static readonly XNamespace SvgNamespace = (XNamespace) "http://www.w3.org/2000/svg";

    public static XDocument CreateImage(IVssRequestContext requestContext, ref BadgeOptions options) => BadgeSvgGenerator.CreateImage(requestContext, options.LeftText, options.LeftForeground, options.LeftBackground, options.RightText, options.RightForeground, options.RightBackground, options.IconFill, options.LogoType);

    private static XDocument CreateImage(
      IVssRequestContext requestContext,
      string leftText,
      string leftTextColorHex,
      string leftBackgroundColorHex,
      string rightText,
      string rightTextColorHex,
      string rightBackgroundColorHex,
      string iconColorHex,
      BadgeLogo logoType)
    {
      ArgumentUtility.CheckForDefinedEnum<BadgeLogo>(logoType, nameof (logoType));
      SizeF sizeF1;
      SizeF sizeF2;
      using (Font font = new Font("Verdana", 11f, GraphicsUnit.Pixel))
      {
        using (Graphics graphics = Graphics.FromImage((Image) new Bitmap(1, 1)))
        {
          sizeF1 = graphics.MeasureString(leftText, font);
          sizeF2 = graphics.MeasureString(rightText, font);
        }
      }
      XElement productLogoElement = BadgeSvgGenerator.CreateProductLogoElement(requestContext, iconColorHex, logoType);
      double num = double.Parse(productLogoElement.Attribute((XName) "width").Value);
      double width1 = 5.0 + num + 3.0 + (double) sizeF1.Width + 2.0 + 2.0 + (double) sizeF2.Width + 4.0;
      double x1 = 5.0 + num + 3.0 + (double) sizeF1.Width + 2.0;
      double width2 = width1 - x1;
      double x2 = x1 - 2.0 - (double) sizeF1.Width / 2.0;
      double x3 = width1 - 4.0 - (double) sizeF2.Width / 2.0;
      return new XDocument(new object[1]
      {
        (object) new XElement(BadgeSvgGenerator.SvgNamespace + "svg", new object[7]
        {
          (object) new XAttribute((XName) "width", (object) width1.ToString("0.0", (IFormatProvider) CultureInfo.InvariantCulture)),
          (object) new XAttribute((XName) "height", (object) 20.0.ToString("0.0", (IFormatProvider) CultureInfo.InvariantCulture)),
          (object) new XElement(BadgeSvgGenerator.SvgNamespace + "linearGradient", new object[5]
          {
            (object) new XAttribute((XName) "id", (object) "a"),
            (object) new XAttribute((XName) "x2", (object) "0"),
            (object) new XAttribute((XName) "y2", (object) "100%"),
            (object) BadgeSvgGenerator.CreateStopElement(0.0, 0.0, "#000"),
            (object) BadgeSvgGenerator.CreateStopElement(1.0, 0.2, "#000")
          }),
          (object) new XElement(BadgeSvgGenerator.SvgNamespace + "clipPath", new object[2]
          {
            (object) new XAttribute((XName) "id", (object) "c"),
            (object) BadgeSvgGenerator.CreateRectElement(width1, 20.0, (string) null, 3.0)
          }),
          (object) new XElement(BadgeSvgGenerator.SvgNamespace + "g", new object[4]
          {
            (object) new XAttribute((XName) "clip-path", (object) "url(#c)"),
            (object) BadgeSvgGenerator.CreateRectElement(width1, 20.0, leftBackgroundColorHex, 0.0),
            (object) BadgeSvgGenerator.CreateRectElement(x1, width2, 20.0, rightBackgroundColorHex, 0.0),
            (object) BadgeSvgGenerator.CreateRectElement(width1, 20.0, "url(#a)", 0.0)
          }),
          (object) productLogoElement,
          (object) new XElement(BadgeSvgGenerator.SvgNamespace + "g", new object[8]
          {
            (object) new XAttribute((XName) "fill", (object) "#fff"),
            (object) new XAttribute((XName) "text-anchor", (object) "middle"),
            (object) new XAttribute((XName) "font-family", (object) "DejaVu Sans,Verdana,Geneva,sans-serif"),
            (object) new XAttribute((XName) "font-size", (object) 11),
            (object) BadgeSvgGenerator.CreateTextElement(x2, 15.0, leftText, "#000", 0.3),
            (object) BadgeSvgGenerator.CreateTextElement(x2, 14.0, leftText, leftTextColorHex),
            (object) BadgeSvgGenerator.CreateTextElement(x3, 15.0, rightText, "#000", 0.3),
            (object) BadgeSvgGenerator.CreateTextElement(x3, 14.0, rightText, rightTextColorHex)
          })
        })
      });
    }

    private static XElement CreateProductLogoElement(
      IVssRequestContext requestContext,
      string color,
      BadgeLogo logoType)
    {
      XElement productLogoElement = new XElement(BadgeSvgGenerator.GetProductLogo(requestContext, logoType).SvgElement);
      foreach (XAttribute xattribute in productLogoElement.Element(BadgeSvgGenerator.SvgNamespace + "g").Descendants().SelectMany<XElement, XAttribute>((Func<XElement, IEnumerable<XAttribute>>) (d => d.Attributes().Where<XAttribute>((Func<XAttribute, bool>) (a => a.Name == (XName) "fill")))))
        xattribute.Value = color;
      return productLogoElement;
    }

    private static XElement CreateStopElement(double offset, double opacity) => new XElement(BadgeSvgGenerator.SvgNamespace + "stop", new object[2]
    {
      (object) new XAttribute((XName) nameof (offset), (object) offset.ToString("0.0", (IFormatProvider) CultureInfo.InvariantCulture)),
      (object) new XAttribute((XName) "stop-opacity", (object) opacity.ToString("0.0", (IFormatProvider) CultureInfo.InvariantCulture))
    });

    private static XElement CreateStopElement(double offset, double opacity, string color)
    {
      XElement stopElement = BadgeSvgGenerator.CreateStopElement(offset, opacity);
      stopElement.SetAttributeValue((XName) "stop-color", (object) color);
      return stopElement;
    }

    private static XElement CreateRectElement(
      double width,
      double height,
      string fillColor,
      double rx)
    {
      List<XAttribute> xattributeList = new List<XAttribute>()
      {
        new XAttribute((XName) nameof (width), (object) width.ToString("0.0", (IFormatProvider) CultureInfo.InvariantCulture)),
        new XAttribute((XName) nameof (height), (object) height.ToString("0.0", (IFormatProvider) CultureInfo.InvariantCulture))
      };
      if (!string.IsNullOrEmpty(fillColor))
        xattributeList.Add(new XAttribute((XName) "fill", (object) fillColor));
      if (rx > 0.0)
        xattributeList.Add(new XAttribute((XName) nameof (rx), (object) rx.ToString("0.0", (IFormatProvider) CultureInfo.InvariantCulture)));
      return new XElement(BadgeSvgGenerator.SvgNamespace + "rect", (object[]) xattributeList.ToArray());
    }

    private static XElement CreateRectElement(
      double x,
      double width,
      double height,
      string fillColor,
      double rx)
    {
      XElement rectElement = BadgeSvgGenerator.CreateRectElement(width, height, fillColor, rx);
      rectElement.SetAttributeValue((XName) nameof (x), (object) x.ToString("0.0", (IFormatProvider) CultureInfo.InvariantCulture));
      return rectElement;
    }

    private static XElement CreateTextElement(double x, double y, string text) => new XElement(BadgeSvgGenerator.SvgNamespace + nameof (text), new object[3]
    {
      (object) new XAttribute((XName) nameof (x), (object) x.ToString("0.0", (IFormatProvider) CultureInfo.InvariantCulture)),
      (object) new XAttribute((XName) nameof (y), (object) y.ToString("0.0", (IFormatProvider) CultureInfo.InvariantCulture)),
      (object) text
    });

    private static XElement CreateTextElement(
      double x,
      double y,
      string text,
      string fillColor,
      double fillOpacity)
    {
      XElement textElement = BadgeSvgGenerator.CreateTextElement(x, y, text);
      textElement.SetAttributeValue((XName) "fill", (object) fillColor);
      textElement.SetAttributeValue((XName) "fill-opacity", (object) fillOpacity.ToString("0.0", (IFormatProvider) CultureInfo.InvariantCulture));
      return textElement;
    }

    private static XElement CreateTextElement(double x, double y, string text, string fillColor)
    {
      XElement textElement = BadgeSvgGenerator.CreateTextElement(x, y, text);
      textElement.SetAttributeValue((XName) "fill", (object) fillColor);
      return textElement;
    }

    private static BadgeSvgGenerator.ProductLogo GetProductLogo(
      IVssRequestContext requestContext,
      BadgeLogo logoType)
    {
      if (logoType == BadgeLogo.Artifacts)
        return BadgeSvgGenerator.s_ArtifactsLogo;
      return logoType == BadgeLogo.Boards ? BadgeSvgGenerator.s_BoardsLogo : BadgeSvgGenerator.s_PipelineLogo;
    }

    private class ProductLogo
    {
      private static readonly object s_loadSvgSyncObject = new object();
      private readonly string m_resourceName;
      private XElement m_svgElement;

      public ProductLogo(string resourceName) => this.m_resourceName = resourceName;

      public XElement SvgElement
      {
        get
        {
          this.EnsureLoaded();
          return this.m_svgElement;
        }
      }

      private void EnsureLoaded()
      {
        if (this.m_svgElement != null)
          return;
        lock (BadgeSvgGenerator.ProductLogo.s_loadSvgSyncObject)
        {
          using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(this.m_resourceName))
            this.m_svgElement = XDocument.Load(manifestResourceStream).Element(BadgeSvgGenerator.SvgNamespace + "svg");
        }
      }
    }
  }
}
