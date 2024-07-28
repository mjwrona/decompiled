// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.SanitizeSvgUtility
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using HtmlAgilityPack;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class SanitizeSvgUtility
  {
    private static readonly Regex ALLOWED_URI_REGEX = new Regex("^(?:(?:(?:f|ht)tps?|mailto|tel|callto|cid|xmpp):|[^a-z]|[a-z+.\\-]+(?:[^a-z+.\\-:]|$))", RegexOptions.IgnoreCase);
    private static readonly Regex ATTR_WHITESPACE_REGEX = new Regex("[\\u0000-\\u0020\\u00A0\\u1680\\u180E\\u2000-\\u2029\\u205f\\u3000]");
    private static readonly ImmutableHashSet<string> URI_SAFE_ATTRS = ImmutableHashSet.Create<string>("class", "id", "name", "pattern", "title", "style", "xmlns");
    private static readonly ImmutableHashSet<string> DATA_URI_TAGS = ImmutableHashSet.Create<string>("audio", "video", "image");
    internal static readonly ImmutableHashSet<string> ALLOWED_TAGS = ImmutableHashSet.Create<string>("svg", "a", "altglyph", "altglyphdef", "altglyphitem", "animatecolor", "animatemotion", "animatetransform", "audio", "canvas", "circle", "clippath", "defs", "desc", "ellipse", "filter", "font", "g", "glyph", "glyphref", "hkern", "image", "line", "lineargradient", "marker", "mask", "metadata", "mpath", "path", "pattern", "polygon", "polyline", "radialgradient", "rect", "stop", "style", "switch", "symbol", "text", "textpath", "title", "tref", "tspan", "video", "view", "vkern", "feblend", "fecolormatrix", "fecomponenttransfer", "fecomposite", "feconvolvematrix", "fediffuselighting", "fedisplacementmap", "fedistantlight", "feflood", "fefunca", "fefuncb", "fefuncg", "fefuncr", "fegaussianblur", "femerge", "femergenode", "femorphology", "feoffset", "fepointlight", "fespecularlighting", "fespotlight", "fetile", "feturbulence", "#text");
    internal static readonly ImmutableHashSet<string> ALLOWED_ATTRS = ImmutableHashSet.Create<string>("accent-height", "accumulate", "additive", "alignment-baseline", "ascent", "attributename", "attributetype", "azimuth", "basefrequency", "baseline-shift", "begin", "bias", "by", "class", "clip", "clip-path", "clip-rule", "color", "color-interpolation", "color-interpolation-filters", "color-profile", "color-rendering", "cx", "cy", "d", "dx", "dy", "diffuseconstant", "direction", "display", "divisor", "dur", "edgemode", "elevation", "end", "fill", "fill-opacity", "fill-rule", "filter", "filterunits", "flood-color", "flood-opacity", "font-family", "font-size", "font-size-adjust", "font-stretch", "font-style", "font-variant", "font-weight", "fx", "fy", "g1", "g2", "glyph-name", "glyphref", "gradientunits", "gradienttransform", "height", "href", "id", "image-rendering", "in", "in2", "k", "k1", "k2", "k3", "k4", "kerning", "keypoints", "keysplines", "keytimes", "lang", "lengthadjust", "letter-spacing", "kernelmatrix", "kernelunitlength", "lighting-color", "local", "marker-end", "marker-mid", "marker-start", "markerheight", "markerunits", "markerwidth", "maskcontentunits", "maskunits", "max", "mask", "media", "method", "mode", "min", "name", "numoctaves", "offset", "operator", "opacity", "order", "orient", "orientation", "origin", "overflow", "paint-order", "path", "pathlength", "patterncontentunits", "patterntransform", "patternunits", "points", "preservealpha", "preserveaspectratio", "primitiveunits", "r", "rx", "ry", "radius", "refx", "refy", "repeatcount", "repeatdur", "restart", "result", "rotate", "scale", "seed", "shape-rendering", "specularconstant", "specularexponent", "spreadmethod", "stddeviation", "stitchtiles", "stop-color", "stop-opacity", "stroke-dasharray", "stroke-dashoffset", "stroke-linecap", "stroke-linejoin", "stroke-miterlimit", "stroke-opacity", "stroke", "stroke-width", "style", "surfacescale", "tabindex", "targetx", "targety", "transform", "text-anchor", "text-decoration", "text-rendering", "textlength", "type", "u1", "u2", "unicode", "values", "viewbox", "visibility", "version", "vert-adv-y", "vert-origin-x", "vert-origin-y", "width", "word-spacing", "wrap", "writing-mode", "xchannelselector", "ychannelselector", "x", "x1", "x2", "xmlns", "y", "y1", "y2", "z", "zoomandpan", "xlink:href", "xml:id", "xlink:title", "xml:space", "xmlns:xlink");

    public static string Sanitize(IVssRequestContext rc, string dirty)
    {
      if (string.IsNullOrWhiteSpace(dirty) || !dirty.Contains("<"))
        return dirty;
      HtmlDocument htmlDocument = new HtmlDocument()
      {
        OptionOutputOriginalCase = true
      };
      ClientTraceService service = rc.GetService<ClientTraceService>();
      ClientTraceData clientTraceData = new ClientTraceData();
      try
      {
        htmlDocument.LoadHtml(dirty);
      }
      catch (Exception ex)
      {
        rc.TraceException(1013884, "Microsoft.TeamFoundation.Sourcecontrol.WebServer", "SanitizeSvg", ex);
        clientTraceData.Add("SanitizeSvgHasLoadException", (object) true);
        service.Publish(rc, "Microsoft.TeamFoundation.Sourcecontrol.WebServer", "SanitizeSvg", clientTraceData);
        throw new SvgSanitizeException(Resources.Get("SvgSanitizeLoadError"));
      }
      if (htmlDocument.ParseErrors != null && htmlDocument.ParseErrors.Count<HtmlParseError>() > 0)
      {
        clientTraceData.Add("SanitizeSvgParseErrors", (object) htmlDocument.ParseErrors);
        service.Publish(rc, "Microsoft.TeamFoundation.Sourcecontrol.WebServer", "SanitizeSvg", clientTraceData);
        throw new InvalidSvgException(Resources.Get("InvalidSvgParseErros"));
      }
      if (htmlDocument.DocumentNode == null)
      {
        clientTraceData.Add("SanitizeSvgNullDocument", (object) true);
        service.Publish(rc, "Microsoft.TeamFoundation.Sourcecontrol.WebServer", "SanitizeSvg", clientTraceData);
        throw new InvalidSvgException(Resources.Get("InvalidSvgNullDocument"));
      }
      IEnumerable<HtmlNode> elements = htmlDocument.DocumentNode.Descendants();
      if (elements == null)
      {
        clientTraceData.Add("SanitizeSvgNullDocument", (object) true);
        service.Publish(rc, "Microsoft.TeamFoundation.Sourcecontrol.WebServer", "SanitizeSvg", clientTraceData);
        throw new InvalidSvgException(Resources.Get("InvalidSvgNullDocument"));
      }
      try
      {
        SanitizeSvgUtility.Sanitize(elements, clientTraceData);
        return htmlDocument.DocumentNode.WriteTo();
      }
      catch (Exception ex)
      {
        rc.TraceException(1013884, "Microsoft.TeamFoundation.Sourcecontrol.WebServer", "SanitizeSvg", ex);
        clientTraceData.Add("SanitizeSvgHasException", (object) true);
        throw new SvgSanitizeException(Resources.Get("SvgSanitizeError"));
      }
      finally
      {
        service.Publish(rc, "Microsoft.TeamFoundation.Sourcecontrol.WebServer", "SanitizeSvg", clientTraceData);
      }
    }

    private static void Sanitize(IEnumerable<HtmlNode> elements, ClientTraceData ctData)
    {
      HashSet<string> stringSet = new HashSet<string>();
      HashSet<string> invalidAttrs = new HashSet<string>();
      for (int index = elements.Count<HtmlNode>() - 1; index >= 0; --index)
      {
        HtmlNode htmlNode = elements.ElementAt<HtmlNode>(index);
        if (!SanitizeSvgUtility.ALLOWED_TAGS.Contains(htmlNode.Name.ToLower()))
        {
          htmlNode.ParentNode.RemoveChild(htmlNode);
          stringSet.Add(htmlNode.Name);
        }
        else if (htmlNode.HasAttributes)
          SanitizeSvgUtility.SanitizeAttributes(htmlNode, invalidAttrs);
      }
      ctData.Add("SanitizeSvgInvalidTags", (object) stringSet);
      ctData.Add("SanitizeSvgInvalidAttrs", (object) invalidAttrs);
    }

    private static void SanitizeAttributes(HtmlNode element, HashSet<string> invalidAttrs)
    {
      for (int index = element.Attributes.Count - 1; index >= 0; --index)
      {
        HtmlAttribute attribute = element.Attributes.ElementAt<HtmlAttribute>(index);
        if (!SanitizeSvgUtility.IsValidAttribute(element.Name, attribute.Name, attribute.Value))
        {
          element.Attributes.Remove(attribute);
          invalidAttrs.Add(attribute.Name);
        }
      }
    }

    private static bool IsValidAttribute(string tagName, string attrName, string attrValue)
    {
      string lower1 = attrName.ToLower();
      string lower2 = tagName.ToLower();
      attrValue = HttpUtility.HtmlDecode(attrValue);
      return SanitizeSvgUtility.ALLOWED_ATTRS.Contains(lower1) && (SanitizeSvgUtility.URI_SAFE_ATTRS.Contains(lower1) || SanitizeSvgUtility.ALLOWED_URI_REGEX.IsMatch(SanitizeSvgUtility.ATTR_WHITESPACE_REGEX.Replace(attrValue, "")) || (lower1 == "src" || lower1 == "xlink:href" || lower1 == "href") && attrValue.StartsWith("data:") && SanitizeSvgUtility.DATA_URI_TAGS.Contains(lower2));
    }
  }
}
