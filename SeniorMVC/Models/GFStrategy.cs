//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SeniorMVC.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class GFStrategy
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GFStrategy()
        {
            this.NPStrategies = new HashSet<NPStrategy>();
        }
    
        public string Title { get; set; }
        public int StrategyId { get; set; }
        public string Description { get; set; }
    
        public virtual GameFrame GameFrame { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NPStrategy> NPStrategies { get; set; }
    }
}
