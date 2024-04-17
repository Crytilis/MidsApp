namespace MidsApp.Models.BuildFile
{
    /// <summary>
    /// Represents the data for a slot within a power, including its level, inherent status,
    /// and any enhancements applied to it.
    /// </summary>
    public class SlotData
    {
        /// <summary>
        /// Gets or sets the level at which the slot becomes available or was added.
        /// This level is relative to the character's progression.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the slot is inherent to the power
        /// or was added by the player. Inherent slots cannot be modified.
        /// </summary>
        public bool IsInherent { get; set; }

        /// <summary>
        /// Gets or sets the primary enhancement data applied to this slot.
        /// This can be null if no enhancement is slotted.
        /// </summary>
        public EnhancementData? Enhancement { get; set; }

        /// <summary>
        /// Gets or sets the flipped or alternate enhancement data for this slot, if any.
        /// This concept applies when an enhancement can be toggled or has an alternate configuration.
        /// Can be null if no alternate enhancement is slotted or the concept does not apply.
        /// </summary>
        public EnhancementData? FlippedEnhancement { get; set; }
    }
}
