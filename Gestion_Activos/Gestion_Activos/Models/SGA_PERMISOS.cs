//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gestion_Activos.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SGA_PERMISOS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SGA_PERMISOS()
        {
            this.SGA_ROLES_PERMISOS = new HashSet<SGA_ROLES_PERMISOS>();
        }
    
        public int ID { get; set; }
        public int IDENTIFICADOR { get; set; }
        public string NOMBRE { get; set; }
        public Nullable<int> CATEGORIA { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SGA_ROLES_PERMISOS> SGA_ROLES_PERMISOS { get; set; }
    }
}
