// Decompiled with JetBrains decompiler
// Type: WebGrease.Activities.RenamedFilesLog
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace WebGrease.Activities
{
  internal sealed class RenamedFilesLog
  {
    internal RenamedFilesLog(string logFile)
    {
      this.RenamedFiles = new List<RenamedFile>();
      if (string.IsNullOrWhiteSpace(logFile) || !File.Exists(logFile))
        return;
      XElement xelement = XDocument.Load(logFile).Element((XName) nameof (RenamedFiles));
      if (xelement == null)
        return;
      foreach (XContainer element in xelement.Elements((XName) "File"))
        this.RenamedFiles.Add(new RenamedFile(element));
    }

    internal List<RenamedFile> RenamedFiles { get; private set; }
  }
}
