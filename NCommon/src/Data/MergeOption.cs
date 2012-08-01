using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCommon.Data
{
    /// <summary>
    /// Determines the synchronization option for sending or receiving entities. 
    /// This value is used when materializing objects. 
    /// Set this property to the appropriate materialization option before executing any queries or updates to the data service. 
    /// The default value is MergeOption.AppendOnly
    /// </summary>
    public enum MergeOption
    {
        /// <summary>
        /// Append new entities only. 
        /// Existing entities or their original values will not be modified. No client-side changes are lost in this merge. This is the default behavior.
        /// </summary>
        AppendOnly = 0,
        /// <summary>
        /// All current values on the client are overwritten with current values from the data service regardless of whether they have been changed on the client. 
        /// </summary>
        OverwriteChanges = 1,
        /// <summary>
        /// Current values that have been changed on the client are not modified, but any unchanged values are updated with current values from the data service. 
        /// No client-side changes are lost in this merge.
        /// </summary>
        PreserveChanges = 2,
        /// <summary>
        /// Objects are always loaded from persisted storage. 
        /// Any property changes made to objects in the object context are overwritten by the data source values.
        /// </summary>
        NoTracking = 3,
    }

}
