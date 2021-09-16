namespace Synnotech.EntityFrameworkCore
{
    /// <summary>
    /// Represents the different Entity Framework Core logging modes.
    /// </summary>
    public enum LoggingBehavior
    {
        /// <summary>
        /// Indicates that EF logging is turned off.
        /// </summary>
        Off = 0,

        /// <summary>
        /// Indicates that logging is enabled.
        /// </summary>
        Enabled = 1,

        /// <summary>
        /// Indicates that logging is enabled and parameters are included.
        /// </summary>
        EnabledWithSensitiveData = 2
    }
}