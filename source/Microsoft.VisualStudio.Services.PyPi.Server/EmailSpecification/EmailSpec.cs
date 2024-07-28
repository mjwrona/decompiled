// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.EmailSpecification.EmailSpec
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.PyPi.Server.EmailSpecification
{
  public class EmailSpec
  {
    public IEnumerable<Email> Emails { get; }

    public EmailSpec(IEnumerable<Email> emails) => this.Emails = (IEnumerable<Email>) emails.ToList<Email>();

    public override string ToString() => string.Join(",", this.Emails.Select<Email, string>((Func<Email, string>) (e => e.ToString())));

    public string Dump(string indent, string newline) => "Emails(" + string.Join(", ", this.Emails.Select<Email, string>((Func<Email, string>) (x => x.Dump(indent, newline)))) + ")";
  }
}
