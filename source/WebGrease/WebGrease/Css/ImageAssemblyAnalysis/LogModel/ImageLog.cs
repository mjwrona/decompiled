// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageLog
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using WebGrease.Css.Extensions;
using WebGrease.Extensions;

namespace WebGrease.Css.ImageAssemblyAnalysis.LogModel
{
  internal class ImageLog
  {
    internal ImageLog() => this.InputImages = new List<AssembledImage>();

    internal ImageLog(XDocument imageMapDocument)
      : this()
    {
      if (imageMapDocument == null)
        throw new ArgumentNullException(nameof (imageMapDocument));
      if (imageMapDocument.Root == null)
        return;
      imageMapDocument.Root.Elements((XName) "output").ForEach<XElement>(new Action<XElement>(this.ProcessOutputElement));
    }

    internal List<AssembledImage> InputImages { get; private set; }

    private void ProcessOutputElement(XElement outputElement)
    {
      int? spriteWidth = (int?) outputElement.Attribute((XName) "width");
      int? spriteHeight = (int?) outputElement.Attribute((XName) "height");
      XAttribute xattribute = outputElement.Attribute((XName) "file");
      if (xattribute == null)
        return;
      string outputFilePath = xattribute.Value;
      if (string.IsNullOrWhiteSpace(outputFilePath))
        return;
      outputFilePath = outputFilePath.GetFullPathWithLowercase();
      if (!File.Exists(outputFilePath))
        throw new FileNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, CssStrings.FileNotFoundError, new object[1]
        {
          (object) outputFilePath
        }));
      outputElement.Descendants((XName) "input").ForEach<XElement>((Action<XElement>) (inputElement => this.ProcessInputElement(inputElement, spriteWidth, spriteHeight, outputFilePath)));
    }

    private void ProcessInputElement(
      XElement inputElement,
      int? spriteWidth,
      int? spriteHeight,
      string outputFilePath)
    {
      this.InputImages.Add(new AssembledImage((XContainer) inputElement, spriteWidth, spriteHeight)
      {
        OutputFilePath = outputFilePath
      });
    }
  }
}
