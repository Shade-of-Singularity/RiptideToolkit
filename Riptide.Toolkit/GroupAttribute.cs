using System;

namespace Riptide.Toolkit
{
    /// <summary>
    /// Holds <c>byte</c> GroupIDs.
    /// </summary>
    /// TODO: Replace array definition with adaptive struct + interface.
    [Obsolete("Not obsolete but WIP. Not in use by any system yet.")]
    public sealed class GroupAttribute : Attribute
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Whether only 1 ID is defined in this attribute.
        /// </summary>
        /// <remarks>
        /// Introduced to reduce allocations in heap (untested).
        /// </remarks>
        public bool HasSingleID => IDs is null;

        /// <summary>
        /// Array of all the IDs defined in this group.
        /// </summary>
        public byte[] IDs { get; private set; }

        /// <summary>
        /// Singular ID. Equals to <see cref="NetworkIndex.InvalidGroupID"/> when there are more than one ID defined.
        /// </summary>
        public byte ID { get; private set; }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public GroupAttribute(byte id) => ID = id;
        public GroupAttribute(params byte[] ids)
        {
            ID = NetworkIndex.InvalidGroupID;
            IDs = ids;
        }
    }
}
