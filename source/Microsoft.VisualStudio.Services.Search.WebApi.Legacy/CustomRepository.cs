// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.CustomRepository
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.CustomRepository;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.WebApi
{
  [DataContract]
  public class CustomRepository
  {
    [DataMember(Name = "repositoryId")]
    public Guid RepositoryId { get; set; }

    [DataMember(Name = "projectName")]
    public string ProjectName { get; set; }

    [DataMember(Name = "repositoryName")]
    public string RepositoryName { get; set; }

    [DataMember(Name = "repositorySize")]
    public int RepositorySize { get; set; }

    [DataMember(Name = "properties")]
    public CustomRepositoryProperties Properties { get; set; }

    [DataMember(Name = "CustomIndexingMode")]
    public virtual CustomIndexingMode CustomIndexingMode { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "indentLevel+1", Justification = "indentLevel is not likely to exceed int.Max range.")]
    public string ToString(int indentLevel)
    {
      StringBuilder sb = new StringBuilder();
      string indentSpacing = Extensions.GetIndentSpacing(indentLevel);
      sb.Append(indentSpacing, "Repository Id: ").AppendLine(this.RepositoryId.ToString());
      sb.Append(indentSpacing, "Project Name: ").AppendLine(this.ProjectName);
      sb.Append(indentSpacing, "Repository Name: ").AppendLine(this.RepositoryName);
      sb.Append(indentSpacing, "Repository Size: ").AppendLine(this.RepositorySize.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (this.Properties != null)
        sb.Append(indentSpacing, "Repository Properties: ").AppendLine(this.Properties.ToString(indentLevel + 1));
      sb.Append(indentSpacing, "CustomIndexingMode: ").AppendLine(this.CustomIndexingMode.ToString());
      sb.Append(indentSpacing, "Repository Size: ").AppendLine(this.RepositorySize.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return sb.ToString();
    }

    public override string ToString() => this.ToString(0);
  }
}
