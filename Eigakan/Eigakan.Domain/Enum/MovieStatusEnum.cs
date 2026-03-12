using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Domain.Enum
{
    public enum MovieStatusEnum
    {
       WAITING_FOR_REVIEWING,
       ACCEPTED_NEGOTIATING,
	   WAITING_FOR_UPLOADING,
	   REJECTED,
       ACTIVE,
       ARCHIVED
    }
}
