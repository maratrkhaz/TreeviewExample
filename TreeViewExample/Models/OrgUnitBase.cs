using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using TreeViewExample.Persistence;

namespace TreeViewExample.Models
{
    public abstract class OrgUnitBase
    {
        /// <summary>
        /// Holds the nodes one level below 
        /// </summary>
        private HashSet<OrgUnitBase> _children; 

        protected OrgUnitBase(string name) // For EF Core
        {
            Name = name;
        }

        protected OrgUnitBase(string name, OrgUnitBase parent)
        {
            Name = name;
            Parent = parent;
            _children = new HashSet<OrgUnitBase>();  
        }

        public abstract string CodeInForm { get; }

        protected static void AddUnitToDatabase(OrgUnitBase newUnit, OrgUnitDbContext context)
        {
            if (newUnit == null) throw new ArgumentNullException(nameof(newUnit));

            if (!(newUnit is Company))
            {
                if (newUnit.Parent == null)
                    throw new ApplicationException($"The parent of {newUnit.GetType().Name} is empty.");
                if (newUnit.Parent.ParentUnitId == 0)
                    throw new ApplicationException($"The parent {newUnit.Parent.Name} is empty.");
                if (newUnit.Parent is RetStore)
                    throw new ApplicationException($"The parent {newUnit.Parent.Name} must not be a shop.");
            }
            if (context.Entry(newUnit).State != EntityState.Detached)
                throw new ApplicationException("A unit is already in the database.");

            context.Add(newUnit);
            context.SaveChanges();
        }
       
        [Key]
        public int UnitId { get; private set; }

        /// <summary>
        /// Name of unit
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Foreign key points to parent - null for Company 
        /// </summary>
        public int? ParentUnitId { get; private set; }

        /// <summary>
        /// Pointer to parent - null for Company
        /// </summary>
        [ForeignKey(nameof(ParentUnitId))]
        public OrgUnitBase Parent { get; private set; }

        /// <summary>
        /// the nodes one level below 
        /// </summary>
        public IEnumerable<OrgUnitBase> Children => _children?.ToList();

        public override string ToString()
        {
            //return $"{GetType().Name}: Name = {Name}";
            return $" {Name}";
        }
        /// <summary>
        /// Changes name of unit
        /// </summary>
        /// <param name="name"></param>
        public void ChangeName(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Load children from database
        /// </summary>
        /// <param name="org"></param>
        /// <param name="context"></param>
        public void LoadChildren (OrgUnitBase org, DbContext context)
        {
           context.Entry(org).Collection(x => x.Children).Load();
        }

        /// <summary>
        /// Deletes node and all its children
        /// </summary>
        /// <param name="context"></param>
        public void DeleteUnit(DbContext context)
        {
            var allChildren = new HashSet<OrgUnitBase>();

            void SetAllChildren(OrgUnitBase org, HashSet<OrgUnitBase> t)
            {
                LoadChildren(org, context);

                if (!org._children.Any())
                    return;

                foreach (var unit in org._children)
                {
                    t.Add(unit);
                    SetAllChildren(unit, t);
                }
            }

            SetAllChildren(this, allChildren);

            using (var transaction = context.Database.BeginTransaction())
            {
                foreach (var child in allChildren)
                    context.Remove(child);

                context.Remove(this);

                context.SaveChanges();
                transaction.Commit();
            }
        }

        public void MoveUnitToNewParent(OrgUnitBase newParent, DbContext context)
        {
            if (this is Company)
                throw new ApplicationException("A Company can not be moved.");
            if (newParent == null)
                throw new ApplicationException("The parent cannot be null.");
            if (newParent.ParentUnitId == 0)
                throw new ApplicationException($"The parent {newParent.Name} must not be empty.");
            if (newParent == this)
                throw new ApplicationException("A unit can not be a parent of itself.");
            if (newParent is RetStore)
                throw new ApplicationException("A shop can not be a parent.");

            void SetHierarchy(OrgUnitBase unit)
            {
                if (unit.Children == null)
                    LoadChildren(unit, context);

                if (!unit._children.Any())
                    return;

                foreach (var u in unit._children)
                {
                    SetHierarchy(u);
                }
            }

            using (var transaction = context.Database.BeginTransaction())
            {
                Parent._children?.Remove(this);
                context.SaveChanges(); 

                Parent = newParent;
                
                SetHierarchy(this);
                context.SaveChanges(); 

                transaction.Commit();
            }
        }

        public void MoveUnitToNewParent(int parentId, DbContext context)
        {
            if (Parent == null)
            {
                Parent = context.Find<OrgUnitBase>(this.ParentUnitId);
            }
            var parent = context.Find<OrgUnitBase>(parentId);
            MoveUnitToNewParent(parent, context);
        }

    }
}

