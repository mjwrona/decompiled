// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FrameworkRegistrationEntry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [SoapType(TypeName = "RegistrationEntry")]
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Public)]
  public class FrameworkRegistrationEntry : ICloneable
  {
    private string m_Type;
    private RegistrationServiceInterface[] m_ServiceInterfaces;
    private RegistrationDatabase[] m_Databases;
    private RegistrationArtifactType[] m_ArtifactTypes;
    private RegistrationExtendedAttribute2[] m_RegistrationExtendedAttributes;
    private RegistrationChangeType m_ChangeType = RegistrationChangeType.NoChange;

    public string Type
    {
      get => this.m_Type;
      set => this.m_Type = value;
    }

    [XmlArrayItem("ServiceInterface")]
    public RegistrationServiceInterface[] ServiceInterfaces
    {
      get => this.m_ServiceInterfaces;
      set => this.m_ServiceInterfaces = value;
    }

    [XmlArrayItem("Database")]
    public RegistrationDatabase[] Databases
    {
      get => this.m_Databases;
      set => this.m_Databases = value;
    }

    [XmlArrayItem("ArtifactType")]
    public RegistrationArtifactType[] ArtifactTypes
    {
      get => this.m_ArtifactTypes;
      set => this.m_ArtifactTypes = value;
    }

    [XmlArrayItem("RegistrationExtendedAttribute")]
    public RegistrationExtendedAttribute2[] RegistrationExtendedAttributes
    {
      get => this.m_RegistrationExtendedAttributes;
      set => this.m_RegistrationExtendedAttributes = value;
    }

    [XmlIgnore]
    public RegistrationChangeType ChangeType
    {
      get => this.m_ChangeType;
      set => this.m_ChangeType = value;
    }

    internal static T[] MergeArrays<T>(
      T[] firstOne,
      T[] secondOne,
      IComparer<T> comparer,
      ICombiner<T> combiner)
    {
      if (firstOne == null || secondOne == null)
        return firstOne;
      T[] array1 = (T[]) firstOne.Clone();
      T[] array2 = (T[]) secondOne.Clone();
      List<T> objList = new List<T>();
      Array.Sort<T>(array1, comparer);
      Array.Sort<T>(array2, comparer);
      int index1 = 0;
      int index2 = 0;
      if (array1.Length == 0)
        return array2;
      if (array2.Length == 0)
        return array1;
      while (index1 < array1.Length && index2 < array2.Length)
      {
        if (comparer.Compare(array1[index1], array2[index2]) < 0)
          objList.Add(array1[index1++]);
        else if (comparer.Compare(array1[index1], array2[index2]) == 0)
        {
          objList.Add(combiner.Combine(array1[index1], array2[index2]));
          ++index1;
          ++index2;
        }
        else
          objList.Add(array2[index2++]);
      }
      while (index1 < array1.Length)
        objList.Add(array1[index1++]);
      while (index2 < array2.Length)
        objList.Add(array2[index2++]);
      return objList.ToArray();
    }

    internal RegistrationExtendedAttribute2 GetExtendedAttribute(string name) => this.RegistrationExtendedAttributes != null ? Array.Find<RegistrationExtendedAttribute2>(this.RegistrationExtendedAttributes, new RegistrationExtendedAttribute2(name, string.Empty).EqualsByName()) : (RegistrationExtendedAttribute2) null;

    internal Predicate<FrameworkRegistrationEntry> EqualsByName() => (Predicate<FrameworkRegistrationEntry>) (that => VssStringComparer.RegistrationEntryName.Equals(this.Type, that.Type));

    object ICloneable.Clone() => (object) this.Clone();

    public FrameworkRegistrationEntry DeepCloneForArtifactTypes()
    {
      FrameworkRegistrationEntry registrationEntry = this.Clone();
      RegistrationArtifactType[] artifactTypes = this.m_ArtifactTypes;
      registrationEntry.ArtifactTypes = artifactTypes != null ? ((IEnumerable<RegistrationArtifactType>) artifactTypes).Select<RegistrationArtifactType, RegistrationArtifactType>((Func<RegistrationArtifactType, RegistrationArtifactType>) (t => t.DeepClone())).ToArray<RegistrationArtifactType>() : (RegistrationArtifactType[]) null;
      return registrationEntry;
    }

    public FrameworkRegistrationEntry Clone() => new FrameworkRegistrationEntry()
    {
      ArtifactTypes = (RegistrationArtifactType[]) this.m_ArtifactTypes.Clone(),
      ChangeType = this.m_ChangeType,
      Databases = (RegistrationDatabase[]) this.m_Databases.Clone(),
      RegistrationExtendedAttributes = (RegistrationExtendedAttribute2[]) this.m_RegistrationExtendedAttributes.Clone(),
      ServiceInterfaces = (RegistrationServiceInterface[]) this.m_ServiceInterfaces.Clone(),
      Type = this.m_Type
    };
  }
}
