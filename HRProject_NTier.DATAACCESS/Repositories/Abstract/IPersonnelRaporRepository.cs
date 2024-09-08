using HRProject_NTier.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HRProject_NTier.DATAACCESS.Repositories.Abstract
{
    public interface IPersonnelRaporRepository
    {
        IQueryable<PersonnelRapor> PersonnelRapors { get; }

        bool InsertPersonnelRapor(PersonnelRapor personnelRapor);

        PersonnelRapor GetByID(int id);

        bool UpdatePersonnelRapor(PersonnelRapor personnelRapor);

        bool DeletePersonnelRapor(int id);
    }
}
