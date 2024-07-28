// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.SubjectDescriptorComparer
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Common
{
  public class SubjectDescriptorComparer : 
    IComparer<SubjectDescriptor>,
    IEqualityComparer<SubjectDescriptor>
  {
    private SubjectDescriptorComparer()
    {
    }

    public int Compare(SubjectDescriptor left, SubjectDescriptor right) => SubjectDescriptor.Compare(left, right);

    public bool Equals(SubjectDescriptor left, SubjectDescriptor right) => left == right;

    public int GetHashCode(SubjectDescriptor subjectDescriptor) => subjectDescriptor.GetHashCode();

    public static SubjectDescriptorComparer Instance { get; } = new SubjectDescriptorComparer();
  }
}
