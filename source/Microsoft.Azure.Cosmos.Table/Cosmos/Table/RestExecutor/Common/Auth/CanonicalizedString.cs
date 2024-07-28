// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.Common.Auth.CanonicalizedString
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System.Text;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.Common.Auth
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
