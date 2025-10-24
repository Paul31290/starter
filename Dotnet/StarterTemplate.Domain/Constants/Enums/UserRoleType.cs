namespace StarterTemplate.Domain.Constants.Enums
{
    /// <summary>
    /// Defines the standard roles available in the system.
    /// </summary>
    public enum UserRoleType
    {
        /// <summary>
        /// Administrator role with full system access.
        /// </summary>
        Admin = 1,

        /// <summary>
        /// Manager role with elevated privileges.
        /// </summary>
        Manager = 2,

        /// <summary>
        /// Standard user role with basic access.
        /// </summary>
        User = 3,

        /// <summary>
        /// Guest role with limited read-only access.
        /// </summary>
        Guest = 4
    }
}

