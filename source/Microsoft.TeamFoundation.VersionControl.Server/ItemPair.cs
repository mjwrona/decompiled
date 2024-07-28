// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ItemPair
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ItemPair : ICacheable, IPropertyMergerItem
  {
    private Item m_previous;
    private Item m_current;
    private ChangeType m_changeType;
    private int m_previousLineCount = -2;
    private DiffSummary m_diffSummary;
    private int m_retryCount = -1;

    internal Item PreviousItem
    {
      get => this.m_previous;
      set => this.m_previous = value;
    }

    internal Item CurrentItem
    {
      get => this.m_current;
      set => this.m_current = value;
    }

    internal ChangeType ChangeType
    {
      get => this.m_changeType;
      set => this.m_changeType = value;
    }

    internal int PreviousLineCount
    {
      get
      {
        if (this.m_previousLineCount == -2)
        {
          this.m_previousLineCount = -1;
          if (this.Properties != null)
          {
            using (IEnumerator<PropertyValue> enumerator = this.Properties.GetEnumerator())
            {
              if (enumerator.MoveNext())
              {
                if (enumerator.Current.Value is byte[] byteValue)
                  CodeChurnUtility.ConvertToInt(byteValue, out this.m_previousLineCount, out int _, out int _, out int _);
              }
            }
          }
        }
        return this.m_previousLineCount;
      }
    }

    internal DiffSummary DiffSummary
    {
      get => this.m_diffSummary;
      set => this.m_diffSummary = value;
    }

    internal int RetryCount
    {
      get => this.m_retryCount;
      set => this.m_retryCount = value;
    }

    internal void WriteInfo(StringBuilder builder)
    {
      builder.Append("ItemPair instance ");
      builder.AppendLine(this.GetHashCode().ToString((IFormatProvider) CultureInfo.InvariantCulture));
      builder.Append("  PreviousItem: ");
      int changesetId;
      if (this.m_previous != null)
      {
        builder.AppendLine();
        builder.Append("        ServerItem: ");
        builder.AppendLine(this.m_previous.ServerItem);
        builder.Append("        ChangesetId: ");
        StringBuilder stringBuilder = builder;
        changesetId = this.m_previous.ChangesetId;
        string str = changesetId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        stringBuilder.AppendLine(str);
      }
      else
        builder.AppendLine("null");
      builder.Append("  CurrentItem: ");
      if (this.m_current != null)
      {
        builder.AppendLine();
        builder.Append("        ServerItem: ");
        builder.AppendLine(this.m_current.ServerItem);
        builder.Append("        ChangesetId: ");
        StringBuilder stringBuilder = builder;
        changesetId = this.m_current.ChangesetId;
        string str = changesetId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        stringBuilder.AppendLine(str);
      }
      else
        builder.AppendLine("null");
      builder.Append("  ChangeType: ");
      builder.AppendLine(this.m_changeType.ToString());
      builder.Append("  PreviousLineCount: ");
      builder.AppendLine(this.m_previousLineCount.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      builder.Append("  DiffSummary: ");
      builder.AppendLine(this.m_diffSummary != null ? this.m_diffSummary.ToString() : "null");
      builder.Append("  RetryCount: ");
      builder.AppendLine(this.m_retryCount.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    public int GetCachedSize()
    {
      int cachedSize = 22;
      if (this.CurrentItem != null)
        cachedSize += this.CurrentItem.GetCachedSize();
      if (this.PreviousItem != null)
        cachedSize += this.PreviousItem.GetCachedSize();
      return cachedSize;
    }

    public ArtifactSpec GetArtifactSpec(Guid artifactKind) => this.PreviousItem == null ? (ArtifactSpec) null : new ArtifactSpec(artifactKind, this.PreviousItem.ItemId, this.PreviousItem.ChangesetId, this.PreviousItem.ItemDataspaceId);

    public StreamingCollection<PropertyValue> Properties { get; set; }

    StreamingCollection<PropertyValue> IPropertyMergerItem.GetProperties(Guid artifactKind) => this.Properties;

    void IPropertyMergerItem.SetProperties(
      Guid artifactKind,
      StreamingCollection<PropertyValue> properties)
    {
      this.Properties = properties;
    }

    public int SequenceId { get; set; }
  }
}
