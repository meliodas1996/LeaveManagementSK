﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LeaveManagementSystemValueCreed.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class VC_LMSEntities : DbContext
    {
        public VC_LMSEntities()
            : base("name=VC_LMSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<approval> approvals { get; set; }
        public virtual DbSet<approver> approvers { get; set; }
        public virtual DbSet<department_tbl> department_tbl { get; set; }
        public virtual DbSet<Leave> Leaves { get; set; }
        public virtual DbSet<RolesTable> RolesTables { get; set; }
        public virtual DbSet<UsersTable> UsersTables { get; set; }
    }
}
