using HRProject_NTier.CORE.Entities;
using HRProject_NTier.CORE.ViewModels;
using HRProject_NTier.DATAACCESS.Context;
using HRProject_NTier.DATAACCESS.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HRProject_NTier.DATAACCESS.Repositories.Concrete
{
    public class PersonnelRaporRepository : IPersonnelRaporRepository
    {
        private ProjectContext _context;
        public PersonnelRaporRepository(ProjectContext context)
        {
            _context = context;
        }
        public IQueryable<PersonnelRapor> PersonnelRapors => _context.PersonnelRapors;

        public bool DeletePersonnelRapor(int id)
        {
            _context.PersonnelRapors.Remove(GetByID(id));
            return _context.SaveChanges() > 0;
        }

        public PersonnelRapor GetByID(int id)
        {
            return _context.PersonnelRapors.Find(id);
        }

        public bool InsertPersonnelRapor(PersonnelRapor personnelRapor)
        {
            _context.PersonnelRapors.Add(personnelRapor);
            return _context.SaveChanges() > 0;
        }

        public bool UpdatePersonnelRapor(PersonnelRapor personnelRapor)
        {
            _context.PersonnelRapors.Update(personnelRapor);
            return _context.SaveChanges() > 0;
        }
    }
}
