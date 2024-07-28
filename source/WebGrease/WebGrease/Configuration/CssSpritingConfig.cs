// Decompiled with JetBrains decompiler
// Type: WebGrease.Configuration.CssSpritingConfig
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using WebGrease.Extensions;
using WebGrease.ImageAssemble;

namespace WebGrease.Configuration
{
  public class CssSpritingConfig : INamedConfig
  {
    public CssSpritingConfig()
    {
      this.ShouldAutoSprite = true;
      this.ImagePadding = 50;
      this.ShouldAutoVersionBackgroundImages = true;
      this.ImagesToIgnore = (IEnumerable<string>) new string[0];
      this.DestinationImageFolder = "images";
      this.OutputUnitFactor = 1.0;
    }

    public CssSpritingConfig(XElement element)
      : this()
    {
      this.Name = (string) element.Attribute((XName) "config") ?? string.Empty;
      foreach (XElement descendant in element.Descendants())
      {
        string str1 = descendant.Name.ToString();
        string str2 = descendant.Value;
        switch (str1)
        {
          case nameof (ForceImageType):
            this.ForceImageType = str2.TryParseToEnum<ImageType>();
            continue;
          case nameof (ImagePadding):
            this.ImagePadding = str2.TryParseInt32();
            continue;
          case nameof (ImagesToIgnore):
            object obj;
            if (!str2.IsNullOrWhitespace())
              obj = (object) ((IEnumerable<string>) str2.Split(',')).Distinct<string>();
            else
              obj = (object) new string[0];
            this.ImagesToIgnore = (IEnumerable<string>) obj;
            continue;
          case "AutoVersionBackgroundImages":
            this.ShouldAutoVersionBackgroundImages = str2.TryParseBool();
            continue;
          case "SpriteImages":
            this.ShouldAutoSprite = str2.TryParseBool();
            continue;
          case nameof (WriteLogFile):
            this.WriteLogFile = str2.TryParseBool();
            continue;
          case nameof (ErrorOnInvalidSprite):
            this.ErrorOnInvalidSprite = str2.TryParseBool();
            continue;
          case nameof (OutputUnit):
            this.OutputUnit = str2;
            continue;
          case nameof (OutputUnitFactor):
            double result;
            if (double.TryParse(str2, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result))
            {
              this.OutputUnitFactor = result;
              continue;
            }
            continue;
          case nameof (IgnoreImagesWithNonDefaultBackgroundSize):
            this.IgnoreImagesWithNonDefaultBackgroundSize = str2.TryParseBool();
            continue;
          default:
            continue;
        }
      }
    }

    public string Name { get; internal set; }

    public int ImagePadding { get; internal set; }

    public IEnumerable<string> ImagesToIgnore { get; internal set; }

    internal bool ShouldAutoVersionBackgroundImages { get; set; }

    internal bool ShouldAutoSprite { get; set; }

    internal string DestinationImageFolder { get; set; }

    internal string OutputUnit { get; set; }

    internal double OutputUnitFactor { get; set; }

    internal bool IgnoreImagesWithNonDefaultBackgroundSize { get; set; }

    internal bool WriteLogFile { get; set; }

    internal bool ErrorOnInvalidSprite { get; set; }

    internal ImageType? ForceImageType { get; set; }
  }
}
