using System.ComponentModel.DataAnnotations;

namespace StarterTemplate.Domain.Entities
{
    /// <summary>
    /// Base entity class that provides common properties for all domain entities.
    /// All entities in the system inherit from this class to ensure consistent primary key structure.
    /// </summary>
    public abstract class BaseEntity : IEntity, ITraceable, IHasRowVersion
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// This is the primary key for all entities in the system.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the entity was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the username of the user who created the entity.
        /// </summary>
        public int? CreatedById { get; set; }

        /// <summary>
        /// Gets or sets the username of the user who created the entity.
        /// </summary>
        public User? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the entity was last modified.
        /// </summary>
        public DateTimeOffset? ModifiedAt { get; set; }

        /// <summary>
        /// Gets or sets the username of the user who last modified the entity.
        /// </summary>
        public int? ModifiedById { get; set; }

        /// <summary>
        /// Gets or sets the username of the user who last modified the entity.
        /// </summary>
        public User? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the row version for concurrency control.
        /// </summary>
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}