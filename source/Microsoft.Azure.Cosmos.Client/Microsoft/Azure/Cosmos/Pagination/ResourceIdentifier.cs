// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Pagination.ResourceIdentifier
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos.Pagination
{
  internal sealed class ResourceIdentifier
  {
    private const int OfferIdLength = 3;
    private const int RbacResourceIdLength = 6;
    private const int SnapshotIdLength = 7;
    public static readonly ushort Length = 20;
    public static readonly ushort MaxPathFragment = 8;
    public static readonly ResourceIdentifier Empty = new ResourceIdentifier();

    public ResourceIdentifier(
      uint offer = 0,
      uint database = 0,
      uint documentCollection = 0,
      ulong storedProcedure = 0,
      ulong trigger = 0,
      ulong userDefinedFunction = 0,
      ulong conflict = 0,
      ulong document = 0,
      ulong partitionKeyRange = 0,
      uint user = 0,
      uint clientEncryptionKey = 0,
      uint userDefinedType = 0,
      ulong permission = 0,
      uint attachment = 0,
      ulong schema = 0,
      ulong snapshot = 0,
      ulong roleAssignment = 0,
      ulong roleDefinition = 0)
    {
      this.Offer = offer;
      this.Database = database;
      this.DocumentCollection = documentCollection;
      this.StoredProcedure = storedProcedure;
      this.Trigger = trigger;
      this.UserDefinedFunction = userDefinedFunction;
      this.Conflict = conflict;
      this.Document = document;
      this.PartitionKeyRange = partitionKeyRange;
      this.User = user;
      this.ClientEncryptionKey = clientEncryptionKey;
      this.Permission = permission;
      this.Attachment = attachment;
      this.Schema = schema;
      this.UserDefinedType = userDefinedType;
      this.Snapshot = snapshot;
      this.RoleAssignment = roleAssignment;
      this.RoleDefinition = roleDefinition;
    }

    public uint Offer { get; }

    public uint Database { get; }

    public uint DocumentCollection { get; }

    public ulong StoredProcedure { get; }

    public ulong Trigger { get; }

    public ulong UserDefinedFunction { get; }

    public ulong Conflict { get; }

    public ulong Document { get; }

    public ulong PartitionKeyRange { get; }

    public uint User { get; }

    public uint ClientEncryptionKey { get; }

    public uint UserDefinedType { get; }

    public ulong Permission { get; }

    public uint Attachment { get; }

    public ulong Schema { get; }

    public ulong Snapshot { get; }

    public ulong RoleAssignment { get; }

    public ulong RoleDefinition { get; }

    public byte[] ToByteArray()
    {
      int length = 0;
      if (this.Offer > 0U)
        length += 3;
      else if (this.Snapshot > 0UL)
        length += 7;
      else if (this.RoleAssignment > 0UL)
        length += 6;
      else if (this.RoleDefinition > 0UL)
        length += 6;
      else if (this.Database > 0U)
        length += 4;
      if (this.DocumentCollection > 0U || this.User > 0U || this.UserDefinedType > 0U || this.ClientEncryptionKey > 0U)
        length += 4;
      if (this.Document > 0UL || this.Permission > 0UL || this.StoredProcedure > 0UL || this.Trigger > 0UL || this.UserDefinedFunction > 0UL || this.Conflict > 0UL || this.PartitionKeyRange > 0UL || this.Schema > 0UL || this.UserDefinedType > 0U || this.ClientEncryptionKey > 0U)
        length += 8;
      if (this.Attachment > 0U)
        length += 4;
      byte[] dst = new byte[length];
      if (this.Offer > 0U)
        ResourceIdentifier.BlockCopy(BitConverter.GetBytes(this.Offer), 0, dst, 0, 3);
      else if (this.Database > 0U)
        ResourceIdentifier.BlockCopy(BitConverter.GetBytes(this.Database), 0, dst, 0, 4);
      else if (this.Snapshot > 0UL)
        ResourceIdentifier.BlockCopy(BitConverter.GetBytes(this.Snapshot), 0, dst, 0, 7);
      else if (this.RoleAssignment > 0UL)
      {
        ResourceIdentifier.BlockCopy(BitConverter.GetBytes(this.RoleAssignment), 0, dst, 0, 4);
        ResourceIdentifier.BlockCopy(BitConverter.GetBytes(4096), 0, dst, 4, 2);
      }
      else if (this.RoleDefinition > 0UL)
        ResourceIdentifier.BlockCopy(BitConverter.GetBytes(this.RoleDefinition), 0, dst, 0, 6);
      if (this.DocumentCollection > 0U)
        ResourceIdentifier.BlockCopy(BitConverter.GetBytes(this.DocumentCollection), 0, dst, 4, 4);
      else if (this.User > 0U)
        ResourceIdentifier.BlockCopy(BitConverter.GetBytes(this.User), 0, dst, 4, 4);
      if (this.StoredProcedure > 0UL)
        ResourceIdentifier.BlockCopy(BitConverter.GetBytes(this.StoredProcedure), 0, dst, 8, 8);
      else if (this.Trigger > 0UL)
        ResourceIdentifier.BlockCopy(BitConverter.GetBytes(this.Trigger), 0, dst, 8, 8);
      else if (this.UserDefinedFunction > 0UL)
        ResourceIdentifier.BlockCopy(BitConverter.GetBytes(this.UserDefinedFunction), 0, dst, 8, 8);
      else if (this.Conflict > 0UL)
        ResourceIdentifier.BlockCopy(BitConverter.GetBytes(this.Conflict), 0, dst, 8, 8);
      else if (this.Document > 0UL)
        ResourceIdentifier.BlockCopy(BitConverter.GetBytes(this.Document), 0, dst, 8, 8);
      else if (this.PartitionKeyRange > 0UL)
        ResourceIdentifier.BlockCopy(BitConverter.GetBytes(this.PartitionKeyRange), 0, dst, 8, 8);
      else if (this.Permission > 0UL)
        ResourceIdentifier.BlockCopy(BitConverter.GetBytes(this.Permission), 0, dst, 8, 8);
      else if (this.Schema > 0UL)
        ResourceIdentifier.BlockCopy(BitConverter.GetBytes(this.Schema), 0, dst, 8, 8);
      else if (this.UserDefinedType > 0U)
      {
        ResourceIdentifier.BlockCopy(BitConverter.GetBytes(this.UserDefinedType), 0, dst, 8, 4);
        ResourceIdentifier.BlockCopy(BitConverter.GetBytes(1U), 0, dst, 12, 4);
      }
      else if (this.ClientEncryptionKey > 0U)
      {
        ResourceIdentifier.BlockCopy(BitConverter.GetBytes(this.ClientEncryptionKey), 0, dst, 8, 4);
        ResourceIdentifier.BlockCopy(BitConverter.GetBytes(2U), 0, dst, 12, 4);
      }
      if (this.Attachment > 0U)
        ResourceIdentifier.BlockCopy(BitConverter.GetBytes(this.Attachment), 0, dst, 16, 4);
      return dst;
    }

    public static ResourceIdentifier Parse(string id)
    {
      ResourceIdentifier rid;
      if (!ResourceIdentifier.TryParse(id, out rid))
        throw new FormatException("Failed to parse ResourceId");
      return rid;
    }

    public static bool TryParse(string id, out ResourceIdentifier rid)
    {
      try
      {
        uint offer = 0;
        uint database = 0;
        uint documentCollection = 0;
        ulong storedProcedure = 0;
        ulong trigger = 0;
        ulong userDefinedFunction = 0;
        ulong conflict = 0;
        ulong document = 0;
        ulong partitionKeyRange = 0;
        uint user = 0;
        uint clientEncryptionKey = 0;
        ulong permission = 0;
        uint attachment = 0;
        ulong schema = 0;
        uint userDefinedType = 0;
        ulong snapshot = 0;
        ulong roleAssignment1 = 0;
        ulong roleDefinition1 = 0;
        if (string.IsNullOrEmpty(id))
        {
          rid = (ResourceIdentifier) null;
          return false;
        }
        if (id.Length % 4 != 0)
        {
          rid = (ResourceIdentifier) null;
          return false;
        }
        byte[] buffer;
        if (!ResourceIdentifier.Verify(id, out buffer))
        {
          rid = (ResourceIdentifier) null;
          return false;
        }
        if (buffer.Length % 4 != 0 && buffer.Length != 3 && buffer.Length != 7 && buffer.Length != 6)
        {
          rid = (ResourceIdentifier) null;
          return false;
        }
        if (buffer.Length == 3)
        {
          uint unsignedLong = (uint) ResourceIdentifier.ToUnsignedLong(buffer);
          rid = new ResourceIdentifier(unsignedLong);
          return true;
        }
        if (buffer.Length == 7)
        {
          ulong unsignedLong = ResourceIdentifier.ToUnsignedLong(buffer);
          rid = new ResourceIdentifier(snapshot: unsignedLong);
          return true;
        }
        if (buffer.Length == 6)
        {
          int num = (int) buffer[5];
          ulong unsignedLong = ResourceIdentifier.ToUnsignedLong(buffer, 4);
          switch ((ResourceIdentifier.RbacResourceType) num)
          {
            case ResourceIdentifier.RbacResourceType.RbacResourceType_RoleDefinition:
              ulong roleDefinition2 = unsignedLong;
              rid = new ResourceIdentifier(offer, database, documentCollection, storedProcedure, trigger, userDefinedFunction, conflict, document, partitionKeyRange, user, clientEncryptionKey, userDefinedType, permission, attachment, schema, snapshot, roleAssignment1, roleDefinition2);
              return true;
            case ResourceIdentifier.RbacResourceType.RbacResourceType_RoleAssignment:
              ulong roleAssignment2 = unsignedLong;
              rid = new ResourceIdentifier(offer, database, documentCollection, storedProcedure, trigger, userDefinedFunction, conflict, document, partitionKeyRange, user, clientEncryptionKey, userDefinedType, permission, attachment, schema, snapshot, roleAssignment2, roleDefinition1);
              return true;
            default:
              rid = (ResourceIdentifier) null;
              return false;
          }
        }
        else
        {
          if (buffer.Length >= 4)
            database = BitConverter.ToUInt32(buffer, 0);
          if (buffer.Length >= 8)
          {
            byte[] dst1 = new byte[4];
            ResourceIdentifier.BlockCopy(buffer, 4, dst1, 0, 4);
            if (((int) dst1[0] & 128) > 0)
            {
              documentCollection = BitConverter.ToUInt32(dst1, 0);
              if (buffer.Length >= 16)
              {
                byte[] dst2 = new byte[8];
                ResourceIdentifier.BlockCopy(buffer, 8, dst2, 0, 8);
                ulong uint64 = BitConverter.ToUInt64(buffer, 8);
                if ((int) dst2[7] >> 4 == 0)
                {
                  document = uint64;
                  if (buffer.Length == 20)
                    attachment = BitConverter.ToUInt32(buffer, 16);
                }
                else if ((int) dst2[7] >> 4 == 8)
                  storedProcedure = uint64;
                else if ((int) dst2[7] >> 4 == 7)
                  trigger = uint64;
                else if ((int) dst2[7] >> 4 == 6)
                  userDefinedFunction = uint64;
                else if ((int) dst2[7] >> 4 == 4)
                  conflict = uint64;
                else if ((int) dst2[7] >> 4 == 5)
                  partitionKeyRange = uint64;
                else if ((int) dst2[7] >> 4 == 9)
                {
                  schema = uint64;
                }
                else
                {
                  rid = (ResourceIdentifier) null;
                  return false;
                }
              }
              else if (buffer.Length != 8)
              {
                rid = (ResourceIdentifier) null;
                return false;
              }
            }
            else
            {
              user = BitConverter.ToUInt32(dst1, 0);
              if (buffer.Length == 16)
              {
                if (user > 0U)
                {
                  permission = BitConverter.ToUInt64(buffer, 8);
                }
                else
                {
                  uint uint32 = BitConverter.ToUInt32(buffer, 8);
                  switch ((ResourceIdentifier.ExtendedDatabaseChildResourceType) BitConverter.ToUInt32(buffer, 12))
                  {
                    case ResourceIdentifier.ExtendedDatabaseChildResourceType.UserDefinedType:
                      userDefinedType = uint32;
                      break;
                    case ResourceIdentifier.ExtendedDatabaseChildResourceType.ClientEncryptionKey:
                      clientEncryptionKey = uint32;
                      break;
                    default:
                      rid = (ResourceIdentifier) null;
                      return false;
                  }
                }
              }
              else if (buffer.Length != 8)
              {
                rid = (ResourceIdentifier) null;
                return false;
              }
            }
          }
          rid = new ResourceIdentifier(offer, database, documentCollection, storedProcedure, trigger, userDefinedFunction, conflict, document, partitionKeyRange, user, clientEncryptionKey, userDefinedType, permission, attachment, schema, snapshot, roleAssignment1, roleDefinition1);
          return true;
        }
      }
      catch (Exception ex)
      {
        rid = (ResourceIdentifier) null;
        return false;
      }
    }

    public static bool Verify(string id, out byte[] buffer)
    {
      if (string.IsNullOrEmpty(id))
        throw new ArgumentNullException(nameof (id));
      buffer = (byte[]) null;
      try
      {
        buffer = ResourceIdentifier.FromBase64String(id);
      }
      catch (FormatException ex)
      {
      }
      if (buffer != null && buffer.Length <= (int) ResourceIdentifier.Length)
        return true;
      buffer = (byte[]) null;
      return false;
    }

    public override string ToString() => ResourceIdentifier.ToBase64String(this.ToByteArray());

    public static byte[] FromBase64String(string s) => Convert.FromBase64String(s.Replace('-', '/'));

    public static ulong ToUnsignedLong(byte[] buffer) => ResourceIdentifier.ToUnsignedLong(buffer, buffer.Length);

    public static ulong ToUnsignedLong(byte[] buffer, int length)
    {
      ulong unsignedLong = 0;
      for (int index = 0; index < length; ++index)
        unsignedLong |= (ulong) ((uint) buffer[index] << index * 8);
      return unsignedLong;
    }

    public static string ToBase64String(byte[] buffer) => ResourceIdentifier.ToBase64String(buffer, 0, buffer.Length);

    public static string ToBase64String(byte[] buffer, int offset, int length) => Convert.ToBase64String(buffer, offset, length).Replace('/', '-');

    public static void BlockCopy(byte[] src, int srcOffset, byte[] dst, int dstOffset, int count)
    {
      int num = srcOffset + count;
      for (int index = srcOffset; index < num; ++index)
        dst[dstOffset++] = src[index];
    }

    private enum CollectionChildResourceType : byte
    {
      Document = 0,
      Conflict = 4,
      PartitionKeyRange = 5,
      UserDefinedFunction = 6,
      Trigger = 7,
      StoredProcedure = 8,
      Schema = 9,
    }

    private enum ExtendedDatabaseChildResourceType
    {
      UserDefinedType = 1,
      ClientEncryptionKey = 2,
    }

    internal enum RbacResourceType : byte
    {
      RbacResourceType_RoleDefinition = 0,
      RbacResourceType_RoleAssignment = 16, // 0x10
    }
  }
}
