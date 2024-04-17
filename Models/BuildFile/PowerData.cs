using System.Collections.Generic;

namespace MidsApp.Models.BuildFile
{
    /// <summary>
    /// Represents the data for a specific power within a character build, including its level, inclusion status for stats and procs,
    /// and associated sub-powers and slots.
    /// </summary>
    public class PowerData
    {
        /// <summary>
        /// Gets or sets the name of the power.
        /// </summary>
        public string PowerName { get; set; } = "";

        /// <summary>
        /// Gets or sets the level at which the power is obtained. A value of -1 indicates the power is not set or applicable.
        /// </summary>
        public int Level { get; set; } = -1;

        /// <summary>
        /// Gets or sets a value indicating whether stats should be included in the power's calculations.
        /// </summary>
        public bool StatInclude { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether procs should be included in the power's calculations.
        /// </summary>
        public bool ProcInclude { get; set; }

        /// <summary>
        /// Gets or sets the variable value associated with the power, which may affect its performance or effects.
        /// </summary>
        public int VariableValue { get; set; }

        /// <summary>
        /// Gets or sets the number of inherent slots used by this power. This is relevant for powers that consume slots without explicit player action.
        /// </summary>
        public int InherentSlotsUsed { get; set; }

        /// <summary>
        /// Gets or sets the collection of sub-powers associated with this power. Sub-powers are additional effects or abilities that are part of the main power.
        /// </summary>
        public List<SubPowerData> SubPowerEntries { get; set; } = new();

        /// <summary>
        /// Gets or sets the collection of slots associated with this power. Slots may contain enhancements that modify the power's effectiveness.
        /// </summary>
        public List<SlotData> SlotEntries { get; set; } = new();
    }
}
