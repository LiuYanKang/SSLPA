﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace SeekerSoft.Base.DB
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class BaseEntities : DbContext
    {
        public BaseEntities()
            : base("name=BaseEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Dic> Dic { get; set; }
        public virtual DbSet<DicItem> DicItem { get; set; }
        public virtual DbSet<FunctionDefine> FunctionDefine { get; set; }
        public virtual DbSet<LoginUser> LoginUser { get; set; }
        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<RoleFuncMap> RoleFuncMap { get; set; }
        public virtual DbSet<RoleMenuMap> RoleMenuMap { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<OnlineUser> OnlineUser { get; set; }
        public virtual DbSet<SMSSend> SMSSend { get; set; }
        public virtual DbSet<EmailSend> EmailSend { get; set; }
        public virtual DbSet<DepartmentFuncMap> DepartmentFuncMap { get; set; }
        public virtual DbSet<DepartmentMenuMap> DepartmentMenuMap { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
    }
}
