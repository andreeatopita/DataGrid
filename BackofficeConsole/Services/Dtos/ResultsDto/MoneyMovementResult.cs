using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackofficeConsole.Services.Dtos.ResultsDto;

public record MoneyMovementResult (bool IsSuccess,string Message, int StudentId, decimal CurrentBalance, decimal NewBalance);

