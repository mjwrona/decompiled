// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.ImageAssemblyAnalysis.LogModel.AssembledImage
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Globalization;
using System.Xml.Linq;
using WebGrease.Css.Extensions;
using WebGrease.Extensions;

namespace WebGrease.Css.ImageAssemblyAnalysis.LogModel
{
  internal class AssembledImage
  {
    internal AssembledImage()
    {
    }

    internal AssembledImage(XContainer element, int? spriteWidth, int? spriteHeight)
    {
      this.SpriteWidth = spriteWidth;
      this.SpriteHeight = spriteHeight;
      if (element == null)
        return;
      element.Elements().ForEach<XElement>(new Action<XElement>(this.ParseElement));
    }

    internal int? SpriteWidth { get; private set; }

    internal int? SpriteHeight { get; private set; }

    internal string RelativeOutputFilePath { get; set; }

    internal string OutputFilePath { get; set; }

    internal string OriginalFilePath { get; set; }

    internal int? X { get; private set; }

    internal int? Y { get; private set; }

    internal WebGrease.ImageAssemble.ImagePosition? ImagePosition { get; private set; }

    private static int LoadDimension(XElement element)
    {
      int result;
      if (int.TryParse(element.Value, out result))
        return result;
      throw new ImageAssembleException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, CssStrings.InvalidDimensionsError, new object[1]
      {
        (object) element.Name
      }));
    }

    private void ParseElement(XElement childElement)
    {
      switch (childElement.Name.ToString())
      {
        case "originalfile":
          this.OriginalFilePath = childElement.Value.GetFullPathWithLowercase();
          break;
        case "xposition":
          this.X = new int?(AssembledImage.LoadDimension(childElement));
          break;
        case "yposition":
          this.Y = new int?(AssembledImage.LoadDimension(childElement));
          break;
        case "positioninsprite":
          this.ImagePosition = new WebGrease.ImageAssemble.ImagePosition?((WebGrease.ImageAssemble.ImagePosition) Enum.Parse(typeof (WebGrease.ImageAssemble.ImagePosition), childElement.Value));
          break;
      }
    }
  }
}
