// Decompiled with JetBrains decompiler
// Type: WebGrease.Activities.RenamedFile
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace WebGrease.Activities
{
  internal sealed class RenamedFile
  {
    public RenamedFile(XContainer fileElement)
    {
      if (fileElement == null)
        throw new ArgumentNullException(nameof (fileElement), "The fileElement cannot be null.");
      this.InputNames = new List<string>();
      XElement xelement = fileElement.Element((XName) "Output");
      if (xelement != null)
        this.OutputName = xelement.Value;
      foreach (XElement element in fileElement.Elements((XName) "Input"))
        this.InputNames.Add(element.Value);
    }

    public string OutputName { get; private set; }

    public List<string> InputNames { get; private set; }
  }
}
