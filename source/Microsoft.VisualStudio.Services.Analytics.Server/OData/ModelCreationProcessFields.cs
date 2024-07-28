// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.ModelCreationProcessFields
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.OData
{
  public class ModelCreationProcessFields
  {
    public ModelCreationProcessFields()
      : this(new List<ProcessFieldInfo>())
    {
    }

    public ModelCreationProcessFields(List<ProcessFieldInfo> processFields)
    {
      ArgumentUtility.CheckForNull<List<ProcessFieldInfo>>(processFields, nameof (processFields));
      this.ProcessFields = processFields.OrderBy<ProcessFieldInfo, string>((Func<ProcessFieldInfo, string>) (f => f.ReferenceName)).ToList<ProcessFieldInfo>();
      this.Level = !this.ProcessFields.Any<ProcessFieldInfo>() ? 0 : this.ProcessFields.Max<ProcessFieldInfo>((Func<ProcessFieldInfo, int>) (f => f.TableLevel));
    }

    public List<ProcessFieldInfo> ProcessFields { get; private set; }

    public List<ProcessFieldInfo> CustomProcessFields => this.ProcessFields.Where<ProcessFieldInfo>((Func<ProcessFieldInfo, bool>) (f => !string.IsNullOrEmpty(f.TableName))).ToList<ProcessFieldInfo>();

    public List<ProcessFieldInfo> BuiltInProcessFields => this.ProcessFields.Where<ProcessFieldInfo>((Func<ProcessFieldInfo, bool>) (f => string.IsNullOrEmpty(f.TableName))).ToList<ProcessFieldInfo>();

    public bool HasCustomFields => this.CustomProcessFields.Any<ProcessFieldInfo>();

    public int Level { get; }

    public override bool Equals(object obj)
    {
      if (!(obj is ModelCreationProcessFields creationProcessFields) || this.ProcessFields.Count != creationProcessFields.ProcessFields.Count)
        return false;
      for (int index = 0; index < this.ProcessFields.Count; ++index)
      {
        if (!this.ProcessFields[index].Equals((object) creationProcessFields.ProcessFields[index]))
          return false;
      }
      return true;
    }

    public override int GetHashCode()
    {
      int hashCode = -2128831035;
      for (int index = 0; index < this.ProcessFields.Count; ++index)
        hashCode = hashCode * 16777619 ^ this.ProcessFields[index].GetHashCode();
      return hashCode;
    }
  }
}
