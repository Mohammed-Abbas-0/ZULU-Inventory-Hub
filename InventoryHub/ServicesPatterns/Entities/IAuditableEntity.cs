using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.ServicesPatterns.Entities
{
    public interface IAuditableEntity
    {
        bool? IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
        int? DeletedByUserId { get; set; }
        DateTime CreatedAt { get; set; }
        int? CreatedByUserId { get; set; }
        DateTime? UpdatedAt { get; set; }
        int? UpdatedByUserId { get; set; }
    }
}
