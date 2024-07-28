// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.Tokens.TagDirective
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace YamlDotNet.Core.Tokens
{
  [Serializable]
  public class TagDirective : Token
  {
    private readonly string handle;
    private readonly string prefix;
    private static readonly Regex tagHandleValidator = new Regex("^!([0-9A-Za-z_\\-]*!)?$", RegexOptions.Compiled);

    public string Handle => this.handle;

    public string Prefix => this.prefix;

    public TagDirective(string handle, string prefix)
      : this(handle, prefix, Mark.Empty, Mark.Empty)
    {
    }

    public TagDirective(string handle, string prefix, Mark start, Mark end)
      : base(start, end)
    {
      if (string.IsNullOrEmpty(handle))
        throw new ArgumentNullException(nameof (handle), "Tag handle must not be empty.");
      this.handle = TagDirective.tagHandleValidator.IsMatch(handle) ? handle : throw new ArgumentException("Tag handle must start and end with '!' and contain alphanumerical characters only.", nameof (handle));
      this.prefix = !string.IsNullOrEmpty(prefix) ? prefix : throw new ArgumentNullException(nameof (prefix), "Tag prefix must not be empty.");
    }

    public override bool Equals(object obj) => obj is TagDirective tagDirective && this.handle.Equals(tagDirective.handle) && this.prefix.Equals(tagDirective.prefix);

    public override int GetHashCode() => this.handle.GetHashCode() ^ this.prefix.GetHashCode();

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} => {1}", new object[2]
    {
      (object) this.handle,
      (object) this.prefix
    });
  }
}
