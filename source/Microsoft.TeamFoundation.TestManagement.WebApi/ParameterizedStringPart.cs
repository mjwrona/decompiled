// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.ParameterizedStringPart
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  public struct ParameterizedStringPart
  {
    private static ParameterizedStringPart s_emptyPart;
    private string m_string;
    private bool m_ignoreValue;
    private bool m_isParameter;
    private const string XmlTextEscapeString = "\\";

    public ParameterizedStringPart(string literalText)
    {
      this.m_string = literalText;
      this.m_isParameter = false;
      this.m_ignoreValue = false;
      this.XmlWrapper = literalText;
    }

    public ParameterizedStringPart(string parameterName, bool ignoreValue)
    {
      this.m_string = parameterName;
      this.m_isParameter = true;
      this.m_ignoreValue = ignoreValue;
    }

    public string ParameterName => this.m_string;

    public string XmlWrapper
    {
      get
      {
        string xmlWrapper = this.m_string != null ? this.m_string.ToString() : string.Empty;
        if (!string.IsNullOrEmpty(xmlWrapper) && (string.IsNullOrEmpty(xmlWrapper.Trim()) || xmlWrapper.StartsWith("\\", StringComparison.Ordinal)))
          xmlWrapper = xmlWrapper.Insert(0, "\\");
        return xmlWrapper;
      }
      private set
      {
        string str = value;
        if (!string.IsNullOrEmpty(str) && str.StartsWith("\\", StringComparison.Ordinal))
          str = str.Remove(0, "\\".Length);
        this.m_string = str;
      }
    }

    public string LiteralValue => this.m_string ?? string.Empty;

    public bool IgnoreValue => this.m_ignoreValue;

    public bool IsParameter => this.m_isParameter;

    public bool Equals(ParameterizedStringPart other) => this.m_isParameter == other.m_isParameter && this.m_ignoreValue == other.m_ignoreValue && string.Equals(this.m_string, other.m_string, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object obj) => obj is ParameterizedStringPart other && this.Equals(other);

    public static bool operator ==(ParameterizedStringPart part1, ParameterizedStringPart part2) => part1.Equals(part2);

    public static bool operator !=(ParameterizedStringPart part1, ParameterizedStringPart part2) => !part1.Equals(part2);

    public override int GetHashCode() => this.m_string.GetHashCode();

    public static ParameterizedStringPart Empty => ParameterizedStringPart.s_emptyPart;
  }
}
