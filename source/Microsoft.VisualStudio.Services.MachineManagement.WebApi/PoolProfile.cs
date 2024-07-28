// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.PoolProfile
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public class PoolProfile
  {
    public override int GetHashCode() => 0;

    public override string ToString() => JsonConvert.SerializeObject((object) this);

    public static PoolProfile ComputeFromPool(MachinePool pool, bool quickAndDirty)
    {
      PoolProfile fromPool = new PoolProfile();
      if (pool.ResourceProviderType == "Azure")
      {
        if (pool.UseNestedVirtualization)
        {
          fromPool.CreateFactoryDiskMachineProvisionedEventTimeout = new int?(300);
          fromPool.DataDiskCount = new int?(7);
          fromPool.DataDiskSizeGB = new int?(256);
          fromPool.FactoryDiskSizeGB = new int?(2048);
          fromPool.FoldAtHomeEnabled = new bool?(true);
          fromPool.DiskFormatExtension = ".vhdx";
          fromPool.EnableNestedInlineImageUpdates = new bool?(false);
          fromPool.DataDiskCachingType = "ReadWrite";
          if (pool.GetRoleSize().StartsWith("Standard_E"))
          {
            fromPool.UseRamDisks = new bool?(true);
            fromPool.RamDiskSizeGB = new int?(8);
          }
          if (quickAndDirty)
            fromPool.FoldAtHomeEnabled = new bool?(false);
        }
        else
        {
          if (Math.Max(Math.Max(GetOSDiskSizeGB(pool.ImageName) ?? int.MaxValue, pool.GetFactoryDiskSize(0)), pool.GetOSDiskSize(0)) <= GetMaxOSDiskGBForEphemeral(pool.GetRoleSize()).GetValueOrDefault())
          {
            fromPool.UseEphemeralDisk = new bool?(true);
            fromPool.UseFactoryGalleryImageVersion = new bool?(true);
            fromPool.FactoryImageReplicaCount = new int?(1);
            fromPool.FactoryImageStorageAccountType = "Premium_LRS";
          }
          if (quickAndDirty)
            fromPool.UseFactoryGalleryImageVersion = new bool?();
        }
      }
      return fromPool;

      static int? GetOSDiskSizeGB(string imageName)
      {
        switch (imageName.ToUpperInvariant())
        {
          case "UBUNTU16":
            return new int?(86);
          case "UBUNTU18":
            return new int?(86);
          case "UBUNTU20":
            return new int?(86);
          case "VS2017":
            return new int?(256);
          case "WINCON":
            return new int?(125);
          case "WINDOWS-2019-VS2019":
            return new int?(256);
          default:
            return new int?();
        }
      }

      static int? GetMaxOSDiskGBForEphemeral(string roleSize)
      {
        string upperInvariant = roleSize.ToUpperInvariant();
        if (upperInvariant != null)
        {
          switch (upperInvariant.Length)
          {
            case 15:
              switch (upperInvariant[11])
              {
                case '1':
                  if (upperInvariant == "STANDARD_DS1_V2")
                    return new int?(43);
                  break;
                case '2':
                  if (upperInvariant == "STANDARD_DS2_V2")
                    return new int?(86);
                  break;
                case '3':
                  if (upperInvariant == "STANDARD_DS3_V2")
                    return new int?(172);
                  break;
                case '4':
                  if (upperInvariant == "STANDARD_DS4_V2")
                    return new int?(344);
                  break;
                case '5':
                  if (upperInvariant == "STANDARD_DS5_V2")
                    return new int?(688);
                  break;
                case 'S':
                  switch (upperInvariant)
                  {
                    case "STANDARD_D2S_V3":
                      return new int?(50);
                    case "STANDARD_D4S_V3":
                      return new int?(100);
                    case "STANDARD_D8S_V3":
                      return new int?(200);
                  }
                  break;
              }
              break;
            case 16:
              switch (upperInvariant[10])
              {
                case '1':
                  if (upperInvariant == "STANDARD_D16S_V3")
                    return new int?(400);
                  break;
                case '3':
                  if (upperInvariant == "STANDARD_D32S_V3")
                    return new int?(800);
                  break;
                case '4':
                  if (upperInvariant == "STANDARD_D48S_V3")
                    return new int?(1200);
                  break;
                case '6':
                  if (upperInvariant == "STANDARD_D64S_V3")
                    return new int?(1600);
                  break;
              }
              break;
          }
        }
        return new int?();
      }
    }

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      return obj is PoolProfile poolProfile && object.Equals((object) this.FactoryImageReplicaCount, (object) poolProfile.FactoryImageReplicaCount) && object.Equals((object) this.FactoryImageStorageAccountType, (object) poolProfile.FactoryImageStorageAccountType) && object.Equals((object) this.UseEphemeralDisk, (object) poolProfile.UseEphemeralDisk) && object.Equals((object) this.UseFactoryGalleryImageVersion, (object) poolProfile.UseFactoryGalleryImageVersion) && object.Equals((object) this.CreateFactoryDiskMachineProvisionedEventTimeout, (object) poolProfile.CreateFactoryDiskMachineProvisionedEventTimeout) && object.Equals((object) this.DataDiskCount, (object) poolProfile.DataDiskCount) && object.Equals((object) this.DataDiskSizeGB, (object) poolProfile.DataDiskSizeGB) && object.Equals((object) this.FactoryDiskSizeGB, (object) poolProfile.FactoryDiskSizeGB) && object.Equals((object) this.FoldAtHomeEnabled, (object) poolProfile.FoldAtHomeEnabled) && object.Equals((object) this.DiskFormatExtension, (object) poolProfile.DiskFormatExtension) && object.Equals((object) this.EnableNestedInlineImageUpdates, (object) poolProfile.EnableNestedInlineImageUpdates) && object.Equals((object) this.DataDiskCachingType, (object) poolProfile.DataDiskCachingType) && object.Equals((object) this.UseRamDisks, (object) poolProfile.UseRamDisks) && object.Equals((object) this.RamDiskSizeGB, (object) poolProfile.RamDiskSizeGB);
    }

    public void SaveToUpdate(PropertiesCollection baseProps, PropertiesCollection updateProps)
    {
      SetIfNeeded<int?>("FactoryImageReplicaCount", this.FactoryImageReplicaCount);
      SetIfNeeded<string>("FactoryImageStorageAccountType", this.FactoryImageStorageAccountType);
      SetIfNeeded<bool?>("UseEphemeralDisk", this.UseEphemeralDisk);
      SetIfNeeded<bool?>("UseFactoryGalleryImageVersion", this.UseFactoryGalleryImageVersion);
      SetIfNeeded<int?>("CreateFactoryDiskMachineProvisionedEventTimeout", this.CreateFactoryDiskMachineProvisionedEventTimeout);
      SetIfNeeded<int?>("DataDiskCount", this.DataDiskCount);
      SetIfNeeded<int?>("DataDiskSizeGB", this.DataDiskSizeGB);
      SetIfNeeded<int?>("FactoryDiskSizeGB", this.FactoryDiskSizeGB);
      SetIfNeeded<bool?>("FoldAtHomeEnabled", this.FoldAtHomeEnabled);
      SetIfNeeded<string>("DiskFormatExtension", this.DiskFormatExtension);
      SetIfNeeded<bool?>("EnableNestedInlineImageUpdates", this.EnableNestedInlineImageUpdates);
      SetIfNeeded<string>("DataDiskCachingType", this.DataDiskCachingType);
      SetIfNeeded<bool?>("UseRamDisks", this.UseRamDisks);
      SetIfNeeded<int?>("RamDiskSizeGB", this.RamDiskSizeGB);

      void SetIfNeeded<T>(string key, T value)
      {
        object objA;
        baseProps.TryGetValue(key, out objA);
        bool result;
        if (bool.TryParse(objA as string, out result))
          objA = (object) result;
        if (object.Equals(objA, (object) value))
          updateProps.Remove(key);
        else
          updateProps[key] = (object) value;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public int? FactoryImageReplicaCount { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string FactoryImageStorageAccountType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? UseEphemeralDisk { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? UseFactoryGalleryImageVersion { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? CreateFactoryDiskMachineProvisionedEventTimeout { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? DataDiskCount { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? DataDiskSizeGB { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? FactoryDiskSizeGB { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? FoldAtHomeEnabled { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DiskFormatExtension { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? EnableNestedInlineImageUpdates { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DataDiskCachingType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? UseRamDisks { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? RamDiskSizeGB { get; set; }
  }
}
