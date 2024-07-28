// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.CanonicalizedString
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System.Text;

namespace Microsoft.Azure.Storage.Core
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
