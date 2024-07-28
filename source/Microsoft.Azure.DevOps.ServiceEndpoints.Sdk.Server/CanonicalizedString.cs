// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.CanonicalizedString
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using System.Text;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  internal class CanonicalizedString
  {
    private const int DefaultCapacity = 300;
    private const char ElementDelimiter = '\n';
    private readonly StringBuilder canonicalizedString;

    public CanonicalizedString(string initialElement)
      : this(initialElement, 300)
    {
    }

    public CanonicalizedString(string initialElement, int capacity) => this.canonicalizedString = new StringBuilder(initialElement, capacity);

    public void AppendCanonicalizedElement(string element)
    {
      this.canonicalizedString.Append('\n');
      this.canonicalizedString.Append(element);
    }

    public override string ToString() => this.canonicalizedString.ToString();
  }
}
