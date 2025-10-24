using System.ComponentModel.DataAnnotations;

namespace StarterTemplate.Domain.Entities
{
    /// <summary>
    /// Traceable entity class that provides common properties for all domain entities.
    /// All entities in the system inherit from this class to ensure consistent primary key structure.
    /// </summary>
    public abstract class TraceableEntity
    // : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// This is the primary key for all entities in the system.
        /// </summary>
        [Key]
        public int Id { get; set; }

        public DateTime? CreationDate { get; set; }
        public int? CreatedById { get; set; }
        public virtual User? CreatedBy { get; set; }
    }
}