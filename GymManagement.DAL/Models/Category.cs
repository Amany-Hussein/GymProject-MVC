using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.DAL.Models
{
    public class Category : BaseEntity
    {

        public string CategoryName { get; set; }

        #region Relationships

        public ICollection<Session> Sessions { get; set; } = default;

        #endregion
    }

}
