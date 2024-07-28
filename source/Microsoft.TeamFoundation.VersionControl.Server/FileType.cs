// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.FileType
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class FileType : IValidatable, IComparable
  {
    private string m_name;
    private int m_id;
    private List<string> m_extensions = new List<string>();
    private bool m_allowMultipleCheckout;

    [XmlAttribute("name")]
    public string Name
    {
      get => this.m_name;
      set => this.m_name = value.Trim();
    }

    internal int Id
    {
      get => this.m_id;
      set => this.m_id = value;
    }

    public List<string> Extensions
    {
      get => this.m_extensions;
      set => this.m_extensions = value;
    }

    [XmlAttribute("multi")]
    public bool AllowMultipleCheckout
    {
      get => this.m_allowMultipleCheckout;
      set => this.m_allowMultipleCheckout = value;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(this.Name + "; Allow Mult. Checkout=" + (object) this.AllowMultipleCheckout, 256 + this.Extensions.Count * 32);
      if (this.Extensions.Count > 0)
      {
        stringBuilder.Append("; Extensions: ");
        bool flag = true;
        foreach (string extension in this.Extensions)
        {
          if (!flag)
            stringBuilder.Append(", ");
          stringBuilder.Append(extension);
          flag = false;
        }
      }
      return stringBuilder.ToString();
    }

    void IValidatable.Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      if (this.Extensions == null || this.Extensions.Count == 0)
        throw new ArgumentNullException(parameterName);
      if (string.IsNullOrEmpty(this.Name))
        throw new ArgumentNullException(parameterName);
      if (this.Name.Length > 64)
        throw new ArgumentOutOfRangeException(parameterName);
    }

    public int CompareTo(object o) => TFStringComparer.FileType.Compare(this.Name, (o as FileType)?.Name);
  }
}
