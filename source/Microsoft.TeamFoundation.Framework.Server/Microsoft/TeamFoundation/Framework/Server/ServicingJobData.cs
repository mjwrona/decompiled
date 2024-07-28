// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingJobData
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [XmlInclude(typeof (TeamProjectCollectionProperties))]
  [XmlInclude(typeof (IdentityDescriptor))]
  [XmlInclude(typeof (DictionaryWrapper<string, string>))]
  [XmlInclude(typeof (SqlConnectionInfoWrapper))]
  [XmlInclude(typeof (DictionaryWrapper<string, SqlConnectionInfoWrapper>))]
  public class ServicingJobData
  {
    public ServicingJobData()
    {
      this.ServicingItems = (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.ServicingTokens = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public ServicingJobData(params string[] servicingOperations)
      : this()
    {
      this.ServicingOperations = servicingOperations;
    }

    [XmlAttribute("title")]
    public string JobTitle { get; set; }

    [XmlAttribute("class")]
    public string OperationClass { get; set; }

    public TeamFoundationLockInfo[] ServicingLocks { get; set; }

    public string[] ServicingOperations { get; set; }

    public ServicingOperationTarget ServicingOperationTarget { get; set; }

    [XmlAttribute("targetid")]
    public Guid ServicingHostId { get; set; }

    [XmlIgnore]
    public ServicingFlags ServicingOptions { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    [XmlAttribute("options")]
    public int ServicingOptionsInt32
    {
      get => (int) this.ServicingOptions;
      set => this.ServicingOptions = (ServicingFlags) value;
    }

    [XmlIgnore]
    public IDictionary<string, object> ServicingItems { get; set; }

    [XmlIgnore]
    public IDictionary<string, string> ServicingTokens { get; set; }

    [XmlElement("ServicingItems", typeof (KeyValue<string, object>[]))]
    public KeyValue<string, object>[] ServicingItemsValue
    {
      get => KeyValue<string, object>.ConvertToArray((ICollection<KeyValuePair<string, object>>) this.ServicingItems);
      set => this.ServicingItems = (IDictionary<string, object>) KeyValue<string, object>.ConvertToDictionary((IEnumerable<KeyValue<string, object>>) value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    [XmlElement("ServicingTokens", typeof (KeyValue<string, string>[]))]
    public KeyValue<string, string>[] ServicingTokensValue
    {
      get => KeyValue<string, string>.ConvertToArray((ICollection<KeyValuePair<string, string>>) this.ServicingTokens);
      set => this.ServicingTokens = (IDictionary<string, string>) KeyValue<string, string>.ConvertToDictionary((IEnumerable<KeyValue<string, string>>) value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }
  }
}
