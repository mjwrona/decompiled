// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PluginRecord
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Xml;

namespace Microsoft.TeamFoundation
{
  public abstract class PluginRecord
  {
    private readonly string id;
    private readonly string name;
    private readonly string description;
    private readonly Guid guid;
    private readonly bool enabled;
    private readonly XmlNode pluginData;

    public string Id => this.Id;

    public string Name => this.name;

    public Guid GUID => this.guid;

    public string Description => this.description;

    public XmlNode PluginData => this.pluginData != null ? this.pluginData.Clone() : (XmlNode) null;

    public bool Enabled => this.enabled;

    public abstract object ActivatePlugin(Type pluginInterface);

    public abstract object ActivatePlugin(Guid serviceType);

    protected PluginRecord(
      string id,
      string name,
      Guid guid,
      string description,
      bool enabled,
      XmlNode pluginData)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(id, nameof (id));
      this.id = id.Trim();
      this.guid = guid;
      this.name = name == null ? string.Empty : name.Trim();
      this.description = description == null ? string.Empty : description;
      this.enabled = enabled;
      this.pluginData = pluginData;
    }
  }
}
