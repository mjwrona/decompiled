// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysisLog
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using WebGrease.Css.Extensions;
using WebGrease.ImageAssemble;

namespace WebGrease.Css.ImageAssemblyAnalysis.LogModel
{
  public class ImageAssemblyAnalysisLog
  {
    private const string PxMessage = "No declaration with absolute vertical position found.";
    private const string NoUrlMessage = "No declaration with background url.";
    private const string NoRepeatMessage = "No declaration with background 'no-repeat'.";
    private const string IgnoreUrlMessage = "The image url is configured to ignore in locale resx file.";
    private const string InvalidDpiMessage = "-wg-dpi was set but was invalid.";
    private const string SpritingIgnoredMessage = "-wg-spriting: ignore.";
    private const string MultipleUrlsMessage = "Multiple url's in a single background are not supported by webgrease at this time.";
    private const string BackgroundRepeatInvalidMessage = "Background-repeat value was invalid (only no-repeat allows spriting)";
    private readonly List<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis> logNodes = new List<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis>();

    internal IEnumerable<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis> FailedSprites => this.logNodes.Where<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis>((Func<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis, bool>) (ln =>
    {
      if (ln.FailureReason.HasValue)
      {
        FailureReason? failureReason1 = ln.FailureReason;
        if ((failureReason1.GetValueOrDefault() != FailureReason.NoUrl ? 1 : (!failureReason1.HasValue ? 1 : 0)) != 0)
        {
          FailureReason? failureReason2 = ln.FailureReason;
          if ((failureReason2.GetValueOrDefault() != FailureReason.IgnoreUrl ? 1 : (!failureReason2.HasValue ? 1 : 0)) != 0)
          {
            FailureReason? failureReason3 = ln.FailureReason;
            return failureReason3.GetValueOrDefault() != FailureReason.SpritingIgnore || !failureReason3.HasValue;
          }
        }
      }
      return false;
    }));

    internal static string GetFailureMessage(WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis analysis)
    {
      FailureReason? failureReason = analysis.FailureReason;
      ref FailureReason? local = ref failureReason;
      FailureReason valueOrDefault = local.GetValueOrDefault();
      if (!local.HasValue)
        return "No failure";
      switch (valueOrDefault)
      {
        case FailureReason.IncorrectPosition:
          return "No declaration with absolute vertical position found.";
        case FailureReason.BackgroundSizeIsSetToNonDefaultValue:
          return "The image url is configured to ignore in locale resx file.";
        case FailureReason.InvalidDpi:
          return "-wg-dpi was set but was invalid.";
        case FailureReason.BackgroundRepeatInvalid:
          return "Background-repeat value was invalid (only no-repeat allows spriting)";
        case FailureReason.MultipleUrls:
          return "Multiple url's in a single background are not supported by webgrease at this time.";
        case FailureReason.NoRepeat:
          return "No declaration with background 'no-repeat'.";
        case FailureReason.NoUrl:
          return "No declaration with background url.";
        case FailureReason.IgnoreUrl:
          return "The image url is configured to ignore in locale resx file.";
        case FailureReason.SpritingIgnore:
          return "-wg-spriting: ignore.";
        default:
          return "Unknown failure reason";
      }
    }

    internal void Add(WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis logNode)
    {
      if (logNode == null)
        return;
      this.logNodes.Add(logNode);
    }

    internal void UpdateSpritedImage(ImageType imageType, string imagePath, string spritedImage) => this.logNodes.Where<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis>((Func<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis, bool>) (ln =>
    {
      string image = ln.Image;
      return image != null && image.Equals(imagePath, StringComparison.OrdinalIgnoreCase);
    })).ForEach<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis>((Action<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis>) (i =>
    {
      i.ImageType = new ImageType?(imageType);
      i.SpritedImage = spritedImage;
    }));

    internal void Save(string path)
    {
      if (!this.logNodes.Any<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis>())
        return;
      IEnumerable<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis> source1 = this.logNodes.Where<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis>((Func<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis, bool>) (ln => !ln.FailureReason.HasValue));
      IEnumerable<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis> source2 = this.logNodes.Where<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis>((Func<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis, bool>) (ln => ln.FailureReason.HasValue)).Where<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis>((Func<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis, bool>) (ln =>
      {
        FailureReason? failureReason = ln.FailureReason;
        return failureReason.GetValueOrDefault() != FailureReason.NoUrl || !failureReason.HasValue;
      }));
      IEnumerable<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis> source3 = source2.Where<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis>((Func<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis, bool>) (ln =>
      {
        FailureReason? failureReason1 = ln.FailureReason;
        if ((failureReason1.GetValueOrDefault() != FailureReason.IgnoreUrl ? 0 : (failureReason1.HasValue ? 1 : 0)) != 0)
          return true;
        FailureReason? failureReason2 = ln.FailureReason;
        return failureReason2.GetValueOrDefault() == FailureReason.SpritingIgnore && failureReason2.HasValue;
      }));
      new XElement((XName) "SpritingLog", new object[3]
      {
        (object) new XElement((XName) "Failed", (object) source2.Where<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis>((Func<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis, bool>) (ln =>
        {
          FailureReason? failureReason3 = ln.FailureReason;
          if ((failureReason3.GetValueOrDefault() != FailureReason.IgnoreUrl ? 1 : (!failureReason3.HasValue ? 1 : 0)) == 0)
            return false;
          FailureReason? failureReason4 = ln.FailureReason;
          return failureReason4.GetValueOrDefault() != FailureReason.SpritingIgnore || !failureReason4.HasValue;
        })).OrderBy<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis, FailureReason?>((Func<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis, FailureReason?>) (i => i.FailureReason)).Select<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis, XElement>(new Func<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis, XElement>(ImageAssemblyAnalysisLog.LogNodeToXElement))),
        (object) new XElement((XName) "Ignored", (object) source3.OrderBy<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis, FailureReason?>((Func<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis, FailureReason?>) (i => i.FailureReason)).Select<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis, XElement>(new Func<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis, XElement>(ImageAssemblyAnalysisLog.LogNodeToXElement))),
        (object) source1.GroupBy(ln => new
        {
          SpritedImage = ln.SpritedImage,
          ImageType = ln.ImageType
        }).Select<IGrouping<\u003C\u003Ef__AnonymousTyped<string, ImageType?>, WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis>, XElement>(logNode =>
        {
          XElement xelement = new XElement((XName) "Sprited", (object) logNode.Select<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis, XElement>(new Func<WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis, XElement>(ImageAssemblyAnalysisLog.LogNodeToXElement)));
          if (logNode.Key.SpritedImage != null)
            xelement.Add((object) new XAttribute((XName) "SpritedImage", (object) logNode.Key.SpritedImage));
          if (logNode.Key.ImageType.HasValue)
            xelement.Add((object) new XAttribute((XName) "ImageType", (object) logNode.Key.ImageType));
          return xelement;
        })
      }).Save(path);
    }

    private static XElement LogNodeToXElement(WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis logNode)
    {
      XElement xelement = new XElement((XName) "SpriteItem", (object) (Environment.NewLine + logNode.AstNode.PrettyPrint() + "\t"));
      if (logNode.FailureReason.HasValue)
      {
        xelement.Add((object) new XAttribute((XName) "FailureReason", (object) logNode.FailureReason));
        xelement.Add((object) new XAttribute((XName) "FailureMessage", (object) ImageAssemblyAnalysisLog.GetFailureMessage(logNode)));
      }
      if (logNode.Image != null)
        xelement.Add((object) new XAttribute((XName) "Image", (object) logNode.Image));
      return xelement;
    }
  }
}
