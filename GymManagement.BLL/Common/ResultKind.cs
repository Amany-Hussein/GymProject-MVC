using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.BLL.Common
{
    public enum ResultKind
    {
        Ok,
        NotFound,
        Conflict,
        ValidationFailed,
        Forbidden
    }
}
