// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.EmailSpecification.Email
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

namespace Microsoft.VisualStudio.Services.PyPi.Server.EmailSpecification
{
  public class Email
  {
    public string Name { get; }

    public string EmailAddress { get; }

    public Email(string name, string email)
    {
      this.Name = name;
      this.EmailAddress = email;
    }

    public override string ToString()
    {
      if (string.IsNullOrEmpty(this.Name))
        return this.EmailAddress;
      return "\"" + this.Name + "\" <" + this.EmailAddress + ">";
    }

    internal string Dump(string indent, string newline)
    {
      if (string.IsNullOrEmpty(this.Name))
        return "Email(" + this.EmailAddress + ")";
      return "Email(" + this.Name + ", " + this.EmailAddress + ")";
    }
  }
}
